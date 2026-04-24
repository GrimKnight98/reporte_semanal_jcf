using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DemoMvc.Data;
using DemoMvc.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace DemoMvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        private async Task RegistrarMovimiento(string entityName, string entityId, string action, string role, string performedBy, string personNumber, string? details = null)
        {
            var log = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Role = role,
                PerformedBy = performedBy,
                PersonNumber = personNumber,
                Details = details
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
public async Task<IActionResult> Index(string? search, string? roleFilter, string? statusFilter, int page = 1, int pageSize = 10)
{
    var query = _userManager.Users.AsQueryable();

    // 🔎 búsqueda
    if (!string.IsNullOrWhiteSpace(search))
    {
        var normalizedSearch = search.ToUpper();
        query = query.Where(u =>
            u.UserName.Contains(search) ||
            u.Email.Contains(search) ||
            u.NormalizedUserName.Contains(normalizedSearch) ||
            u.NormalizedEmail.Contains(normalizedSearch) ||
            u.PersonNumber.Contains(search));
    }

    // 🔎 filtro de estado
    if (!string.IsNullOrWhiteSpace(statusFilter))
    {
        bool isActive = statusFilter == "true";
        query = query.Where(u => u.IsActive == isActive);
    }

    // 🔎 filtro por rol
    if (!string.IsNullOrWhiteSpace(roleFilter))
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleFilter);
        var ids = usersInRole.Select(u => u.Id).ToList();
        query = query.Where(u => ids.Contains(u.Id));
    }

    var totalUsers = await query.CountAsync();

    var users = await query
        .OrderBy(u => u.UserName)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // 📌 roles de los usuarios
    var rolesDict = new Dictionary<string, IList<string>>();
    foreach (var u in users)
    {
        rolesDict[u.Id] = await _userManager.GetRolesAsync(u);
    }

    // 📌 asignaciones Instructor ↔ JCF
    var assignations = await _context.InstructorJcfAssignments
        .Include(a => a.Instructor)
        .Include(a => a.Jcf)
        .OrderByDescending(a => a.CreatedAt)
        .ToListAsync();

    var viewModel = new UsersIndexViewModel
    {
        Users = users,
        CurrentPage = page,
        PageSize = pageSize,
        TotalCount = totalUsers,
        UserRoles = rolesDict,
        Assignations = assignations
    };

    // filtros
    ViewBag.CurrentSearch = search;
    ViewBag.CurrentRoleFilter = roleFilter;
    ViewBag.CurrentStatusFilter = statusFilter;

    ViewBag.RoleFilterOptions = new SelectList(
        _roleManager.Roles.ToList(),
        "Name",
        "Name",
        roleFilter
    );

    ViewBag.StatusFilterOptions = new SelectList(new[]
    {
        new { Value = "", Text = "-- Todos --" },
        new { Value = "true", Text = "Activo" },
        new { Value = "false", Text = "Inactivo" }
    }, "Value", "Text", statusFilter);

    // 📌 combos para asignar Instructor ↔ JCF
    ViewBag.Instructors = new SelectList(
        (await _userManager.GetUsersInRoleAsync("Instructor"))
            .Where(u => u.IsActive)
            .Select(u => new { u.Id, u.FullName }),
        "Id", "FullName"
    );

    ViewBag.Jcfs = new SelectList(
        (await _userManager.GetUsersInRoleAsync("JCF"))
            .Where(u => u.IsActive)
            .Select(u => new { u.Id, u.FullName }),
        "Id", "FullName"
    );

    return View(viewModel);
}



// GET: Users/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Obtener roles asignados al usuario
        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = roles;

        return View(user);
    }



        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string email, string password, string fullName, string firstName, string lastName, string department, string selectedRole, string personNumber)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                PersonNumber = personNumber
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var actor = await _userManager.GetUserAsync(User);
                var actorPersonNumber = actor?.PersonNumber ?? "N/A";

                if (!string.IsNullOrEmpty(selectedRole))
                {
                    await _userManager.AddToRoleAsync(user, selectedRole);
                    await RegistrarMovimiento("UserRole", $"{user.Id}-{selectedRole}", "Asignación de rol", "SysAdmin",
                        User.Identity?.Name ?? "Sistema", actorPersonNumber,
                        $"Se asignó el rol {selectedRole} al usuario {user.Email}");
                }

                await RegistrarMovimiento("User", user.Id, "Creación", "SysAdmin",
                    User.Identity?.Name ?? "Sistema", actorPersonNumber,
                    $"Usuario creado con correo {user.Email}, PersonNumber: {user.PersonNumber}");

                return RedirectToAction("Index");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);

            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return View(user);
        }

       [HttpPost]
public async Task<IActionResult> Edit(ApplicationUser model, string selectedRole)
{
    var user = await _userManager.FindByIdAsync(model.Id);
    if (user == null) return NotFound();

    user.Email = model.Email;
    user.UserName = model.Email; // 👈 aquí forzamos que el UserName sea el correo
    user.FullName = model.FullName;
    user.FirstName = model.FirstName;
    user.LastName = model.LastName;
    user.IsActive = model.IsActive;
    user.PersonNumber = model.PersonNumber;

    var result = await _userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
        var actor = await _userManager.GetUserAsync(User);
        var actorPersonNumber = actor?.PersonNumber ?? "N/A";

        if (!string.IsNullOrEmpty(selectedRole))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            await RegistrarMovimiento("UserRole", $"{user.Id}-{selectedRole}", "Cambio de rol", "SysAdmin",
                User.Identity?.Name ?? "Sistema", actorPersonNumber,
                $"Se cambió el rol a {selectedRole} para el usuario {user.Email}");
        }

        await RegistrarMovimiento("User", user.Id, "Edición", "SysAdmin",
            User.Identity?.Name ?? "Sistema", actorPersonNumber,
            $"Usuario actualizado: {user.FullName}, PersonNumber: {user.PersonNumber}");

        return RedirectToAction("Index");
    }

    foreach (var err in result.Errors)
        ModelState.AddModelError("", err.Description);

    ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
    return View(model);
}


        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var actor = await _userManager.GetUserAsync(User);
                    var actorPersonNumber = actor?.PersonNumber ?? "N/A";

                    await RegistrarMovimiento("User", user.Id, "Eliminación", "SysAdmin",
                        User.Identity?.Name ?? "Sistema", actorPersonNumber,
                        $"Usuario eliminado: {user.Email}");

                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        // BULK DELETE
        [HttpPost]
        public async Task<IActionResult> BulkDelete(List<string> ids)
        {
            if (ids == null || !ids.Any())
                return RedirectToAction("Index");

            var actor = await _userManager.GetUserAsync(User);
            var actorPersonNumber = actor?.PersonNumber ?? "N/A";

            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        await RegistrarMovimiento("User", user.Id, "Eliminación", "SysAdmin",
                            User.Identity?.Name ?? "Sistema", actorPersonNumber,
                            $"Usuario eliminado (bulk): {user.Email}");
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}

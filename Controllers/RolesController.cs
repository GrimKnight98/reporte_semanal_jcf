using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DemoMvc.Data;   // DbContext
using DemoMvc.Models; // AuditLog, ApplicationUser, RolesIndexViewModel
using System.Linq;

namespace DemoMvc.Controllers
{
    [Authorize(Roles = "SysAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public RolesController(RoleManager<IdentityRole> roleManager, 
                               UserManager<ApplicationUser> userManager,
                               AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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

        // READ: Lista de roles con búsqueda y paginación
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _roleManager.Roles.AsQueryable();

            // Filtrar si hay búsqueda
            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.ToUpper();
                query = query.Where(r => r.Name.Contains(search) || r.NormalizedName.Contains(normalizedSearch));
            }

            var totalRoles = query.Count();

            var roles = query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new RolesIndexViewModel
            {
                Roles = roles,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRoles
            };

            return View(viewModel);
        }

        // CREATE
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);
                    await RegistrarMovimiento("Role", roleName, "Creación", "SysAdmin", 
                        User.Identity?.Name ?? "Sistema", 
                        user?.PersonNumber ?? "N/A", 
                        $"Rol creado: {roleName}");
                    return RedirectToAction("Index");
                }
                foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
            }
            return View();
        }

        // DETAILS
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role == null ? NotFound() : View(role);
        }

        // EDIT
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role == null ? NotFound() : View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            var existing = await _roleManager.FindByIdAsync(role.Id);
            if (existing == null) return NotFound();

            existing.Name = role.Name;
            var result = await _roleManager.UpdateAsync(existing);
            if (result.Succeeded)
            {
                var user = await _userManager.GetUserAsync(User);
                await RegistrarMovimiento("Role", role.Id, "Edición", "SysAdmin", 
                    User.Identity?.Name ?? "Sistema", 
                    user?.PersonNumber ?? "N/A", 
                    $"Rol actualizado a: {role.Name}");
                return RedirectToAction("Index");
            }

            foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
            return View(role);
        }

        // DELETE GET: Mostrar confirmación
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            return View(role);
        }

        // DELETE POST: Ejecutar eliminación
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);
                    await RegistrarMovimiento("Role", role.Id, "Eliminación", "SysAdmin",
                        User.Identity?.Name ?? "Sistema",
                        user?.PersonNumber ?? "N/A",
                        $"Rol eliminado: {role.Name}");
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        // BULK DELETE: Eliminar múltiples roles seleccionados
[HttpPost]
public async Task<IActionResult> BulkDelete(List<string> ids)
{
    if (ids == null || !ids.Any())
    {
        // Nada seleccionado
        return RedirectToAction("Index");
    }

    var user = await _userManager.GetUserAsync(User);

    foreach (var id in ids)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                await RegistrarMovimiento("Role", role.Id, "Eliminación", "SysAdmin",
                    User.Identity?.Name ?? "Sistema",
                    user?.PersonNumber ?? "N/A",
                    $"Rol eliminado (bulk): {role.Name}");
            }
        }
    } 

    return RedirectToAction("Index");
}

    }
}

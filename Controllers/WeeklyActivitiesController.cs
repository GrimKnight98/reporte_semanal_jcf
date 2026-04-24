using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Data;
using DemoMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DemoMvc.Controllers
{
    [Authorize(Roles = "JCF")]
    public class WeeklyActivitiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WeeklyActivitiesController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       // GET: WeeklyActivities
public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
{
    // 🔑 Obtener el usuario autenticado
    var userId = _userManager.GetUserId(User);

    // 🔑 Filtrar solo las actividades creadas por ese usuario
    var query = _context.WeeklyActivities
        .Include(w => w.CreatedByUser)
        .Where(w => w.CreatedByUserId == userId) // 👈 filtro agregado
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(a =>
            a.Name.Contains(search) ||
            a.Description.Contains(search));
    }

    var totalCount = await query.CountAsync();

    var activities = await query
        .OrderByDescending(a => a.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var viewModel = new WeeklyActivitiesIndexViewModel
    {
        Activities = activities,
        CurrentPage = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        CurrentSearch = search
    };

    return View(viewModel);
}


        // GET: WeeklyActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var weeklyActivity = await _context.WeeklyActivities
                .Include(w => w.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (weeklyActivity == null) return NotFound();

            return View(weeklyActivity);
        }

        // GET: WeeklyActivities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeeklyActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] WeeklyActivity weeklyActivity)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "No se pudo obtener el usuario autenticado. Inicia sesión antes de crear actividades.");
                    return View(weeklyActivity);
                }

                weeklyActivity.CreatedByUserId = userId;
                weeklyActivity.CreatedAt = DateTime.UtcNow;
                weeklyActivity.UpdatedAt = null;

                _context.Add(weeklyActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(weeklyActivity);
        }

        // GET: WeeklyActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var weeklyActivity = await _context.WeeklyActivities.FindAsync(id);
            if (weeklyActivity == null) return NotFound();

            return View(weeklyActivity);
        }

        // POST: WeeklyActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedAt")] WeeklyActivity weeklyActivity)
        {
            if (id != weeklyActivity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.WeeklyActivities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(w => w.Id == id);

                    if (existing == null) return NotFound();

                    // 🔑 Mantener el usuario creador original
                    weeklyActivity.CreatedByUserId = existing.CreatedByUserId;
                    weeklyActivity.UpdatedAt = DateTime.UtcNow;

                    _context.Update(weeklyActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeeklyActivityExists(weeklyActivity.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weeklyActivity);
        }

        // GET: WeeklyActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var weeklyActivity = await _context.WeeklyActivities
                .Include(w => w.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (weeklyActivity == null) return NotFound();

            return View(weeklyActivity);
        }

        // POST: WeeklyActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weeklyActivity = await _context.WeeklyActivities.FindAsync(id);
            if (weeklyActivity != null)
            {
                _context.WeeklyActivities.Remove(weeklyActivity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔥 BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] ids)
        {
            if (ids != null && ids.Any())
            {
                var activities = await _context.WeeklyActivities
                    .Where(a => ids.Contains(a.Id))
                    .ToListAsync();

                if (activities.Any())
                {
                    _context.WeeklyActivities.RemoveRange(activities);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool WeeklyActivityExists(int id)
        {
            return _context.WeeklyActivities.Any(e => e.Id == id);
        }
    }
}

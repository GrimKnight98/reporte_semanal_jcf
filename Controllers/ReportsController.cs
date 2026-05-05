using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Data;
using DemoMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DinkToPdf;
using DinkToPdf.Contracts;
using DemoMvc.ViewModels.Reports;

namespace DemoMvc.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConverter _converter;

        public ReportsController(AppDbContext context, UserManager<ApplicationUser> userManager, IConverter converter)
        {
            _context = context;
            _userManager = userManager;
            _converter = converter;
        }

        // GET: Reports
public async Task<IActionResult> Index(
    int page = 1,
    int pageSize = 5,
    ReportStatus? status = null,
    DateTime? startDate = null,
    DateTime? endDate = null)
{
    var userId = _userManager.GetUserId(User);

    // 🔹 Normalización
    page = page <= 0 ? 1 : page;
    pageSize = pageSize <= 0 ? 5 : pageSize > 50 ? 50 : pageSize;

    // 🔴 Validación de fechas
    if (startDate.HasValue && endDate.HasValue && startDate > endDate)
    {
        ModelState.AddModelError("", "La fecha inicio no puede ser mayor a la fecha fin");
    }

    // 🔥 Default inteligente
    if (!status.HasValue && !startDate.HasValue && !endDate.HasValue)
    {
        startDate = DateTime.UtcNow.AddDays(-30);
    }

    // 🧠 Query optimizada
    var query = _context.Reports
        .AsNoTracking() // 🔥 mejora clave
        .Where(r => r.CreatedById == userId);

    // 🔍 Filtros dinámicos
    if (status.HasValue)
        query = query.Where(r => r.Status == status.Value);

    if (startDate.HasValue)
        query = query.Where(r => r.EndDate >= startDate.Value);

    if (endDate.HasValue)
        query = query.Where(r => r.StartDate <= endDate.Value);

    // 🔢 Total (antes de paginar)
    var totalItems = await query.CountAsync();

    // 📄 Datos paginados
    var items = await query
        .OrderByDescending(r => r.CreatedAt) // 🔥 ordenar antes de Skip/Take
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Include(r => r.CreatedBy) // 👉 SOLO lo necesario
        .ToListAsync();

    // 🔧 Ajustar página
    var totalPages = totalItems == 0
        ? 1
        : (int)Math.Ceiling((double)totalItems / pageSize);

    if (page > totalPages)
        page = totalPages;

    // 📦 ViewModel
    var model = new ReportPagedViewModel
    {
        Items = items,
        CurrentPage = page,
        PageSize = pageSize,
        TotalItems = totalItems,
        Status = status,
        StartDate = startDate,
        EndDate = endDate
    };

    return View(model);
}

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports
                .Include(r => r.Details)
                    .ThenInclude(d => d.ReportActivities)
                        .ThenInclude(ra => ra.WeeklyActivity)
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Reports/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
        int id,
        ReportStatus targetStatus,
        string? rejectionComment)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Reglas de transición
            switch (targetStatus)
            {
                case ReportStatus.SubmittedToInstructor:
                    if (report.Status == ReportStatus.Draft || report.Status == ReportStatus.Rejected)
                    {
                        report.Status = ReportStatus.SubmittedToInstructor;
                        report.UpdatedById = user.Id;
                        report.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case ReportStatus.Approved:
                    if (report.Status == ReportStatus.SubmittedToInstructor)
                    {
                        report.Status = ReportStatus.Approved;
                        report.ApprovedById = user.Id;
                        report.ApprovedAt = DateTime.UtcNow;
                        report.UpdatedById = user.Id;
                        report.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case ReportStatus.Rejected:
                    if (report.Status == ReportStatus.SubmittedToInstructor)
                    {
                        report.Status = ReportStatus.Rejected;
                        report.RejectedById = user.Id;
                        report.RejectedAt = DateTime.UtcNow;

                        report.RejectionComment = rejectionComment;

                        report.UpdatedById = user.Id;
                        report.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case ReportStatus.SubmittedToRI:
                    if (report.Status == ReportStatus.Approved)
                    {
                        report.Status = ReportStatus.SubmittedToRI;
                        report.UpdatedById = user.Id;
                        report.UpdatedAt = DateTime.UtcNow;
                    }
                    break;
            }

            _context.Update(report);
            await _context.SaveChangesAsync();

            // 🔑 Redirección dinámica según rol
            if (await _userManager.IsInRoleAsync(user, "JCF"))
            {
                return RedirectToAction("Details", "Reports", new { id = report.Id });
            }
            else if (await _userManager.IsInRoleAsync(user, "Instructor"))
            {
                return RedirectToAction("SubmittedReportDetails", "InstructorDashboard", new { id = report.Id });
            }

            return RedirectToAction("Index", "Reports");
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate")] Report report)
        {
            if (!ModelState.IsValid)
            {
                return View(report);
            }

            var userId = _userManager.GetUserId(User);
            report.CreatedById = userId;
            report.CreatedAt = DateTime.UtcNow;
            report.Status = ReportStatus.Draft;

            _context.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "ReportDetails", new { reportId = report.Id });
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Reports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate")] Report report)
        {
            if (id != report.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(report);
            }

            try
            {
                var existingReport = await _context.Reports.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                if (existingReport == null) return NotFound();

                // Mantener datos de auditoría
                report.Status = existingReport.Status;
                report.CreatedById = existingReport.CreatedById;
                report.CreatedAt = existingReport.CreatedAt;
                report.UpdatedById = _userManager.GetUserId(User);
                report.UpdatedAt = DateTime.UtcNow;

                _context.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(report.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Details)
                .ThenInclude(d => d.ReportActivities)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }

        // GET: Reports/ReportPrint/5
        public async Task<IActionResult> ReportPrint(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports
                .Include(r => r.Details)
                    .ThenInclude(d => d.ReportActivities)
                        .ThenInclude(ra => ra.WeeklyActivity)
                .Include(r => r.CreatedBy)   // aprendiz
                .Include(r => r.ApprovedBy)  // instructor/aprobador
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }
    }
}

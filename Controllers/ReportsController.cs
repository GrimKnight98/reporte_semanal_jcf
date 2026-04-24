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
        public async Task<IActionResult> Index()
        {
            // 🔑 Obtener el usuario autenticado
            var userId = _userManager.GetUserId(User);

            // 🔑 Filtrar solo los reportes creados por ese usuario
            var reports = await _context.Reports
                .Include(r => r.Details)
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedById == userId) // 👈 filtro agregado
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reports);
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
        public async Task<IActionResult> UpdateStatus(int id, ReportStatus targetStatus)
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

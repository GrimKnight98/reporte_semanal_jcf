using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Data;
using DemoMvc.Models;
using DemoMvc.ViewModels;
using DemoMvc.ViewModels.Instructor;

namespace DemoMvc.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorDashboardController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
public async Task<IActionResult> Dashboard()
{
    var instructorId = _userManager.GetUserId(User);

    var model = new InstructorDashboardViewModel
    {
        UserName = User.Identity?.Name ?? "Instructor"
    };

    // -------------------------
    // JCF Activos
    // -------------------------

    var jcfIds = await _context.InstructorJcfAssignments
        .Where(a =>
            a.InstructorUserId == instructorId &&
            a.Status == "Activo")
        .Select(a => a.JcfUserId)
        .ToListAsync();

    // -------------------------
    // Métricas
    // -------------------------

    model.ReportsPending = await _context.Reports
        .CountAsync(r =>
            jcfIds.Contains(r.CreatedById) &&
            r.Status == ReportStatus.SubmittedToInstructor);

    model.ReportsApproved = await _context.Reports
        .CountAsync(r =>
            jcfIds.Contains(r.CreatedById) &&
            r.Status == ReportStatus.Approved);

    model.ReportsRejected = await _context.Reports
        .CountAsync(r =>
            jcfIds.Contains(r.CreatedById) &&
            r.Status == ReportStatus.Rejected);

    // -------------------------
    // 🔥 Semana actual (lunes a domingo)
    // -------------------------

    var today = DateTime.UtcNow;

    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
    var startOfWeek = today.AddDays(-diff).Date;
    var endOfWeek = startOfWeek.AddDays(6);

    model.StartOfWeek = startOfWeek;
    model.EndOfWeek = endOfWeek;

    // -------------------------
    // 🔴 Pendientes de la semana
    // -------------------------

    model.RecentPendingReports = await _context.Reports
        .Include(r => r.CreatedBy)
        .Where(r =>
            jcfIds.Contains(r.CreatedById) &&
            r.Status == ReportStatus.SubmittedToInstructor &&
            r.StartDate >= startOfWeek &&
            r.EndDate <= endOfWeek)
        .OrderByDescending(r => r.CreatedAt)
        .Take(5)
        .ToListAsync();

    return View(model);
}
        [HttpGet]
        public async Task<IActionResult> MyJCFs(string search, int page = 1, int pageSize = 10)
        {
            var instructorId = _userManager.GetUserId(User);

            IQueryable<InstructorJcfAssignment> query = _context.InstructorJcfAssignments
                .Where(a => a.InstructorUserId == instructorId)
                .Include(a => a.Jcf)
                .OrderBy(a => a.Status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Jcf.UserName.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var asignaciones = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new MyJCFsViewModel
            {
                Asignaciones = asignaciones,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Search = search
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var asignacion = await _context.InstructorJcfAssignments
                .Include(a => a.Instructor)
                .Include(a => a.Jcf)
                .Include(a => a.CreatedByUser)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

[HttpGet]
public async Task<IActionResult> SubmittedReports(
    string? selectedJcfUserId,
    ReportStatus? status,
    DateTime? startDate,
    DateTime? endDate,
    int page = 1,
    int pageSize = 10)
{
    var instructorUserId = _userManager.GetUserId(User);

    // -----------------------------
    // Obtener JCF asignados al instructor
    // -----------------------------

    var jcfAssignments = await _context.InstructorJcfAssignments
        .Where(a =>
            a.InstructorUserId == instructorUserId &&
            a.Status == "Activo")
        .Include(a => a.Jcf)
        .ToListAsync();

    var jcfUserIds = jcfAssignments
        .Select(a => a.JcfUserId)
        .ToList();

    // -----------------------------
    // Query base
    // -----------------------------

    var query = _context.Reports
        .Include(r => r.CreatedBy)
        .Where(r =>
            jcfUserIds.Contains(r.CreatedById) &&
            (r.Status == ReportStatus.SubmittedToInstructor ||
             r.Status == ReportStatus.Approved ||
             r.Status == ReportStatus.Rejected ||
             r.Status == ReportStatus.SubmittedToRI))
        .AsQueryable();

    // -----------------------------
    // Filtro por JCF
    // -----------------------------

    if (!string.IsNullOrWhiteSpace(selectedJcfUserId))
    {
        query = query.Where(r => r.CreatedById == selectedJcfUserId);
    }

    // -----------------------------
    // Filtro por status
    // -----------------------------

    if (status.HasValue)
    {
        query = query.Where(r => r.Status == status.Value);
    }

    // -----------------------------
    // Fecha inicio
    // -----------------------------

    if (startDate.HasValue)
    {
        query = query.Where(r => r.StartDate >= startDate.Value);
    }

    // -----------------------------
    // Fecha fin
    // -----------------------------

    if (endDate.HasValue)
    {
        query = query.Where(r => r.EndDate <= endDate.Value);
    }

    // -----------------------------
    // Total registros
    // -----------------------------

    var totalItems = await query.CountAsync();

    // -----------------------------
    // Paginación
    // -----------------------------

    var items = await query
        .OrderByDescending(r => r.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // -----------------------------
    // Construir ViewModel
    // -----------------------------

    var vm = new SubmittedReportsPagedViewModel
    {
        Items = items,

        SelectedJcfUserId = selectedJcfUserId,
        Status = status,
        StartDate = startDate,
        EndDate = endDate,

        CurrentPage = page,
        PageSize = pageSize,

        TotalItems = totalItems,
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),

        FromItem = totalItems == 0 ? 0 : ((page - 1) * pageSize) + 1,
        ToItem = Math.Min(page * pageSize, totalItems),

        JcfOptions = jcfAssignments
            .Where(a => a.Jcf != null)
            .Select(a => a.Jcf!)
            .Distinct()
            .ToList()
    };

    return View(vm);
}

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> SubmittedReportDetails(int id)
        {
            var instructorUserId = _userManager.GetUserId(User);

            var jcfAssignments = await _context.InstructorJcfAssignments
                .Where(a =>
                    a.InstructorUserId == instructorUserId &&
                    a.Status == "Activo")
                .Select(a => a.JcfUserId)
                .ToListAsync();

            var report = await _context.Reports
    .Include(r => r.Details)
        .ThenInclude(d => d.ReportActivities)
            .ThenInclude(ra => ra.WeeklyActivity)
    .Include(r => r.CreatedBy)
    .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            if (!jcfAssignments.Contains(report.CreatedById))
            {
                return Forbid();
            }

            return View(report);
        }
    }
}
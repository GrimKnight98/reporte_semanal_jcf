using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DemoMvc.Data;
using DemoMvc.Models;

namespace DemoMvc.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorDashboardController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

[HttpGet]
public async Task<IActionResult> MyJCFs(string search, int page = 1, int pageSize = 10)
{
    var instructorId = _userManager.GetUserId(User);

    IQueryable<InstructorJcfAssignment> query = _context.InstructorJcfAssignments
        .Where(a => a.InstructorUserId == instructorId) // 👈 quitamos el filtro de Status
        .Include(a => a.Jcf)
        .OrderBy(a => a.Status); // 👈 primero "Activo", luego "Inactivo"

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

public async Task<IActionResult> SubmittedReports()
{
    // Usuario actual (Instructor en sesión)
    var instructorUserId = _userManager.GetUserId(User);

    // Buscar las asignaciones activas de este instructor
    var jcfAssignments = await _context.InstructorJcfAssignments
        .Where(a => a.InstructorUserId == instructorUserId && a.Status == "Activo")
        .ToListAsync();

    // Obtener los IDs de los JCF asignados
    var jcfUserIds = jcfAssignments.Select(a => a.JcfUserId).ToList();

    // Filtrar reportes creados por esos JCF y en estados relevantes
    var reports = await _context.Reports
        .Include(r => r.CreatedBy)
        .Where(r => jcfUserIds.Contains(r.CreatedById) &&
                    (r.Status == ReportStatus.SubmittedToInstructor ||
                     r.Status == ReportStatus.Approved ||
                     r.Status == ReportStatus.Rejected ||
                     r.Status == ReportStatus.SubmittedToRI))
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

    return View(reports);
}
[Authorize(Roles = "Instructor")]
public async Task<IActionResult> SubmittedReportDetails(int id)
{
    var instructorUserId = _userManager.GetUserId(User);

    // Validar que el reporte pertenece a un JCF asignado al instructor
    var jcfAssignments = await _context.InstructorJcfAssignments
        .Where(a => a.InstructorUserId == instructorUserId && a.Status == "Activo")
        .Select(a => a.JcfUserId)
        .ToListAsync();

    var report = await _context.Reports
        .Include(r => r.Details)
            .ThenInclude(d => d.ReportActivities)
                .ThenInclude(ra => ra.WeeklyActivity)
        .Include(r => r.CreatedBy)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (report == null) return NotFound();

    // Seguridad extra: solo permitir si el reporte fue creado por un JCF asignado
    if (!jcfAssignments.Contains(report.CreatedById))
    {
        return Forbid(); // o Unauthorized()
    }

    return View(report); // Renderiza SubmittedReportDetails.cshtml
}





    }
}

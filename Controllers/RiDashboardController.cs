using DemoMvc.Models;
using DemoMvc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DemoMvc.Controllers
{
    public class RiDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        // ✅ Único constructor
        public RiDashboardController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Acción principal del Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Traer todos los usuarios
            var allUsers = _userManager.Users.ToList();

            // Filtrar los que tienen rol JCF
            var jcfUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("JCF"))
                {
                    jcfUsers.Add(user);
                }
            }

            // Calcular inicio y fin de la semana actual (lunes a viernes)
            var today = DateTime.Today;
            var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var monday = today.AddDays(-diff);
            var friday = monday.AddDays(4);

            // Reportes enviados a RI en la semana actual
            var enviadosSemana = await _context.Reports
                .Where(r => r.Status == ReportStatus.SubmittedToRI
                            && r.StartDate.Date >= monday.Date
                            && r.EndDate.Date <= friday.Date)
                .CountAsync();

            // Reportes no enviados (todos los estados distintos de SubmittedToRI) en la semana actual
            var noEnviadosSemana = await _context.Reports
                .Where(r => r.Status != ReportStatus.SubmittedToRI
                            && r.StartDate.Date >= monday.Date
                            && r.EndDate.Date <= friday.Date)
                .CountAsync();

            // Construir ViewModel con conteos
            var viewModel = new RiDashboardViewModel
            {
                JcfActivos = jcfUsers.Count,
                ReportesEnviadosSemana = enviadosSemana,
                ReportesNoEnviadosSemana = noEnviadosSemana,
                ReportesAtrasadosSemana = 0, // luego conectamos con lógica real
                CurrentPage = 1,
                TotalPages = 1,
                TotalCount = jcfUsers.Count,
                Detalles = jcfUsers.Select(u => new JcfDetalle
                {
                    Jcf = u.UserName,
                    Instructor = u.Email,
                    UltimoReporte = DateTime.Now.AddDays(-7), // dummy temporal
                    Estado = "Pendiente"
                }).ToList()
            };

            return View(viewModel);
        }
    }
}

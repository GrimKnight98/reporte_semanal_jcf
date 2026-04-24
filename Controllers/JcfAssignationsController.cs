using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DemoMvc.Data;
using DemoMvc.Models;
using DemoMvc.ViewModels;

namespace DemoMvc.Controllers
{
    public class JcfAssignationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JcfAssignationsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Index con paginación
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var totalItems = _context.InstructorJcfAssignments.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var asignaciones = _context.InstructorJcfAssignments
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new JcfAssignationsIndexViewModel
            {
                Assignations = asignaciones,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: Create (formulario)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create (guardar nueva asignación)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string instructorId, string jcfId, string observaciones)
        {
            if (string.IsNullOrEmpty(instructorId) || string.IsNullOrEmpty(jcfId))
            {
                ModelState.AddModelError("", "Instructor y JCF son obligatorios.");
                return View();
            }

            // Validación: ¿ya existe esta asignación activa?
            var existeActiva = _context.InstructorJcfAssignments
                .Any(a => a.InstructorUserId == instructorId 
                       && a.JcfUserId == jcfId 
                       && a.Status == "Activo");

            if (existeActiva)
            {
                ModelState.AddModelError("", "Esta relación ya está registrada y activa.");
                 return RedirectToAction("Index", "Users");
            }

            var currentUserId = _userManager.GetUserId(User);

            var asignacion = new InstructorJcfAssignment
            {
                InstructorUserId = instructorId,
                JcfUserId = jcfId,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.Now,
                EffectiveStartDate = DateTime.Now,
                Status = "Activo",
                Observaciones = observaciones
            };

            _context.InstructorJcfAssignments.Add(asignacion);
            _context.SaveChanges();

            return RedirectToAction("Index", "Users");
        }

        // POST: Finalizar asignación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalizar(int id)
        {
            var asignacion = _context.InstructorJcfAssignments.Find(id);
            if (asignacion == null)
            {
                return NotFound();
            }

            // Solo se puede finalizar si está activa
            if (asignacion.Status == "Activo")
            {
                asignacion.Status = "Finalizado"; // o "Reemplazado", según tu flujo
                asignacion.EffectiveEndDate = DateTime.Now;
                asignacion.UpdatedAt = DateTime.Now;

                _context.InstructorJcfAssignments.Update(asignacion);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Users");
        }
    }
}

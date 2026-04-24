using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Data;
using DemoMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DemoMvc.Controllers
{
    [Authorize]
    public class ReportDetailsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportDetailsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ReportDetails/Create
        public IActionResult Create(int reportId)
        {
            ViewData["ActivitiesList"] = new MultiSelectList(_context.WeeklyActivities, "Id", "Name");
            return View(new ReportDetail { ReportId = reportId });
        }

        // POST: ReportDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportDetail detail, List<int>? ActivitiesIds)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ActivitiesList"] = new MultiSelectList(_context.WeeklyActivities, "Id", "Name", ActivitiesIds);
                return View(detail);
            }

            detail.UpdatedById = _userManager.GetUserId(User);
            detail.UpdatedAt = DateTime.UtcNow;

            // Poblar LearnedActivities con las descripciones de las actividades seleccionadas
            if (ActivitiesIds != null && ActivitiesIds.Any())
            {
                var activities = await _context.WeeklyActivities
                    .Where(a => ActivitiesIds.Contains(a.Id))
                    .ToListAsync();

                detail.LearnedActivities = string.Join("; ", activities.Select(a => a.Description));

                foreach (var activity in activities)
                {
                    detail.ReportActivities.Add(new ReportActivity
                    {
                        WeeklyActivityId = activity.Id,
                        ReportDetail = detail
                    });
                }
            }

            _context.Add(detail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Reports", new { id = detail.ReportId });
        }

        // GET: ReportDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var detail = await _context.ReportDetails
                .Include(d => d.ReportActivities)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null) return NotFound();

            ViewData["ActivitiesList"] = new MultiSelectList(
                _context.WeeklyActivities,
                "Id",
                "Name",
                detail.ReportActivities.Select(ra => ra.WeeklyActivityId)
            );

            return View(detail);
        }

        // POST: ReportDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReportDetail detail, List<int>? ActivitiesIds)
        {
            if (id != detail.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ActivitiesList"] = new MultiSelectList(_context.WeeklyActivities, "Id", "Name", ActivitiesIds);
                return View(detail);
            }

            try
            {
                var existingDetail = await _context.ReportDetails
                    .Include(d => d.ReportActivities)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (existingDetail == null) return NotFound();

                // Actualizar campos editables
                existingDetail.TrainingSessions = detail.TrainingSessions;
                existingDetail.Observations = detail.Observations;
                existingDetail.UpdatedById = _userManager.GetUserId(User);
                existingDetail.UpdatedAt = DateTime.UtcNow;

                // Actualizar actividades
                _context.ReportActivities.RemoveRange(existingDetail.ReportActivities);

                if (ActivitiesIds != null && ActivitiesIds.Any())
                {
                    var activities = await _context.WeeklyActivities
                        .Where(a => ActivitiesIds.Contains(a.Id))
                        .ToListAsync();

                    existingDetail.LearnedActivities = string.Join("; ", activities.Select(a => a.Description));

                    foreach (var activity in activities)
                    {
                        existingDetail.ReportActivities.Add(new ReportActivity
                        {
                            WeeklyActivityId = activity.Id,
                            ReportDetailId = existingDetail.Id
                        });
                    }
                }

                _context.Update(existingDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ReportDetails.Any(e => e.Id == detail.Id)) return NotFound();
                throw;
            }

            return RedirectToAction("Details", "Reports", new { id = detail.ReportId });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Models
{
    public class ReportDetail
    {
        public int Id { get; set; }

        // FK hacia Report (cabecera)
        public int ReportId { get; set; }
        public Report? Report { get; set; }

        // Este campo se llenará automáticamente en el controlador
        // concatenando las descripciones de las WeeklyActivities seleccionadas
        [MaxLength(2000)]
        public string LearnedActivities { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string TrainingSessions { get; set; }

        public string? Observations { get; set; }

        // Auditoría
        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relación muchos-a-muchos con WeeklyActivity
        public ICollection<ReportActivity> ReportActivities { get; set; } = new List<ReportActivity>();
    }
}

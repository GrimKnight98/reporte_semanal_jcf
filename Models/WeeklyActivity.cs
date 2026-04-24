using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Models
{
    public class WeeklyActivity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }

        // Relación con Identity: se asigna en el controller, no en la vista
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Relación muchos-a-muchos con ReportActivity
        public ICollection<ReportActivity> ReportActivities { get; set; } = new List<ReportActivity>();
    }
}

using System;

namespace DemoMvc.Models
{
    public class InstructorJcfAssignment
    {
        public int Id { get; set; }

        // Relación con Instructor
        public string? InstructorUserId { get; set; }
        public ApplicationUser? Instructor { get; set; }

        // Relación con JCF
        public string? JcfUserId { get; set; }
        public ApplicationUser? Jcf { get; set; }

        // Auditoría
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }   // 👈 navegación al admin creador
        public DateTime CreatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }
        public ApplicationUser? UpdatedByUser { get; set; }   // 👈 opcional, para auditoría de cambios
        public DateTime? UpdatedAt { get; set; }

        // Vigencia
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        // Estado lógico
        public string Status { get; set; } = "Activo";

        // Observaciones
        public string? Observaciones { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Models
{
    public enum ReportStatus
    {
        Draft,                // Editable, antes de enviar al instructor
        SubmittedToInstructor,// Enviado al instructor, bloqueado
        Approved,             // Validado por el instructor, listo para RI
        SubmittedToRI,        // Enviado a RI, bloqueado
        Rejected              // Rechazado por el instructor, editable para corrección
    }

    public class Report
    {
        public int Id { get; set; }

        // Metadatos principales
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Estado controlado por enum
        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Draft;

        // Auditoría
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Nuevo: quién aprobó
        public string? ApprovedById { get; set; }
        public ApplicationUser? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

    // Rechazo
    public string? RejectedById { get; set; }
    public ApplicationUser? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }

        // Relación con detalles (editable mientras esté en Draft/Rejected)
        public ICollection<ReportDetail> Details { get; set; } = new List<ReportDetail>();
    }
}

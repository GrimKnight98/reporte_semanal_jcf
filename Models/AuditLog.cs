using System;
using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        // Entidad afectada (ej. "User", "Product", "Report")
        [Required]
        public string EntityName { get; set; }

        // Id de la entidad afectada
        [Required]
        public string EntityId { get; set; }

        // Acción realizada
        [MaxLength(100)]
        public string Action { get; set; }

        // Rol que ejecutó la acción (SysAdmin, JCF, RI, Instructor)
        [MaxLength(50)]
        public string Role { get; set; }

        // Usuario que ejecutó la acción
        [MaxLength(200)]
        public string PerformedBy { get; set; }

        // Fecha y hora
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Detalles adicionales
        [MaxLength(1000)]
        public string? Details { get; set; }

        [MaxLength(50)]
        public string PersonNumber { get; set; }
    }
}

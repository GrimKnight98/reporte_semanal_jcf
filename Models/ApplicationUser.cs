// File: Models/ApplicationUser.cs
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DemoMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Nombre completo para mostrar en la UI
        [MaxLength(200)]
        public string? FullName { get; set; }

        // Opcional: separar nombre y apellido si lo prefieres
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        // Ruta o nombre de archivo de la foto de perfil
        [MaxLength(500)]
        public string? ProfilePicturePath { get; set; }

        // Fecha de creación del usuario (útil para auditoría)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string PersonNumber { get; set; }

        // Flag opcional para desactivar cuentas sin borrarlas
        public bool IsActive { get; set; } = true;

        // Firma del Instructor
        public string? SignaturePath { get; set; }
        // Ejemplo de campo extra para roles o metadatos
        // [MaxLength(250)]
        // public string? Department { get; set; }

        // Propiedades de Identity ya incluidas en IdentityUser:
        // - Id (string)
        // - UserName
        // - NormalizedUserName
        // - Email
        // - EmailConfirmed
        // - PhoneNumber
        // - PasswordHash
        // - SecurityStamp
        // - etc.
    }
}

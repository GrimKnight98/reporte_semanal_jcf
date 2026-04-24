using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace DemoMvc.Models
{
    public class UsersIndexViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        // Diccionario: UserId -> Lista de roles asignados
        public Dictionary<string, IList<string>> UserRoles { get; set; } = new Dictionary<string, IList<string>>();

        // Paginación
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // 🔑 Nueva propiedad para asignaciones
        public IEnumerable<InstructorJcfAssignment> Assignations { get; set; }
    }
}

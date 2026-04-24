using System.Collections.Generic;

namespace DemoMvc.Models
{
    public class MyJCFsViewModel
    {
        // Lista de asignaciones
        public IEnumerable<InstructorJcfAssignment> Asignaciones { get; set; }

        // Conteo total de asignaciones
        public int TotalCount { get; set; }

        // Paginación
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Búsqueda
        public string Search { get; set; }
    }
}

using System.Collections.Generic;
using DemoMvc.Models;

namespace DemoMvc.ViewModels
{
    public class JcfAssignationsIndexViewModel
    {
        public IEnumerable<InstructorJcfAssignment> Assignations { get; set; }

        // Paginación
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

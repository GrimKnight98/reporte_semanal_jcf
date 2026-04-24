using System.Collections.Generic;
using DemoMvc.Models;

namespace DemoMvc.Models
{
    public class WeeklyActivitiesIndexViewModel
    {
        public IEnumerable<WeeklyActivity> Activities { get; set; } = new List<WeeklyActivity>();

        // Paginación
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // Filtros activos
        public string CurrentSearch { get; set; }
    }
}

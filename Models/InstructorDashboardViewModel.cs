using DemoMvc.Models;
namespace DemoMvc.ViewModels
{
    public class InstructorDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = "Instructor";

        // Métricas principales
        public int JcfCount { get; set; }          // Número total de JCF asignados
        public int JcfActivosCount { get; set; }   // Número de JCF activos
        public int JcfInactivosCount { get; set; } // Número de JCF inactivos

        public int ReportsPending { get; set; }    // Reportes en aprobación
        public int ReportsApproved { get; set; }   // Reportes aprobados
        public int ReportsRejected { get; set; }   // Reportes rechazados
        public List<Report> RecentPendingReports { get; set; } = new();

public DateTime StartOfWeek { get; set; }
public DateTime EndOfWeek { get; set; }
    }
}

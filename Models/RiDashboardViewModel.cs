using System;
using System.Collections.Generic;

namespace DemoMvc.Models
{
    public class RiDashboardViewModel
    {
        public int JcfActivos { get; set; }

        // ✅ Propiedades semanales
        public int ReportesEnviadosSemana { get; set; }
        public int ReportesNoEnviadosSemana { get; set; }
        public int ReportesAtrasadosSemana { get; set; }

        // Paginación y conteo
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }

        // Detalle de JCF
        public List<JcfDetalle> Detalles { get; set; } = new List<JcfDetalle>();
    }

    public class JcfDetalle
    {
        public string Jcf { get; set; }
        public string Instructor { get; set; }
        public DateTime UltimoReporte { get; set; }
        public string Estado { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAA.Models
{
    public class ReporteAsistenciaViewModel
    {
        public ReporteAsistenciaViewModel() { }
        public reporte Reporte { get; set; }
        public virtual ICollection<asistencia> asistencias { get; set; }
    }
}
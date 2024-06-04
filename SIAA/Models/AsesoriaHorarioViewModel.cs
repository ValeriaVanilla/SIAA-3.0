using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAA.Models
{
    public class AsesoriaHorarioViewModel
    {
        public AsesoriaHorarioViewModel() { }  
        public asesoria Asesoria { get; set; }
        public cat_horario Horario { get; set; }

        public int IdAsesor { get; set; }

        public int IdUnidadAprendizaje { get; set; }

        public int IdLugar { get; set; }
    }
}
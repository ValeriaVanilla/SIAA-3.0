//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIAA
{
    using System;
    using System.Collections.Generic;
    
    public partial class solicitud
    {
        public int Idsolicitud { get; set; }
        public int IdAlumno { get; set; }
        public int IdAsesoria { get; set; }
        public Nullable<System.DateTime> FechaSolicitud { get; set; }
        public Nullable<System.DateTime> FechaAceptacion { get; set; }
        public Nullable<int> IdEstado { get; set; }
    
        public virtual alumno alumno { get; set; }
        public virtual asesoria asesoria { get; set; }
        public virtual cat_estado cat_estado { get; set; }
    }
}

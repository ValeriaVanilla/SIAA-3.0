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
    
    public partial class usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Genero { get; set; }
        public Nullable<int> IdProgramaEducativo { get; set; }
        public int IdTipoUsuario { get; set; }
        public int IdEstatus { get; set; }
    
        public virtual alumno alumno { get; set; }
        public virtual asesor asesor { get; set; }
        public virtual cat_estatus cat_estatus { get; set; }
        public virtual cat_programa_educativo cat_programa_educativo { get; set; }
        public virtual cat_tipo_usuario cat_tipo_usuario { get; set; }
        public virtual encargado encargado { get; set; }
    }
}

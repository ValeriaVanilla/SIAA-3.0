﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class siaaEntities : DbContext
    {
        public siaaEntities()
            : base("name=siaaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<alumno> alumnoes { get; set; }
        public virtual DbSet<asesor> asesors { get; set; }
        public virtual DbSet<asesoria> asesorias { get; set; }
        public virtual DbSet<asistencia> asistencias { get; set; }
        public virtual DbSet<cat_estado> cat_estado { get; set; }
        public virtual DbSet<cat_estatus> cat_estatus { get; set; }
        public virtual DbSet<cat_horario> cat_horario { get; set; }
        public virtual DbSet<cat_lugar> cat_lugar { get; set; }
        public virtual DbSet<cat_programa_educativo> cat_programa_educativo { get; set; }
        public virtual DbSet<cat_temas> cat_temas { get; set; }
        public virtual DbSet<cat_tipo_usuario> cat_tipo_usuario { get; set; }
        public virtual DbSet<cat_unidad_aprendizaje> cat_unidad_aprendizaje { get; set; }
        public virtual DbSet<encargado> encargadoes { get; set; }
        public virtual DbSet<evaluacion> evaluacions { get; set; }
        public virtual DbSet<reporte> reportes { get; set; }
        public virtual DbSet<solicitud> solicituds { get; set; }
        public virtual DbSet<usuario> usuarios { get; set; }
    }
}

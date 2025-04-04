﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Microsoft.Win32;
using SIAA;
using SIAA.Models;

namespace SIAA.Controllers
{
    public class encargadoController : Controller
    {
        private siaaEntities db = new siaaEntities();
        encargado encargado1 = System.Web.HttpContext.Current.Session["LOGIN"] as encargado;

        public ActionResult Layout_Encargado()
        {
            encargado encargado1 = System.Web.HttpContext.Current.Session["LOGIN"] as encargado;
            return View();
        }

        public class reportesController : Controller
        {
            // GET: encargado
            public ActionResult ReporteEncargado()
            {
                return View();
            }
        }

        public ActionResult InformacionPersonal()
        {
            usuario usuario = encargado1.usuario;
            return View(usuario);
        }

        #region Asesoria


        public ActionResult ConsultaAsesoria(int pagina = 1, int registros = 9, string searchString = "")// Se obtiene la lista de las asesorias registradas en el sistema y las conexiones que hay entre las tablas
        {
            
            var unidadesAprendizaje = db.cat_unidad_aprendizaje.ToList();
            ViewBag.UnidadesAprendizaje = unidadesAprendizaje;

            var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Include(a => a.cat_lugar);

            if (!String.IsNullOrEmpty(searchString))
            {
                asesorias = asesorias.Where(a => a.cat_unidad_aprendizaje.NombreUnidadAprendizaje.Contains(searchString) ||
                                                 a.asesor.usuario.Nombre.Contains(searchString) ||
                                                 a.asesor.usuario.ApellidoPaterno.Contains(searchString) ||
                                                 a.asesor.usuario.ApellidoMaterno.Contains(searchString) ||
                                                 a.cat_lugar.Descripcion.Contains(searchString) ||
                                                 a.cat_horario.Dia.Contains(searchString) ||
                                                 a.CicloEscolar.Contains(searchString));
            }

            asesorias = asesorias.OrderBy(a => a.IdAsesoria);

            var totalAsesorias = asesorias.Count();
            var asesoriasP = asesorias.Skip((pagina - 1) * registros).Take(registros).ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalAsesorias / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;

            return View(asesoriasP);


        }


        public ActionResult RegistroAsesoria() // Se inicializan las varibles de desplazamiento
        {
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana);
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje");
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion");
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "usuario.Nombre");
            var ultimosRegistros = db.asesorias.OrderByDescending(a => a.IdAsesoria).Take(2).ToList();
            ViewBag.UltimosRegistros = ultimosRegistros;
            return View();
        }



        [HttpPost]  // Guarda los registros
        [ValidateAntiForgeryToken]
        public ActionResult RegistroAsesoria(AsesoriaHorarioViewModel viewModel) // Toma el Modela AsesoriaHorarioViewModel para hacer el registro en las tablas horario y asesoria
        {
            DateTime fecha;
            var horario = viewModel.Horario;
            asesoria asesoria = new asesoria();
            if (db.asesorias.Any())
            {
                asesoria.IdAsesoria = db.asesorias.ToList().Last().IdAsesoria + 1;
            }
            else { asesoria.IdAsesoria = 1; }
            horario.IdHorario = asesoria.IdAsesoria;
            asesoria.IdHorario = asesoria.IdAsesoria;
            asesoria.IdAsesor = viewModel.IdAsesor;
            asesoria.IdLugar = viewModel.IdLugar;
            asesoria.IdUnidadAprendizaje = viewModel.IdUnidadAprendizaje;

            if (ModelState.IsValid)
            {
                fecha = System.DateTime.Now;
                if (fecha.Month / 2 >= 6)
                {
                    asesoria.CicloEscolar = fecha.Year.ToString() + "-2";
                }
                else
                {
                    asesoria.CicloEscolar = fecha.Year.ToString() + "-1";
                }
                db.cat_horario.Add(horario);
                db.SaveChanges(); // Se guardan los datos de horario               
                db.asesorias.Add(asesoria);
                db.SaveChanges(); // Se guardan los datos de asesoria
                TempData["MsgRegistroExitoso"] = "La asesoria se registró exitosamente.";
                return RedirectToAction("RegistroAsesoria");
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana, horario.Dia);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "usuario.Nombre", asesoria.IdAsesor);
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje", asesoria.IdUnidadAprendizaje);
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion", asesoria.IdLugar);
            return View(asesoria);
        }

        public bool Validar(asesoria asesoria) // Funcion para validar los datos antes de guardarlos
        {
            var use = db.asesorias.ToList();
            for (int i = 0; i < use.Count(); i++)
            {
                if (asesoria.IdAsesoria == use[i].IdAsesoria)
                {
                    MessageBox.Show("Matricula duplicada", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            if (asesoria.IdAsesor == 0 || asesoria.cat_unidad_aprendizaje == null || asesoria.IdUnidadAprendizaje == 0)
            {
                MessageBox.Show("Ingrese todos los datos", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        public ActionResult ModificaAsesoria(int? id) // Valida antes de modificar
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AsesoriaHorarioViewModel viewModel = new AsesoriaHorarioViewModel();
            viewModel.Asesoria = db.asesorias.Find(id);
            viewModel.Horario = db.cat_horario.Find(id);
            if (viewModel.Asesoria == null)
            {
                return HttpNotFound();
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "usuario.Nombre");
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje");
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion");
            return View(viewModel);
        }

        [HttpPost] // Se modifican los registros de alumnos
        [ValidateAntiForgeryToken]
        public ActionResult ModificaAsesoria(AsesoriaHorarioViewModel viewModel)
        {
            DateTime fecha;

            var horario = viewModel.Horario;
            var asesoria = viewModel.Asesoria;
            horario.IdHorario = asesoria.IdAsesoria;
            asesoria.IdHorario = asesoria.IdAsesoria;
            asesoria.IdAsesor = viewModel.IdAsesor;
            asesoria.IdLugar = viewModel.IdLugar;
            asesoria.IdUnidadAprendizaje = viewModel.IdUnidadAprendizaje;

            if (ModelState.IsValid)
            {
                fecha = System.DateTime.Now;
                if (fecha.Month / 2 >= 6)
                {
                    asesoria.CicloEscolar = fecha.Year.ToString() + "-2";
                }
                else
                {
                    asesoria.CicloEscolar = fecha.Year.ToString() + "-1";
                }
                db.Entry(horario).State = EntityState.Modified;
                db.Entry(asesoria).State = EntityState.Modified;
                db.SaveChanges();

                TempData["MsgModificacionExitosa"] = "La asesoría se modificó con éxito.";
                return RedirectToAction("ConsultaAsesoria");
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana, horario.Dia);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "usuario.Nombre", asesoria.IdAsesor);
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje", asesoria.IdUnidadAprendizaje);
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion", asesoria.IdLugar);
            return View(asesoria);
        }

        public ActionResult EliminaAsesoria(int? id) // Elimina registros de asesorías
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asesoria asesoria = db.asesorias.Find(id);
            if (asesoria == null)
            {
                return HttpNotFound();
            }
            return View(asesoria);
        }

        // POST: asesorias/Delete/5
        [HttpPost, ActionName("ConfirmarEliminaAsesoria")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarEliminaAsesoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asesoria asesoria = db.asesorias.Find(id);
            cat_horario horario = db.cat_horario.Find(id);
            if (asesoria == null)
            {
                return HttpNotFound();
            }
            db.asesorias.Remove(asesoria);
            db.cat_horario.Remove(horario);
            db.SaveChanges();
            TempData["MsgEliminacionExitosa"] = "La asesoria se eliminó exitosamente.";
            return RedirectToAction("ConsultaAsesoria");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Asesor
        public ActionResult ConsultaAsesor(int pagina = 1, int registros = 6, string searchString = "") // Se obtiene la lista de los asesores registrados en el sistema y las conexiones que hay entre las tablas
        {

            var asesores = db.asesors.Include(a => a.usuario).Include(a => a.asesorias);
            if (!String.IsNullOrEmpty(searchString))
            {
                asesores = asesores.Where(a => a.usuario.Nombre.Contains(searchString) ||
                                             a.usuario.ApellidoPaterno.Contains(searchString) ||
                                             a.usuario.ApellidoMaterno.Contains(searchString) ||
                                             a.usuario.cat_estatus.Descripcion.Contains(searchString) ||
                                             a.usuario.cat_programa_educativo.NombreProgramaEducativo.Contains(searchString) ||
                                             a.usuario.cat_tipo_usuario.NombreUsuario.Contains(searchString));
            }

            asesores = asesores.OrderBy(a => a.IdAsesor);

            var totalAsesores = asesores.Count();
            var asesoresP = asesores.Skip((pagina - 1) * registros).Take(registros).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalAsesores / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;
            return View(asesoresP);

        }
        #endregion

        #region AlumnoAsesor

        public ActionResult ConsultaAlumno(int pagina = 1, int registros = 6, string searchString = "") // Se obtiene la lista de los alumnos registrados en el sistema y las conexiones que hay entre las tablas
        {

            var alumnos = db.alumnoes.Include(a => a.usuario).Include(a => a.usuario.cat_estatus).Include(a => a.usuario.cat_programa_educativo).Include(a => a.usuario.cat_tipo_usuario).Include(a => a.solicituds);            

            if (!String.IsNullOrEmpty(searchString))
            {
                alumnos = alumnos.Where(a => a.usuario.Nombre.Contains(searchString) ||
                                             a.usuario.ApellidoPaterno.Contains(searchString) ||
                                             a.usuario.ApellidoMaterno.Contains(searchString) ||
                                             a.usuario.cat_estatus.Descripcion.Contains(searchString) ||
                                             a.usuario.cat_programa_educativo.NombreProgramaEducativo.Contains(searchString) ||
                                             a.usuario.cat_tipo_usuario.NombreUsuario.Contains(searchString));                                                                                                                                      
            }

            alumnos = alumnos.OrderBy(a => a.IdAlumno);

            var totalAlumnos = alumnos.Count();
            var alumnosP = alumnos.Skip((pagina - 1) * registros).Take(registros).ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalAlumnos / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;

            return View(alumnosP);
        }

        public ActionResult ConsultaAlumoAsesor() // Se obtiene la lista de los alumnos asesores registrados en el sistema y las conexiones que hay entre las tablas
        {
            var usuarios = db.usuarios.Include(a => a.alumno).Include(a => a.cat_tipo_usuario).Include(a => a.cat_programa_educativo).Include(a => a.cat_estatus);

            return View(usuarios.ToList());
        }

        public ActionResult RegistroAlumnoAsesor(int? id) // Elimina registros de asesorías
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            alumno alumno = db.alumnoes.Find(id);
            if (alumno == null)
            {
                return HttpNotFound();
            }
            return View(alumno);
        }

        // POST: asesorias/Delete/5
        [HttpPost, ActionName("ConfirmarRegistroAlumnoAsesor")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarRegistroAlumnoAsesor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            alumno alumno = db.alumnoes.Find(id);
            if (alumno == null)
            {
                return HttpNotFound();
            }
            if(alumno.usuario.IdTipoUsuario == 4)
            {                
                alumno.usuario.IdTipoUsuario = 1;
            } else
            {                
                alumno.usuario.IdTipoUsuario = 4;
            }                
            db.Entry(alumno).State = EntityState.Modified;
            db.SaveChanges();
            TempData["MsgRegistroAlumnoAsesorExitoso"] = "El cambio de rol se registró exitosamente";
            return RedirectToAction("ConsultaAlumno");
        }


        public ActionResult BuscarPorMatricula([Bind(Include = "IdAlumno")] alumno alumno)
        {
            Debug.WriteLine(alumno.IdAlumno);
            alumno alumnoAsesor = db.alumnoes.Find(alumno.IdAlumno);
            if (alumnoAsesor.IdAlumno == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (alumnoAsesor == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario");
            return View("RegistraAlumnoAsesor", alumnoAsesor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistraAlumnoAsesor([Bind(Include = "IdUsuario, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Genero, IdProgramaEducativo, IdEstatus, IdTipoUsuario")] alumno alumno)
        {
            DialogResult result1 = MessageBox.Show("Desea modificar el registro?", "Modifica Alumno Asesor", MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);
            if (result1 == DialogResult.Yes)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(alumno).State = EntityState.Modified;
                    db.SaveChanges();
                    MessageBox.Show("La modificación se ha guardado con exito", "Modificación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return RedirectToAction("ConsultaAlumno");
                }
            }
            else
                return RedirectToAction("RegistraAlumnoAsesor");

            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", alumno.usuario.IdTipoUsuario);
            return View(alumno);
        }

        #endregion

    }
}

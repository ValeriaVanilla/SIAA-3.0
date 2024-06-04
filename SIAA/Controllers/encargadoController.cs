using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using SIAA;
using SIAA.Models;

namespace SIAA.Controllers
{
    public class encargadoController : Controller
    {
        private siaaEntities db = new siaaEntities();
        encargado encargado1 = System.Web.HttpContext.Current.Session["LOGIN"] as encargado;

        public class reportesController : Controller
        {
            // GET: encargado
            public ActionResult ReporteEncargado()
            {
                return View();
            }
        }

        #region asesoria


        public ActionResult ConsultaAsesoria()// Se obtiene la lista de las asesorias registradas en el sistema y las conexiones que hay entre las tablas
        {
            var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Include(a => a.cat_lugar);
            var asesoriasOrdenadas = asesorias.OrderBy(a => a.cat_unidad_aprendizaje.NombreUnidadAprendizaje).ToList();
            // Pasa la lista ordenada a la vista
            return View(asesoriasOrdenadas);
        }


        public ActionResult RegistroAsesoria(String nombre) // Se inicializan las varibles de desplazamiento
        {
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "IdAsesor");
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje");
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion");
            return View();
        }



        [HttpPost]  // Guarda los registros
        [ValidateAntiForgeryToken]
        public ActionResult RegistroAsesoria(AsesoriaHorarioViewModel viewModel) // Toma el Modela AsesoriaHorarioViewModel para hacer el registro en las tablas horario y asesoria
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
                db.cat_horario.Add(horario);
                db.SaveChanges(); // Se guardan los datos de horario               
                db.asesorias.Add(asesoria);
                db.SaveChanges(); // Se guardan los datos de asesoria
                return RedirectToAction("ConsultaAsesoria");
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana, horario.Dia);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "Nombre", asesoria.IdAsesor);
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
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "IdAsesor");
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

                TempData["MensajeExito"] = "La asesoría se modificó con éxito.";
                return RedirectToAction("ConsultaAsesoria");
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana, horario.Dia);
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "Nombre", asesoria.IdAsesor);
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
            //TempData["MensajeEliminacion"] = "La asesoría se eliminó correctamente.";
            MessageBox.Show("La asesoría se eliminó correctamente.", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Agrega una línea de registro
            System.Diagnostics.Debug.WriteLine("La asesoría se eliminó correctamente.");
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

        #region AlumnoAsesor
        public ActionResult ConsultaAlumoAsesor() // Se obtiene la lista de los alumnos asesores registrados en el sistema y las conexiones que hay entre las tablas
        {
            var usuarios = db.usuarios.Include(a => a.alumno).Include(a => a.cat_tipo_usuario).Include(a => a.cat_programa_educativo).Include(a => a.cat_estatus);

            return View(usuarios.ToList());
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

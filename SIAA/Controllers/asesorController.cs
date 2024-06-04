using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using SIAA;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using SIAA.Models;

namespace SIAA.Controllers
{
    public class asesorController : Controller
    {
        private siaaEntities db = new siaaEntities();
        asesor asesor1 = System.Web.HttpContext.Current.Session["LOGIN"] as asesor;

        #region asesor
        public ActionResult ConsultaAsesor() // Se obtiene la lista de los asesores registrados en el sistema y las conexiones que hay entre las tablas
        {

            var asesors = db.asesors.Include(a => a.usuario).Include(a => a.asesorias);
            var usuarios = db.usuarios;            
            return View(asesors.ToList());

        }

        public ActionResult Details(int? id) // Muestra los detalles
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asesor asesor = db.asesors.Find(id);
            if (asesor == null)
            {
                return HttpNotFound();
            }
            return View(asesor);
        }

        public ActionResult RegistraAsesor() // Se inicializan las varibles de desplazamiento
        {
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion");
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo");
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario");
            return View();
        }

        private bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }


        public bool Validar(asesor asesor) //Se validan los datos antes de realizar el registro
        {
            var use = db.asesors.ToList();
            for (int i = 0; i < use.Count(); i++)
            {
                if (asesor.IdAsesor == use[i].IdAsesor)
                {
                    return false;
                }
            }
            return true;
        }


        public ActionResult ModificaAsesor(int? id) // Valida antes de modificar
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asesor asesor = db.asesors.Find(id);
            if (asesor == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", asesor.usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", asesor.usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", asesor.usuario.IdUsuario);
            return View(asesor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaAsesor([Bind(Include = "IdAsesor,Nombre,ApellidoPaterno,ApellidoMaterno,Correo,IdProgramaEducativo,IdEstatus,IdTipo")] asesor asesor) // Se modifican los registros de asesores
        {
            if (asesor.usuario.Nombre == null || asesor.usuario.ApellidoMaterno == null || asesor.usuario.ApellidoPaterno == null || asesor.usuario.Correo == null || asesor.usuario.IdProgramaEducativo == 0 || asesor.usuario.IdEstatus == null)
            {
                MessageBox.Show("Ingrese todos los datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("ModificaAsesor", "asesor");
            }
            {
                DialogResult result1 = MessageBox.Show("Desea modificar el registro?", "Modificar Asesor", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                if (result1 == DialogResult.Yes)
                {

                    if (ModelState.IsValid)
                    {
                        asesor.usuario.Nombre = asesor.usuario.Nombre.ToUpper();
                        asesor.usuario.ApellidoPaterno = asesor.usuario.ApellidoPaterno.ToUpper();
                        asesor.usuario.ApellidoMaterno = asesor.usuario.ApellidoMaterno.ToUpper();
                        db.Entry(asesor).State = EntityState.Modified;
                        db.SaveChanges();
                        MessageBox.Show("La modificación se ha guardado con éxito", "Modificación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return RedirectToAction("ConsultaAsesor");
                    }
                }
                else
                    return RedirectToAction("ModificaAsesor");
                ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", asesor.usuario.IdEstatus);
                ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", asesor.usuario.IdProgramaEducativo);
                ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", asesor.usuario.IdUsuario);
                return View(asesor);
            }

        }


        public ActionResult EliminarAsesor(int? id) // Elimina registros de asesores
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asesor asesor = db.asesors.Find(id);
            if (asesor == null)
            {
                return HttpNotFound();
            }
            DialogResult result1 = MessageBox.Show("Desea eliminar el registro?", "Eliminar Asesor", MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);
            if (result1 == DialogResult.Yes)
            {

                db.asesors.Remove(asesor);
                db.SaveChanges();
                MessageBox.Show("El registro se ha eliminado con éxito", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return RedirectToAction("ConsultaAsesor");
            }
            else
                return RedirectToAction("ConsultaAsesor");
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

        
        public ActionResult BuscarPorNombre(string nombre) // Busqueda por nombre
        {
            var resultados = db.asesors.Where(a => a.usuario.Nombre.Contains(nombre)).ToList();
            return PartialView("_ResultadosBusqueda", resultados);
        }

        #region asesoria

        public ActionResult ConsultaAsesoria()// Se obtiene la lista de las asesorias registradas en el sistema y las conexiones que hay entre las tablas
        {
            var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Include(a => a.cat_lugar).Where(a => a.IdAsesor == asesor1.IdAsesor); ;
            var asesoriasOrdenadas = asesorias.OrderBy(a => a.cat_unidad_aprendizaje.NombreUnidadAprendizaje).ToList();
            // Pasa la lista ordenada a la vista
            return View(asesoriasOrdenadas);
        }


        public ActionResult RegistroAsesoria(String nombre) // Se inicializan las varibles de desplazamiento
        {
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(semana);            
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
            asesoria.IdAsesor = asesor1.IdAsesor;
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
            asesoria.IdAsesor = asesor1.IdAsesor;
            asesoria.IdLugar = viewModel.IdLugar;
            asesoria.IdUnidadAprendizaje = viewModel.IdUnidadAprendizaje;
            Debug.WriteLine(viewModel.Asesoria.IdAsesor);
            Debug.WriteLine(viewModel.Asesoria.IdUnidadAprendizaje);
            if (ModelState.IsValid)
            {
                /*fecha = System.DateTime.Now;
                if (fecha.Month / 2 >= 6)
                {
                    viewModel.Asesoria.CicloEscolar = fecha.Year.ToString() + "-2";
                }
                else
                {
                    asesoria.CicloEscolar = fecha.Year.ToString() + "-1";
                */
                db.Entry(viewModel.Horario).State = EntityState.Modified;
                db.Entry(viewModel.Asesoria).State = EntityState.Modified;
                db.SaveChanges();

                TempData["MensajeExito"] = "La asesoría se modificó con éxito.";
                return RedirectToAction("ConsultaAsesoria");
            }
            List<string> semana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            ViewBag.dia = new SelectList(horario.Dia);           
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje", asesoria.IdUnidadAprendizaje);
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion", asesoria.IdLugar);
            return View(viewModel.Asesoria);
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

        #endregion

        #region solicitud
        public ActionResult SolicitudesAsesoria() // se llama a la vista de solicitudes
        {
            var solicitudes = db.solicituds.Include(a => a.alumno).Include(a => a.asesoria).Where(a => a.IdEstado == 1 && a.asesoria.IdAsesor==asesor1.IdAsesor);           
            return View(solicitudes.ToList());
        }

        ///// Funciones para el componente de solicitudes

        public ActionResult AprobarSolicitud(int idSolicitud)
        {
            solicitud solicitudAprobada = db.solicituds.Find(idSolicitud);
            solicitudAprobada.IdEstado = 2;
            solicitudAprobada.FechaAceptacion = System.DateTime.Now;
            db.Entry(solicitudAprobada).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SolicitudesAsesoria");
        }

        public ActionResult RechazarSolicitud(int idSolicitud)
        {
            solicitud solicitudAprobada = db.solicituds.Find(idSolicitud);
            solicitudAprobada.IdEstado = 3;
            db.Entry(solicitudAprobada).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SolicitudesAsesoria");
        }

        #endregion

        #region reportes
        ///// Funciones para el componente de reportes

        public ActionResult ConsultaReporte() // Se obtiene la lista de los reportes registrados en el sistema y las conexiones que hay entre las tablas
        {

            var reportes = db.reportes.Include(a => a.cat_temas).Include(a => a.asesoria);
            return View(reportes.ToList());
        }


        public ActionResult ReporteAsesor() // Se llama la vista reporte de asesor donde se registran los reportes
        {
            ViewBag.IdAsesoria = new SelectList(db.asesorias, "IdAsesoria", "CicloEscolar");
            ViewBag.IdTemaVisto = new SelectList(db.cat_temas, "IdTemas", "NombreTema");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteAsesor([Bind(Include = "IdReporte, IdAsesoria, Fecha, Duracion, IdTemaVisto, Comentarios")] reporte reporte) // Se guarda el registro del reporte
        {
            if (ModelState.IsValid)
            {
                db.reportes.Add(reporte);
                db.SaveChanges();

                return RedirectToAction("ReporteAsesor");
            }
        
            // En caso de validación fallida, devolver la vista con el modelo y mostrar los errores
            ViewBag.IdAsesoria = new SelectList(db.asesorias, "IdAsesoria", "CicloEscolar", reporte.IdAsesoria);
            ViewBag.IdTemaVisto = new SelectList(db.cat_temas, "IdTemas", "NombreTema", reporte.IdTemaVisto);
            return View(reporte);
        }

        #endregion
    }
}
    


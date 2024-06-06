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

        public ActionResult BuscarPorNombre(string nombre) // Busqueda por nombre
        {
            var resultados = db.asesors.Where(a => a.usuario.Nombre.Contains(nombre)).ToList();
            return PartialView("_ResultadosBusqueda", resultados);
        }

        #region asesoria
        
        public ActionResult ConsultaAsesoria(int pagina = 1, int registros = 9, string searchString = "")// Se obtiene la lista de las asesorias registradas en el sistema y las conexiones que hay entre las tablas
        {
            //var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Include(a => a.cat_lugar).Where(a => a.IdAsesor == asesor1.IdAsesor); 
            //var asesoriasOrdenadas = asesorias.OrderBy(a => a.cat_unidad_aprendizaje.NombreUnidadAprendizaje).ToList();
            // Pasa la lista ordenada a la vista
            //return View(asesoriasOrdenadas);
            var unidadesAprendizaje = db.cat_unidad_aprendizaje.ToList();
            ViewBag.UnidadesAprendizaje = unidadesAprendizaje;

            var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Include(a => a.cat_lugar).Where(a => a.IdAsesor == asesor1.IdAsesor);

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
            var ultimosRegistros = db.asesorias.Where(a => a.IdAsesor == asesor1.IdAsesor).OrderByDescending(a => a.IdAsesoria).Take(2).ToList();
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
                TempData["MsgRegistroExitoso"] = "La asesoria se registró exitosamente.";
                return RedirectToAction("RegistroAsesoria");                
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

                TempData["MsgModificacionExitosa"] = "La asesoría se modificó con éxito.";
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
            TempData["MsgEliminacionExitosa"] = "La asesoria se eliminó exitosamente.";
            return RedirectToAction("ConsultaAsesoria");


        }

        #endregion

        #region solicitud
        public ActionResult SolicitudesAsesoria() // se llama a la vista de solicitudes
        {
            var solicitudes = db.solicituds.Include(a => a.alumno).Include(a => a.asesoria).Where(a => a.IdEstado == 1 && a.asesoria.IdAsesor==asesor1.IdAsesor);           
            return View(solicitudes.ToList());
        }

        public ActionResult AprobarSolicitud(int idSolicitud)
        {
            solicitud solicitudAprobada = db.solicituds.Find(idSolicitud);
            solicitudAprobada.IdEstado = 2;
            solicitudAprobada.FechaAceptacion = System.DateTime.Now;
            db.Entry(solicitudAprobada).State = EntityState.Modified;
            db.SaveChanges();
            TempData["MsgAprobacionExitosa"] = "La asesoria ha sido aprobada exitosamente.";
            return RedirectToAction("SolicitudesAsesoria");
        }

        public ActionResult RechazarSolicitud(int idSolicitud)
        {
            solicitud solicitudAprobada = db.solicituds.Find(idSolicitud);
            solicitudAprobada.IdEstado = 3;
            db.Entry(solicitudAprobada).State = EntityState.Modified;
            db.SaveChanges();
            TempData["MsgRechazoExitoso"] = "La asesoria ha sido rechazada exitosamente.";
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
            ViewBag.IdAsesoria = new SelectList(db.asesorias, "IdAsesoria", "cat_Unidad_Aprendizaje.NombreUnidadAprendizaje");
            ViewBag.IdTemaVisto = new SelectList(db.cat_temas, "IdTemas", "NombreTema");            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteAsesor([Bind(Include = "IdReporte, IdAsesoria, Fecha, Duracion, IdTemaVisto, Comentarios")] reporte reporte) // Se guarda el registro del reporte
        {

            var alumnosInscritos = db.solicituds.Where(s => s.IdAsesoria == 2).Select(s => s.alumno).ToList();

            if (db.reportes.Any())
            {
                reporte.IdReporte = db.reportes.ToList().Last().IdReporte + 1;
            }
            else { reporte.IdReporte = 1; }

            if (ModelState.IsValid)
            {
                db.reportes.Add(reporte);
                db.SaveChanges();
                TempData["MsgRegistroReporteExitoso"] = "El reporte se registró exitosamente.";                
                return RedirectToAction("ReporteAsesor");
            }
        
            // En caso de validación fallida, devolver la vista con el modelo y mostrar los errores
            ViewBag.IdAsesoria = new SelectList(db.asesorias, "IdAsesoria", "cat_Unidad_Aprendizaje.NombreUnidadAprendizaje", reporte.IdAsesoria);
            ViewBag.IdTemaVisto = new SelectList(db.cat_temas, "IdTemas", "NombreTema", reporte.IdTemaVisto);
            return View(reporte);
        }

        #endregion
    }
}
    


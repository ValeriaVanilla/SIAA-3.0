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
    public class asesoriaController : Controller
    {
        private siaaEntities db = new siaaEntities();


        public ActionResult ConsultaAsesoria(int pagina = 1, int registros = 9, string searchString = "")
        {

            var unidadesAprendizaje = db.cat_unidad_aprendizaje.ToList();
            ViewBag.UnidadesAprendizaje = unidadesAprendizaje;

            var asesorias = db.asesorias
                .Include(a => a.asesor)
                .Include(a => a.cat_unidad_aprendizaje)
                .Include(a => a.cat_lugar);

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


        public ActionResult RegistroAsesoria(String nombre) // Se inicializan las varibles de desplazamiento
        {

            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "IdAsesor");
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje");
            ViewBag.IdLugar = new SelectList(db.cat_lugar, "IdLugar", "Descripcion");
            var ultimosRegistros = db.asesorias.OrderByDescending(a => a.IdAsesoria).Take(2).ToList();
            ViewBag.UltimosRegistros = ultimosRegistros;
            return View();
        }



        [HttpPost]  // Guarda los registros
        [ValidateAntiForgeryToken]
        public ActionResult RegistroAsesoria( AsesoriaHorarioViewModel viewModel) // Toma el Modela AsesoriaHorarioViewModel para hacer el registro en las tablas horario y asesoria
        {
            DateTime fecha;
            var horario = viewModel.Horario;
            var asesoria = viewModel.Asesoria;            
            asesoria.IdHorario = viewModel.Horario.IdHorario;
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
                db.SaveChanges(); // Se guaradn los datos de horario               
                db.asesorias.Add(asesoria);
                db.SaveChanges(); // Se guardan los datos de asesoria
                return RedirectToAction("ConsultaAsesoria");
            }

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
            asesoria asesoria = db.asesorias.Find(id);
            if (asesoria == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "Nombre", asesoria.IdAsesor);
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje", asesoria.IdUnidadAprendizaje);
            return View(asesoria);
        }

        [HttpPost] // Se modifican los registros de alumnos
        [ValidateAntiForgeryToken]
        public ActionResult ModificaAsesoria([Bind(Include = "IdAsesoria,IdAsesor,IdUnidadAprendizaje,Lugar,Horario,Cicloescolar")] asesoria asesoria)
        {
            DateTime fecha;
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
                db.Entry(asesoria).State = EntityState.Modified;
                db.SaveChanges();

                TempData["MensajeExito"] = "La asesoría se modificó con éxito.";
                return RedirectToAction("ConsultaAsesoria");
            }
            ViewBag.IdAsesor = new SelectList(db.asesors, "IdAsesor", "Nombre", asesoria.IdAsesor);
            ViewBag.IdUnidadAprendizaje = new SelectList(db.cat_unidad_aprendizaje, "IdUnidadAprendizaje", "NombreUnidadAprendizaje", asesoria.IdUnidadAprendizaje);
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
            if (asesoria == null)
            {
                return HttpNotFound();
            }
            db.asesorias.Remove(asesoria);
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
    }
}


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

namespace SIAA.Controllers
{
    public class asesorController : Controller
    {
        private siaaEntities db = new siaaEntities();

        public ActionResult ConsultaAsesor() // Se obtiene la lista de los asesores registrados en el sistema y las conexiones que hay entre las tablas
        {

            var asesors = db.asesors.Include(a => a.usuario).Include(a => a.asesorias);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistraAsesor([Bind(Include = "IdAsesor,Nombre,ApellidoPaterno,ApellidoMaterno,Correo,IdProgramaEducativo,IdEstatus,IdTipo")] asesor asesor)
        {
            if (asesor.IdAsesor < 0)
            {
                ModelState.AddModelError("IdAsesor", "La matricula no puede ser negativa.");
            }
            else if (!IsNumeric(asesor.IdAsesor.ToString()))
            {
                ModelState.AddModelError("IdAsesor", "Ingrese solo números positivos en el campo de Matrícula.");
            }
            else if (!Validar(asesor))
            {
                ModelState.AddModelError("IdAsesor", "Matricula repetida.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    asesor.usuario.Nombre = asesor.usuario.Nombre.ToUpper();
                    asesor.usuario.ApellidoPaterno = asesor.usuario.ApellidoPaterno.ToUpper();
                    asesor.usuario.ApellidoMaterno = asesor.usuario.ApellidoMaterno.ToUpper();
                    asesor.usuario.cat_tipo_usuario.IdTipoUsuario = 2;
                    db.asesors.Add(asesor);
                    db.SaveChanges();

                    return RedirectToAction("RegistraAsesor");
                }
            }

            // En caso de validación fallida, devolver la vista con el modelo y mostrar los errores
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", asesor.usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", asesor.usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", asesor.usuario.IdUsuario);
            return View(asesor);
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
            if (asesor.usuario.Nombre == null || asesor.usuario.ApellidoMaterno == null || asesor.usuario.ApellidoPaterno == null || asesor.Correo == null || asesor.usuario.IdProgramaEducativo == 0 || asesor.usuario.IdEstatus == null)
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

        public ActionResult BuscarPorNombre(string nombre) // Busqueda por nombre
        {
            var resultados = db.asesors.Where(a => a.usuario.Nombre.Contains(nombre)).ToList();
            return PartialView("_ResultadosBusqueda", resultados);
        }

        public ActionResult SolicitudesAsesoria() // se llama a la vista de solicitudes
        {
            var solicitudes = db.solicituds.Include(a => a.alumno).Include(a => a.asesoria);
            return View(solicitudes.ToList());
        }
        public ActionResult AprobarSolicitud(int idAsesoria, int idAlumno)
        {

            return RedirectToAction("SolicitudesAsesoria");
        }
    }
}

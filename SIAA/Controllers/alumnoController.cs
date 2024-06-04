using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using SIAA;

namespace SIAA.Controllers
{
    public class alumnoController : Controller
    {
        private siaaEntities db = new siaaEntities();
        alumno alumno1 = System.Web.HttpContext.Current.Session["LOGIN"] as alumno;

        public ActionResult ConsultaAlumno() // Se obtiene la lista de los alumnos registrados en el sistema y las conexiones que hay entre las tablas
        {
            var alumnoes = db.alumnoes.Include(a => a.usuario).Include(a => a.solicituds).Include(a => a.asistencias);        
            return View(alumnoes.ToList());
         
        }

        public ActionResult RegistroAlumno() // Se inicializan las varibles de desplazamiento
        {
            ViewBag.IdAlumno = new SelectList(db.solicituds, "IdAlumno", "Duracion");
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion");
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo");
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario");
            return View();
        }

        public ActionResult RegistraAlumnoAsesor() // Se inicializan las varibles de desplazamiento
        {
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario");
            return View();
        }

        public ActionResult ModificaAlumno(int? id) // Valida antes de modificar
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
            ViewBag.IdAlumno = new SelectList(db.solicituds, "IdAlumno", "Duracion", alumno.IdAlumno);
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", alumno.usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", alumno.usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", alumno.usuario.IdTipoUsuario);
            return View(alumno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaAlumno([Bind(Include = "IdAlumno, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, IdEstatus, IdProgramaEducativo, IdTipo")] alumno alumno) // Se modifican los registros de alumnos
        {
            if (alumno.usuario.Nombre == null || alumno.usuario.ApellidoMaterno == null || alumno.usuario.ApellidoPaterno == null || alumno.usuario.Correo == null || alumno.usuario.IdProgramaEducativo == 0 || alumno.usuario.IdEstatus == null)
            {
                MessageBox.Show("Ingrese todos los datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("ModificaAlumno", "alumno");
            }
            {
                DialogResult result1 = MessageBox.Show("Desea modificar el registro?", "Modificar Alumno", MessageBoxButtons.YesNo,
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
                    return RedirectToAction("ModificaAlumno");

                ViewBag.IdAlumno = new SelectList(db.solicituds, "IdAlumno", "Duracion", alumno.IdAlumno);
                ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", alumno.usuario.IdEstatus);
                ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", alumno.usuario.IdProgramaEducativo);
                ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", alumno.usuario.IdTipoUsuario);
                
                return View(alumno);
            }

        }

        public ActionResult EliminaAlumno(int? id) // Elimina registros de alumnos
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
            DialogResult result1 = MessageBox.Show("Seguro que quieres borrar ese dato", "Borrar Alumno", MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);
            if (result1 == DialogResult.Yes)
            {

                db.alumnoes.Remove(alumno);
                db.SaveChanges();
                return RedirectToAction("ConsultaAlumno");
            }
            else

                return RedirectToAction("ConsultaAlumno");
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
            var resultados = db.alumnoes.Where(a => a.usuario.Nombre.Contains(nombre)).ToList();
            return PartialView("_ResultadosBusqueda", resultados);
        }
        #region Solicitud

        public ActionResult ConsultaAsesoria()// Se obtiene la lista de las asesorias registradas en el sistema y las conexiones que hay entre las tablas
        {
            var asesorias = db.solicituds.Where(s => s.IdAlumno == alumno1.IdAlumno && s.IdEstado == 2).Select(s => s.asesoria).Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje);
            var asesoriasOrdenadas = asesorias.OrderBy(a => a.cat_unidad_aprendizaje.NombreUnidadAprendizaje).ToList();
            // Pasa la lista ordenada a la vista
            return View(asesoriasOrdenadas);
        }

        public ActionResult SolicitudesAlumno() // Inicializa la consulta de horarios
        {
            var AsesoriasActualizada = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Where(a => !db.solicituds.Any(s => s.IdAsesoria == a.IdAsesoria && s.IdAlumno == alumno1.IdAlumno));
            return View(AsesoriasActualizada.ToList());
        }

        public ActionResult EstadoSolicitudesAlumno() // Inicializa la consulta de horarios
        {
            var solicitudes = db.solicituds.Where(s => s.IdAlumno == alumno1.IdAlumno).Include(a => a.alumno).Include(a => a.asesoria);            
            return View(solicitudes.ToList());
        }

        public ActionResult Solicitar(int id)  // Apartado para el funcionamiento de las solicitudes del alumno
        {
            asesoria asesoria = db.asesorias.Find(id);
            solicitud nuevaSolicitud = new solicitud();
            if (db.solicituds.Any()) {                
                nuevaSolicitud.Idsolicitud = db.solicituds.ToList().Last().Idsolicitud + 1;
            }
            else { nuevaSolicitud.Idsolicitud = 1; }
            nuevaSolicitud.IdAsesoria = id;
            nuevaSolicitud.IdAlumno = alumno1.IdAlumno;
            nuevaSolicitud.FechaSolicitud = System.DateTime.Now;
            nuevaSolicitud.IdEstado = 1;
            db.solicituds.Add(nuevaSolicitud);
            db.SaveChanges();
            MessageBox.Show("La solicitud se ha enviado exitosamente", "Solicitud exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var asesorias = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje);
            return RedirectToAction("EstadoSolicitudesAlumno");
        }

        #endregion
       
        #region Evaluaciones
        public ActionResult EvaluacionAlumno() // Inicializa la consulta de horarios
        {
            return View();
        }
        #endregion

    }
}

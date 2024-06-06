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
using Microsoft.Win32;
using SIAA;

namespace SIAA.Controllers
{
    public class alumnoController : Controller
    {
        private siaaEntities db = new siaaEntities();
        alumno alumno1 = System.Web.HttpContext.Current.Session["LOGIN"] as alumno;
        
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
            int pagina = 1, registros = 4;
            //var AsesoriasActualizada = db.asesorias.Include(a => a.asesor).Include(a => a.cat_unidad_aprendizaje).Where(a => !db.solicituds.Any(s => s.IdAsesoria == a.IdAsesoria && s.IdAlumno == alumno1.IdAlumno));
            //return View(AsesoriasActualizada.ToList());

            var asesorias = db.asesorias.Include(a => a.asesor.usuario).Include(a => a.cat_unidad_aprendizaje).Where(a => !db.solicituds.Any(s => s.IdAsesoria == a.IdAsesoria && s.IdAlumno == alumno1.IdAlumno)).OrderBy(a => a.IdAsesoria);

            var totalAsesorias = asesorias.Count();
            var asesoriasPaginadas = asesorias.Skip((pagina - 1) * registros).Take(registros).ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalAsesorias / registros);
            ViewBag.RegistrosPorPagina = registros;

            return View(asesoriasPaginadas);

        }

        public ActionResult EstadoSolicitudesAlumno() // Inicializa la consulta de horarios
        {
            var solicitudes = db.solicituds.Where(s => s.IdAlumno == alumno1.IdAlumno).Include(a => a.alumno).Include(a => a.asesoria);            
            return View(solicitudes.ToList());
        }

        public ActionResult Solicitar(int id)  // Apartado para el funcionamiento de las solicitudes del alumno
        {            
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
            TempData["MsgRegistroExitoso"] = "La solicitud se registro exitosamente.";            
            return RedirectToAction("SolicitudesAlumno");
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

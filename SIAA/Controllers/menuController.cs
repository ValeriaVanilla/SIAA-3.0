using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAA.Controllers
{
    public class menuController : Controller
    {
        private siaaEntities db = new siaaEntities();
        // Se llama al menu principal del encargado
        public ActionResult MenuEncargado()
        {
            encargado encargado1 = System.Web.HttpContext.Current.Session["LOGIN"] as encargado;
            return View();
        }

        // Se llama al menu principal del asesor
        public ActionResult MenuAsesor()
        {
            asesor asesor1 = System.Web.HttpContext.Current.Session["LOGIN"] as asesor;
            return View(asesor1);
        }

        // Se llama al menu principal del alumno
        public ActionResult MenuAlumno()
        {
            alumno alumno1 = System.Web.HttpContext.Current.Session["LOGIN"] as alumno;
            return View();
        }



    }
}
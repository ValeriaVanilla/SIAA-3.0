using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAA.Controllers
{
    public class menuController : Controller
    {
        // Se llama al menu principal del encargado
        public ActionResult MenuEncargado()
        {
            return View();
        }

        // Se llama al menu principal del asesor
        public ActionResult MenuAsesor()
        {
            return View();
        }

        // Se llama al menu principal del alumno
        public ActionResult MenuAlumno()
        {
            return View();
        }



    }
}
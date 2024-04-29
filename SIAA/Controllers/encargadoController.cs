using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAA;

namespace SIAA.Controllers
{
    public class encargadoController : Controller
    {
        private siaaEntities db = new siaaEntities();

        public class reportesController : Controller
        {
            // GET: encargado
            public ActionResult ReporteEncargado()
            {
                return View();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace SIAA.Controllers
{
    public class accesoController : Controller
    {
        private siaaEntities db = new siaaEntities();

        // GET: acceso
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Login", "acceso");
        }

        [HttpPost]
        public ActionResult Login(string correo, string cont)
        {
            Debug.WriteLine(correo);
            Debug.WriteLine(cont);

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(cont))
            {
                MessageBox.Show("Ingresa todos los datos", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return RedirectToAction("Login", "Acceso");
            }
            try
            {


                usuario usuario = (from o in db.usuarios where o.Correo == correo && o.Contrasena == cont select o).ToList().ElementAt(0);

                if (usuario.IdTipoUsuario== 1)
                {
                    alumno alumno = db.alumnoes.Find(usuario.IdUsuario);
                    System.Web.HttpContext.Current.Session["LOGIN"] = alumno;
                    string msg = "Bienvenido " + alumno.usuario.Nombre + " " + alumno.usuario.ApellidoPaterno + " " + alumno.usuario.ApellidoMaterno;
                    MessageBox.Show(msg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return RedirectToAction("MenuAlumno", "menu");

                }
                else if (usuario.IdTipoUsuario == 2)
                {
                    asesor asesor = db.asesors.Find(usuario.IdUsuario);
                    System.Web.HttpContext.Current.Session["LOGIN"] = asesor;
                    string msg = "Bienvenido " + asesor.usuario.Nombre + " " + asesor.usuario.ApellidoPaterno + " " + asesor.usuario.ApellidoMaterno;
                    MessageBox.Show(msg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return RedirectToAction("MenuAsesor", "menu");
                }
                else if (usuario.IdTipoUsuario == 3)
                {
                    encargado encargado = db.encargadoes.Find(usuario.IdUsuario);
                    System.Web.HttpContext.Current.Session["LOGIN"] = encargado;
                    MessageBox.Show("Bienvenido Encargado", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return RedirectToAction("MenuEncargado", "menu");
                }
                if (usuario.IdTipoUsuario == 4)
                {
                    alumno alumno = db.alumnoes.Find(usuario.IdUsuario);
                    System.Web.HttpContext.Current.Session["LOGIN"] = alumno;
                    string msg = "Bienvenido " + alumno.usuario.Nombre + " " + alumno.usuario.ApellidoPaterno + " " + alumno.usuario.ApellidoMaterno;
                    MessageBox.Show(msg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return RedirectToAction("MenuAlumno", "menu");
                }


                else
                {
                    MessageBox.Show("El usuario o contraseña son incorrectos", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return RedirectToAction("Login", "Acceso");


                }



            }
            catch (SqlException s)
            {
                String linea = "" + s.ErrorCode;
                MessageBox.Show(linea, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;

            }
            catch (Exception ex)
            {
                MessageBox.Show("El usuario o contraseña son incorrectos", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return RedirectToAction("Login", "Acceso");

            }


        }
    }
}
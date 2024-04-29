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
    public class usuarioController : Controller
    {
        private siaaEntities db = new siaaEntities();

        // GET: usuario
        public ActionResult Index()
        {
            var usuarios = db.usuarios.Include(u => u.alumno).Include(u => u.asesor).Include(u => u.cat_estatus).Include(u => u.cat_programa_educativo).Include(u => u.cat_tipo_usuario).Include(u => u.encargado);
            return View(usuarios.ToList());
        }

        // GET: usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: usuario/Create
        public ActionResult Create()
        {
            ViewBag.IdUsuario = new SelectList(db.alumnoes, "IdAlumno", "Correo");
            ViewBag.IdUsuario = new SelectList(db.asesors, "IdAsesor", "Correo");
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion");
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo");
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario");
            ViewBag.IdUsuario = new SelectList(db.encargadoes, "IdEncargado", "Correo");
            return View();
        }

        // POST: usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUsuario,Nombre,ApellidoPaterno,ApellidoMaterno,Contrasena,IdProgramaEducativo,IdTipoUsuario,IdEstatus")] usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUsuario = new SelectList(db.alumnoes, "IdAlumno", "Correo", usuario.IdUsuario);
            ViewBag.IdUsuario = new SelectList(db.asesors, "IdAsesor", "Correo", usuario.IdUsuario);
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", usuario.IdTipoUsuario);
            ViewBag.IdUsuario = new SelectList(db.encargadoes, "IdEncargado", "Correo", usuario.IdUsuario);
            return View(usuario);
        }

        // GET: usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuario = new SelectList(db.alumnoes, "IdAlumno", "Correo", usuario.IdUsuario);
            ViewBag.IdUsuario = new SelectList(db.asesors, "IdAsesor", "Correo", usuario.IdUsuario);
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", usuario.IdTipoUsuario);
            ViewBag.IdUsuario = new SelectList(db.encargadoes, "IdEncargado", "Correo", usuario.IdUsuario);
            return View(usuario);
        }

        // POST: usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUsuario,Nombre,ApellidoPaterno,ApellidoMaterno,Contrasena,IdProgramaEducativo,IdTipoUsuario,IdEstatus")] usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuario = new SelectList(db.alumnoes, "IdAlumno", "Correo", usuario.IdUsuario);
            ViewBag.IdUsuario = new SelectList(db.asesors, "IdAsesor", "Correo", usuario.IdUsuario);
            ViewBag.IdEstatus = new SelectList(db.cat_estatus, "IdEstatus", "Descripcion", usuario.IdEstatus);
            ViewBag.IdProgramaEducativo = new SelectList(db.cat_programa_educativo, "IdProgramaEducativo", "NombreProgramaEducativo", usuario.IdProgramaEducativo);
            ViewBag.IdTipoUsuario = new SelectList(db.cat_tipo_usuario, "IdTipoUsuario", "NombreUsuario", usuario.IdTipoUsuario);
            ViewBag.IdUsuario = new SelectList(db.encargadoes, "IdEncargado", "Correo", usuario.IdUsuario);
            return View(usuario);
        }

        // GET: usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            usuario usuario = db.usuarios.Find(id);
            db.usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
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

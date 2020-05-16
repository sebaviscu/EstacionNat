using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBlog.Core.Database;
using MVCBlog.Core.Entities;

namespace MVCBlog.Website.Controllers
{
    public class TipoProductoesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: TipoProductoes
        public ActionResult Index()
        {
            return View(db.TipoProductoes.ToList());
        }

        // GET: TipoProductoes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProducto tipoProducto = db.TipoProductoes.Find(id);
            if (tipoProducto == null)
            {
                return HttpNotFound();
            }
            return View(tipoProducto);
        }

        // GET: TipoProductoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoProductoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Created,Modified")] TipoProducto tipoProducto)
        {
            if (ModelState.IsValid)
            {
                tipoProducto.Id = Guid.NewGuid();
                db.TipoProductoes.Add(tipoProducto);
                tipoProducto.Created = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoProducto);
        }

        // GET: TipoProductoes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProducto tipoProducto = db.TipoProductoes.Find(id);
            if (tipoProducto == null)
            {
                return HttpNotFound();
            }
            return View(tipoProducto);
        }

        // POST: TipoProductoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Created,Modified")] TipoProducto tipoProducto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoProducto).State = EntityState.Modified;
                tipoProducto.Modified = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoProducto);
        }

        // GET: TipoProductoes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProducto tipoProducto = db.TipoProductoes.Find(id);
            if (tipoProducto == null)
            {
                return HttpNotFound();
            }
            return View(tipoProducto);
        }

        // POST: TipoProductoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TipoProducto tipoProducto = db.TipoProductoes.Find(id);
            db.TipoProductoes.Remove(tipoProducto);
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

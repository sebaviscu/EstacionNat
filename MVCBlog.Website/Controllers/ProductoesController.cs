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
    public class ProductoesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Productoes
        public ActionResult Index()
        {
            var productoes = db.Productoes.Include(p => p.TipoProducto);
            return View(productoes.ToList());
        }

        // GET: Productoes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Include(p => p.TipoProducto).FirstOrDefault(_=>_.Id ==id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        public ActionResult Create()
        {
            ViewBag.TipoProductoId = new SelectList(db.TipoProductoes, "Id", "Description");
            return View();
        }

        // POST: Productoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Estado,Unidad,Photo,TipoProductoId,Created,Modified")] Producto producto, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                #region Photo

                if (image != null && image.ContentLength > 0 && image.ContentLength < 155000)
                {
                    var content = new byte[image.ContentLength];
                    image.InputStream.Read(content, 0, image.ContentLength);
                    int indexOfLastDot = image.FileName.LastIndexOf('.');
                    string extension = image.FileName.Substring(indexOfLastDot + 1, image.FileName.Length - indexOfLastDot - 1);

                    if (extension == "jpg" || extension == "jpeg")
                    {
                        #region New Photo
                        Random r = new Random();
                        string newPhoto = String.Format("{0}_{1}", producto.Description, r.Next(0, 99));
                        string newPath = System.IO.Path.Combine(producto.FullPath, String.Format("{0}.{1}", newPhoto, extension));
                        #endregion

                        #region Delete Old Photo
                        if (!String.IsNullOrEmpty(producto.Photo))
                        {
                            string oldPhoto = System.IO.Path.Combine(producto.FullPath, producto.Photo);
                            if (System.IO.File.Exists(oldPhoto))
                                System.IO.File.Delete(oldPhoto);
                        }
                        #endregion

                        producto.Photo = String.Format("{0}.{1}", newPhoto, extension);
                        image.SaveAs(newPath);
                    }
                }
                #endregion


                producto.Id = Guid.NewGuid();
                producto.Created = DateTime.Now;
                db.Productoes.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TipoProductoId = new SelectList(db.TipoProductoes, "Id", "Description", producto.TipoProductoId);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Include(p => p.TipoProducto).FirstOrDefault(_ => _.Id == id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            var precioActu = db.PreciosHistoricos.Where(_ => _.IdProducto == id).OrderByDescending(_ => _.FechaDesde).FirstOrDefault();

            producto.PrecioActual = precioActu != null ? precioActu.Precio : 0;

            ViewBag.TipoProductoId = new SelectList(db.TipoProductoes, "Id", "Description", producto.TipoProductoId);
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Estado,Unidad,Photo,TipoProductoId,Created,Modified")] Producto producto, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                producto.Description = producto.Description.Trim();
                #region Photo

                if (image != null && image.ContentLength > 0 && image.ContentLength < 155000)
                {
                    var content = new byte[image.ContentLength];
                    image.InputStream.Read(content, 0, image.ContentLength);
                    int indexOfLastDot = image.FileName.LastIndexOf('.');
                    string extension = image.FileName.Substring(indexOfLastDot + 1, image.FileName.Length - indexOfLastDot - 1);

                    if (extension == "jpg" || extension == "jpeg")
                    {
                        #region New Photo
                        Random r = new Random();
                        string newPhoto = String.Format("{0}_{1}", producto.Description, r.Next(0, 99));
                        string newPath = System.IO.Path.Combine(producto.FullPath, String.Format("{0}.{1}", newPhoto, extension));
                        #endregion

                        #region Delete Old Photo
                        if (!String.IsNullOrEmpty(producto.Photo))
                        {
                            string oldPhoto = System.IO.Path.Combine(producto.FullPath, producto.Photo);
                            if (System.IO.File.Exists(oldPhoto))
                                System.IO.File.Delete(oldPhoto);
                        }
                        #endregion

                        producto.Photo = String.Format("{0}.{1}", newPhoto, extension);
                        image.SaveAs(newPath);
                    }
                }
                #endregion


                db.Entry(producto).State = EntityState.Modified;
                producto.Modified = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoProductoId = new SelectList(db.TipoProductoes, "Id", "Description", producto.TipoProductoId);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Producto producto = db.Productoes.Find(id);
            db.Productoes.Remove(producto);
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

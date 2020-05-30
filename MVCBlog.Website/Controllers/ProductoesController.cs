using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBlog.Core.Database;
using MVCBlog.Core.Entities;
using MVCBlog.Website.Models.EstacionNatural;
using OfficeOpenXml;

namespace MVCBlog.Website.Controllers
{
    public class ProductoesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Productoes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Productoes/Details/5
        public ActionResult Details(Guid? id)
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

            var historicPrice = db.PreciosHistoricos.Where(_ => _.IdProducto == id).OrderByDescending(_ => _.FechaDesde).Take(10).ToList();
            producto.HistoricPrice = historicPrice;

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
        public ActionResult Create([Bind(Include = "Id,Description,Estado,Unidad,Photo,TipoProductoId, PrecioActual,Created,Modified")] Producto producto, HttpPostedFileBase image)
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

                producto.Description = producto.Description.Trim();
                producto.Id = Guid.NewGuid();
                producto.Created = DateTime.Now;
                db.Productoes.Add(producto);
                db.SaveChanges();

                if (producto.PrecioActual > 0m)
                {
                    var newPrice = new PrecioHistorico()
                    {
                        Created = DateTime.Now,
                        FechaDesde = DateTime.Now,
                        IdProducto = producto.Id,
                        Precio = producto.PrecioActual,
                        Id = Guid.NewGuid()
                    };
                    db.PreciosHistoricos.Add(newPrice);
                    db.SaveChanges();
                }

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

            producto.PrecioFuturo = 0;
            producto.Description = producto.Description.Trim();

            ViewBag.TipoProductoId = new SelectList(db.TipoProductoes, "Id", "Description", producto.TipoProductoId);
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Estado,Unidad,Photo,TipoProductoId,Created,Modified, PrecioActual, PrecioFuturo")] Producto producto, HttpPostedFileBase image)
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

                var historicPrice = db.PreciosHistoricos.Where(_ => _.IdProducto == producto.Id).OrderByDescending(_ => _.FechaDesde).FirstOrDefault();
                if (historicPrice != null && historicPrice.Precio != producto.PrecioActual)
                {
                    var newPrice = new PrecioHistorico()
                    {
                        Created = DateTime.Now,
                        FechaDesde = DateTime.Now,
                        IdProducto = producto.Id,
                        Precio = producto.PrecioFuturo,
                        Id = Guid.NewGuid()
                    };
                    db.PreciosHistoricos.Add(newPrice);
                }

                producto.Description = producto.Description.Trim();
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

        public ActionResult MassiveEdit()
        {
            var productos = db.Productoes.Include(p => p.TipoProducto);

            foreach (var p in productos)
            {
                p.Description = p.Description.Trim();
            }

            return View(productos);
        }

        [HttpPost]
        public ActionResult SaveMassiveEdit(List<Producto> productosList)
        {
            foreach (var p in productosList)
            {
                bool isUpdate = false;

                var prod = db.Productoes.FirstOrDefault(_ => _.Id == p.Id);

                if(p.PrecioActual != prod.PrecioActual)
                {
                    prod.PrecioActual = p.PrecioActual;
                    var newPrice = new PrecioHistorico()
                    {
                        Created = DateTime.Now,
                        FechaDesde = DateTime.Now,
                        IdProducto = prod.Id,
                        Precio = prod.PrecioActual,
                        Id = Guid.NewGuid()
                    };
                    db.PreciosHistoricos.Add(newPrice);
                    isUpdate = true;
                }

                if (p.Description != prod.Description)
                {
                    prod.Description = p.Description;
                    isUpdate = true;
                }

                if (p.Unidad != prod.Unidad)
                {
                    prod.Unidad = p.Unidad;
                    isUpdate = true;
                }

                if (p.Estado != prod.Estado)
                {
                    prod.Estado = p.Estado;
                    isUpdate = true;
                }

                if(isUpdate)
                {
                    db.Entry(prod).State = EntityState.Modified;
                    prod.Modified = DateTime.Now;
                }

            }
            db.SaveChanges();

            return Json(new { redirectUrl = Url.Action("Index", "Productoes") });
        }

        public JsonResult GetProduct(Guid? id)
        {
            Producto producto = db.Productoes.Include(p => p.TipoProducto).FirstOrDefault(_ => _.Id == id);

            return Json(producto);
        }


        public ActionResult Search(FiltroProductos viewModel)
        {
            var productoes = GetProductsByFilter(viewModel);

            return this.PartialView("_list", productoes);
        }

        private IQueryable<Producto> GetProductsByFilter(FiltroProductos viewModel)
        {
            var productoes = db.Productoes.Include(p => p.TipoProducto);

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                productoes = productoes.Where(_ => _.Description.Contains(viewModel.Descripcion));

            if (viewModel.TipoProducto != null && viewModel.TipoProducto != Guid.Empty)
                productoes = productoes.Where(_ => _.TipoProductoId == viewModel.TipoProducto);

            return productoes.OrderBy(_ => _.Description);
        }

        readonly string descrFile = "Productos";

        [HttpPost]
        public virtual JsonResult Export(FiltroProductos viewModel)
        {
            var result = GetProductsByFilter(viewModel);
            string pathName = ConfigurationManager.AppSettings["ExportFilesPath"] + descrFile + ".xlsx";
            string serverMapPath = Server.MapPath("~/" + pathName);

            FileInfo file = new FileInfo(serverMapPath);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(serverMapPath);
            }

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add(descrFile);

                var j = 1;
                var cell = ws.Cells;

                cell[1, 1].Value = "Producto";
                cell[1, 2].Value = "Tipo";
                cell[1, 3].Value = "Estado";
                cell[1, 4].Value = "Precio Actual";
                cell[1, 5].Value = "Presentacion";
                cell[1, 6].Value = "Ultima Modificacion";
                var i = 2;
                foreach (var p in result)
                {
                    j = 1;
                    cell[i, j++].Value = p.Description.Trim();
                    cell[i, j++].Value = p.TipoProducto.Description;
                    cell[i, j++].Value = p.Estado;
                    cell[i, j++].Value = p.PrecioActual;
                    cell[i, j++].Value = p.Unidad;
                    cell[i, j++].Value = p.Modified.HasValue ? p.Modified.Value.ToShortDateString() : string.Empty;

                    i++;
                }
                package.Save();
            }

            return Json(new
            {
                result = serverMapPath,
                success = true
            });
        }

        public virtual FileResult Download(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            var fileDownloadName = descrFile + ".xlsx";
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, contentType, fileDownloadName);
        }
    }
}

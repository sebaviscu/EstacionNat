using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MVCBlog.Core.Database;
using MVCBlog.Core.Entities;

namespace MVCBlog.Website.Controllers
{
    public class PedidoesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.Pedidoes.Include(s => s.Usuario).ToList());
        }

        public ActionResult Details(Guid? id)
        {
            var pedido = new Pedido();
            if (User.Identity.IsAuthenticated)
            {
                var pedidoUser = db.Pedidoes.Include(s => s.Usuario).FirstOrDefault(_ => _.Id == id);
                if (pedidoUser != null)
                {
                    pedido = pedidoUser;
                    var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedido.Id).ToList();
                    pedido.ProductosPedidos.AddRange(listProductosPedido);
                }
            }
            return View(pedido);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TotalCharge,FormaDePago,HoraEntrega,Estado,UsuarioId,Created,Modified")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                pedido.Id = Guid.NewGuid();
                db.Pedidoes.Add(pedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pedido);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TotalCharge,FormaDePago,HoraEntrega,Estado,UsuarioId,Created,Modified")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pedido);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Pedido pedido = db.Pedidoes.Find(id);
            var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == id).ToList();

            foreach (var p in listProductosPedido)
            {
                db.Entry(p).State = EntityState.Deleted;
            }

            db.Pedidoes.Remove(pedido);
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

        public ActionResult ListaPedidos(string id)
        {
            var pedidos = db.Pedidoes.Where(_=>_.UsuarioId == id);

            return View(pedidos.ToList());
        }

        public ActionResult TerminarPedido(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido != null)
            {
                pedido.Estado = EstadoPedido.Terminado;
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}

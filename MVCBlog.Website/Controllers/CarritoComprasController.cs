using Microsoft.AspNet.Identity;
using MVCBlog.Core.Database;
using MVCBlog.Core.Entities;
using MVCBlog.Website.Models;
using MVCBlog.Website.Models.OutputModels.Blog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVCBlog.Website.Controllers
{
    public class CarritoComprasController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public virtual ActionResult Index()
        {
            var model = new IndexModel();

            model.UsuarioId = User.Identity.GetUserId();
            model.TiposProductos = db.TipoProductoes.ToList();
            model.Productos = db.Productoes.ToList();
            ViewBag.PossibleCC = model.Productos;
            return View(model);
        }

        public ActionResult Grill(Guid? idTipoProd)
        {
            var prods = db.Productoes.OfType<Producto>().Where(_=>_.Estado == EstadoProducto.Disponible);

            if (idTipoProd.HasValue)
                prods = prods.Where(_ => _.TipoProductoId == idTipoProd);

            var productos = prods.OrderBy(_ => _.Description).ToList();

            var prodViewModel = new ProductosViewModel()
            {
                Productos = productos
            };

            if (productos.Count > 0)
            {
                return this.PartialView("_ListaProductos", prodViewModel);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult SetPedidoProducto(PedidoProducto pedProd)
        {
            var pedido = new Pedido();
            var listProductosPedido = new List<PedidoProducto>();

            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                pedido = db.Pedidoes.FirstOrDefault(_ => _.UsuarioId == userId && _.Estado == EstadoPedido.Creado);
                if (pedido == null) pedido = new Pedido(userId);

                var prodoriginal = db.Productoes.FirstOrDefault(_ => _.Id == pedProd.ProductoId);
                decimal totParcial = pedProd.Cantidad * prodoriginal.PrecioActual;
                listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedido.Id).ToList();
                var prodRepetido = listProductosPedido.FirstOrDefault(_ => _.ProductoId == pedProd.ProductoId);
                if (prodRepetido != null)
                {
                    prodRepetido.Cantidad += pedProd.Cantidad;
                    prodRepetido.TotalParcial = prodRepetido.Cantidad * prodoriginal.PrecioActual;
                    db.Entry(prodRepetido).State = EntityState.Modified;
                    prodRepetido.Modified = DateTime.Now;
                }
                else
                {
                    listProductosPedido.Add(pedProd);
                    pedProd.PedidoId = pedido.Id;
                    pedProd.TotalParcial = totParcial;
                    db.PedidosProductos.Add(pedProd);
                }
                pedido.TotalCharge += totParcial;

                if (db.Entry(pedido).State == EntityState.Detached)
                {
                    pedido.Created = DateTime.Now;
                    db.Pedidoes.Add(pedido);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(pedido).State = EntityState.Modified;
                    pedido.Modified = DateTime.Now;
                    db.SaveChanges();
                }

                pedido.ProductosPedidos.AddRange(listProductosPedido);
            }

            return this.PartialView("_Carrito", pedido);
        }

        public ActionResult GetPedidoPendiente()
        {
            var pedido = new Pedido();
            var listProductosPedido = new List<PedidoProducto>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var pedidoUser = db.Pedidoes.Where(_ => _.UsuarioId == userId && _.Estado == EstadoPedido.Creado).FirstOrDefault();
                if (pedidoUser != null)
                {
                    pedido = pedidoUser;
                    listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedido.Id).ToList();
                    pedido.ProductosPedidos.AddRange(listProductosPedido);
                }
            }

            return this.PartialView("_Carrito", pedido);
        }

        public ActionResult RemoveProduct(Guid idProdPed)
        {
            var prodPed = db.PedidosProductos.Include(s => s.Pedido).Include(a => a.Producto).FirstOrDefault(_ => _.Id == idProdPed);
            var listProductosPedido = db.PedidosProductos.Include(a => a.Producto).Where(_ => _.PedidoId == prodPed.PedidoId).ToList();

            var pedido = prodPed.Pedido;
            listProductosPedido.Remove(prodPed);
            pedido.ProductosPedidos = listProductosPedido;
            pedido.TotalCharge -= prodPed.TotalParcial;

            db.Entry(prodPed).State = EntityState.Deleted;
            db.Entry(pedido).State = EntityState.Modified;
            pedido.Modified = DateTime.Now;
            db.SaveChanges();

            return this.PartialView("_Carrito", pedido);
        }

        public ActionResult RemovePedido(string idUser)
        {
            var pedidoUser = db.Pedidoes.Where(_ => _.UsuarioId == idUser && _.Estado == EstadoPedido.Creado).FirstOrDefault();
            if (pedidoUser != null)
            {
                var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedidoUser.Id).ToList();

                foreach (var p in listProductosPedido)
                {
                    db.Entry(p).State = EntityState.Deleted;
                }

                db.Entry(pedidoUser).State = EntityState.Deleted;
                db.SaveChanges();
            }
            var pedido = new Pedido(idUser);

            return this.PartialView("_Carrito", pedido);
        }

        public ActionResult TerminarPedido(Guid idPedido)
        {
            var pedidoUser = db.Pedidoes.Include(s => s.Usuario).Where(_ => _.Id == idPedido && _.Estado == EstadoPedido.Creado).FirstOrDefault();
            if (pedidoUser != null)
            {
                var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedidoUser.Id).ToList();

                if (listProductosPedido.Count == 0)
                    return this.PartialView("_Carrito", pedidoUser);

                pedidoUser.ProductosPedidos = listProductosPedido;

                return View("FinalizarPedido", pedidoUser);
            }

            return this.PartialView("_Carrito", pedidoUser);
        }


        public JsonResult FinalizarPedido(Guid idPedido, FormaDePago formaPago, string HoraEntrega, string Comentario)
        {
            var pedidoUser = db.Pedidoes.Include(s => s.Usuario).Where(_ => _.Id == idPedido && _.Estado == EstadoPedido.Creado).FirstOrDefault();
            if (pedidoUser != null)
            {
                var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == pedidoUser.Id).ToList();

                pedidoUser.ProductosPedidos = listProductosPedido;
                pedidoUser.FormaDePago = formaPago;
                pedidoUser.HoraEntrega = HoraEntrega;
                pedidoUser.Comentario = Comentario;
                pedidoUser.Estado = EstadoPedido.Pendiente;
                db.Entry(pedidoUser).State = EntityState.Modified;
                db.SaveChanges();

                //SendEmail();
            }

            return Json(new { success = true });
        }

        void SendEmail()
        {
            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "YourGamilId@gmail.com";
                WebMail.Password = "YourGmailPassword";

                //Sender email address.  
                WebMail.From = "SenderGamilId@gmail.com";

                //Send email  
                WebMail.Send(to: "", subject: "🍎🍌 Nuevo Pedido", body: "", null, null, isBodyHtml: true);
            }
            catch (Exception)
            {

            }
        }

    }

}
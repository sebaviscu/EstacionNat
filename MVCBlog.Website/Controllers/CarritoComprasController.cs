using Microsoft.AspNet.Identity;
using MVCBlog.Core.Database;
using MVCBlog.Core.Entities;
using MVCBlog.Website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlog.Website.Controllers
{
    public class CarritoComprasController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        private readonly IRepository repository;


        // GET: CarritoCompras
        public ActionResult Index()
        {
            var productos = db.Productoes.ToList();

            var prodViewModel = new ProductosViewModel()
            {
                Productos = productos
            };

            return View(prodViewModel);
        }


        public ActionResult Grill(Guid? idTipoProd)
        {
            var prods = db.Productoes.OfType<Producto>();

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
                pedido = db.Pedidoes.FirstOrDefault(_ => _.UsuarioId == userId && _.Estado == EstadoPedido.Pendiente);
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
                var pedidoUser = db.Pedidoes.Where(_ => _.UsuarioId == userId && _.Estado == EstadoPedido.Pendiente).FirstOrDefault();
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
            var prodPed = db.PedidosProductos.Include(a => a.Producto).Include(s => s.Pedido).FirstOrDefault(_ => _.Id == idProdPed);
            var listProductosPedido = db.PedidosProductos.Include(s => s.Producto).Where(_ => _.PedidoId == prodPed.PedidoId).ToList();

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


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public class Pedido : EntityBase
    {
        public Pedido(string usuarioId) : this()
        {
            UsuarioId = usuarioId;
        }
        public Pedido()
        {
            Estado = EstadoPedido.Pendiente;
            ProductosPedidos = new List<PedidoProducto>();
        }

        public decimal TotalCharge { get; set; }
        public FormaDePago FormaDePago { get; set; }
        public string HoraEntrega { get; set; }
        public EstadoPedido Estado { get; set; }
        public string Comentario { get; set; }
        public virtual AspNetUser Usuario { get; set; }
        public string UsuarioId { get; set; }

        [NotMapped]
        public List<PedidoProducto> ProductosPedidos { get; set; }
    }
}

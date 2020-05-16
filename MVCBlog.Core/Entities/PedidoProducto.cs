using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public class PedidoProducto : EntityBase
    {

        public Guid? PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        public Guid? ProductoId { get; set; }
        public virtual Producto Producto { get; set; }
        public decimal Cantidad { get; set; }
    }
}

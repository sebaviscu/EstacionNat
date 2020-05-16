using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public class Pedido : EntityBase
    {

        public decimal TotalCharge { get; set; }
        public string FormaDePago { get; set; }
        public string HoraEntrega { get; set; }
        public EstadoPedido Estado { get; set; }

        public Guid? UsuarioId { get; set; }
        public virtual AspNetUser Usuario { get; set; }

    }
}

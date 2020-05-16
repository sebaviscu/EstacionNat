using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public class PrecioHistorico : EntityBase
    {
        public decimal Precio { get; set; }

        public DateTime FechaDesde { get; set; }

        public Guid? IdProducto { get; set; }
    }
}

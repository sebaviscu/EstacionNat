using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBlog.Website.Models.EstacionNatural
{
    public class FiltroProductos
    {
        public string Descripcion { get; set; }
        public Guid? TipoProducto { get; set; }

    }
}
using MVCBlog.Core.Entities;
using System.Collections.Generic;

namespace MVCBlog.Website.Models.OutputModels.Blog
{
    /// <summary>
    /// Model for a several <see cref="BlogEntry">BlogEntries</see>.
    /// </summary>
    public class IndexModel
    {

        public string UsuarioId { get; set; }

        public List<TipoProducto> TiposProductos { get; set; }

        public List<PedidoProducto> ProductosPedidos { get; set; }

        public List<Producto> Productos { get; set; } 
    }
}
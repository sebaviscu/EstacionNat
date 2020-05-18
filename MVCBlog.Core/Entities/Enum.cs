using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public enum EstadoPedido
    {
        Pendiente,
        Terminado
    }

    public enum EstadoProducto
    {
        Disponible,
        NoDisponible
    }

    public enum Presentacion
    {
        Unidad,
        Peso
    }
}

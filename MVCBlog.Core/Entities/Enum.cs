using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Core.Entities
{
    public enum EstadoPedido
    {
        Creado,
        Pendiente,
        Terminado
    }

    public enum EstadoProducto
    {
        Disponible,
        NoDisponible
    }

    public enum FormaDePago
    {
        Efectivo,
        Tarjeta
    }

    public enum Presentacion
    {
        Unidad,
        Peso
    }
}

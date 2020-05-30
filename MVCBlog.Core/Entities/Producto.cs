using MVCBlog.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;

namespace MVCBlog.Core.Entities
{
    public class Producto : EntityBase
    {

        public string Description { get; set; }
        public EstadoProducto Estado { get; set; }

        [Display(Name = "Presentacion")]
        public Presentacion Unidad { get; set; }


        [Display(Name = nameof(Labels.Photo), ResourceType = typeof(Labels))]
        public string Photo { get; set; }

        [Display(Name = "Tipo de Producto")]
        public Guid? TipoProductoId { get; set; }
        public virtual TipoProducto TipoProducto { get; set; }

        [Display(Name = "Precio")]
        public int PrecioActual { get; set; }

        [NotMapped]
        public string RelativePath
        {
            get
            {
                return ConfigurationManager.AppSettings["UserPhotosPath"];
            }
        }

        [NotMapped]
        public string FullPath
        {
            get
            {
                var applicationPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                return Path.Combine(applicationPath, this.RelativePath);
            }
        }


        [NotMapped]
        public int PrecioFuturo { get; set; }

        [Display(Name = "Precios Historicos")]
        [NotMapped]
        public List<PrecioHistorico> HistoricPrice { get; set; }
    }
}

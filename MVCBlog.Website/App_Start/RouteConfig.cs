using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCBlog.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            // Short URL for '404'
            routes.MapRoute(
                "Short_404",
                "404",
                new { controller = MVC.Error.Name, action = MVC.Error.ActionNames.NotFound });

            // Short URL for 'Logout'
            routes.MapRoute(
                "Short_Logout",
                "Logout",
                new { controller = MVC.Login.Name, action = MVC.Login.ActionNames.Logout });

           
            routes.MapRoute(
                Routes.DEFAULT,
                "{controller}/{action}/{id}",
                new { controller = "CarritoCompras", action = "Index", id = UrlParameter.Optional });
        }
    }
}

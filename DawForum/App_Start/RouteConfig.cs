using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DawForum
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Categories", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Topic",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Topic", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

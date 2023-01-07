using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdvertisingAgency
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" },
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("Agency", "Agency/{action}/{name}", new { controller = "Agency", action = "Index", name = UrlParameter.Optional },
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, 
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" }, 
                new[] { "AdvertisingAgency.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, 
                new[] { "AdvertisingAgency.Controllers" });
            

            //.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace jdean_blog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            // added
            routes.MapRoute(
                name: "NewSlug",
                url: "Blog/{slug}",
                defaults: new
                {
                    controller = "Posts",
                    action = "Details",
                    slug = UrlParameter.Optional
                });

            // existed
            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}

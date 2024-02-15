using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Tameenk.IVR.InquiryApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

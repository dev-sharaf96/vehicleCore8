using Newtonsoft.Json;
using PrometheusLib.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Tameenk.Services.Capcha.API
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

            PrometheusConfig.UseMetricsServer(config, "metrics");
        }
    }
}

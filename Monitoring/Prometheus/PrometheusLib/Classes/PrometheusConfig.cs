using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PrometheusLib.Classes
{
    public class PrometheusConfig
    {
        /// <summary>
        /// Adds an endpoint for displaying prometheus metrics. 
        /// </summary>
        /// <param name="config">Obtained by calling from WebApiConfig.Register</param>
        /// <param name="url">eg, "metrics" or "api/metrics"</param>
        public static void UseMetricsServer(HttpConfiguration config, string url = "metrics")
        {
            config.Routes.MapHttpRoute("Prometheus", url, new { controller = "PrometheusMetrics" });
        }

        /// <summary>
        /// Adds an endpoint for displaying prometheus metrics, protected by basic auth 
        /// </summary>
        /// <param name="config">Obtained by calling from WebApiConfig.Register</param>
        /// <param name="url">eg, "metrics" or "api/metrics"</param>
        /// <param name="BasicAuthUsername">Username for basic auth</param>
        /// <param name="BasicAuthPassword">Password for basic auth</param>
        public static void UseMetricsServer(HttpConfiguration config, string url, string BasicAuthUsername, string BasicAuthPassword)
        {
            PrometheusMetricsController.BasicAuthUsername = BasicAuthUsername;
            PrometheusMetricsController.BasicAuthPassword = BasicAuthPassword;
            PrometheusMetricsController.UseBasicAuth = true;

            config.Routes.MapHttpRoute("Prometheus", url, new { controller = "PrometheusMetrics" });
        }


    }
}

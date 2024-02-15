using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Tameenk.App_Start;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //EngineContext.Current.Resolve<ILogger>().Log($">>>>>>> Tameenk Application Started <<<<<<<");
        }
        //Application_BeginRequest
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];

            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {


                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("Ar");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("Ar");
            }
        }

        //Handle any unhandled exception on the application
        protected void Application_Error()
        {
            //Exception ex = Server.GetLastError();

            if (!HttpContext.Current.Request.IsLocal)
            {
                Exception ex = Server.GetLastError();
                if (ex is HttpException)
                {
                    if (((HttpException)(ex)).GetHttpCode() == 404)
                        Response.Redirect("~/", true);
                }
                //log the error!
                var erroCode = DateTime.Now.GetTimestamp();
                EngineContext.Current.Resolve<ILogger>().Log($"Application Error in Tameenk Global [key={erroCode}]", ex);
                HttpContext.Current.ClearError();
                Response.RedirectToRoute("Default", new { controller = "Error", action = "Index", Id = erroCode });
            }
        }
    }
}

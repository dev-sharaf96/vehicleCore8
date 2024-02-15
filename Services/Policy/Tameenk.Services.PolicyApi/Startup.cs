using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.PolicyApi.Infrastructure;
using System.Web.Routing;
using Tameenk.Security.Services;

[assembly: OwinStartup(typeof(Tameenk.Services.PolicyApi.Startup))]
namespace Tameenk.Services.PolicyApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);

            OAuthConfig.ConfigureOAuth(app);

            //UnityWebApiActivator.Start(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
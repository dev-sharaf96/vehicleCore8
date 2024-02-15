using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;
using Tameenk.Services.YakeenIntegrationApi.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.YakeenIntegrationApi.Startup))]
namespace Tameenk.Services.YakeenIntegrationApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always // Add this line to enable detail mode in release
            };
            WebApiConfig.Register(config);
            EngineContext.InitializeWebApi(false, config);

            OAuthConfig.ConfigureOAuth(app);

            SwaggerConfig.Register(config);
            app.UseWebApi(config);
            AutoMapperConfiguration.Init();
           // Tameenk.Services.YakeenIntegration.Business.Infrastructure.AutoMapperConfiguration.Init();
            Tameenk.Services.Inquiry.Components.AutoMapperConfiguration.Init();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
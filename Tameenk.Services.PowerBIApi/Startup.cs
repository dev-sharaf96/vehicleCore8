using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Tameenk.Core.Infrastructure;
using System.Web.Routing;


[assembly: OwinStartup(typeof(Tameenk.Services.PowerBIApi.Startup))]
namespace Tameenk.Services.PowerBIApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app )
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always // Add this line to enable detail mode in release
            };
            WebApiConfig.Register(config);
            EngineContext.InitializeWebApi(false, config);

            // OAuthConfig.ConfigureOAuth(app);
            app.UseWebApi(config);
            AutoMapperConfiguration.Init();
            //Tameenk.Services.YakeenIntegration.Business.Infrastructure.AutoMapperConfiguration.Init();
            //Tameenk.Services.Inquiry.Components.AutoMapperConfiguration.Init();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
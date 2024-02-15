using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Tameenk.Core.Infrastructure;
using Tameenk.PaymentNotificationsApi.Infrastructure;
using Tameenk.Security.Services;

[assembly: OwinStartup(typeof(Tameenk.PaymentNotificationsApi.Startup))]
namespace Tameenk.PaymentNotificationsApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            AutoMapperConfiguration.Init();

            EngineContext.InitializeWebApi(false, config);

            SwaggerConfig.Register(config);
            OAuthConfig.ConfigureOAuth(app);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            app.UseWebApi(config);
        }
    }
}
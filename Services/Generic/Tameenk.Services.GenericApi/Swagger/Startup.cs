using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.InquiryGateway.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.InquiryGateway.Startup))]
namespace Tameenk.Services.InquiryGateway
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            EngineContext.InitializeWebApi(false, config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);
            AutoMapperConfiguration.Init();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
    }
}
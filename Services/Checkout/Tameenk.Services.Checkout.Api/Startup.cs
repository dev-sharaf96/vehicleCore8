using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Api.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.Checkout.Api.Startup))]
namespace Tameenk.Services.Checkout.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration()
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };
            WebApiConfig.Register(config);
            CacheManagerConfig.Initialize();
            RateLimitConfiguration.Register(config);
            EngineContext.InitializeWebApi(false, config);
            OAuthConfig.ConfigureOAuth(app);
            Tameenk.Services.Quotation.Components.AutoMapperConfiguration.Init();

            // SwaggerConfig.Register(config);
            app.UseWebApi(config);
            //   AutoMapperConfiguration.Init();
            // Tameenk.Services.YakeenIntegration.Business.Infrastructure.AutoMapperConfiguration.Init();
            //  Tameenk.Services.Inquiry.Components.AutoMapperConfiguration.Init();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
    }
}
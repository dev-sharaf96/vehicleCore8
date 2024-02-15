using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;

[assembly: OwinStartup(typeof(Tameenk.Services.QuotationNew.Api.Startup))]
namespace Tameenk.Services.QuotationNew.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //HttpConfiguration config = new HttpConfiguration();
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always // Add this line to enable detail mode in release
            };

            WebApiConfig.Register(config);
            log4net.Config.XmlConfigurator.Configure();
            CacheManagerConfig.Initialize();
            RateLimitConfiguration.Register(config);
            // AutoMapperConfiguration.Init();
            Components.AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);
            //SwaggerConfig.Register(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            OAuthConfig.ConfigureOAuth(app);
            app.UseWebApi(config);
        }
    }
}
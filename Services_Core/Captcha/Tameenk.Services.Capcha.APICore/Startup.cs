//using Tameenk.Core.Infrastructure;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.Capcha.API.Startup))]
namespace Tameenk.Services.Capcha.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always // Add this line to enable detail mode in release
            };
            
            WebApiConfig.Register(config);
            CacheManagerConfig.Initialize();
            RateLimitConfiguration.Register(config);
            EngineContext.InitializeWebApi(false, config);
            app.UseWebApi(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
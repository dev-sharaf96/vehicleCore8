using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
//using Tameenk.Security.Services;

[assembly: OwinStartup(typeof(Tameenk.Services.QuotationDependancy.Api.Startup))]
namespace Tameenk.Services.QuotationDependancy.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };

            WebApiConfig.Register(config);
            RateLimitConfiguration.Register(config);
            CacheManagerConfig.Initialize();
            EngineContext.InitializeWebApi(false, config);
            //OAuthConfig.ConfigureOAuth(app);
            app.UseWebApi(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
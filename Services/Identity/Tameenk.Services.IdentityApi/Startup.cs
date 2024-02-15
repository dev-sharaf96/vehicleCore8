using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;
using Tameenk.Services.IdentityApi.App_Start;
using Tameenk.Services.IdentityApi.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.IdentityApi.Startup))]
namespace Tameenk.Services.IdentityApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            CacheManagerConfig.Initialize();
            RateLimitConfiguration.Register(config);
            AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);

            OAuthConfig.ConfigureOAuth(app);
            ConfigureAuth(app);
            //UnityWebApiActivator.Start(config);
            app.UseWebApi(config);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SwaggerConfig.Register(config);
        }
    }
}
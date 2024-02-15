using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.AdministrationApi.Infrastructure;
using Tameenk.Services.Policy.Components;

[assembly: OwinStartup(typeof(Tameenk.Services.AdministrationApi.Startup))]
namespace Tameenk.Services.AdministrationApi
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
            Infrastructure.AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);

          //  OAuthConfig.ConfigureOAuth(app);
           AuthConfig.ConfigureAuth(app);
            Policy.Components.AutoMapperConfiguration.Init();
            //UnityWebApiActivator.Start(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
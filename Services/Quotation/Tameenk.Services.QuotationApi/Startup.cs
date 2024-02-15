using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;
using Tameenk.Services.QuotationApi.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.Services.QuotationApi.Startup))]
namespace Tameenk.Services.QuotationApi
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
           // AutoMapperConfiguration.Init();
            Tameenk.Services.Quotation.Components.AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);
            SwaggerConfig.Register(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            OAuthConfig.ConfigureOAuth(app);
            app.UseWebApi(config);

        }
    }
}
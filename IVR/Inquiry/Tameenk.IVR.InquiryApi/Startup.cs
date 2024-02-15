using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Tameenk.Core.Infrastructure;
using System.Web.Routing;
using Tameenk.IVR.InquiryApi.Infrastructure;

[assembly: OwinStartup(typeof(Tameenk.IVR.InquiryApi.Startup))]
namespace Tameenk.IVR.InquiryApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always // Add this line to enable detail mode in release
            };
            WebApiConfig.Register(config);
            AutoMapperConfiguration.Init();
            EngineContext.InitializeWebApi(false, config);

            //OAuthConfig.ConfigureOAuth(app);
            //Tameenk.Services.Quotation.Components.AutoMapperConfiguration.Init();

            // SwaggerConfig.Register(config);
            app.UseWebApi(config);
            //   AutoMapperConfiguration.Init();
            // Tameenk.Services.YakeenIntegration.Business.Infrastructure.AutoMapperConfiguration.Init();
            //  Tameenk.Services.Inquiry.Components.AutoMapperConfiguration.Init();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
    }
}
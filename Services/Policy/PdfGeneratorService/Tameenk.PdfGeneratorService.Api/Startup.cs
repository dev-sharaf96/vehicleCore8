using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Tameenk.PdfGeneratorService.Api.Startup))]
namespace Tameenk.PdfGeneratorService.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            UnityWebApiActivator.Start(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
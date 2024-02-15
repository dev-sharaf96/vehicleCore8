using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Profile.Component.Membership;

namespace Tameenk.Services.IdentityApi
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(() => { return EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>(); });
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<ClientSignInManager>(ClientSignInManager.Create);
        }
    }
}
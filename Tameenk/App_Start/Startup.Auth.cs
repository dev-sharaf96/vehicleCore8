using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Profile.Component.Membership;

namespace Tameenk
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(() => { return EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>(); });
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<ClientSignInManager>(ClientSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(Constants.LoginUrl),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, AspNetUser>(
                        validateInterval: TimeSpan.FromDays(1),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),

                    OnResponseSignIn = context =>
                    {
                        context.Properties.AllowRefresh = true;
                        context.Properties.ExpiresUtc = DateTimeOffset.Now.AddDays(1);
                    }
                },


            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ApplicationCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorCookie);
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}
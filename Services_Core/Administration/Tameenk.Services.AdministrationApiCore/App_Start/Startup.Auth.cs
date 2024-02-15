using System;
using Tameenk.Data;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Repositories;
using Tameenk.Services.Administration.Identity.Services;

namespace Tameenk.Services.AdministrationApi
{
    public partial class AuthConfig
    {
       // private static readonly IdentityLogService identityLogService;

        //private readonly IDbContext dbContext;

        public AuthConfig(IdentityLogService identityLogService)
        {
           
            //this.dbContext = dbContext;
        }
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public static void ConfigureAuth(IAppBuilder app)
        {
            //this.identityLogService = identityLogService;
            //// Configure the db context and user manager to use a single instance per request
            // AdminIdentityContext ddd = new AdminIdentityContext();
            //  app.CreatePerOwinContext(AdminIdentityContext.Create);
            //  app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new AuthenticationContext(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}

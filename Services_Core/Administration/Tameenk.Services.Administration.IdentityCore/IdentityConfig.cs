using System.Web;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Validators;
using Tameenk.Services.Administration.Identity.Repositories;

namespace Tameenk.Services.Administration.Identity
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
UserManager<AppUser, int>
    {
        public ApplicationUserManager(IUserStore<AppUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var request = HttpContext.Current.Request;
           // var providerName = request.Headers["providername"];

            var manager = new ApplicationUserManager(new UserStore<AppUser, RoleEntity, int, UserLogin, UserRole, UserClaim>(context.Get<AdminIdentityContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new CustomUserValidator<AppUser>(manager)// new UserValidator<AppUser,int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}

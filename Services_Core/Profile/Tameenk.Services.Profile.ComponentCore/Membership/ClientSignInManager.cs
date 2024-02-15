using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Profile.Component.Membership
{
    public class ClientSignInManager : // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
SignInManager<AspNetUser, string>
    {
        public ClientSignInManager(UserManager manager, IAuthenticationManager AuthManager)
            : base(manager, AuthManager)
        {
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AspNetUser user)
        {
            // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
            // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
                        // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
                                                // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
                                                                                                return user.GenerateUserIdentityAsync((UserManager)UserManager);
        }
        public static ClientSignInManager Create(IdentityFactoryOptions<ClientSignInManager> options,
            IOwinContext context)
        {
            return new ClientSignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }
}

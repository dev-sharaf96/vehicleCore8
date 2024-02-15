using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Profile.Component.Membership
{
    public class ClientSignInManager : SignInManager<AspNetUser, string>
    {
        public ClientSignInManager(UserManager manager, IAuthenticationManager AuthManager)
            : base(manager, AuthManager)
        {
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AspNetUser user)
        {
            return user.GenerateUserIdentityAsync((UserManager)UserManager);
        }
        public static ClientSignInManager Create(IdentityFactoryOptions<ClientSignInManager> options,
            IOwinContext context)
        {
            return new ClientSignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }
}

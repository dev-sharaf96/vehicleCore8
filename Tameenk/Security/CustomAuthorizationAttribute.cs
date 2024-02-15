using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Principal;
using Tameenk.App_Start;
using Tameenk.Security.Services;

namespace Tameenk.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        public string[] Roles { get; set; }
        public CustomAuthorizationAttribute(params string[] roles)
        {
            Roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var webContext = EngineContext.Current.Resolve<WebContext>();
            IPrincipal user = webContext.CurrentUser;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }
            var userManager = httpContext.GetOwinContext().GetUserManager<UserManager>();
            var clientUser = userManager.FindById(httpContext.User.Identity.GetUserId<string>());
            var authService = EngineContext.Current.Resolve<IAuthorizationService>();
            var dbRoles = authService.GetAllRoles();
            if (Roles != null && Roles.Length > 0){
                var roles = Roles.Select(r => dbRoles.FirstOrDefault(dbr => dbr.RoleNameEN.Equals(r, StringComparison.OrdinalIgnoreCase))).ToList();
                //Only role access is implemented here
                /*if ((this._usersSplit.Length > 0) && !this._usersSplit.Contains<string>(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }*/
                if (!roles.Any(r => r.ID == clientUser.RoleId))
                {
                    return false;
                }
            }
            return true;
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Security.CustomAttributes
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string claimType;
        private string claimValue;
        public ClaimsAuthorizeAttribute(string type, string value)
        {
            this.claimType = type;
            this.claimValue = value;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (user != null && user.Identities != null)
            {
                if (user.Identities.Any(x => x.HasClaim(claimType, claimValue)))
                {
                    base.OnAuthorization(actionContext);
                    return;
                }
            }
            base.HandleUnauthorizedRequest(actionContext);
            return;
        }
    }
}
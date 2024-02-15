using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Infrastructure;

namespace Tameenk.Security.CustomAttributes
{
    public class IpAddressAuthorizeAttribute : AuthorizeAttribute
    {
        private const string CLAIM_TYPE = "IpAddress";

        public IpAddressAuthorizeAttribute()
        {
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (user != null && user.HasClaim(c => c.Type == CLAIM_TYPE))
            {
                var IpAddressClaim = user.Claims.FirstOrDefault(c => c.Type == CLAIM_TYPE);
                if (IpAddressClaim != null)
                {
                    var httpContext = EngineContext.Current.Resolve<HttpContextBase>();
                    if (httpContext.GetOwinContext().Request.RemoteIpAddress == IpAddressClaim.Value)
                    {
                        base.OnAuthorization(actionContext);
                        return;
                    }
                }

            }
            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}
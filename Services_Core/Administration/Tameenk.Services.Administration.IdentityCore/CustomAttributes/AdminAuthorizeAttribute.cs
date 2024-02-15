using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Repositories;

namespace Tameenk.Services.Administration.Identity
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        private int pageNumber;

        public AdminAuthorizeAttribute(int pageNumber)
        {
            this.pageNumber = pageNumber;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var _tokensService = EngineContext.Current.Resolve<IExpiredTokensService>();
            var _userPageService = EngineContext.Current.Resolve<IUserPageService>();
            var _userService = EngineContext.Current.Resolve<IUserService>();
            var user = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (user == null || user.Identities == null)
            {
                base.HandleUnauthorizedRequest(actionContext);
                return;
            }
            if (user != null && user.Identities != null)
            {
                if (user.Claims.FirstOrDefault(x => x.Type == "key") == null)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                if (!user.Identities.Any(x => x.HasClaim("dashboardUser", "true")))
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                var token = user.Claims.FirstOrDefault(x => x.Type == "key");
                if (token == null)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                int tokenCount = _tokensService.GetToken(token.Value).Count;
                if (tokenCount > 0)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                var userId = user.Claims.FirstOrDefault(x => x.Type == "Id");
                if (userId == null)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                if (pageNumber != 0 && pageNumber!=10000)
                {
                    bool isAuthorized = _userPageService.IsAuthorizedUser(pageNumber, userId.Value);
                    if (!isAuthorized)
                    {
                        base.HandleUnauthorizedRequest(actionContext);
                        return;
                    }
                }
                var u_Id = int.Parse(userId.Value);
                var userInfo = _userService.Find(x => x.Id == u_Id).FirstOrDefault();
                if (userInfo == null)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                if (pageNumber == 10000 && !userInfo.IsAdmin)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
            }
            base.OnAuthorization(actionContext);
            return;
        }
    }
}
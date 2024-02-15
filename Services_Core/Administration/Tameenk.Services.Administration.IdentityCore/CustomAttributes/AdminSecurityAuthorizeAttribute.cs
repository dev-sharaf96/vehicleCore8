using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity
{
    public class AdminSecurityAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ForbiddenRequestLog log = new ForbiddenRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Referer = Utilities.GetFullUrlReferrer();
            log.UserID = actionContext.RequestContext.Principal.Identity.GetUserId();
            log.UserName = actionContext.RequestContext.Principal.Identity.GetUserName();

            try
            {
                if (log.Headers["User-Agent"].ToString().ToLower().Contains("postman"))
                {
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SignOut();
                    log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                    log.ErrorDescription = "Postman is ignored";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    //base.HandleUnauthorizedRequest(actionContext);
                    HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "Go and play away young man");
                    return;
                }
                if (string.IsNullOrEmpty(log.Referer) || !log.Referer.ToLower().Contains("admin.bcare.com.sa"))
                {
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SignOut();
                    log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                    log.ErrorDescription = "Referer is not from dashboard";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    //base.HandleUnauthorizedRequest(actionContext);
                    HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "Referer is not from dashboard");
                    return;
                }

                var requesterURL = Utilities.GetUrlReferrer();
                if (string.IsNullOrEmpty(requesterURL) || !requesterURL.ToLower().Contains("admin.bcare.com.sa"))
                {
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SignOut();
                    log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                    log.ErrorDescription = "RequesterURL is not from dashboard";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    //base.HandleUnauthorizedRequest(actionContext);
                    HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "RequesterURL is not from dashboard");
                    return;
                }

                var u_Id = int.Parse(log.UserID);
                var _userService = EngineContext.Current.Resolve<IUserService>();
                var userInfo = _userService.Find(x => x.Id == u_Id).FirstOrDefault();
                if (userInfo == null)
                {
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SignOut();
                    log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                    log.ErrorDescription = "userInfo == null";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "userInfo == null");
                    return;
                }
                if (userInfo.Email != log.UserName)
                {
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SignOut();
                    log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                    log.ErrorDescription = "You are trying to use account of some one else";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "userInfo.Email != log.UserName");
                    return;
                }

                base.OnAuthorization(actionContext);
                return;
            }
            catch (Exception ex)
            {
                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                FormsAuthentication.SignOut();
                log.ErrorCode = (int)HttpStatusCode.InternalServerError;
                log.ErrorDescription = $"AdminAuthorizeAttribute Exception: {ex.ToString()}";
                ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                base.HandleUnauthorizedRequest(actionContext);
                //HandleUnauthorizedSingleSessionRequest(actionContext, HttpStatusCode.Unauthorized, "This session is ended as there is another session opened from another device");
                return;
            }
        }

        public void HandleUnauthorizedSingleSessionRequest(HttpActionContext context, HttpStatusCode statusCode, string message)
        {
            context.Response = context.ControllerContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Enums;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;

namespace Tameenk.Security.CustomAttributes
{
    public class SingleSessionAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly List<string> ExceptionList = new List<string>()
        {
            Channel.ios.ToString().ToLower(),
            Channel.android.ToString().ToLower()
        };

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
                var url = HttpContext.Current.Request.Url.Host;
                var header = actionContext.Request.Headers;
                if (header.Contains("Channel"))
                {
                    var channel = header.GetValues("Channel").FirstOrDefault();
                    if (ExceptionList.Contains(channel.ToLower()))
                    {
                        base.OnAuthorization(actionContext);
                        return;
                    }
                }

                var _loginActiveTokensService = EngineContext.Current.Resolve<IRepository<LoginActiveTokens>>();
                var user = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (user != null)
                {
                    var userId = user.Claims.FirstOrDefault(x => x.Type == "curent_user_id");
                    if (userId == null)
                    {
                        // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                        FormsAuthentication.SignOut();
                        log.ErrorCode = (int)HttpStatusCode.Unauthorized;
                        log.ErrorDescription = "userId == null";
                        ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                        base.HandleUnauthorizedRequest(actionContext);
                        return;
                    }
                    log.UserID = userId.Value;

                    var sessionData = _loginActiveTokensService.Table.Where(X => X.UserId == userId.Value).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (sessionData != null)
                    {
                        if (ExceptionList.Contains(sessionData.Channel))
                        {
                            base.OnAuthorization(actionContext);
                            return;
                        }
                        if (sessionData.IsValid && (sessionData.UserIP != log.UserIP || sessionData.Headers["User-Agent"].ToString() != log.Headers["User-Agent"].ToString())) // sessionData.IsValid && (sessionData.UserIP != log.UserIP || sessionData.UserAgent != log.UserAgent)
                        {
                            //var send = SendSingleSessionSMS(sessionData);
                            
                            // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                                                                                    FormsAuthentication.SignOut();
                            log.ErrorCode = (int)CustomHttpStatusCodeEnum.SessionEnded;
                            log.ErrorDescription = "This session is expired, as there is another active session";
                            ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                            //base.HandleUnauthorizedRequest(actionContext);
                            HandleUnauthorizedSingleSessionRequest(actionContext, (HttpStatusCode)CustomHttpStatusCodeEnum.SessionEnded, "This session is ended as there is another session opened from another device");
                            return;
                        }
                        if (!sessionData.IsValid || DateTime.Now > sessionData.ExpiredDate)
                        {
                            // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                            FormsAuthentication.SignOut();
                            log.ErrorCode = (int)CustomHttpStatusCodeEnum.SessionExpired;
                            log.ErrorDescription = "Session is expired";
                            ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                            //base.HandleUnauthorizedRequest(actionContext);
                            HandleUnauthorizedSingleSessionRequest(actionContext, (HttpStatusCode)CustomHttpStatusCodeEnum.SessionExpired, "This session is expired as token is exceeded 30 minutes");
                            return;
                        }

                        sessionData.IsValid = true;
                        sessionData.ExpiredDate = DateTime.Now.AddMinutes(30);
                        _loginActiveTokensService.Update(sessionData);
                    }
                }
                base.OnAuthorization(actionContext);
                return;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\SingleSessionAuthorizeAttribute_Exception_" + log.ServerIP + "_" + log.UserIP + "___" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());
                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                FormsAuthentication.SignOut();
                log.ErrorCode = (int)HttpStatusCode.InternalServerError;
                log.ErrorDescription = ex.ToString();
                ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                base.HandleUnauthorizedRequest(actionContext);
                return;
            }
        }

        public void HandleUnauthorizedSingleSessionRequest(HttpActionContext context, HttpStatusCode statusCode, string message)
        {
            context.Response = context.ControllerContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}

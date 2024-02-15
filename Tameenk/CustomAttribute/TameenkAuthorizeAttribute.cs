using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;

namespace Tameenk
{
    public class TameenkAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            ForbiddenRequestLog log = new ForbiddenRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Referer = Utilities.GetFullUrlReferrer();
            log.UserID = actionContext.HttpContext.User.Identity.GetUserId();
            log.UserName = actionContext.HttpContext.User.Identity.GetUserName();
            try
            {
                if (!actionContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                if (string.IsNullOrEmpty(log.UserID))
                {
                    base.HandleUnauthorizedRequest(actionContext);
                    return;
                }
                var cookie = Utilities.GetCookie("_authCookie");
                if (string.IsNullOrEmpty(cookie))
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 1;
                    log.ErrorDescription = "_authCookie Is Null Or Empty";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }
                var userTicket = System.Web.Security.FormsAuthentication.Decrypt(cookie);
                log.Request = JsonConvert.SerializeObject(userTicket);
                if(userTicket.Expired)
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 2;
                    log.ErrorDescription = "_authCookie Is expired as expiry date is : "+userTicket.Expiration.ToString();
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }
                string[] values = userTicket.UserData.Split(';');
                string keyValue = "";
                string userId = "";
                foreach (var value in values)
                {
                    if (value.Contains("Key"))
                    {
                        keyValue = value.Substring(value.LastIndexOf("=") + 1);
                    }
                    if (value.Contains("UserID"))
                    {
                        userId = value.Substring(value.LastIndexOf("=") + 1);
                    }
                }
                if (string.IsNullOrWhiteSpace(userId))
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 3;
                    log.ErrorDescription = "user id comming from _authCookie is null";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }

                var currentUserId = actionContext.HttpContext.User.Identity.GetUserId();
                log.UserID = currentUserId;
                if (string.IsNullOrEmpty(currentUserId) || currentUserId != userId)
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 4;
                    log.ErrorDescription = "currentUserId not match cookie user id as current user id is : "+ currentUserId +" and cookie user id is : "+ userId;
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }
                if (string.IsNullOrWhiteSpace(keyValue))
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 5;
                    log.ErrorDescription = "keyValue comming from _authCookie is null";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }
                var _cookiesService = EngineContext.Current.Resolve<IExpiredTokensService>();
                if (_cookiesService.GetFromExpiredCookie(keyValue) != null)
                {
                    FormsAuthentication.SignOut();
                    RemoveLoginCookie("_authCookie");
                    base.HandleUnauthorizedRequest(actionContext);
                    log.ErrorCode = 6;
                    log.ErrorDescription = "current user is not signed in";
                    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                    return;
                }
                base.OnAuthorization(actionContext);
                return;
            }
            catch (Exception ex)
            {
                FormsAuthentication.SignOut();
                RemoveLoginCookie("_authCookie");
                base.HandleUnauthorizedRequest(actionContext);
                log.ErrorCode = 401;
                log.ErrorDescription = ex.ToString();
                ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                return;

            }
        }
        public static void RemoveLoginCookie(string name)
        {
            try
            {
                HttpCookie cookieToRemove = new HttpCookie(name);
                cookieToRemove.Value = string.Empty;
                cookieToRemove.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookieToRemove);
            }
            catch
            {
            }
        }

    }
}
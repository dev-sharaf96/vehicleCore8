using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Enums;
using Tameenk.Services.Core;

namespace Tameenk.Services.IdentityApi
{
    public class TameenkAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly List<string> ExceptionList = new List<string>() 
        {
            Channel.ios.ToString().ToLower(),
            Channel.android.ToString().ToLower()
        };

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            #region Old Code

            //ForbiddenRequestLog log = new ForbiddenRequestLog();
            //log.ServerIP = Utilities.GetInternalServerIP();
            //log.UserAgent = Utilities.GetUserAgent();
            //log.UserIP = Utilities.GetUserIPAddress();
            //log.Referer = Utilities.GetFullUrlReferrer();
            //log.UserID = actionContext.RequestContext.Principal.Identity.GetUserId();
            //log.UserName = actionContext.RequestContext.Principal.Identity.GetUserName();
            //try
            //{
            //    var _tokensService = EngineContext.Current.Resolve<IRepository<ExpiredTokens>>();
            //    var user = HttpContext.Current.User.Identity as ClaimsIdentity;
            //    if (user == null)
            //    {
            //        FormsAuthentication.SignOut();
            //        log.ErrorCode = 1;
            //        log.ErrorDescription = "user is null";
            //        ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
            //        base.HandleUnauthorizedRequest(actionContext);
            //        return;
            //    }
            //    if (user != null)
            //    {
            //        var token = user.Claims.FirstOrDefault(x => x.Type == "UserSessionId");
            //        if (token == null)
            //        {
            //            FormsAuthentication.SignOut();
            //            log.ErrorCode = 3;
            //            log.ErrorDescription = "UserSessionId is null";
            //            ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
            //            base.HandleUnauthorizedRequest(actionContext);
            //            return;
            //        }
            //        var tokenCount = _tokensService.TableNoTracking.Where(X => X.Key == token.Value).FirstOrDefault();
            //        if (tokenCount != null)
            //        {
            //            FormsAuthentication.SignOut();
            //            log.ErrorCode = 4;
            //            log.ErrorDescription = "UserSessionId already exist";
            //            ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
            //            base.HandleUnauthorizedRequest(actionContext);
            //            return;
            //        }
            //        var userId = user.Claims.FirstOrDefault(x => x.Type == "curent_user_id");
            //        if (userId == null)
            //        {
            //            FormsAuthentication.SignOut();
            //            log.ErrorCode = 5;
            //            log.ErrorDescription = "userid no relate to this token";
            //            ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
            //            base.HandleUnauthorizedRequest(actionContext);
            //            return;
            //        }
            //    }
            //    base.OnAuthorization(actionContext);
            //    return;
            //}
            //catch (Exception ex)
            //{
            //    FormsAuthentication.SignOut();
            //    base.HandleUnauthorizedRequest(actionContext);
            //    log.ErrorCode = 401;
            //    log.ErrorDescription = ex.ToString();
            //    ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
            //    return;

            //}

            #endregion

            ForbiddenRequestLog log = new ForbiddenRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Referer = Utilities.GetFullUrlReferrer();
            log.UserID = actionContext.RequestContext.Principal.Identity.GetUserId();
            log.UserName = actionContext.RequestContext.Principal.Identity.GetUserName();

            try
            {
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

                if (header.Authorization != null)
                {
                    var token = header.Authorization.Parameter;
                    if (!string.IsNullOrEmpty(token))
                    {
                        var activeLoginTokens = EngineContext.Current.Resolve<IRepository<LoginActiveTokens>>();
                        var tokenData = activeLoginTokens.Table.Where(a => a.SessionId == token).FirstOrDefault();
                        if (tokenData != null)
                        {
                            if (ExceptionList.Contains(tokenData.Channel))
                            {
                                base.OnAuthorization(actionContext);
                                return;
                            }
                            if (!tokenData.IsValid || DateTime.Now > tokenData.ExpiredDate)
                            {
                                tokenData.IsValid = false;
                                activeLoginTokens.Update(tokenData);

                                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                                FormsAuthentication.SignOut();
                                log.ErrorCode = (int)CustomHttpStatusCodeEnum.SessionExpired;
                                log.ErrorDescription = "Session is expired";
                                ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                                HandleUnauthorizedSingleSessionRequest(actionContext, (HttpStatusCode)CustomHttpStatusCodeEnum.SessionExpired, "This session is expired as token is exceeded 30 minutes");
                                return;
                            }

                            tokenData.IsValid = true;
                            tokenData.ExpiredDate = DateTime.Now.AddMinutes(30);
                            activeLoginTokens.Update(tokenData);
                        }

                        var user = HttpContext.Current.User.Identity as ClaimsIdentity;
                        if (user != null)
                        {
                            var userId = user.Claims.FirstOrDefault(x => x.Type == "curent_user_id");
                            if (userId != null)
                            {
                                tokenData = activeLoginTokens.Table.Where(a => a.UserId == userId.Value && a.UserIP == log.UserIP && a.Headers["User-Agent"].ToString() == log.Headers["User-Agent"].ToString()).OrderByDescending(a => a.Id).FirstOrDefault();
                                if (tokenData != null)
                                {
                                    if (ExceptionList.Contains(tokenData.Channel))
                                    {
                                        base.OnAuthorization(actionContext);
                                        return;
                                    }
                                    if (!tokenData.IsValid || DateTime.Now > tokenData.ExpiredDate)
                                    {
                                        tokenData.IsValid = false;
                                        activeLoginTokens.Update(tokenData);

                                        // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                                        FormsAuthentication.SignOut();
                                        log.ErrorCode = (int)CustomHttpStatusCodeEnum.SessionExpired;
                                        log.ErrorDescription = "Session is expired";
                                        ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                                        HandleUnauthorizedSingleSessionRequest(actionContext, (HttpStatusCode)CustomHttpStatusCodeEnum.SessionExpired, "This session is expired as token is exceeded 30 minutes");
                                        return;
                                    }

                                    tokenData.IsValid = true;
                                    tokenData.ExpiredDate = DateTime.Now.AddMinutes(30);
                                    activeLoginTokens.Update(tokenData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\TameenkAuthorizeAttribute_Exception_" + log.ServerIP + "_" + log.UserIP + "___" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());

                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                FormsAuthentication.SignOut();
                base.HandleUnauthorizedRequest(actionContext);
                log.ErrorCode = 401;
                log.ErrorDescription = ex.ToString();
                ForbiddenRequestLogDataAccess.AddForbiddenRequestLog(log);
                return;
            }
        }

        public void HandleUnauthorizedSingleSessionRequest(HttpActionContext context, HttpStatusCode statusCode, string message)
        {
            context.Response = context.ControllerContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}
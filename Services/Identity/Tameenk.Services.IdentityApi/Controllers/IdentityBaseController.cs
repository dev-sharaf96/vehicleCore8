using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Loggin.DAL;
using Tameenk.Resources;
using Tameenk.Resources.WebResources;
using Tameenk.Services.IdentityApi.Output;

namespace Tameenk.Services.IdentityApi.Controllers
{
    /// <summary>
    /// IdentityBaseController
    /// </summary>
    public class IdentityBaseController:BaseApiController
    {
        /// <summary>
        /// Generic output Handler Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="output"></param>
        /// <param name="logType"></param>
        /// <param name="code"></param>
        /// <param name="logMessage"></param>
        /// <param name="culture"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        public IHttpActionResult OutputHandler<T>(Output<T> output, dynamic logType, Output<T>.ErrorCodes code, string logMessage, string culture, string exceptionMessage = null)

        {
           // QueryResources query = new QueryResources();
            output.ErrorCode = code;
            output.ErrorDescription = WebResources.ResourceManager.GetString(logMessage, CultureInfo.GetCultureInfo(culture));
            if (logType.GetType() == typeof(RegistrationRequestsLog))
            {
                if (logType is RegistrationRequestsLog registrationRequestsLog)
                {
                    registrationRequestsLog.ErrorCode = (int)code;
                    registrationRequestsLog.ErrorDescription = !string.IsNullOrEmpty(exceptionMessage) ? exceptionMessage : WebResources.ResourceManager.GetString(logMessage, CultureInfo.GetCultureInfo("en"));
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(registrationRequestsLog);
                }
            }
            else if (logType.GetType() == typeof(LoginRequestsLog))
            {
                if (logType is LoginRequestsLog loginRequestsLog)
                {
                    loginRequestsLog.ErrorCode = (int)code;
                    loginRequestsLog.ErrorDescription = !string.IsNullOrEmpty(exceptionMessage) ? exceptionMessage : WebResources.ResourceManager.GetString(logMessage, CultureInfo.GetCultureInfo("en"));
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(loginRequestsLog);
                }
            }
            else if (logType.GetType() == typeof(ProfileRequestsLog))
            {
                if (logType is ProfileRequestsLog profileRequestsLog)
                {
                    profileRequestsLog.ErrorCode = (int)code;
                    profileRequestsLog.ErrorDescription = logMessage;
                    // output.ErrorDescription = "      profileRequestsLog.ErrorDescription : " + profileRequestsLog.ErrorDescription;
                    //if (!string.IsNullOrEmpty(profileRequestsLog.ErrorDescription))
                    //    output.ErrorDescription = profileRequestsLog.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileRequestsLog);
                }
            }
            else if (logType.GetType() == typeof(PromotionRequestLog))
            {
                if (logType is PromotionRequestLog promotionRequestLog)
                {
                    promotionRequestLog.ErrorCode = (int)code;
                    promotionRequestLog.ErrorDescription = !string.IsNullOrEmpty(exceptionMessage) ? exceptionMessage : WebResources.ResourceManager.GetString(logMessage, CultureInfo.GetCultureInfo("en"));
                    // output.ErrorDescription = "      profileRequestsLog.ErrorDescription : " + profileRequestsLog.ErrorDescription;
                    output.ErrorDescription = promotionRequestLog.ErrorDescription;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(promotionRequestLog);
                }
            }

            return Single(output);
        }
    }
}
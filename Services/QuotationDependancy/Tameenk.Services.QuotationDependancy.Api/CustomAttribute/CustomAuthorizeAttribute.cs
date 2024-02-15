using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Tameenk.Services.QuotationDependancy.Api
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null || !actionContext.Request.Headers.Authorization.ToString().StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string[] credentials = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(actionContext.Request.Headers.Authorization.ToString().Substring(6))).Split(':');
                if (credentials == null || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    if (!VaidateUser(credentials[0], credentials[1]))
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }

        private static bool VaidateUser(string username, string password)
        {
            // Check if it is valid credential  
            return username == WebConfigurationManager.AppSettings["UserName"] && password == WebConfigurationManager.AppSettings["Password"];
        }

        public void HandleUnauthorizedSingleSessionRequest(HttpActionContext context, HttpStatusCode statusCode, string message)
        {
            context.Response = context.ControllerContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}

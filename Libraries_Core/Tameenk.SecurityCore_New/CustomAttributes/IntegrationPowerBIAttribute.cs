using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Tameenk.Services.InsuranceCompaniesCallBack.Services;

namespace Tameenk.Security.CustomAttributes
{
  public  class IntegrationPowerBIAttribute : AuthorizeAttribute
    {
        public override  void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                // Gets header parameters  
               string authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                // Gets username and password  
                string usrename = originalString.Split(':')[0];
                string password = originalString.Split(':')[1];

                // Validate username and password
                if (!VaidateUser(usrename, password))
                {
                    // returns unauthorized error  
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }

        }
        private bool VaidateUser(string username, string password)
        {
            // Check if it is valid credential  
            return username == WebConfigurationManager.AppSettings["UserName"] && password == WebConfigurationManager.AppSettings["Password"];
        }
        
    }
}

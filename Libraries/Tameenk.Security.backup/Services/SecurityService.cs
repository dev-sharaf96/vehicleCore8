using System.Web.Configuration;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Services
{
    public class SecurityService
    {
        public static bool VaidateUser(string username, string password)
        {
            // Check if it is valid credential  
            return username == WebConfigurationManager.AppSettings["UploadNotificationUID"] && password == WebConfigurationManager.AppSettings["UploadNotificationPWD"]; 
        }
    }
}
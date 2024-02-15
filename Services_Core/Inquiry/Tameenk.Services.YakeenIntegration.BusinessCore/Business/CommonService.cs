using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Loggin.DAL;
using System.Diagnostics;
using System.Reflection;
using System.Web;

using log4net.Core;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.YakeenIntegration.Business
{
   public static class CommonService
    {
     

        public static void SetYakeenOutput(int statusCode, string description, ref YakeenOutputModel model)
        {
            model.StatusCode = statusCode;
            model.Description = description;
          ///  LogException(model);
        }

        private static void LogException(YakeenOutputModel model)
        {
            try
            {
         
                //Todo :: Trace this 
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InquiryRequestLog log = new InquiryRequestLog()
                {
                    ErrorCode = model.StatusCode,
                    ErrorDescription = model.Description,
                    //MethodName = stackTrace.GetFrame(1).GetMethod().Name, 
                    MethodName = methodBase.Name,
                    CreatedDate = DateTime.Now,
                    UserIP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.Headers["User-Agent"].ToString(),
                  //  CalledUrl = HttpContext.Current.Request.Url.OriginalString,

                };
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
            }
            catch(Exception ex)
            {
                throw ex;
                
            }
            


        }
    }
}

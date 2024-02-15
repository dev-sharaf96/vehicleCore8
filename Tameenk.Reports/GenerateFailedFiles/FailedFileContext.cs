using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data.DAL;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.PolicyApi.Services;

namespace GenerateFailedFiles
{
   public class FailedFileContext
    {
       
        public static void GenerateFailedFiles()
        {
            ErrorLogging logging = new ErrorLogging();
            try
            {
                logging.LogDebug("1");
                var failedFiles = CheckoutDetailDataAccess.GetFailedFiles(60 * 60 * 60);
                logging.LogDebug("failedFiles.count "+failedFiles.Count);
                ErrorLogger.LogDebug("PolicyApiUrl is " + Utilities.GetAppSetting("PolicyApiUrl") + "policy/GetPolicyFile");
                if (failedFiles != null)
                {
                    HttpClient client = new HttpClient();
                    foreach (var file in failedFiles)
                    {
                        var policy = ServiceRequestLogDataAccess.GetPolicyByRefernceId(file.ReferenceId);
                        if (policy != null)
                        {
                            logging.LogDebug("policy is not null");
                            var policyResponse = JsonConvert.DeserializeObject<PolicyResponse>(policy.ServiceResponse);
                            var formParamters = new Dictionary<string, string>();
                            //formParamters.Add("referenceId", policy.ReferenceId);
                            //formParamters.Add("companyId", policy.CompanyID?.ToString());
                            formParamters.Add("policyResponseMessage", JsonConvert.SerializeObject(policyResponse));
                            var content = new FormUrlEncodedContent(formParamters);
                            var result = client.PostAsync(Utilities.GetAppSetting("PolicyApiUrl") + "policy/GetPolicyFile", content).Result;
                            ErrorLogger.LogDebug("result is " + result?.ToString());
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                logging.LogError(exp.Message,exp,false);
            }
        }

    }
}

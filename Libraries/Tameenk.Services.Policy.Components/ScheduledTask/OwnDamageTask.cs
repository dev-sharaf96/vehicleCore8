using iTextSharp.text.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Policies;

namespace Tameenk.Services.Policy.Components
{
    public class OwnDamageTask : ITask
    {
        #region Fields
        private readonly TameenkConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly IPolicyNotificationContext _policyNotificationContext;
        private readonly IPolicyContext _policyContext;
        #endregion

        #region Ctor
        public OwnDamageTask(
            IHttpClient httpClient,
            TameenkConfig config, 
            IPolicyContext PolicyContext,
            IPolicyNotificationContext policyNotificationContext
            )
        {
            _config = config;
            _httpClient = httpClient;
            _policyNotificationContext = policyNotificationContext;
            _policyContext = PolicyContext;

        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            string exception = string.Empty;
            var policiesToProcess = _policyNotificationContext.GetFromOwnDamageQueue(out exception);
            if (!string.IsNullOrEmpty(exception))
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\OwnDamageTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_getpoliciesToProcessError.txt", " Exception is:" + exception.ToString());
                return;
            }
            foreach (var policy in policiesToProcess)
            {
                try
                {
                    var output = _policyContext.SendOwnDamageSmsMsg(policy, out exception, policy.SelectedLanguage == 2 ? "en" : "ar");
                    if (output.ErrorCode == 1)
                    {
                        policy.ProcessedOn = DateTime.Now;
                        policy.ErrorDescription = "Success";
                    }
                    else
                    {
                        policy.ErrorDescription = output.ErrorDescription;
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\OwnDamageTask_ExceptionBlock_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_getpoliciesToProcessError.txt", " Exception is:" + ex.ToString());
                    policy.ModifiedDate = DateTime.Now;
                    policy.ErrorDescription = ex.ToString();
                }
                finally
                {
                    exception = string.Empty;
                    bool value = _policyNotificationContext.GetAndUpdateOwnDamageQueue(policy, out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\damage_FinallyBlock_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                    }
                }
            }

            #endregion
        }
    }
}

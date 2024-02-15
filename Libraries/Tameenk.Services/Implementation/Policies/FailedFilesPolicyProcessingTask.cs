using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class FailedFilesPolicyProcessingTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IHttpClient _httpClient;
        private readonly INotificationService _notificationService;
        #endregion

        #region Ctor
        public FailedFilesPolicyProcessingTask(IPolicyProcessingService policyProcessingService,
            IHttpClient httpClient,
            ILogger logger,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            INotificationService notificationService,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository)
        {
            _logger = logger;
            _config = config;
            _policyProcessingService = policyProcessingService;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _notificationService = notificationService;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            var policiesToProcess = _policyProcessingService.GetQueueForFailedFilesPolicyProcessingTask(null, null, true, true, maxTrials);
            foreach (var policy in policiesToProcess)
            {
                //policy.Chanel = "RetrialMechanism-FailedFilesPolicyProcessingTask";
                DateTime dtBefore = DateTime.Now;
                try
                {
                    var accessToken = GetAccessToken();
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        policy.ErrorDescription = "accessToken is empty";
                        continue;
                    }
                    var checkout = _checkoutDetailRepository.Table.Where(x => x.ReferenceId == policy.ReferenceId).FirstOrDefault();
                    if (checkout == null)
                    {
                        policy.ErrorDescription = "These is no checkout details object for this policy";
                        continue;
                    }
                    // escape successful policies
                    if (checkout.PolicyStatusId == 4)
                    {
                        policy.ProcessedOn = DateTime.Now;
                        continue;
                    }
                    if (checkout.PolicyStatusId != 6 && checkout.PolicyStatusId != 7)
                    {
                        continue;
                    }
                    // redownload or regenerate the failed files only ( without calling the insuranc company again)
                    string path = $"{_config.Policy.Url + "policy/GetPolicyFile"}?referenceId={policy.ReferenceId}&channel=RetrialMechanism";
                    var result =  _httpClient.GetAsync(path, accessToken).Result;
                    var responseStr = await result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(responseStr) && responseStr.ToLower().Trim() == "success")
                    {
                        policy.ErrorDescription = "Success";
                        policy.ProcessedOn = DateTime.Now;
                    }
                    else
                    {
                        policy.ErrorDescription = "Failed to Get Pdf file. response is " + responseStr;
                    }
                }
                catch (Exception ex)
                {
                    policy.ErrorDescription = ex.GetBaseException().ToString();
                }
                finally
                {
                    policy.ProcessingTries = policy.ProcessingTries + 1;
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    _policyProcessingService.UpdatePolicyProcessingQueue(policy);
                }
            }
            
        }

        #endregion
        private string GetAccessToken()
        {
            try
            {
                var formParamters = new Dictionary<string, string>();
                formParamters.Add("grant_type", "client_credentials");
                formParamters.Add("client_Id", _config.Identity.ClientId);
                formParamters.Add("client_secret", _config.Identity.ClientSecret);

                var content = new FormUrlEncodedContent(formParamters);
                var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                postTask.ConfigureAwait(false);
                postTask.Wait();
                var response = postTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                    return result.access_token;
                }
                return "";

            }
            catch (Exception ex)
            {
                var logId = DateTime.Now.GetTimestamp();
                _logger.Log($"PolicyProcessingTask -> GetAccessToken [key={logId}]", ex);
                return "";
            }
        }
        public class AccessTokenResult
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }
            [JsonProperty("expires_in")]
            public int expires_in { get; set; }
        }
    }
   
    
}

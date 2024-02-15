using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class SendFailurePolicyTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IHttpClient _httpClient;
        #endregion

        #region Ctor
        public PolicyProcessingTask(IPolicyProcessingService policyProcessingService, IHttpClient httpClient, ILogger logger, TameenkConfig config, IRepository<ScheduleTask> scheduleTaskRepository)
        {
            _logger = logger;
            _config = config;
            _policyProcessingService = policyProcessingService;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials)
        {
            //int maxTries = 1;
            var policiesToProcess = _policyProcessingService.GetPolicyProcessingQueue(null, null, true, true, maxTrials);
            var accessToken = GetAccessToken();
            foreach (var policy in policiesToProcess)
            {
                try
                {
                    _logger.Log($"PolicyProcessingTask >>> call policy generation service <<< (Reference id : {policy.ReferenceId}, policy queue Id : {policy.Id}, trial no : {policy.ProcessingTries})");
                    var url = $"{_config.Policy.PolicyAndInvoiceGeneratorApiUrl}?referenceId={policy.ReferenceId}&language=Ar";
                    var response = _httpClient.GetAsync(url, accessToken).Result;
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(responseString))
                    {


                        var responseModel = JsonConvert.DeserializeObject<ReposneModel>(responseString);
                        if (responseModel.Data)
                        {
                            _logger.Log($"PolicyProcessingTask >>> call policy generation service succeeded <<< (Reference id : {policy.ReferenceId}, policy queue Id : {policy.Id}, response : {responseString}, trial no : {policy.ProcessingTries})");
                            policy.ProcessedOnUtc = DateTime.UtcNow;
                        }
                        else
                        {
                            _logger.Log($"PolicyProcessingTask >>> call policy generation service failed <<< (Reference id : {policy.ReferenceId}, policy queue Id : {policy.Id}, response : {responseString}, trial no : {policy.ProcessingTries})");
                        }
                    }
                    else
                    {
                        _logger.Log($"PolicyProcessingTask >>> call policy generation service return nothing <<< (Reference id : {policy.ReferenceId}, policy queue Id : {policy.Id}, trial no : {policy.ProcessingTries})");
                    }

                }
                catch (Exception ex)
                {
                    _logger.Log($"Policy Generation Service - Reference No. {policy.ReferenceId}, policy queue Id : {policy.Id}, trial no : {policy.ProcessingTries}", ex);
                }
                finally
                {
                    policy.ProcessingTries = policy.ProcessingTries + 1;
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
                var logId = DateTime.UtcNow.GetTimestamp();
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
    public class ErrorModel
    {

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
    public class ReposneModel
    {
        /// <summary>
        /// Get or set the data.
        /// </summary>
        [JsonProperty("data")]
        public bool Data { get; set; }

        /// <summary>
        /// Get or set the total count of returned data.
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Get or set the list of errors.
        /// </summary>
        [JsonProperty("errors")]
        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}

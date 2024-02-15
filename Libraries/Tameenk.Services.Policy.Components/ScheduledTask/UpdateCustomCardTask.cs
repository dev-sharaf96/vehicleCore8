using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
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

namespace Tameenk.Services.Policy.Components
{
    public class UpdateCustomCardTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly IPolicyNotificationContext _policyNotificationContext;
        private readonly ICustomCardQueueService _customCardQueueService;
        private readonly IRepository<CustomCardQueue> _customCardRepository;
        #endregion

        #region Ctor
        public UpdateCustomCardTask(
            IHttpClient httpClient,
            ILogger logger,
            TameenkConfig config,
            IPolicyNotificationContext policyNotificationContext
            , ICustomCardQueueService customCardQueueService
            , IRepository<CustomCardQueue> customCardRepository
            )
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
            _policyNotificationContext = policyNotificationContext;
            _customCardQueueService = customCardQueueService;
            _customCardRepository = customCardRepository;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            string exception = string.Empty;
            var policiesToProcess = _customCardQueueService.GetFromCustomCardQueue(out exception);
            if(!string.IsNullOrEmpty(exception))
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\UpdateCustomCardTaskTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                return;
            }
            foreach (var policy in policiesToProcess)
            {
                if (policy.CompanyID == 7 && policy.Channel.ToLower() != Channel.autoleasing.ToString())
                    continue;
                if (policy.CompanyID == 9&& policy.Channel.ToLower() == Channel.autoleasing.ToString())
                    continue;
                DateTime dtBefore = DateTime.Now;
                try
                {
                    var output = _policyNotificationContext.UpdateCustomCard(policy);
                  
                    policy.ErrorCode = (int)output.ErrorCode;
                    policy.ErrorDescription = output.ErrorDescription;
                    policy.ServiceRequest = output.ServiceRequest;
                    policy.ServiceResponse = output.ServiceResponse;
                    policy.PolicyStatusId = output.PolicyStatusId;
                    if (output.ErrorCode == UpdateCustomCardOutput.ErrorCodes.Success)
                    {
                        policy.ProcessedOn = DateTime.Now;
                        policy.PolicyStatusId = 4;
                    }

                }
                catch (Exception ex)
                {
                    policy.ModifiedDate = DateTime.Now;
                    policy.ErrorCode = (int)UpdateCustomCardOutput.ErrorCodes.ServiceDown;
                    policy.ErrorDescription = ex.ToString();
                    policy.PolicyStatusId = 5;
                }
                finally
                {
                    exception = string.Empty;
                    policy.ServerIP = Utilities.GetInternalServerIP();
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    bool value  = _customCardQueueService.GetAndUpdateCustomCardProcessingQueue(policy.Id, policy, policy.ServerIP,out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\UpdateCustomCardTaskTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                    }
                }
            }

            #endregion
        }
    }
}
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
    public class PolicyProcessingTask : ITask
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
        private readonly IPolicyContext _policyContext;
        #endregion

        #region Ctor
        public PolicyProcessingTask(IPolicyProcessingService policyProcessingService,
            IHttpClient httpClient,
            ILogger logger,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            INotificationService notificationService,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository, IPolicyContext policyContext)
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
            _policyContext = policyContext;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            //int maxTries = 1;
            //string serverIP = Utilities.GetAppSetting("ServerIP");
            //var policiesToProcess = _policyProcessingService.GetQueueForPolicyProcessingTaskWithOutPdfTemplate(null, null, true, true, maxTrials);
            //foreach (var policy in policiesToProcess)
            //{
            //    DateTime dtBefore = DateTime.Now;
            //    try
            //    {
            //        var checkout = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == policy.ReferenceId).FirstOrDefault();
            //        var selectedLang = LanguageTwoLetterIsoCode.Ar;
            //        if (checkout == null)
            //        {
            //            policy.ErrorDescription = "These is no checkout details object for this policy";
            //            continue;
            //        }
            //        if (checkout.PolicyStatusId == 10)
            //        {
            //            policy.ModifiedDate = DateTime.Now;
            //            continue;
            //        }
            //        // escape successful policies
            //        if (checkout.PolicyStatusId == 4)
            //        {
            //            policy.ProcessedOn = DateTime.Now;
            //            continue;
            //        }
            //        if (checkout.PolicyStatusId == 6 || checkout.PolicyStatusId == 7)
            //        {
            //            continue;
            //        }
                    
            //        if (checkout.SelectedLanguage == LanguageTwoLetterIsoCode.En)
            //            selectedLang = LanguageTwoLetterIsoCode.En;

            //        var responseModel = _policyContext.SubmitPolicy(policy.ReferenceId, selectedLang,Utilities.GetInternalServerIP(),Channel.Portal.ToString());
            //        if (responseModel.ErrorCode == 1)
            //        {
            //            policy.ProcessedOn = DateTime.Now;
            //        }
            //        else if (responseModel.ErrorCode == 12)
            //        {
            //            policy.ErrorDescription = responseModel.ErrorDescription;
            //        }
            //        else if (!string.IsNullOrEmpty(responseModel.ErrorDescription))
            //        {
            //           policy.ErrorDescription = responseModel.ErrorDescription;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        policy.ErrorDescription = ex.ToString();
            //    }
            //    finally
            //    {
            //        policy.ProcessingTries = policy.ProcessingTries + 1;
            //        policy.ServerIP = serverIP;
            //        DateTime dtAfter = DateTime.Now;
            //        policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
            //        _policyProcessingService.UpdatePolicyProcessingQueue(policy);
            //    }
            //}
        }
       
      
        #endregion
    }

}
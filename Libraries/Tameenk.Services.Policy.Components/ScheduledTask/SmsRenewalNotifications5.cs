﻿using Newtonsoft.Json;
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
    public class SmsRenewalNotifications5 : ITask
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
        public SmsRenewalNotifications5(IPolicyProcessingService policyProcessingService,
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
            //if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 22 && DateTime.Now.Day == 27) 
            //{
            //        DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(11);
            //        DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(13);
            //        _policyContext.SendSmsRenewalNotifications(start, end, 5, "SmsRenewalNotifications5");
            //}
        }
      
        #endregion
    }

}
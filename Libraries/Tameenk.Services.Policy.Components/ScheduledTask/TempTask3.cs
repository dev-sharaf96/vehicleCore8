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
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class TempTask3 : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IHttpClient _httpClient;
        private readonly ICheckoutContext _checkoutContext;
        private readonly IPolicyContext _policyContext;
        private readonly IPolicyProcessingService _policyProcessingService;

        #endregion

        #region Ctor
        public TempTask3(IPolicyProcessingService policyProcessingService, IHttpClient httpClient,
            ILogger logger,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository, ICheckoutContext checkoutContext, IPolicyContext policyContext)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _checkoutContext = checkoutContext;
            _policyContext = policyContext;
            _policyProcessingService = policyProcessingService;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                var checkoutList = _checkoutContext.GetCheckOutDetailsTemp3("");
                
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask3Exp.txt", exp.ToString());
            }
        }


        #endregion
    }

}
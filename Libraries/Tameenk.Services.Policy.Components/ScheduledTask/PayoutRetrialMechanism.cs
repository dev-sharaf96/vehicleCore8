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
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class PayoutRetrialMechanism : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IHttpClient _httpClient;
        private readonly IHyperpayPaymentService _hyperpayPaymentService;

        #endregion

        #region Ctor
        public PayoutRetrialMechanism(IHttpClient httpClient,
            ILogger logger,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository, IHyperpayPaymentService hyperpayPaymentService)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _hyperpayPaymentService = hyperpayPaymentService;



        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                string exception = string.Empty;
                _hyperpayPaymentService.RetryFailedSplitOperation(out exception);

                if(!string.IsNullOrEmpty(exception))
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\PayoutRetrialMechanism.txt", exception);

                }
            }
            catch(Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\PayoutRetrialMechanism.txt", exp.ToString());

            }
        }
       
      
        #endregion
    }

}
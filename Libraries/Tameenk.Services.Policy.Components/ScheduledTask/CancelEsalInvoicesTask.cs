using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Payment.Esal.Component;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class CancelEsalInvoicesTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IHttpClient _httpClient;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly IRepository<SadadRequest> _sadadrequestRepo;
        private readonly IRepository<QuotationResponse> _quotationResponseRepo;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IEsalPaymentService _esalPaymentService;
        #endregion

        #region Ctor
        public CancelEsalInvoicesTask(IPolicyProcessingService policyProcessingService, IHttpClient httpClient, ILogger logger, TameenkConfig config, IRepository<ScheduleTask> scheduleTaskRepository
            , IRepository<SadadRequest> sadadrequestRepo
            , ISadadPaymentService sadadPaymentService
            , IRepository<QuotationResponse> quotationResponseRepo
            , IRepository<CheckoutDetail> checkoutDetailRepo
            , IRepository<Invoice> invoiceRepo,
             IEsalPaymentService esalPaymentService
            )
        {
            _logger = logger;
            _config = config;
            _policyProcessingService = policyProcessingService;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _sadadrequestRepo = sadadrequestRepo;
            _sadadPaymentService = sadadPaymentService;
            _quotationResponseRepo = quotationResponseRepo;
            _checkoutDetailRepo = checkoutDetailRepo;
            _invoiceRepo = invoiceRepo;
            _esalPaymentService = esalPaymentService;

        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                _esalPaymentService.CancelExpiredInvoices();
            }
            catch (Exception exp)
            {
                _logger.Log("CancelEsalInvoicesTask error ", exp);
            }
        }

        #endregion



    }

}

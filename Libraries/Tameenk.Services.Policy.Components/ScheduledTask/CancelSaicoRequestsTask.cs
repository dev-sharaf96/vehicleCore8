using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class CancelSaicoRequestsTask : ITask
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
        #endregion

        #region Ctor
        public CancelSaicoRequestsTask(IPolicyProcessingService policyProcessingService, IHttpClient httpClient, ILogger logger, TameenkConfig config, IRepository<ScheduleTask> scheduleTaskRepository
            , IRepository<SadadRequest> sadadrequestRepo
            , ISadadPaymentService sadadPaymentService
            , IRepository<QuotationResponse> quotationResponseRepo
            , IRepository<CheckoutDetail> checkoutDetailRepo
            , IRepository<Invoice> invoiceRepo
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

        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                if (DateTime.Now.Hour >= 23 && DateTime.Now.Minute >= 45)
                {
                    var start =new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0);
                    var end =new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,23,59,59);
                    var quotations = (from checkoutDetail in _checkoutDetailRepo.Table
                                      join quotationResponse in _quotationResponseRepo.Table on checkoutDetail.ReferenceId equals quotationResponse.ReferenceId
                                      join sadadrequest in _sadadrequestRepo.Table on checkoutDetail.ReferenceId equals sadadrequest.ReferenceId
                                      where quotationResponse.CreateDateTime>= start&& quotationResponse.CreateDateTime <= end
                                      && checkoutDetail.PaymentMethodId == 2
                                      && checkoutDetail.PolicyStatusId == 1
                                      && checkoutDetail.InsuranceCompanyName == "SAICO" && quotationResponse.InsuranceCompanyId == 21
                                      select new { quotationResponse, sadadrequest }
                                ).ToList();

                    foreach (var request in quotations)
                    {
                        SadadRequest sadadRequest = new SadadRequest
                        {
                            BillerId = _config.Sadad.BillerId,
                            ExactFlag = _config.Sadad.ExactFlag,
                            CustomerAccountNumber = request.sadadrequest.CustomerAccountNumber,
                            CustomerAccountName = request.sadadrequest.CustomerAccountName,
                            BillAmount = Math.Round(request.sadadrequest.BillAmount, 2, MidpointRounding.AwayFromZero),
                            BillOpenDate = request.sadadrequest.BillOpenDate,
                            BillDueDate = request.sadadrequest.BillDueDate,
                            BillExpiryDate = request.sadadrequest.BillExpiryDate,
                            BillCloseDate = request.sadadrequest.BillCloseDate
                        };
                        var sadadResponse = _sadadPaymentService.ExecuteSadadPayment(sadadRequest, false, request.sadadrequest.CompanyId, request.sadadrequest.CompanyName, request.sadadrequest.ReferenceId,string.Empty);
                        request.sadadrequest.IsActive = false;
                        _sadadrequestRepo.Update(request.sadadrequest);

                        // modifiy created date to make quotation expire 
                        request.quotationResponse.CreateDateTime = DateTime.Now.AddDays(-1);
                    }
                    if (quotations != null)
                    {
                        // update quotation in database 
                        _quotationResponseRepo.Update(quotations.Select(x => x.quotationResponse));
                    }
                }
                

            }
            catch (Exception exp)
            {
                //Failed
                _logger.Log("CancelSaicoRequestsTask error ", exp);
            }
        }

        #endregion



    }

}

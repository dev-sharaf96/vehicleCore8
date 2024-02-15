using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Tasks;
using System.Linq;
using Tameenk.Core.Domain.Dtos;
using System.Globalization;

namespace Tameenk.Services.Implementation.Payments
{
    public class CancelSadadTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IHttpClient _httpClient;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly IRepository<SadadRequest> _sadadrequestRepo;
        #endregion

        #region Ctor
        public CancelSadadTask(IPolicyProcessingService policyProcessingService, IHttpClient httpClient, ILogger logger, TameenkConfig config, IRepository<ScheduleTask> scheduleTaskRepository, IRepository<SadadRequest> sadadrequestRepo, ISadadPaymentService sadadPaymentService)
        {
            _logger = logger;
            _config = config;
            _policyProcessingService = policyProcessingService;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _sadadrequestRepo = sadadrequestRepo;
            _sadadPaymentService = sadadPaymentService;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
           try
            {
                var requests = _sadadrequestRepo.Table.Where(a => a.BillExpiryDate <= DateTime.Now && a.IsActive == true).ToList();

                //CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");
                //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
                //var convertDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", cultureEnglish.DateTimeFormat);
                //var format = "dd/MM/yyyy HH:mm:ss";
                //var requestDateTime = DateTime.ParseExact(convertDate, format, cultureEnglish);
                foreach (var request in requests)
                {
                    SadadRequest sadadRequest = new SadadRequest
                    {
                        BillerId = _config.Sadad.BillerId,
                        ExactFlag = _config.Sadad.ExactFlag,
                        CustomerAccountNumber =request.CustomerAccountNumber,
                        CustomerAccountName = request.CustomerAccountName,
                        BillAmount = Math.Round(request.BillAmount, 2, MidpointRounding.AwayFromZero),
                        BillOpenDate =request.BillOpenDate,
                        BillDueDate = request.BillDueDate,
                        BillExpiryDate = request.BillExpiryDate,
                        BillCloseDate = request.BillCloseDate
                    };
                    var sadadResponse = _sadadPaymentService.ExecuteSadadPayment(sadadRequest, false);

                   //if (sadadResponse != null)
                   // {
                     //   if (sadadResponse.ErrorCode == 0)
                       // {
                            request.IsActive = false;
                            _sadadrequestRepo.Update(request);
                       // }
                   // }
                   
                }
            }
            catch (Exception exp)
            {
                //Failed
                _logger.Log("CancelSadadTask error ",exp);
            }
        }

        #endregion


        
    }
 
}

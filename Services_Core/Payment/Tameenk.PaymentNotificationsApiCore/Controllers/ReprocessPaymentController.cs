using System;
using Tameenk.Api.Core;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Logging;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class ReprocessPaymentController : BaseApiController
    {
        #region Fields
        private readonly IPayfortNotificationsServices _payfortNotificationServices;
        private readonly ILogger _logger;
        private readonly IPayfortPaymentService _payfortPaymentService;
        #endregion

        #region Ctor
        public ReprocessPaymentController(IPayfortNotificationsServices payfortNotificationsServices, ILogger logger, IPayfortPaymentService payfortPaymentService
            )
        {
            _payfortNotificationServices = payfortNotificationsServices;
            _logger = logger;
            _payfortPaymentService = payfortPaymentService;
        }
        #endregion

        #region Action
        //[HttpGet]
        //[Route("api/ReprocessPayment/reprocess-payment/{merchantReference}")]
        //public IHttpActionResult ReProcessPayment(string merchantReference)
        //{
        //    try
        //    {
        //        //construct the request by adding required keys in Form.Keys
        //        _payfortPaymentService.ReProcessPayment(merchantReference);
        //        _logger.LogIntegrationTransaction(merchantReference, "OK,200", "api/ReprocessPayment/reprocess-payment/{merchantReference}", 1);
        //        return Ok(true);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogIntegrationTransaction(merchantReference, ex.ToString(), "api/ReprocessPayment/reprocess-payment/{merchantReference}", 2);
        //        return Error("An error happend while reprocessing the payment.");
        //    }



        //}

        #endregion


    }
}

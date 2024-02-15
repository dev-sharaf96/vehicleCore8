using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Payment.Esal.Component;
using Tameenk.Payment.Esal.Component.Model.Settlement;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Repository;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.PaymentNotificationsApi.Services.Helpers;
using Tameenk.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsalController : ControllerBase
    {
        private readonly IEsalPaymentService _esalPaymentService;
        private readonly ILogger _logger;
        private readonly IPaymentNotificationSmsSender _smsSender;

        public EsalController(IEsalPaymentService esalPaymentService, ILogger logger,
                        IPaymentNotificationSmsSender smsSender)
        {
            _esalPaymentService = esalPaymentService;
            _logger = logger;
            _smsSender = smsSender;

        }

        ////Receive SADAD Bill ID 
        //[ResponseType(typeof(ResponseMessage))]
        //[Route("api/Esal/invoice/upload/notification")]
        //public IHttpActionResult Post([FromBody]EsalUploadInvoiceNotification invoiceNotification)
        //{
        //    try
        //    {
        //        EsalError esal = new EsalError();
        //        if (invoiceNotification != null && invoiceNotification.Invoices != null)
        //        {
        //            string ip = Utilities.GetUserIPAddress();
        //            esal = _esalPaymentService.UpdateInvoiceWithSadatID(invoiceNotification,ip);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //        if (esal.Code != EsalOutput.ErrorCodes.Success.ToString())
        //        {
        //            return BadRequest(esal.Message);
        //        }
        //        return StatusCode(HttpStatusCode.NoContent);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.GetBaseException().Message);
        //    }
        //}

        ////Receive Payment Notification
        //[ResponseType(typeof(ResponseMessage))]
        //[Route("api/Esal/notification")]
        //public IHttpActionResult Post([FromBody]EsalPaymentNotification paymentNotification)
        //{
        //    try
        //    {
        //        EsalError esal = new EsalError();
        //        if (paymentNotification != null && paymentNotification.Payment != null)
        //        {

        //            CheckoutDetail checkoutDetail = new CheckoutDetail();
        //            esal = _esalPaymentService.UpdateInvoicePayment(paymentNotification, out checkoutDetail);
        //            if (esal.Code != EsalOutput.ErrorCodes.Success.ToString())
        //            {
        //                return BadRequest(esal.Message);
        //            }
        //            LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
        //                    LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;
        //            _smsSender.SendSms(checkoutDetail, decimal.Parse(paymentNotification.Payment.CurAmt), culture);
        //        }
        //        else
        //        {
        //            return BadRequest(esal.Message);
        //        }

        //        return StatusCode(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}

        //[ResponseType(typeof(ResponseMessage))]
        //[Route("api/Esal/settlement")]
        //public IHttpActionResult Post([FromBody]EsalSettlementModel esalSettlement)
        //{
        //    try
        //    {

        //        //EsalOutput esal = new EsalOutput();
        //        EsalOutput esal = _esalPaymentService.SaveEsalSettlement(esalSettlement);
        //        if (esal.ErrorCode != EsalOutput.ErrorCodes.Success)
        //        {
        //            return BadRequest(esal.ErrorDescription);
        //        }

        //        return StatusCode(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.GetBaseException().Message);
        //    }
        //}


    }
}

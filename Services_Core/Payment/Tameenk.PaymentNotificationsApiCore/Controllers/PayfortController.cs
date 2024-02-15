using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayfortController : ControllerBase
    {
        private readonly IPayfortNotificationsServices _payfortNotificationServices;
        private readonly ILogger _logger;

        public PayfortController(IPayfortNotificationsServices payfortNotificationsServices, ILogger logger)
        {
            _payfortNotificationServices = payfortNotificationsServices;
            _logger = logger;
        }

        //[HttpPost]
        //public IHttpActionResult Post()
        //{
        //    var formParams = HttpContext.Current.Request.Form.AllKeys.OrderBy(k => k).ToDictionary(k => k, v => HttpContext.Current.Request.Form[v]);
        //    _logger.Log($"PayfortController -> Post <<start>> form: {JsonConvert.SerializeObject(formParams)}");
        //    string outputParams = string.Empty;
        //    string inputParams = $"Request Form AllKeys : {Environment.NewLine} {string.Join(",", HttpContext.Current.Request.Form.AllKeys)}";
        //    inputParams = inputParams + $"request[signature] : {HttpContext.Current.Request["signature"]}";
        //    string methodPath = "PaymentNotificationsApi/PayfortController/Post";
        //    int resultStatus = 1;
        //    try
        //    {
        //        _payfortNotificationServices.SaveAndProcessPayfortNotification(HttpContext.Current.Request);
        //        _logger.Log($"PayfortController -> Post <<end>> end with success");
        //        outputParams = "OK, 200";
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log($"PayfortController -> Post an exception happend while processing the payment", ex);
        //        outputParams = ex.ToString();
        //        resultStatus = 2;
        //        return BadRequest();
        //    }
        //    finally
        //    {
        //        _logger.LogIntegrationTransaction(inputParams, outputParams, methodPath, resultStatus);
        //    }
        //}


    }
}

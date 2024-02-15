using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Implementation.Payments.Tabby;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TabbyWebhookController : ControllerBase
    {

        private readonly ICheckoutContext _checkoutContext;
        public TabbyWebhookController(ICheckoutContext checkoutContext)
        {
            _checkoutContext = checkoutContext;
        }

        // [ResponseType(typeof(EdaatResponseMessage))]
        [Route("~/api/TabbyWebhook/WebHook")]
        public IActionResult WebHookNotification([FromBody] TabbyWebhookModel tabbyWebHookModel)
        {
            try
            {

                var result = _checkoutContext.TabbyPaymentWebhook(tabbyWebHookModel);
                if (result.ErrorCode == TabbyOutput.ErrorCodes.Unauthorized)
                {
                    return Unauthorized();
                }
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest();
            }
        }
    }
}

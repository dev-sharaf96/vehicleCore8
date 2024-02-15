using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Implementation.Payments.Tabby;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class TabbyWebhookController : ApiController
    {

        private readonly ICheckoutContext _checkoutContext;
        public TabbyWebhookController(ICheckoutContext checkoutContext)
        {
            _checkoutContext = checkoutContext;
        }
         
       // [ResponseType(typeof(EdaatResponseMessage))]
        [Route("api/TabbyWebhook/WebHook")]
        public IHttpActionResult WebHookNotification([FromBody]TabbyWebhookModel tabbyWebHookModel)
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
            catch(Exception exp)
            {
                return BadRequest();
            }
        }
    }
}

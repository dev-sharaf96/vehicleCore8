using System;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.PaymentNotificationsApi.Services.Core;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class HyperPayController : ApiController
    {
        private readonly IHyperPayNotificationSersvice _hyperPayNotificationSersvice;
        public HyperPayController(IHyperPayNotificationSersvice hyperPayNotificationSersvice)
        {
            _hyperPayNotificationSersvice = hyperPayNotificationSersvice;
        }

        //Receive Payment Notification
        [ResponseType(typeof(HyperPayNotificationOutput))]
        [System.Web.Http.Route("api/HyperPay/notification")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult GetNotification([FromBody] HyperPaysplitModel model)
        {
            try
            {
                var iv = Request.Headers.GetValues("X-Initialization-Vector");
                var tag = Request.Headers.GetValues("X-Authentication-Tag");
                HyperPayNotificationOutput output = _hyperPayNotificationSersvice.HandleHyperPayNotification(model, iv, tag);
                if (output.Code == "401")
                {
                    return Unauthorized();
                }
                return Ok(output);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}

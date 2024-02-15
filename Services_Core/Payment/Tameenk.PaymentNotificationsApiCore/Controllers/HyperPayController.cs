using System;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HyperPayController : ControllerBase
    {
        private readonly IHyperPayNotificationSersvice _hyperPayNotificationSersvice;
        public HyperPayController(IHyperPayNotificationSersvice hyperPayNotificationSersvice)
        {
            _hyperPayNotificationSersvice = hyperPayNotificationSersvice;
        }

        //Receive Payment Notification
        [ResponseType(typeof(HyperPayNotificationOutput))]
        [Route("~/api/HyperPay/notification")]
        [HttpPost]
        public IActionResult GetNotification([FromBody] HyperPaysplitModel model)
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

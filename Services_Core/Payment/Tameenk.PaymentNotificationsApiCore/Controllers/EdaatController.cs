using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EdaatController : ControllerBase
    {

        private readonly IEdaatNotificationsServices edaatNotificationsServices;
        public EdaatController(IEdaatNotificationsServices edaatNotificationsServices)
        {
            this.edaatNotificationsServices = edaatNotificationsServices;
        }

        [ResponseType(typeof(EdaatResponseMessage))]
        [Route("~/api/Edaat/notification")]
        public IActionResult Post([FromBody] List<EdaatPayment> message)
        {
            try
            {
                EdaatResponseMessage result = edaatNotificationsServices.SaveAndProcessEdaatNotification(message);
                if (result.ErrorCode == 401)
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

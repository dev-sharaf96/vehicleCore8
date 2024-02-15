using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Services.Core;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class EdaatController : ApiController
    {

        private readonly IEdaatNotificationsServices edaatNotificationsServices;
        public EdaatController (IEdaatNotificationsServices edaatNotificationsServices)
        {
            this.edaatNotificationsServices = edaatNotificationsServices;
        }
         
        [ResponseType(typeof(EdaatResponseMessage))]
        [Route("api/Edaat/notification")]
        public IHttpActionResult Post([FromBody]List<EdaatPayment> message)
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
            catch(Exception exp)
            {
                return BadRequest();
            }
        }
    }
}

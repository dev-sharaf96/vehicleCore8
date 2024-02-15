using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.Notifications;

namespace Tameenk.Controllers
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private readonly INotificationServiceProvider _notificationServiceProvider;
        public NotificationsController(INotificationServiceProvider notificationServiceProvider)
        {
            _notificationServiceProvider = notificationServiceProvider;
        }
       
        [HttpPost]
        [Route("api/identity/RegisterToken")]
        public IHttpActionResult RegisterToken([FromBody]RegisterTokenModel registerTokenModel)
        {
            return Ok(_notificationServiceProvider.RegisterNotificationToken(registerTokenModel.UserId, registerTokenModel.Token));
            
        }
        

    }
}
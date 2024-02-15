using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Tameenk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationServiceProvider _notificationServiceProvider;
        public NotificationsController(INotificationServiceProvider notificationServiceProvider)
        {
            _notificationServiceProvider = notificationServiceProvider;
        }

        [HttpPost]
        [Route("~/api/identity/RegisterToken")]
        public IActionResult RegisterToken([FromBody] RegisterTokenModel registerTokenModel)
        {
            return Ok(_notificationServiceProvider.RegisterNotificationToken(registerTokenModel.UserId, registerTokenModel.Token));

        }
    }
}
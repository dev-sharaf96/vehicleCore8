using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tamkeen.bll.Services;

namespace Tameenk.Services
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var emailService = new EmailServices();
            return emailService.SendEmailAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
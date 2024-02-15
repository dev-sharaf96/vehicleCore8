using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Notifications;
using Tamkeen.bll.Services;

namespace Tameenk.Services
{
    public class SmsService : IIdentityMessageService
    {
        private readonly ISmsProvider _smsService;

        public SmsService(ISmsProvider smsService)
        {
            _smsService = smsService ?? throw new TameenkArgumentNullException(nameof(ISmsProvider));
        }

        public Task SendAsync(IdentityMessage message)
        {
            if (Utilities.GetAppSetting("SmsProvider") == "MobiShastra")
            {
               return _smsService.SendSmsMobiShastraAsync(message.Destination, message.Body, SMSMethod.Validation.ToString());
            }
            else
            {
                return _smsService.SendSmsAsync(message.Destination, message.Body, SMSMethod.Validation.ToString());
            }
        }
    }
}
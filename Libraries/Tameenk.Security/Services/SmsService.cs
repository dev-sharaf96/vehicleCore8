using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Notifications;

namespace Tameenk.Security.Services
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
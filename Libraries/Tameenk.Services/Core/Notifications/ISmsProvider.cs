using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Core.Notifications
{
    public interface ISmsProvider
    {
        Task SendSmsAsync(string phoneNumber, string message,string method);
        Task SendWhatsAppMessageAsync(string phoneNumber, string message,string method, string referenceId, string langCode);
        bool SendSmsSTC(string phoneNumber, string message, string method, out string exception);
        Task SendWhatsAppMessageForPolicyRenewalAsync(string phoneNumber, string message, string make, string model, string plateText, string url, string method, string referenceId, string langCode, string expiryDate);
        bool SendSmsMobiShastra(string phoneNumber, string message, string method, out string exception);
        Task SendSmsMobiShastraAsync(string phoneNumber, string message, string method);
        bool SendWhatsAppMessageForShareQuoteAsync(string phoneNumber, string url, string externalId,string lang, out string exception);
        SMSOutput SendSmsByMobiShastra(SMSModel model);
        SMSOutput SendSmsBySTC(SMSModel model);
        Task SendWhatsAppMessageUpdateCustomCardAsync(string phoneNumber, string message, string method, string referenceId, string langCode, string make, string model, string plateText);
    }
}

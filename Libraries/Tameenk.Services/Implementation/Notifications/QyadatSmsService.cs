using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Repository;

namespace Tameenk.Services.Implementation.Notifications
{
    public class QyadatSmsService : ISmsNotificationService
    {
        private static readonly HttpClient client;

        static QyadatSmsService()
        {
            client = new HttpClient();
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            phoneNumber = validatePhoneNumber(phoneNumber);

            var values = new Dictionary<string, string>
            {
                { "username", RepositoryConstants.QyadatSmsAccountUsername },
                { "password", RepositoryConstants.QyadatSmsAccountPassword },
                { "numbers", phoneNumber},
                { "message",  message},
                { "sender", RepositoryConstants.QyadatSmsAccountSender }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(RepositoryConstants.QyadatSmsServiceUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            int responseCode = -1;
            if (int.TryParse(responseString, out responseCode))
            {
                switch (responseCode)
                {
                    case (int)EQyadatSmsResponse.Success:
                        break;
                    case (int)EQyadatSmsResponse.InvalidNumber:
                        throw new Exception("Invalid receiver number.");
                    default:
                        // TODO: Log the detailed resonse from sms service and notify suger
                        break;
                }
            }
            else
            {
                // TODO: Log error reading response from sms service
            }
        }

        private string validatePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith(RepositoryConstants.InternationalPhoneCode))
                phoneNumber = phoneNumber.Substring(RepositoryConstants.InternationalPhoneCode.Length);
            else if (phoneNumber.StartsWith(RepositoryConstants.InternationalPhoneSymbol))
                phoneNumber = phoneNumber.Substring(RepositoryConstants.InternationalPhoneSymbol.Length);

            if (!phoneNumber.StartsWith(RepositoryConstants.SaudiInternationalPhoneCode))
            {
                if (phoneNumber.StartsWith(RepositoryConstants.Zero))
                    phoneNumber = phoneNumber.Substring(RepositoryConstants.Zero.Length);

                phoneNumber = RepositoryConstants.SaudiInternationalPhoneCode + phoneNumber;
            }

            return phoneNumber;
        }
    }
}

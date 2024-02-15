using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface IPaymentNotificationSmsSender
    {
        void SendSms(CheckoutDetail checkoutDetail, decimal paidAmount, LanguageTwoLetterIsoCode culture);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Services
{
    public class SmsNotificationServices
    {

        public SmsNotificationServices()
        {

        }
        public void SendPaymentSuccessSmsNotification(string productType, string companyName, string amount)
        {
            string smsBody = string.Format("تم بنجاح شراء تأمين {0} من شركة {1} بمبلغ {2}", productType, companyName, amount);
        }
    }
}
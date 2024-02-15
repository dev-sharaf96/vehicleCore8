using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    public class PaymentSuccessNotificationSmsModel
    {
        public string PhoneNumber { get; set; }
        public string ProductType { get; set; }
        public string CompanyName { get; set; }
        public string PaymentAmount { get; set; }
    }
}
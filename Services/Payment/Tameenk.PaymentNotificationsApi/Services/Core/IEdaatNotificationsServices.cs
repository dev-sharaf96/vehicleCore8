using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.PaymentNotificationsApi.Models;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface IEdaatNotificationsServices
    {
        EdaatResponseMessage SaveAndProcessEdaatNotification(List< EdaatPayment> message);
    }
}
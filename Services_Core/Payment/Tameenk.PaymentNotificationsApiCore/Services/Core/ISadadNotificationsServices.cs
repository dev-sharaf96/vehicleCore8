using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.PaymentNotificationsApi.Models;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface ISadadNotificationsServices
    {
        ResponseMessage SaveAndProcessSadadNotification(NotificationMessage message);

    }
}

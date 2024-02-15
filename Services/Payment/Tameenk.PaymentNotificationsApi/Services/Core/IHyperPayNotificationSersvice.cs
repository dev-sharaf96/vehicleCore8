using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface IHyperPayNotificationSersvice
    {
        HyperPayNotificationOutput HandleHyperPayNotification(HyperPaysplitModel model, IEnumerable<string> iv, IEnumerable<string> tag);
    }
}
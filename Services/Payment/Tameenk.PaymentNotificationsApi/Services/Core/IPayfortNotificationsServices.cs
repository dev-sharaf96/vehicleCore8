using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface IPayfortNotificationsServices
    {
        void SaveAndProcessPayfortNotification(HttpRequest request);
    }
}

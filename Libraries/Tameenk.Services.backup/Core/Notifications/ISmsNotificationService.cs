using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Notifications
{
    public interface ISmsNotificationService
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}

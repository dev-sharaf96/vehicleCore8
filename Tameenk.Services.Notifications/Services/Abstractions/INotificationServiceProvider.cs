using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Notifications
{
    public interface INotificationServiceProvider
    {
        Task<NotificationServiceOutput> SendNotification(string userId, string title, string body, string imageUrl = null);
        NotificationServiceOutput RegisterNotificationToken(string userId, string token);
        NotificationServiceOutput SendFireBaseNotification(string userId, string title, string body, string method, string referenceId, string channel, string imageUrl = null);
    }
}

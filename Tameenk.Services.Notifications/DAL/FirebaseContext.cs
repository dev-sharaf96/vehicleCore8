using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
namespace Tameenk.Services.Notifications
{
    public class FirebaseContext : DbContext
       
    {
        public FirebaseContext()
            : base("name=Firebase")
        {

        }
        public DbSet<UserFireBaseRegisterationToken> UserFireBaseRegisterationTokens { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<SendNotificationLog> SendNotificationLogs { get; set; }
        public DbSet<RegisterTokenLog> RegisterTokenLogs { get; set; }
    }
}

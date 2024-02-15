using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain;

namespace Tameenk.Data.Mapping.Payments.HyperPay
{
    public class HyperPayNotificationMap : EntityTypeConfiguration<HyperPayNotification>
    {
        public HyperPayNotificationMap()
        {
            ToTable("HyperPayNotifications");
            HasKey(e => e.Id);
            HasMany(e => e.Transactions)
                .WithRequired(e => e.Notification)
                .HasForeignKey(e => e.NotificationId)
                .WillCascadeOnDelete(false);
        }
    }
}

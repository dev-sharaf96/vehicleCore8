using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Data.Mapping.Payments.Sadad
{
    public class SadadNotificationResponseMap : EntityTypeConfiguration<SadadNotificationResponse>
    {
        public SadadNotificationResponseMap()
        {
            ToTable("SadadNotificationResponse");
            Property(e => e.HeadersReceiver).HasMaxLength(50);
            Property(e => e.HeadersSender).HasMaxLength(50);
            Property(e => e.HeadersMessageType).HasMaxLength(10);
            Property(e => e.Status).IsRequired().HasMaxLength(10);
        }
    }
}

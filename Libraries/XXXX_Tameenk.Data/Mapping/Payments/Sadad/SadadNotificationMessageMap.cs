using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Data.Mapping.Payments.Sadad
{
    public class SadadNotificationMessageMap : EntityTypeConfiguration<SadadNotificationMessage>
    {
        public SadadNotificationMessageMap()
        {
            ToTable("SadadNotificationMessage");
            Property(e => e.HeadersReceiver).HasMaxLength(50);
            Property(e => e.HeadersSender).HasMaxLength(50);
            Property(e => e.HeadersMessageType).HasMaxLength(10);
            Property(e => e.BodysAccountNo).HasMaxLength(25);
            Property(e => e.BodysCustomerRefNo).HasMaxLength(25);
            Property(e => e.BodysTransType).HasMaxLength(10);
            Property(e => e.BodysDescription).HasMaxLength(200);

            Property(e => e.BodysAmount)
                .HasPrecision(18, 4);
            HasMany(e => e.SadadNotificationResponses)
                            .WithRequired(e => e.SadadNotificationMessage)
                            .HasForeignKey(e => e.NotificationMessageId)
                            .WillCascadeOnDelete(false);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Messages;

namespace Tameenk.Data.Mapping.Messages
{
    public class NotificationParameterMap : EntityTypeConfiguration<NotificationParameter>
    {
        public NotificationParameterMap()
        {
            ToTable("NotificationParameter");
            HasKey(e => e.Id);
            Property(e => e.Name).HasMaxLength(256).IsRequired();
            Property(e => e.Value).IsRequired();
            HasRequired(e => e.Notification).WithMany(e => e.Parameters).HasForeignKey(e => e.NotificationId);
        }
    }
}

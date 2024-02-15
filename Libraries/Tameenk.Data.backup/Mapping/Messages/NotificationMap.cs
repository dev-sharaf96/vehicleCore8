using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Messages;

namespace Tameenk.Data.Mapping
{
    /// <summary>
    /// Represent the EF mapping for notification entity
    /// </summary>
    public class NotificationMap : EntityTypeConfiguration<Notification>
    {

        public NotificationMap()
        {
            ToTable("Notification");
            HasKey(e => e.Id);
            Property(e => e.Group).HasMaxLength(256).IsRequired();
            Property(e => e.GroupReferenceId).HasMaxLength(256).IsRequired();
            Property(e => e.TypeId).IsRequired();
            Property(e => e.StatusId).IsRequired();
            Ignore(e => e.Status);
            Ignore(e => e.Type);
        }
    }
}

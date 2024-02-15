using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ProfileNotificationMap : EntityTypeConfiguration<ProfileNotification>
    {
        public ProfileNotificationMap() {
            ToTable("ProfileNotification");
            HasKey(c => c.Id);
        }
    }
}
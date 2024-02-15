using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class MobileAppVersionsMap : EntityTypeConfiguration<MobileAppVersions>
    {
        public MobileAppVersionsMap() {
            ToTable("MobileAppVersions");
            HasKey(c => c.Id);
            Property(c => c.Version).HasMaxLength(20);
            
            
        }
    }
}
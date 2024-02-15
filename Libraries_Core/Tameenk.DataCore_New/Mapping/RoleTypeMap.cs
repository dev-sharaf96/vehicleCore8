using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class RoleTypeMap : EntityTypeConfiguration<RoleType>
    {
        public RoleTypeMap()
        {
            ToTable("RoleType");
            Property(e => e.TypeNameEN).IsRequired().HasMaxLength(50);
            Property(e => e.TypeNameAR).IsRequired().HasMaxLength(50);

            HasMany(e => e.Roles)
                .WithRequired(e => e.RoleType)
                .WillCascadeOnDelete(false);
        }
    }
}

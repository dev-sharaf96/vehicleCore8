using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            ToTable("Role");
            Property(e => e.RoleNameEN).IsRequired().HasMaxLength(50);
            Property(e => e.RoleNameAR).IsRequired().HasMaxLength(50);

           

            HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Identity
{
    public class CorporateUsersMap : EntityTypeConfiguration<CorporateUsers>
    {
        public CorporateUsersMap()
        {
            ToTable("CorporateUsers");
            HasKey(e => e.UserId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Claim
{
    public class UserClaimStatusMap : EntityTypeConfiguration<UserClaimStatus>
    {
        public UserClaimStatusMap()
        {
            ToTable("UserClaimStatus");
            HasKey(c => c.Id);
        }
    }
}

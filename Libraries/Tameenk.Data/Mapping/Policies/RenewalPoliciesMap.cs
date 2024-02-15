using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class RenewalPoliciesMap : EntityTypeConfiguration<RenewalPolicies>

    {
        public RenewalPoliciesMap()
        {
            ToTable("RenewalPolicies");
            HasKey(e => e.Id);
        }
    }
}

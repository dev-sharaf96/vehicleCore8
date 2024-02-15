using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class PolicyModificationMap : EntityTypeConfiguration<PolicyModification>

    {
        public PolicyModificationMap()
        {
            ToTable("PolicyModification");
            HasKey(e => e.Id);
        }
    }
}

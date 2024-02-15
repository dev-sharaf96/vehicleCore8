using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    class PolicyRenewedByCompanyMap : EntityTypeConfiguration<PolicyRenewedByCompany>
    {
        public PolicyRenewedByCompanyMap()
        {
            ToTable("PolicyRenewedByCompany");
            HasKey(e => e.Id);
        }
    }
}

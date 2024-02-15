using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Data.Mapping.Payments.Esal
{
    class EsalSettlementMap : EntityTypeConfiguration<EsalSettlement>
    {
        public EsalSettlementMap()
        {
            ToTable("EsalSettlement");
            HasKey(e => e.ID);
        }
    }
}

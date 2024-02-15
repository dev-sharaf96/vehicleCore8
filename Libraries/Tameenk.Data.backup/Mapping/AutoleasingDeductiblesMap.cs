using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AutoleasingDeductiblesMap : EntityTypeConfiguration<AutoleasingDeductibles>
    {
        public AutoleasingDeductiblesMap()
        {
            ToTable("AutoleasingDeductibles");
            HasKey(e => e.Id);

        }
    }
}

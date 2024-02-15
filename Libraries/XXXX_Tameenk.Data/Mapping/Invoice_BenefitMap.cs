using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class Invoice_BenefitMap : EntityTypeConfiguration<Invoice_Benefit>
    {
        public Invoice_BenefitMap()
        {
            ToTable("Invoice_Benefit");
            HasKey(e => e.Id);
            Property(e => e.BenefitPrice).HasPrecision(8, 2);
        }
    }
}

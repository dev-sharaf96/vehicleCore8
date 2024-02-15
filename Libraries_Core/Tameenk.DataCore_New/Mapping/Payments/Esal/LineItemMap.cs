using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Data.Mapping.Payments.Esal
{
    public class LineItemMap: EntityTypeConfiguration<LineItem>
    {
        public LineItemMap()
        {
            ToTable("EsalLineItem");
            HasKey(e => e.Id);
            Property(e => e.Price)
                .HasPrecision(18, 2);
            Property(e => e.Amount)
                .HasPrecision(18, 2);
            Property(e => e.DiscountPercent)
                .HasPrecision(18, 2);
            Property(e => e.DiscountAmount)
                .HasPrecision(18, 2);
            Property(e => e.AmountAfterDiscount)
                .HasPrecision(18, 2);
            Property(e => e.VatPercent)
                .HasPrecision(18, 2);
            Property(e => e.TotalVat)
                .HasPrecision(18, 2);
            Property(e => e.PriceAfterVat)
                .HasPrecision(18, 2);
        }
    }
}

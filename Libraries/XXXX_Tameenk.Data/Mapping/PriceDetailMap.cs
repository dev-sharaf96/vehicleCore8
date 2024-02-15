using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PriceDetailMap : EntityTypeConfiguration<PriceDetail>
    {
        public PriceDetailMap()
        {
            ToTable("PriceDetail");
            HasKey(e => e.DetailId);
            Property(e => e.PriceValue).HasPrecision(8, 2);
            Property(e => e.PercentageValue).HasPrecision(8, 2);

            Property(e => e.PriceValue)
                .HasPrecision(8, 2);

            Property(e => e.PercentageValue)
                .HasPrecision(8, 2);
        }
    }
}
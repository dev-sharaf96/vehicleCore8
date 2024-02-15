using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap() {
            ToTable("City");
            HasKey(c => c.Code);
            Property(c => c.EnglishDescription).HasMaxLength(128);
            Property(c => c.ArabicDescription).HasMaxLength(128);
            HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.City)
                .WillCascadeOnDelete(false);
            HasOptional(e => e.Region).WithMany(e => e.Cities).HasForeignKey(e => e.RegionId);
        }
    }
}
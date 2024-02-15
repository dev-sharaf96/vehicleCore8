using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class RegionMap : EntityTypeConfiguration<Region>
    {
        public RegionMap()
        {
            ToTable("Region");
            HasKey(e => e.Id);
            Property(e => e.Name).HasMaxLength(100);
            
        }
    }
}

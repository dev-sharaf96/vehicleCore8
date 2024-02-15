using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CityCenterMap : EntityTypeConfiguration<CityCenter>
    {
        public CityCenterMap() {
            ToTable("CityCenter");
            HasKey(c => c.Id);
        }
    }
}
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleColorMap : EntityTypeConfiguration<VehicleColor>
    {
        public VehicleColorMap()
        {
            ToTable("VehicleColor");
            HasKey(vc => vc.Code);
            Property(vc => vc.EnglishDescription).HasMaxLength(200);
            Property(vc => vc.ArabicDescription).HasMaxLength(200);
        }
    }
}
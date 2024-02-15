using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class CustomCardInfoMap : EntityTypeConfiguration<CustomCardInfo>
    {
        public CustomCardInfoMap()
        {
            ToTable("CustomCardInfo");
            HasKey(e => e.Id);
        }
    }
}
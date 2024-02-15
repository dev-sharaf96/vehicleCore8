using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleSpecificationMap : EntityTypeConfiguration<VehicleSpecification>
    {
        public VehicleSpecificationMap()
        {
            ToTable("VehicleSpecification");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.DescriptionAr).HasMaxLength(100);
            Property(e => e.DescriptionEn).HasMaxLength(100);
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    class VehicleUsagePercentageMap : EntityTypeConfiguration<VehicleUsagePercentage>
    {
        public VehicleUsagePercentageMap()
        {
            ToTable("VehicleUsagePercentage");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}

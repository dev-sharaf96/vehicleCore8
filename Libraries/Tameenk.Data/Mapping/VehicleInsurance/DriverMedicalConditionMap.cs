using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class DriverMedicalConditionMap : EntityTypeConfiguration<DriverMedicalCondition>
    {
        public DriverMedicalConditionMap()
        {
            ToTable("DriverMedicalCondition");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}

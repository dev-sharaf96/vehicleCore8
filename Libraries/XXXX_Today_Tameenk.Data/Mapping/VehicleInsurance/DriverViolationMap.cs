using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class DriverViolationMap : EntityTypeConfiguration<DriverViolation>
    {
        public DriverViolationMap()
        {
            ToTable("DriverViolation");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(e => e.Driver).WithMany(e => e.DriverViolations).HasForeignKey(e => e.DriverId);
        }
    }
}

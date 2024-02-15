using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    class DriverExtraLicenseMap : EntityTypeConfiguration<DriverExtraLicense>
    {
        public DriverExtraLicenseMap()
        {
            ToTable("DriverExtraLicense");
            HasKey(d => d.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(e => e.Driver)
                .WithMany(e => e.DriverExtraLicenses)
                .HasForeignKey(e => e.DriverId);
        }
    }
}

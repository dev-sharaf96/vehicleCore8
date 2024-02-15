using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class DriverLicenseMap : EntityTypeConfiguration<DriverLicense>
    {
        public DriverLicenseMap()
        {
            ToTable("DriverLicense");
            HasKey(e => e.LicenseId);
            Property(e => e.ExpiryDateH).HasMaxLength(20);
        }
    }
}

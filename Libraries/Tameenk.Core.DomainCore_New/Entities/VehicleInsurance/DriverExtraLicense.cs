using System;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class DriverExtraLicense : BaseEntity
    {
        public int Id { get; set; }
        public Guid DriverId { get; set; }
        public short CountryCode { get; set; }
        public int LicenseYearsId { get; set; }

        public Driver Driver { get; set; }
    }
}

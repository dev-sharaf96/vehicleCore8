using System;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class DriverLicense : BaseEntity
    {
        public int LicenseId { get; set; }

        public Guid DriverId { get; set; }

        public short? TypeDesc { get; set; }
        
        public string ExpiryDateH { get; set; }

        public string IssueDateH { get; set; }

        public string licnsTypeDesc { get; set; }

        public Driver Driver { get; set; }
    }
}

using System;

namespace Tamkeen.bll.Model
{
    [Serializable]
    public class DriverModel
    {
        public long DriverId { get; set; }
        public int LicenseExpirationYear { get; set; }
        public string DriverlicenseExpiryDate { get; set; }
    }
}
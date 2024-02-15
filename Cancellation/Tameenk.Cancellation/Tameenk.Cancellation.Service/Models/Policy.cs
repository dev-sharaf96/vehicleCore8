using System;

namespace Tameenk.Cancellation.Service.Models
{
    public class Policy
    {
        public Policy()
        {

        }

        public int ProductTypeCode { get; set; }
        public string PolicyNo { get; set; }

        public DateTime PolicyEffectiveDate { get; set; }

        public DateTime PolicyExpiryDate { get; set; }
        public string InsuredName { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleMaker { get; set; }
        public string VehiclePlate { get; set; }
        public int VehicleModelYear { get; set; }
        public decimal RefundAmount { get; set; }
        public string PolicyFileUrl { get; set; }
        public byte[] PolicyFile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Core.Domain.Entities
{
    public class YakeenVehicles : BaseEntity
    {
        public YakeenVehicles(){}

        public Guid ID { get; set; }

        public string SequenceNumber { get; set; }

        public string CustomCardNumber { get; set; }

        public bool? IsRegistered { get; set; }

        public short? Cylinders { get; set; }

        public string LicenseExpiryDate { get; set; }

        public string MajorColor { get; set; }

        public string MinorColor { get; set; }

        public short? ModelYear { get; set; }

        public short? PlateTypeCode { get; set; }

        public string RegisterationPlace { get; set; }

        public short? VehicleBodyCode { get; set; }

        public int? VehicleWeight { get; set; }

        public int? VehicleLoad { get; set; }

        public string VehicleMaker { get; set; }

        public string VehicleModel { get; set; }

        public string ChassisNumber { get; set; }

        public int? VehicleMakerCode { get; set; }

        public int? VehicleModelCode { get; set; }

        public int? LogId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

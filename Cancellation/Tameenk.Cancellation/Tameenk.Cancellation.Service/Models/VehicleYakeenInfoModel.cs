using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Cancellation.Service.Yakeen
{
    public class VehicleYakeenModel
    {
        public bool Success { get; set; }

        public YakeenInfoErrorModel Error { get; set; }

        public Guid TameenkId { get; set; }

        public string SequenceNumber { get; set; }

        public string CustomCardNumber { get; set; }

        public bool IsRegistered { get; set; }

        public byte? Cylinders { get; set; }

        public string LicenseExpiryDate { get; set; }

        public string MajorColor { get; set; }

        public string MinorColor { get; set; }

        public short? ModelYear { get; set; }

        public byte PlateTypeCode { get; set; }

        public string RegisterationPlace { get; set; }

        public byte BodyCode { get; set; }

        public int Weight { get; set; }

        public int Load { get; set; }

        public string Maker { get; set; }

        public string Model { get; set; }

        public string ChassisNumber { get; set; }

        public short? MakerCode { get; set; }

        public long? ModelCode { get; set; }

        public string CarPlateText1 { get; set; }

        public string CarPlateText2 { get; set; }

        public string CarPlateText3 { get; set; }

        public short? CarPlateNumber { get; set; }

        public string CarOwnerNIN { get; set; }

        public string CarOwnerName { get; set; }

        public int? Value { get; set; }

        public bool? IsUsedCommercially { get; set; }
    }
}
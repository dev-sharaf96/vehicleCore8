using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Quotations
{
   public class QuoteRequestVehicleInfo
    {
        public string ExternalId { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public Guid ID { get; set; }
        public string VehicleMaker { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public byte VehicleBodyCode { get; set; }
        public short? ModelYear { get; set; }
        public byte? PlateTypeCode { get; set; }
        public int VehicleIdTypeId { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public long? VehicleModelCode { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public short? CarPlateNumber { get; set; }
        public bool? IsRenewal { get; set; }
        public string PreviousReferenceId { get; set; }
    }
}

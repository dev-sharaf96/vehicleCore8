using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class CustomCardInfo : BaseEntity
    {
        public Guid? Id { get; set; }
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string CustomCardNumber { get; set; }
        public Guid? VehicleId { get; set; }
        public int? SequenceNumber { get; set; }
        public string ChassisNumber { get; set; }
        public short? Cylinders { get; set; }
        public int? LogId { get; set; }
        public string MajorColor { get; set; }
        public short? ModelYear { get; set; }
        public string CarOwnerName { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public short? PlateTypeCode { get; set; }
        public string RegisterationPlace { get; set; }
        public int? VehicleCapacity { get; set; }
        public string VehicleMaker { get; set; }
        public int? VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public int? VehicleModelCode { get; set; }
        public int? VehicleWeight { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

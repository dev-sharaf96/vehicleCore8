using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class UpdateCustomCardRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string CustomCardNumber { get; set; }
        public string VehiclePlateNumber { get; set; }
        public string VehiclePlateText1 { get; set; }
        public string VehiclePlateText2 { get; set; }
        public string VehiclePlateText3 { get; set; }
        public string SequenceNumber { get; set; }
        public string VehiclePlateTypeCode { get; set; }
        public string VehicleRegPlaceCode { get; set; }
        public string VehicleRegPlace { get; set; }
        public int VehicleLoad { get; set; }
    }
}

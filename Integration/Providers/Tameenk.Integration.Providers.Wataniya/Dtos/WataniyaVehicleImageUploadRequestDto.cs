using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaVehicleImageUploadRequestDto
    {
        public int PolicyRequestReferenceNo { get; set; }
        public int PolicyReferenceNo { get; set; }
        public List<VehicleImages> VehicleImages { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
    }

    public class VehicleImages
    {
        public int ImageID { get; set; }
        public short ImageTitle { get; set; }
        public string ImageMedia { get; set; }
        public string ImageDateTime { get; set; }
        public float ImageLong { get; set; }
        public float ImageLat { get; set; }
    }
}

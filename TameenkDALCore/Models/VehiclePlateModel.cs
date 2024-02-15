using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    public class VehiclePlateModel
    {
        public string PlateColor { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public Nullable<short> CarPlateNumber { get; set; }
        public string CarPlateNumberAr { get; set; }
        public string CarPlateNumberEn { get; set; }
        public string CarPlateTextAr { get; set; }
        public string CarPlateTextEn { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class CarPlateResult
    {
        public CarPlateResult()
        {
            Error = new YakeenError();
        }

        public bool Success { get; set; }
        public YakeenError Error { get; set; }

        public string ChassisNumber { get; set; }

        public int LogId { get; set; }

        public string OwnerName { get; set; }

        public short PlateNumber { get; set; }

        public string PlateText1 { get; set; }

        public string PlateText2 { get; set; }

        public string PlateText3 { get; set; }
    }
}

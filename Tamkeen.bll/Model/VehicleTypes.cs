using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class VehicleType
    {
        public int code { get; set; }
        public String description { get; set; }
    }


    public class VehicleTypesRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
    public class VehicleTypesResponseMessage
    {
        public int status { get; set; }
        public List<VehicleType> vehicleTypesList { get; set; }
        public string errorMsg { get; set; }
    }
}

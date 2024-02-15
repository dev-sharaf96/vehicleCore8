using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class YakeenResponse
    {
        public YakeenResponse()
        {
            ErrorMessages = new List<YakeenError>();
        }
        public Guid VehicleInternalId { get; set; }
        public Guid[] DriversInternalIds { get; set; }

        public List<YakeenError> ErrorMessages { get; set; }
    }
}

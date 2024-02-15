using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    public class BIQuotationServiceLogsModel : BIBaseLogModel
    {
        public double? ServiceResponseTimeInSeconds { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public int? VehicleModelYear { get; set; }
    }
}

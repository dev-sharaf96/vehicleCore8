using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    public class BIBaseLogModel
    {
        public string CompanyName { get; set; }
        public string ReferenceId { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleModelCode { get; set; }
        public string City { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class QuotationResponseModel
    {
        public string QtRqstExtrnlId { get; set; }
        public string ReferenceId { get; set; }
        public int QuotationResponseId { get; set; }
        public VehicleModel Vehicle { get; set; }
        public string NCDFreeYears { get; set; }
        public int TypeOfInsurance { get; set; }
        public string TypeOfInsuranceText { get; set; }
        public short? DeductibleValue { get; set; }
        public bool VehicleAgencyRepair { get; set; }
    }
}

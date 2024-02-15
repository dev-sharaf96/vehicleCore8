using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Quotation
{
    public class QuotationResponseModelForProfile
    {
        public string QtRqstExtrnlId { get; set; }
        public string ReferenceId { get; set; }
        public int QuotationResponseId { get; set; }
        public VehicleModelForProfile Vehicle { get; set; }
        public string NCDFreeYears { get; set; }
        public int TypeOfInsurance { get; set; }
        public string TypeOfInsuranceText { get; set; }
        public short? DeductibleValue { get; set; }
        public bool VehicleAgencyRepair { get; set; }
        public string NCDFreeYearsAr { get; set; }
        public string NCDFreeYearsEn { get; set; }

        public string TypeOfInsuranceTextAr { get; set; }
        public string TypeOfInsuranceTextEn { get; set; }

        public bool IsRenewal { get; set; }
        public string RenewalReferenceId { get; set; }
    }
}

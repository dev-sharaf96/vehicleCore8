using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class IntialQuotationOutput
    {
        public string NIN { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public int? VehicleValue { get; set; }
        public bool? AutoleasingInitialOption { get; set; } = false;
        public string ExternalId { get; set; }
        public short? VehicleMakerCode { get; set; }
        public short? ModelYear { get; set; }
        public string FullName { get; set; }
        public int? DeductibleValue { get; set; }
        public string RepairMethod { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string BirthDate { get; set; }
        public string BirthMonth { get; set; }
        public string Birthyear { get; set; }
        public DateTime? RequestPolicyEffectiveDate { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }
        public int? ContractDuration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.QuotationNew.Components
{
    public class GetQuotationModel
    {
        public int insuranceCompanyId { get; set; }
        public string qtRqstExtrnlId { get; set; }
        public Guid parentRequestId { get; set; }
        public int insuranceTypeCode { get; set; }
        public bool vehicleAgencyRepair { get; set; }
        public int? deductibleValue { get; set; } = null;
        public string policyNo { get; set; } = null;
        public string policyExpiryDate { get; set; } = null;
        public string hashed { get; set; } = null;
        public string channel { get; set; } = "Portal";
        public bool OdQuotation { get; set; } = false;
    }
}

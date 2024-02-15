using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AddVechileBenefitRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string BenefitStartDate { get; set; }
        public int? QuotationRequestId { set; get; }
        public string PolicyReferenceId { set; get; }

    }
}

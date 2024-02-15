using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AddBenefitRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string BenefitStartDate { get; set; }

        [JsonIgnore]
        public int? QuotationRequestId { set; get; }

        [JsonIgnore]
        public string QuotationReferenceId { get; set; }

        [JsonIgnore]
        public int? BankId { get; set; }

        [JsonIgnore]
        public int? CompanyId { get; set; }
    }
}

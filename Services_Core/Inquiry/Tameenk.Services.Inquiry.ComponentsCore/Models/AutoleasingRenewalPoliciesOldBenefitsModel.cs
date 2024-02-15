using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class AutoleasingRenewalPoliciesOldBenefitsModel
    {
        [JsonProperty("benefitCode")]
        public short? BenefitCode { get; set; }

        [JsonProperty("benefitExternalId")]
        public string BenefitExternalId { get; set; }

        [JsonProperty("benefitPrice")]
        public decimal BenefitPrice { get; set; }

        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("arabicName")]
        public string ArabicName { get; set; }

        [JsonProperty("englishName")]
        public string EnglishName { get; set; }
    }
}

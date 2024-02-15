using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class PurchaseBenefitRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public double PaymentAmount { get; set; }
        public string PaymentBillNumber { get; set; }
        public List<AdditionalBenefit> Benefits { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LesseeID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LessorID { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class AutoLeaseQuotationRequest
    {
        [JsonProperty("PolicyRequestReferenceNo")]
        public string PolicyRequestReferenceNo { get; set; }
        [JsonProperty("RequestReferenceNo")]
        public string RequestReferenceNo { get; set; }
        [JsonProperty("SourceCode")]
        public int SourceCode { get; set; }
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("QuoteReferenceNo")]
        public int QuoteReferenceNo { get; set; }
        [JsonProperty("Details")]
        public Details Details { get; set; }
        [JsonProperty("CustomizedParameter")]
        public string CustomizedParameter { get; set; }
        
    }


    public class Details
    {
        //[JsonProperty("Email")]
        //public string Email { get; set; }
        //[JsonProperty("MobileNo")]
        //public string MobileNo { get; set; }
        //[JsonProperty("DeductibleReferenceNo")]
       // public int? DeductibleReferenceNo { get; set; }
        [JsonProperty("DeductibleAmount")]
        public string DeductibleAmount { get; set; }
        [JsonProperty("PolicyPremiumFeatures")]
        public List<PolicyPremiumFeatures> PolicyPremiumFeatures { get; set; }
        //[JsonProperty("DynamicPremiumFeatures")]
        //public List<DynamicPremiumFeatures> DynamicPremiumFeatures { get; set; }
    }
}

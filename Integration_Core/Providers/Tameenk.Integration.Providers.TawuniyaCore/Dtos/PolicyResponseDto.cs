using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class TawuniyaPolicyInfo
    {

        [JsonProperty("quotationNumber")]
        public string QuotationNumber { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonProperty("policyInceptionDate")]
        public string PolicyInceptionDate { get; set; }

        [JsonProperty("policyExpiryDate")]
        public string PolicyExpiryDate { get; set; }

        [JsonProperty("policyDocPrintingLink")]
        public string PolicyDocPrintingLink { get; set; }
    }

    public class PolicyResult
    {

        [JsonProperty("resultCode")]
        public string ResultCode { get; set; }

        [JsonProperty("resultMessage")]
        public string ResultMessage { get; set; }
    }

    public class TawuniyaPolicyResponse
    {

        [JsonProperty("policyInfo")]
        public TawuniyaPolicyInfo PolicyInfo { get; set; }

        [JsonProperty("policyResult")]
        public PolicyResult PolicyResult { get; set; }
    }


    public class PolicyError
    {

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
    }

    public class CreatePolicyResponse
    {

        [JsonProperty("policyResponse")]
        public TawuniyaPolicyResponse PolicyResponse { get; set; }

        [JsonProperty("exception")]
        public IList<PolicyError> Errors { get; set; }
    }

    public class PolicyResponseDto
    {

        [JsonProperty("createPolicyResponse")]
        public CreatePolicyResponse CreatePolicyResponse { get; set; }
    }


}

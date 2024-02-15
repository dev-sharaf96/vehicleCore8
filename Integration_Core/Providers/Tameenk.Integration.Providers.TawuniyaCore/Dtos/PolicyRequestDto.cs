using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class PolicyRequestHeader
    {

        [JsonProperty("initiatedDateTime")]
        public string InitiatedDateTime { get; set; }

        [JsonProperty("messsageType")]
        public string MesssageType { get; set; }

        [JsonProperty("routingIdentifier")]
        public string RoutingIdentifier { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("trackingId")]
        public string TrackingId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class PolicyInfo
    {

        [JsonProperty("quotationNumber")]
        public string QuotationNumber { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty("policyInceptionDate")]
        public string PolicyInceptionDate { get; set; }
    }

    public class PolicyChannelDetail
    {

        [JsonProperty("applicationType")]
        public string ApplicationType { get; set; }

        [JsonProperty("consumerApplicationTypeReference")]
        public string ConsumerApplicationTypeReference { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("sourceCode")]
        public string SourceCode { get; set; }

        [JsonProperty("channelCode")]
        public string ChannelCode { get; set; }
    }

    public class PaymentDetail
    {

        [JsonProperty("paymentAmount")]
        public string PaymentAmount { get; set; }

        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        [JsonProperty("paymentDate")]
        public string PaymentDate { get; set; }
    }

    public class CustomerDetails
    {
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("additionalNumber")]
        public string AdditionalNumber { get; set; }

        [JsonProperty("buildingNo")]
        public string BuildingNo { get; set; }

        [JsonProperty("districtName")]
        public string DistrictName { get; set; }

        [JsonProperty("streetName")]
        public string StreetName { get; set; }

        [JsonProperty("unitNumber")]
        public string UnitNumber { get; set; }

        [JsonProperty("zipcode")]
        public string Zipcode { get; set; }
    }

    public class TawuniyaPolicyRequest
    {

        [JsonProperty("policyInfo")]
        public PolicyInfo PolicyInfo { get; set; }

        [JsonProperty("channelDetails")]
        public IList<PolicyChannelDetail> ChannelDetails { get; set; }

        [JsonProperty("paymentDetails")]
        public IList<PaymentDetail> PaymentDetails { get; set; }

        [JsonProperty("customerDetails")]
        public CustomerDetails CustomerDetails { get; set; }
    }

    public class CreatePolicyRequest
    {

        [JsonProperty("header")]
        public PolicyRequestHeader Header { get; set; }

        [JsonProperty("policyRequest")]
        public TawuniyaPolicyRequest PolicyRequest { get; set; }
    }

    public class PolicyRequestDto
    {

        [JsonProperty("createPolicyRequest")]
        public CreatePolicyRequest CreatePolicyRequest { get; set; }
    }


}

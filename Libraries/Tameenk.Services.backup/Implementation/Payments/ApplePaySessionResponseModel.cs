using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class ApplePaySessionResponseModel
    {

        [JsonProperty("epochTimestamp")]
        public string EpochTimestamp { get; set; }
        [JsonProperty("expiresAt")]
        public string ExpiresAt { get; set; }
        [JsonProperty("merchantSessionIdentifier")]
        public string MerchantSessionIdentifier { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("merchantIdentifier")]
        public string MerchantIdentifier { get; set; }
        [JsonProperty("domainName")]
        public string DomainName { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("operationalAnalyticsIdentifier")]
        public string OperationalAnalyticsIdentifier { get; set; }
        [JsonProperty("retries")]
        public string Retries { get; set; }
        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }
        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }
            
    }
}

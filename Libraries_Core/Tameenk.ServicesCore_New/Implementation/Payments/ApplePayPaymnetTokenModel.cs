using Newtonsoft.Json;


namespace Tameenk.Services
{
    public class ApplePayPaymnetTokenModel
    {
        [JsonProperty("paymentData")]
        public PaymentData? PaymentData { get; set; }

        [JsonProperty("paymentMethod")]
        public PaymentMethodData? PaymentMethod { get; set; }

        [JsonProperty("transactionIdentifier")]
        public string? TransactionIdentifier { get; set; }
    }
    public class PaymentData
    {
        [JsonProperty("data")]
        public string? Data { get; set; }
        [JsonProperty("signature")]
        public string? Signature { get; set; }

        [JsonProperty("header")]
        public Header? Header { get; set; }

        [JsonProperty("version")]
        public string? Version { get; set; }



    }
    public class Header
    {
        [JsonProperty("publicKeyHash")]
        public string? PublicKeyHash { get; set; }
        [JsonProperty("ephemeralPublicKey")]
        public string? EphemeralPublicKey { get; set; }
        [JsonProperty("transactionId")]
        public string? TransactionId { get; set; }
       
    }
    public class PaymentMethodData
    {
        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }

        [JsonProperty("network")]
        public string? Network { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }
}

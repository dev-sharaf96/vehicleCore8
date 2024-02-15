using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class ApplePayPaymnetResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }
        [JsonProperty("paymentBrand")]
        public string PaymentBrand { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }
        [JsonProperty("result")]
        public Result Result { get; set; }
        [JsonProperty("resultDetails")]
        public ResultDetails ResultDetails { get; set; }
        [JsonProperty("card")]
        public Card Card { get; set; }
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
        [JsonProperty("customParameters")]
        public CustomParameters CustomParameters { get; set; }
        [JsonProperty("risk")]
        public Risk Risk { get; set; }
        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("ndc")]
        public string Ndc { get; set; }
      
    }
    public class Result
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
    public class ResultDetails
    {
        [JsonProperty("CscResultCode")]
        public string CscResultCode { get; set; }
        [JsonProperty("ExtendedDescription")]
        public string ExtendedDescription { get; set; }
        [JsonProperty("ConnectorTxID1")]
        public string ConnectorTxID1 { get; set; }
        [JsonProperty("TransactionIdentfier")]
        public string TransactionIdentfier { get; set; }
        [JsonProperty("ConnectorTxID3")]
        public string ConnectorTxID3 { get; set; }
        [JsonProperty("connectorId")]
        public string connectorId { get; set; }
        [JsonProperty("ConnectorTxID2")]
        public string ConnectorTxID2 { get; set; }
        [JsonProperty("AcquirerResponse")]
        public string AcquirerResponse { get; set; }
        [JsonProperty("BatchNo")]
        public string BatchNo { get; set; }
        [JsonProperty("endToEndId")]
        public string EndToEndId { get; set; }
        [JsonProperty("AuthorizeId")]
        public string AuthorizeId { get; set; }
        [JsonProperty("AvsResultCode")]
        public string AvsResultCode { get; set; }

    }
    public class Card
    {
        [JsonProperty("bin")]
        public string Bin { get; set; }
        [JsonProperty("last4Digits")]
        public string Last4Digits { get; set; }
        [JsonProperty("expiryMonth")]
        public string ExpiryMonth { get; set; }
        [JsonProperty("expiryYear")]
        public string ExpiryYear { get; set; }
        [JsonProperty("holder")]
        public string Holder { get; set; }
    }
    public class Risk
    {
        [JsonProperty("score")]
        public string Score { get; set; }
    }
    public class Customer
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
    public class CustomParameters
    {
        [JsonProperty("payout")]
        public string Payout { get; set; }
    }
}

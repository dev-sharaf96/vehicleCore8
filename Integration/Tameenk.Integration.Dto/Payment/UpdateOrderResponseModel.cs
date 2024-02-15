using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Payment
{
    public class UpdateOrderResponseModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public string Errors { get; set; }

        public List<DataInfo> data { get; set; }
    }

    [JsonObject("data")]
    public class DataInfo
    {
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("payoutStatus")]
        public string PayoutStatus { get; set; }

        [JsonProperty("payoutDebitCurrency")]
        public string PayoutDebitCurrency { get; set; }

        [JsonProperty("payoutTransferAmount")]
        public string PayoutTransferAmount { get; set; }

        [JsonProperty("payoutTransferCurrency")]
        public string PayoutTransferCurrency { get; set; }

        [JsonProperty("payoutBeneficiaryName")]
        public string PayoutBeneficiaryName { get; set; }

        [JsonProperty("payoutBeneficiaryAccountId")]
        public string PayoutBeneficiaryAccountId { get; set; }

        [JsonProperty("payoutBeneficiaryAddress1")]
        public string PayoutBeneficiaryAddress1 { get; set; }

        [JsonProperty("payoutBeneficiaryAddress2")]
        public string payoutBeneficiaryAddress2 { get; set; }

        [JsonProperty("payoutBeneficiaryAddress3")]
        public string PayoutBeneficiaryAddress3 { get; set; }

        [JsonProperty("payoutTransferOption")]
        public string PayoutTransferOption { get; set; }

        [JsonProperty("executionDate")]
        public string ExecutionDate { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }

        [JsonProperty("b2bconfig_id")]
        public string B2bconfigId { get; set; }

        [JsonProperty("batch_id")]
        public string BatchId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("bankIdBIC")]
        public string BankIdBIC { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("paymentBrand")]
        public string PaymentBrand { get; set; }

        [JsonProperty("entityId")]
        public string EntityId { get; set; }

        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; set; }

        [JsonProperty("merchantRequestId")]
        public string MerchantRequestId { get; set; }

        [JsonProperty("batch_description")]
        public string BatchDescription { get; set; }

        [JsonProperty("swift")]
        public string Swift { get; set; }

        [JsonProperty("deleted_at")]
        public string DeletedAt { get; set; }


    }

    [JsonObject("response")]
    public class Response
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }

    [JsonObject("payload")]
    public class Payload
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

        [JsonProperty("presentationAmount")]
        public string PresentationAmount { get; set; }

        [JsonProperty("presentationCurrency")]
        public string PresentationCurrency { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }

        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("resultDetails")]
        public ResultDetails ResultDetails { get; set; }

        [JsonProperty("card")]
        public Card Card { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("authentication")]
        public AuthenticationObject Authentication { get; set; }

        [JsonProperty("customParameters")]
        public CustomParameters CustomParameters { get; set; }

        [JsonProperty("redirect")]
        public Redirect Redirect { get; set; }

        [JsonProperty("risk")]
        public Risk Risk { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("ndc")]
        public string NDC { get; set; }

        [JsonProperty("merchantAccountId")]
        public string MerchantAccountId { get; set; }

        [JsonProperty("channelName")]
        public string ChannelName { get; set; }
    }

    [JsonObject("result")]
    public class Result
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("randomField1795320865")]
        public string RandomField1795320865 { get; set; }
    }

    [JsonObject("resultDetails")]
    public class ResultDetails
    {
        [JsonProperty("ConnectorTxID1")]
        public string ConnectorTxID1 { get; set; }

        [JsonProperty("ConnectorTxID2")]
        public string ConnectorTxID2 { get; set; }

        [JsonProperty("ConnectorTxID3")]
        public string ConnectorTxID3 { get; set; }
    }

    [JsonObject("card")]
    public class Card
    {
        [JsonProperty("ConnectorTxID1")]
        public string ConnectorTxID1 { get; set; }

        [JsonProperty("ConnectorTxID2")]
        public string ConnectorTxID2 { get; set; }

        [JsonProperty("ConnectorTxID3")]
        public string ConnectorTxID3 { get; set; }
    }

    [JsonObject("customer")]
    public class Customer
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("ip")]
        public string IP { get; set; }
    }

    [JsonObject("authentication")]
    public class AuthenticationObject
    {
        [JsonProperty("entityId")]
        public string EntityId { get; set; }
    }

    [JsonObject("customParameters")]
    public class CustomParameters
    {
        [JsonProperty("SHOPPER_EndToEndIdentity")]
        public string SHOPPER_EndToEndIdentity { get; set; }

        [JsonProperty("bill_number")]
        public string BillNumber { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("teller_id")]
        public string TellerId { get; set; }

        [JsonProperty("branch_id")]
        public string BranchId { get; set; }

        [JsonProperty("CTPE_DESCRIPTOR_TEMPLATE")]
        public string CTPE_DESCRIPTOR_TEMPLATE { get; set; }

        [JsonProperty("payout")]
        public string Payout { get; set; }
    }

    [JsonObject("redirect")]
    public class Redirect
    {
        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("parameters")]
        public List<Parameters> Parameters { get; set; }
    }

    [JsonObject("parameters")]
    public class Parameters
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [JsonObject("risk")]
    public class Risk
    {
        [JsonProperty("score")]
        public string Score { get; set; }

    }
}

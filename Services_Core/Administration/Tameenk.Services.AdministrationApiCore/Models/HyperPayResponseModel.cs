using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// HyperPay Response Model
    /// </summary>
    [JsonObject("hyperPayResponse")]
    public class HyperPayResponseModel
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// HyperpayResponseId
        /// </summary>
        [JsonProperty("hyperpayResponseId")]
        public string HyperpayResponseId { get; set; }

        /// <summary>
        /// Response Code
        /// </summary>
        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Ndc
        /// </summary>
        [JsonProperty("ndc")]
        public string Ndc { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Descriptor
        /// </summary>
        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }

        /// <summary>
        /// PaymentBrand
        /// </summary>
        [JsonProperty("paymentBrand")]
        public string PaymentBrand { get; set; }

        /// <summary>
        /// CardBin
        /// </summary>
        [JsonProperty("cardBin")]
        public string CardBin { get; set; }

        /// <summary>
        /// Last4Digits
        /// </summary>
        [JsonProperty("last4Digits")]
        public string Last4Digits { get; set; }

        /// <summary>
        /// Holder
        /// </summary>
        [JsonProperty("holder")]
        public string Holder { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// HyperpayRequestId
        /// </summary>
        [JsonProperty("hyperpayRequestId")]
        public int HyperpayRequestId { get; set; }

        /// <summary>
        /// BuildNumber
        /// </summary>
        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }

        /// <summary>
        /// ExpiryMonth
        /// </summary>
        [JsonProperty("expiryMonth")]
        public string ExpiryMonth { get; set; }

        /// <summary>
        /// ExpiryYear
        /// </summary>
        [JsonProperty("expiryYear")]
        public string ExpiryYear { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// ServiceResponse
        /// </summary>
        [JsonProperty("serviceResponse")]
        public string ServiceResponse { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// HyperpayRequest
        /// </summary>
        [JsonProperty("hyperpayRequest")]
        public HyperpayRequest HyperpayRequest { get; set; }

        /// <summary>
        /// IsCancelled
        /// </summary>
        [JsonProperty("isCancelled")]
        public bool? IsCancelled { get; set; } = false;

        /// <summary>
        /// CancelationDate
        /// </summary>
        [JsonProperty("cancelationDate")]
        public DateTime? CancelationDate { get; set; }

        /// <summary>
        /// CancelledBy
        /// </summary>
        [JsonProperty("cancelledBy")]
        public string CancelledBy { get; set; }
    }
}
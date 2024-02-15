using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Checkouts Model
    /// </summary>
    [JsonObject("CheckoutsModel")]
    public class CheckoutsModel
    {
        /// <summary>
        /// Driver
        /// </summary>
        [JsonProperty("driver")]
        public DriverModel Driver { get; set; }

        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// Payment Method Name
        /// </summary>
        [JsonProperty("paymentMethodName")]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Policy Status Arabic
        /// </summary>
        [JsonProperty("policyStatus")]
        public PolicyStatusModel PolicyStatus { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// referenceId
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// phone
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// iban
        /// </summary>
        [JsonProperty("iban")]
        public string IBAN { get; set; }

        /// <summary>
        /// createdDateTime
        /// </summary>
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// bankCodeId
        /// </summary>
        [JsonProperty("bankCodeId")]
        public int BankCodeId { get; set; }

        /// <summary>
        /// selectedLanguage
        /// </summary>
        [JsonProperty("selectedLanguage")]
        public string SelectedLanguage { get; set; }

        /// <summary>
        /// insuranceCompanyName
        /// </summary>
        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        /// <summary>
        /// ModifiedDate
        /// </summary>
        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// IsCancelled
        /// </summary>
        [JsonProperty("isCancelled")]
        public bool IsCancelled { get; set; }

        /// <summary>
        /// CancelationDate
        /// </summary>
        [JsonProperty("cancelationDate")]
        public DateTime CancelationDate { get; set; }

        /// <summary>
        /// CancelledBy
        /// </summary>
        [JsonProperty("cancelledBy")]
        public string CancelledBy { get; set; }

        /// <summary>
        /// Selected Insurance Type Code
        /// </summary>
        [JsonProperty("insuranceTypeCode")]
        public short? SelectedInsuranceTypeCode { get; set; }

        [JsonProperty("imageBack")]
        public Image ImageBack { get; set; }

        [JsonProperty("imageBody")]
        public Image ImageBody { get; set; }

        [JsonProperty("imageFront")]
        public Image ImageFront { get; set; }

        [JsonProperty("imageLeft")]
        public Image ImageLeft { get; set; }

        [JsonProperty("imageRight")]
        public Image ImageRight { get; set; }
        [JsonProperty("merchantId")]
        public Guid? MerchantTransactionId { get; set; }
    }

    /// <summary>
    /// image model
    /// </summary>
    [JsonObject("Image")]
    public class Image
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("imageData")]
        public byte[] ImageData { get; set; }

        [JsonProperty("newImageData")]
        public byte[] NewImageData { get; set; }
    }
}
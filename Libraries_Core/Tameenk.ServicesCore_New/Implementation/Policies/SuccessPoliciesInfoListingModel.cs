using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    ///  new success policies listing model
    /// </summary>
    public class SuccessPoliciesInfoListingModel
    {
        /// <summary>
        /// policyNo
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// insuranceCompanyNameEn
        /// </summary>
        [JsonProperty("insuranceCompanyNameEn")]
        public string InsuranceCompanyNameEn { get; set; }

        /// <summary>
        /// insuranceCompanyNameAr
        /// </summary>
        [JsonProperty("insuranceCompanyNameAr")]
        public string InsuranceCompanyNameAr { get; set; }

        /// <summary>
        /// referenceNo
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// najmStatusNameEn
        /// </summary>
        [JsonProperty("najmStatusNameEn")]
        public string NajmStatusNameEN { get; set; }

        /// <summary>
        /// najmStatusNameAr
        /// </summary>
        [JsonProperty("najmStatusNameAr")]
        public string NajmStatusNameAr { get; set; }

        /// <summary>
        /// invoiceNo
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int InvoiceNo { get; set; }

        /// <summary>
        /// paymentMethodNameEn
        /// </summary>
        [JsonProperty("paymentMethodNameEn")]
        public string PaymentMethodNameEN { get; set; }

        /// <summary>
        /// paymentMethodNameAr
        /// </summary>
        [JsonProperty("paymentMethodNameAr")]
        public string PaymentMethodNameAr { get; set; }
    }
}
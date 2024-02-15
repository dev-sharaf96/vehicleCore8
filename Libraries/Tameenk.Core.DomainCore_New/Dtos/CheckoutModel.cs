using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Tameenk.Core.Domain.Dtos
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("email")]
        public String Email { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("referenceId")]
        public String ReferenceId { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("phone")]
        public String Phone { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("bankCode")]
        public Nullable<int> BankCode { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("IBAN")]
        public String IBAN { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("imageRight")]
        public HttpPostedFileBase ImageRight { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("imageLeft")]
        public HttpPostedFileBase ImageLeft { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("imageFront")]
        public HttpPostedFileBase ImageFront { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("imageBack")]
        public HttpPostedFileBase ImageBack { get; set; }

        [Required(ErrorMessage = "* مطلوب")]
        [JsonProperty("imageBody")]
        public HttpPostedFileBase ImageBody { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("selectedProduct")]
        public ProductOrderModel SelectedProduct { get; set; }

        [JsonProperty("paymentAmount")]
        public decimal PaymentAmount { get; set; }

        [JsonProperty("quotationResponseId")]
        public int QuotationResponseId { get; set; }

        [JsonProperty("typeOfInsurance")]
        public int TypeOfInsurance { get; set; }

        [JsonProperty("paymentMethodCode")]
        public int? PaymentMethodCode { get; set; }

        [JsonProperty("lowestComperehensiveQoutPrice")]
        public decimal LowestComperehensiveQoutPrice { get; set; }

        [JsonProperty("qtRqstExtrnlId")]
        public string QtRqstExtrnlId { get; set; }

        [JsonProperty("insuranceCompanyId")]
        public int InsuranceCompanyId { get; set; }

        [JsonProperty("bankCodes")]
        public List<Lookup> BankCodes { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    /// <summary>
    /// The quotation response
    /// </summary>
    [JsonObject("quotationResponse")]
    public class QuotationResponseModel
    {
        public QuotationResponseModel()
        {
            Products = new HashSet<ProductModel>();
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("companyAllowAnonymous")]
        public bool CompanyAllowAnonymous { get; set; }
        [JsonProperty("anonymousRequest")]
        public bool AnonymousRequest { get; set; }
        [JsonProperty("hasDiscount")]
        public bool HasDiscount { get; set; }
        [JsonProperty("discountText")]
        public string DiscountText { get; set; }

        [JsonProperty("requestId")]
        public int? RequestId { get; set; }

        [JsonProperty("insuranceTypeCode")]
        public short? InsuranceTypeCode { get; set; }

        [JsonProperty("createDateTime")]
        public DateTime CreateDateTime { get; set; }

        [JsonProperty("vehicleAgencyRepair")]
        public bool? VehicleAgencyRepair { get; set; }

        [JsonProperty("deductibleValue")]
        public short? DeductibleValue { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductModel> Products { get; set; }

        [JsonProperty("productType")]
        public ProductTypeModel ProductType { get; set; }

        [JsonProperty("quotationRequest")]
        public QuotationRequestModel QuotationRequest { get; set; }

        //[NotMapped]
        //[JsonProperty("QuotationExceptionModel")]
        //public QuotationExceptionModel QuotationExceptionModel { set; get; }
    }
}
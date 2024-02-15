using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Quotation
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
        public int? DeductibleValue { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductModel> Products { get; set; }

        [JsonProperty("productType")]
        public ProductTypeModel ProductType { get; set; }

        [JsonProperty("quotationRequest")]
        public QuotationRequestModel QuotationRequest { get; set; }

        [JsonProperty("insurancecompanyid")]
        public int InsuranceCompanyId { get; set; }

        [JsonProperty("deductibleValuesList")]
        public List<int> DeductibleValuesList { get; set; }
        [JsonProperty("insurancecompanyKey")]
        public string insuranceCompanyKey { get; set; }
        [JsonProperty("arabicDriverName")]
        public string ArabicDriverName { get; set; }

        [JsonProperty("englishDriverName")]
        public string EnglishDriverName { get; set; }

        [JsonProperty("autoLeasingBulkSelectedBenfits")]
        public List<short> AutoLeasingSelectedBenfits { get; set; }

        [JsonProperty("allowToPurchase")]
        public bool AllowToPurchase { get; set; }

        [JsonProperty("vehicleValue")]
        public int VehicleValue { get; set; }

        [JsonProperty("bankId")]
        public int? BankId { get; set; }

        [JsonProperty("showTabby")]
        public bool ShowTabby
        {
            get;
            set;
        }
    }
}

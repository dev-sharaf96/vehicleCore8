using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Implementation.Invoices
{
   [JsonObject("CommissionListing")]
    public class CommissionAndFeesModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyKey { get; set; }
        public string Key { get; set; }
        public decimal? FixedFees { get; set; }
        public decimal? Percentage { get; set; }
        public int InsuranceTypeCode { get; set; }
        public bool CalculatedFromBasic { get; set; }
        public bool IsCommission { get; set; }
        public bool IsPercentageNegative { get; set; }
        public bool IsFixedFeesNegative { get; set; }
        [JsonProperty("PaymentMethod")]
        public int? PaymentMethodId { get; set; }
        public bool? IncludeAdditionalDriver { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
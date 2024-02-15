using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalQuotationResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("productTypeCode")]
        public int ProductTypeCode { get; set; }

        [JsonProperty("companyNameAr")]
        public string CompanyNameAr { get; set; }

        [JsonProperty("companyNameEn")]
        public string CompanyNameEn { get; set; }

        [JsonProperty("productPrice")]
        public decimal ProductPrice { get; set; }

        [JsonProperty("tkn")]
        public string Token { get; set; }
    }
}

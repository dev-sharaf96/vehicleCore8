using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    [JsonObject("priceType")]
    public class PriceTypeModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}
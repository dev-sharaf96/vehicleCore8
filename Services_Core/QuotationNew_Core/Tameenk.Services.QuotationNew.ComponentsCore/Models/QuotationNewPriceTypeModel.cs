using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationNew.Components
{
    [JsonObject("priceType")]
    public class QuotationNewPriceTypeModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }
    }
}
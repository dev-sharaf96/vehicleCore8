using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Enums.Payments;

namespace Tameenk.Core.Domain.Dtos
{
    public class PaymentMethodModel
    {
        /// <summary>
        /// The payment method name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }



        /// <summary>
        /// The payment method code.
        /// </summary>
        [JsonProperty("id")]
        public int Code { get; set; }
        [JsonProperty("active")]        public bool Active { get; set; }        [JsonProperty("order")]        public int Order { get; set; }
        [JsonProperty("Brands")]
        public string Brands { get; set; }

        [JsonProperty("AndroidEnabled")]
        public bool? AndroidEnabled { get; set; }

        [JsonProperty("IosEnabled")]
        public bool? IosEnabled { get; set; }

        [JsonProperty("LogoUrl")]
        public string LogoUrl { get; set; }
        [JsonProperty("EnglishDescription")]
        public string EnglishDescription { get; set; }
        [JsonProperty("ArabicDescription")]
        public string ArabicDescription { get; set; }
    }
}

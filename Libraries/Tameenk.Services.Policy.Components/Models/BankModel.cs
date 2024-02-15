using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// Bank Model
    /// </summary>
    [JsonObject("bank")]
    public class BankModel
    {
        
        /// <summary>
        /// code.
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// English Description.
        /// </summary>
        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        /// <summary>
        /// Arabic Description.
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }

        /// <summary>
        /// Validation Code.
        /// </summary>
        [JsonProperty("validationCode")]
        public int? ValidationCode { get; set; }

        /// <summary>
        /// BankAccountNo
        /// </summary>
        [JsonProperty("bankAccountNo")]
        public string IBAN { get; set; }
    }
}
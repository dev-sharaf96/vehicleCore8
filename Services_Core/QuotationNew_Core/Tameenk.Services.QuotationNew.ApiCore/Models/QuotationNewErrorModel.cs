using Newtonsoft.Json;

namespace Tameenk.Services.QuotationNew.Api.Models
{
    /// <summary>
    /// Class for error info.
    /// </summary>
    [JsonObject("error")]
    public class QuotationNewErrorModel
    {
        #region Ctor
        public QuotationNewErrorModel(string description = null, string code = null)
        {
            Description = description;
            Code = code;
        }

        #endregion

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Dtos
{
    public class SaudiPostResponseModel
    {

        /// <summary>
        /// Get or set the data.
        /// </summary>
        [JsonProperty("data")]
        public SaudiPostApiResultModel Data { get; set; }

        /// <summary>
        /// Get or set the total count of returned data.
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Get or set the list of errors.
        /// </summary>
        [JsonProperty("errors")]
        public IEnumerable<SaudiPostErrorModel> Errors { get; set; }
    }

    public class SaudiPostErrorModel
    {

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
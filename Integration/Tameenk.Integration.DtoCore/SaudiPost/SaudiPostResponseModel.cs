using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.SaudiPost
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
}

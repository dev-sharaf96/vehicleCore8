using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    /// <summary>
    /// Json response model
    /// </summary>
    [JsonObject("result")]
    public class CommonResponseModel<T>
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public CommonResponseModel()
        {

        }
        /// <summary>
        /// Empty Constructor
        /// </summary>
        /// <param name="data">The data result to be serialized.</param>
        /// <param name="totalCount">The total count of data result.</param>
        public CommonResponseModel(T data, int totalCount = 0)
        {
            Data = data;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Get or set the data.
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }

        /// <summary>
        /// Get or set the total count of returned data.
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Get or set the list of errors.
        /// </summary>
        [JsonProperty("errors")]
        public IEnumerable<ErrorModel> Errors { get; set; }

    }
}
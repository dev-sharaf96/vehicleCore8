﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tameenk.Services.QuotationNew.Api.Models
{
    /// <summary>
    /// Json response model
    /// </summary>
    [JsonObject("result")]
    public class QuotationNewCommonResponseModel<T>
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public QuotationNewCommonResponseModel()
        {

        }
        /// <summary>
        /// Empty Constructor
        /// </summary>
        /// <param name="data">The data result to be serialized.</param>
        /// <param name="totalCount">The total count of data result.</param>
        public QuotationNewCommonResponseModel(T data, int totalCount = 0)
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
        public IEnumerable<QuotationNewErrorModel> Errors { get; set; }

        /// <summary>
        /// Serialze this object to json string.
        /// </summary>
        /// <returns>serialized json string</returns>
        public string Serialize() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
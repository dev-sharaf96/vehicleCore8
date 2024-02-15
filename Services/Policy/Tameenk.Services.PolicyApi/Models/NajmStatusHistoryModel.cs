using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Najm status history model.
    /// </summary>
    [JsonObject("najmStatusHistory")]
    public class NajmStatusHistoryModel
    {
        /// <summary>
        /// The najm status history identity
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// The policy reference id.
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        /// <summary>
        /// The policy no.
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        /// <summary>
        /// The status code.
        /// </summary>
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
        /// <summary>
        /// The status description.
        /// </summary>
        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }
        /// <summary>
        /// The upload date.
        /// </summary>
        [JsonProperty("uploadedDate")]
        public DateTime? UploadedDate { get; set; }
        /// <summary>
        /// The upload reference.
        /// </summary>
        [JsonProperty("uploadedReference")]
        public string UploadedReference { get; set; }
    }
}
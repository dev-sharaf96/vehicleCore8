using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// Represent a class for policy update request attachment model.
    /// </summary>
    [JsonObject("policyUpdateRequestAttachment")]
    public class PolicyUpdateRequestAttachmentModel
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Policy update request identifier.
        /// </summary>
        [JsonProperty("policyUpdReqId")]
        public int PolicyUpdReqId { get; set; }

        /// <summary>
        /// Attachment identifier.
        /// </summary>
        [JsonProperty("attachmentId")]
        public int AttachmentId { get; set; }


        /// <summary>
        /// type of attachment (ex: user id, Driving license..)
        /// </summary>
        [JsonProperty("attachmentTypeId")]
        public int AttachmentTypeId { get; set; }
    }
}
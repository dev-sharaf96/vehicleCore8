using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Represent the update request model for a certain policy.
    /// </summary>
    [JsonObject("policyUpdateRequest")]
    public class PolicyUpdateRequestModel
    {
        /// <summary>
        /// Initialize a new instance of PolicyUpdateRequest.
        /// </summary>
        public PolicyUpdateRequestModel()
        {
            PolicyUpdateRequestAttachments = new HashSet<PolicyUpdateRequestAttachmentModel>();
            PolicyUpdatePayments = new HashSet<PolicyUpdatePaymentModel>();
        }
        /// <summary>
        /// The identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// External reference Id as global unique identifier.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Reference to Policy Id.
        /// </summary>
        [JsonProperty("policyId")]
        public int PolicyId { get; set; }

        /// <summary>
        /// Reference to Request Type Id
        /// </summary>
        [JsonProperty("requestTypeId")]
        public int RequestTypeId { get; set; }

        /// <summary>
        /// The policy update request status.
        /// Pending = 0,
        /// Approved = 1,
        /// Rejected = 2,
        /// AwaitingPayment = 3,
        /// PaidAwaitingApproval = 4
        /// </summary>
        [JsonProperty("statusId")]
        public int StatusId { get; set; }

        /// <summary>
        /// Get os set the policy update request attachments.
        /// </summary>
        [JsonProperty("attachments")]
        public virtual ICollection<PolicyUpdateRequestAttachmentModel> PolicyUpdateRequestAttachments { get; set; }


        /// <summary>
        /// Get os set the policy update request payments.
        /// </summary>
        [JsonProperty("payments")]
        public virtual ICollection<PolicyUpdatePaymentModel> PolicyUpdatePayments { get; set; }

        /// <summary>
        /// Get or set the policy linked to this request. 
        /// </summary>
        [JsonProperty("policy")]
        public virtual PolicyModel Policy { get; set; }
    }
}
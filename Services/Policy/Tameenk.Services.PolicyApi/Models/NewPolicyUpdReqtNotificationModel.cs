using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Represent New Policy Upate Request Notification .
    /// </summary>
    [JsonObject("newPolicyUpdReqtNotification")]
    public class NewPolicyUpdReqtNotificationModel : NotificationModel
    {
        #region Ctor

        /// <summary>
        /// The Constructor.
        /// </summary>
        public NewPolicyUpdReqtNotificationModel()
        {

        }


        /// <summary>
        /// The Constructor.
        /// </summary>
        /// <param name="notification">Notification</param>
        public NewPolicyUpdReqtNotificationModel(NotificationModel notification)
        {
            CreatedAt = notification.CreatedAt;
            Id = notification.Id;
            Status = notification.Status;
            Type = notification.Type;
        }
        #endregion

        /// <summary>
        /// The Policy Update Request Guid.
        /// </summary>
        [JsonProperty("policyUpdateRequestGuid")]
        public string PolicyUpdateRequestGuid { get; set; }

        /// <summary>
        /// The Policy Id.
        /// </summary>
        [JsonProperty("policyId")]
        public string PolicyId { get; set; }

        /// <summary>
        /// The Policy File Id.
        /// </summary>
        [JsonProperty("policyFileId")]
        public string PolicyFileId { get; set; }
    }
}
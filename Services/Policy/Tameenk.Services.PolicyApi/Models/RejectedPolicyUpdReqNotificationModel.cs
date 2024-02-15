using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Represent Rejected Policy Update Requset Notification.
    /// </summary>
    [JsonObject("RejectedPolicyUpdReqNotification")]
    public class RejectedPolicyUpdReqNotificationModel : NotificationModel
    {
        #region Ctor
        /// <summary>
        /// The Constructor
        /// </summary>
        public RejectedPolicyUpdReqNotificationModel()
        {

        }
        /// <summary>
        /// The Constructor
        /// </summary>
        /// <param name="notification"></param>
        public RejectedPolicyUpdReqNotificationModel(NotificationModel notification)
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

    }
    }
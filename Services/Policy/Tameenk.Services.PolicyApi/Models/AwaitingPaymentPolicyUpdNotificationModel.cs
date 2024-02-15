using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Represent Awaiting Payment Policy Update Notification.
    /// </summary>
    [JsonObject("awaitingPaymentPolicyUpdNotification")]
    public class AwaitingPaymentPolicyUpdNotificationModel : NotificationModel
    {
        #region Ctor
        /// <summary>
        /// the Constructor .
        /// </summary>
        /// <param name="notification">Notification model</param>
        public AwaitingPaymentPolicyUpdNotificationModel(NotificationModel notification)
        {
            CreatedAt = notification.CreatedAt;
            Id = notification.Id;
            Status = notification.Status;
            Type = notification.Type;
        }
        /// <summary>
        /// the constructor
        /// </summary>
        public AwaitingPaymentPolicyUpdNotificationModel()
        {

        }
        #endregion

        #region Properties
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
        /// enable payment button 
        /// </summary>
        [JsonProperty("enablePaymentButton")]
        public bool EnablePaymentButton { get; set; }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Messages;

namespace TameenkDAL.Models.Notifications
{
    public class AwaitingPaymentPolicyUpdNotificationModel : NotificationModel
    {
        #region Ctor
        public AwaitingPaymentPolicyUpdNotificationModel(Notification notification)
        {
            CreatedAt = notification.CreatedAt;
            Id = notification.Id;
            Status = notification.Status;
            Type = notification.Type;
        }

        public AwaitingPaymentPolicyUpdNotificationModel()
        {

        }
        #endregion

        #region Properties
        public string PolicyUpdateRequestGuid { get; set; }
        public string PolicyId { get; set; }
        public bool EnablePaymentButton { get; set; }

        #endregion
    }
}

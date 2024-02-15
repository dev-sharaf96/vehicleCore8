using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Enums.Messages
{
    /// <summary>
    /// The notification type.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Notification for general purpose.
        /// </summary>
        General = 0,
        /// <summary>
        /// Notification for new policy update request added to system
        /// </summary>
        NewPolicyUpdateRequest = 10,
        /// <summary>
        /// Notification for policy update request approved by insurance provider.
        /// </summary>
        PolicyUpdateRequestApproved = 11,
        /// <summary>
        /// Notification for policy update request rejected by insurance provider.
        /// </summary>
        PolicyUpdateRequestRejected = 12,
        /// <summary>
        /// Notification for policy update request is awiting payment from the user.
        /// </summary>
        PolicyUpdateRequestAwaitingPayment = 13,
        /// <summary>
        /// Notification for policy update request is paid
        /// </summary>
        PolicyUpdateRequestPaid = 14
    }
}

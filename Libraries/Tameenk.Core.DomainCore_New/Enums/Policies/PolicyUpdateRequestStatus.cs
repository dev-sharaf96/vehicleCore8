using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Enums.Policies
{
    /// <summary>
    /// Represent policy update request status.
    /// </summary>
    public enum PolicyUpdateRequestStatus
    {
        /// <summary>
        /// Pending status.
        /// </summary>
        Pending = 0,
        /// <summary>
        /// Approved status.
        /// </summary>
        Approved = 1,
        /// <summary>
        /// Rejected status.
        /// </summary>
        Rejected = 2,
        /// <summary>
        /// Awaiting payment status.
        /// </summary>
        AwaitingPayment = 3,
        /// <summary>
        /// Paid awaiting approval status.
        /// </summary>
        PaidAwaitingApproval = 4,
        /// <summary>
        /// If request had payment and user didnt pay it within 16 hour, then Request status become expired
        /// </summary>
        Expired = 5

    }
}

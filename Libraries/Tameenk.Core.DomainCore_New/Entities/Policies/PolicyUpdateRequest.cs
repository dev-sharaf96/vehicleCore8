using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Enums.Policies;

namespace Tameenk.Core.Domain.Entities.Policies
{
    /// <summary>
    /// Represent the update request for a certain policy.
    /// </summary>
    public class PolicyUpdateRequest : BaseEntity
    {
        /// <summary>
        /// Initialize a new instance of PolicyUpdateRequest.
        /// </summary>
        public PolicyUpdateRequest()
        {
            PolicyUpdateRequestAttachments = new HashSet<PolicyUpdateRequestAttachment>();
            PolicyUpdatePayments = new HashSet<PolicyUpdatePayment>();
            PayfortPaymentRequests = new HashSet<PayfortPaymentRequest>();
        }
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// External reference Id as global unique identifier.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Reference to Policy Id
        /// </summary>
        public int PolicyId { get; set; }

        /// <summary>
        /// Reference to Request Type Id
        /// </summary>
        public int RequestTypeId { get; set; }

        /// <summary>
        /// Get or set the policy update request status identifier.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Get os set the policy update request attachments.
        /// </summary>
        public virtual ICollection<PolicyUpdateRequestAttachment> PolicyUpdateRequestAttachments { get; set; }

        /// <summary>
        /// Get or set the policy linked to this request. 
        /// </summary>
        public virtual Policy Policy { get; set; }

        /// <summary>
        /// The policy update payment(s).
        /// </summary>
        public virtual ICollection<PolicyUpdatePayment> PolicyUpdatePayments { get; set; }

        /// <summary>
        /// Get or set the request type.
        /// </summary>
        public PolicyUpdateRequestType RequestType {
            get { return (PolicyUpdateRequestType)RequestTypeId; }
            set { RequestTypeId = (int)value; }
        }

        /// <summary>
        /// Get or set the policy status.
        /// </summary>
        public PolicyUpdateRequestStatus Status {
            get { return (PolicyUpdateRequestStatus)StatusId; }
            set { StatusId = (int)value; }
        }
        public virtual ICollection<PayfortPaymentRequest> PayfortPaymentRequests { get; set; }

    }
}

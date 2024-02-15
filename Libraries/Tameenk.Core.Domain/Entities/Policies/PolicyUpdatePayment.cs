using System;

namespace Tameenk.Core.Domain.Entities.Policies
{
    /// <summary>
    /// Represent the policy update payment.
    /// </summary>
    public class PolicyUpdatePayment : BaseEntity
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Policy update request id
        /// </summary>
        public int PolicyUpdateRequestId { get; set; }

        /// <summary>
        /// Policy update amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Created by
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Policy update request.
        /// </summary>
        public virtual PolicyUpdateRequest PolicyUpdateRequest { get; set; }
    }
}

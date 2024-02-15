using System;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class RenewalPolicies : BaseEntity
    {

        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the checkout reference id.
        /// </summary>
        public string VehicleId { get; set; }
        public string OldNin { get; set; }
        public string NewNin { get; set; }
        public string OldPolicyNo { get; set; }
        public string NewPolicyNo { get; set; }
        public string OldReferenceId { get; set; }
        public string NewReferenceId { get; set; }
        public DateTime? OldPolicyIssueDate { get; set; }
        public DateTime? OldPolicyEffectiveDate { get; set; }
        public DateTime? OldPolicyExpiryDate { get; set; }
        public DateTime? OldCheckoutCreatedDate { get; set; }
        public DateTime? NewPolicyIssueDate { get; set; }
        public DateTime? NewPolicyEffectiveDate { get; set; }
        public DateTime? NewPolicyExpiryDate { get; set; }
        public DateTime? NewCheckoutCreatedDate { get; set; }
        public int? OldCompanyId { get; set; }
        public int? NewCompanyId { get; set; }
        public string OldCompanyKey { get; set; }
        public string NewCompanyKey { get; set; }
        public short? OldProductType { get; set; }
        public short? NewProductType { get; set; }
        public decimal? OldTotalPrice { get; set; }
        public decimal? NewTotalPrice { get; set; }
        public string OldChannel { get; set; }
        public string NewChannel { get; set; }
        public bool? FromNotification { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}

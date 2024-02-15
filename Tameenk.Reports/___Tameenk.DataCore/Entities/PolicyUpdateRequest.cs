namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyUpdateRequest")]
    public partial class PolicyUpdateRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PolicyUpdateRequest()
        {
            PolicyUpdatePayments = new HashSet<PolicyUpdatePayment>();
            PolicyUpdateRequestAttachments = new HashSet<PolicyUpdateRequestAttachment>();
            PayfortPaymentRequests = new HashSet<PayfortPaymentRequest>();
        }

        public int Id { get; set; }

        public int? PolicyId { get; set; }

        public int RequestTypeId { get; set; }

        public int StatusId { get; set; }

        [StringLength(50)]
        public string Guid { get; set; }

        public virtual Policy Policy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolicyUpdatePayment> PolicyUpdatePayments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolicyUpdateRequestAttachment> PolicyUpdateRequestAttachments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayfortPaymentRequest> PayfortPaymentRequests { get; set; }
    }
}

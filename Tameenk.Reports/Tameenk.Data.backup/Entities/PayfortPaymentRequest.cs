namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PayfortPaymentRequest")]
    public partial class PayfortPaymentRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PayfortPaymentRequest()
        {
            PayfortPaymentResponses = new HashSet<PayfortPaymentResponse>();
            CheckoutDetails = new HashSet<CheckoutDetail>();
            PolicyUpdateRequests = new HashSet<PolicyUpdateRequest>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public decimal? Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string ReferenceNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayfortPaymentResponse> PayfortPaymentResponses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolicyUpdateRequest> PolicyUpdateRequests { get; set; }
    }
}

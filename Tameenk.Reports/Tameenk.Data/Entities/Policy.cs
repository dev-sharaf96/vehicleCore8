namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Policy")]
    public partial class Policy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Policy()
        {
            Invoices = new HashSet<Invoice>();
            PolicyUpdateRequests = new HashSet<PolicyUpdateRequest>();
        }

        public int Id { get; set; }

        public int? InsuranceCompanyID { get; set; }

        public byte StatusCode { get; set; }

        [Required]
        [StringLength(36)]
        public string PolicyNo { get; set; }

        public DateTime? PolicyIssueDate { get; set; }

        public DateTime? PolicyEffectiveDate { get; set; }

        public DateTime? PolicyExpiryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string CheckOutDetailsId { get; set; }

        public Guid? PolicyFileId { get; set; }

        public string NajmStatus { get; set; }

        public bool IsRefunded { get; set; }

        public int NajmStatusId { get; set; }

        public virtual CheckoutDetail CheckoutDetail { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        public virtual NajmStatu NajmStatu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolicyUpdateRequest> PolicyUpdateRequests { get; set; }

        public virtual PolicyFile PolicyFile { get; set; }

        public virtual PolicyDetail PolicyDetail { get; set; }
    }
}

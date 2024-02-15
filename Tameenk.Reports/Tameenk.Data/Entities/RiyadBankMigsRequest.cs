namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RiyadBankMigsRequest")]
    public partial class RiyadBankMigsRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RiyadBankMigsRequest()
        {
            Checkout_RiyadBankMigsRequest = new HashSet<Checkout_RiyadBankMigsRequest>();
            Checkout_RiyadBankMigsRequest1 = new HashSet<Checkout_RiyadBankMigsRequest>();
            RiyadBankMigsResponses = new HashSet<RiyadBankMigsResponse>();
        }

        public int Id { get; set; }

        [StringLength(200)]
        public string AccessCode { get; set; }

        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Command { get; set; }

        [StringLength(200)]
        public string Locale { get; set; }

        [StringLength(200)]
        public string MerchTxnRef { get; set; }

        [StringLength(200)]
        public string MerchantId { get; set; }

        [StringLength(200)]
        public string OrderInfo { get; set; }

        [StringLength(200)]
        public string ReturnUrl { get; set; }

        [StringLength(200)]
        public string Version { get; set; }

        [StringLength(200)]
        public string SecureHash { get; set; }

        [StringLength(200)]
        public string SecureHashType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Checkout_RiyadBankMigsRequest> Checkout_RiyadBankMigsRequest { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Checkout_RiyadBankMigsRequest> Checkout_RiyadBankMigsRequest1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RiyadBankMigsResponse> RiyadBankMigsResponses { get; set; }
    }
}

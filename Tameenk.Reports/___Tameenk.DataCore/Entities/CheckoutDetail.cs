namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CheckoutDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CheckoutDetail()
        {
            CheckoutAdditionalDrivers = new HashSet<CheckoutAdditionalDriver>();
            OrderItems = new HashSet<OrderItem>();
            Policies = new HashSet<Policy>();
            PayfortPaymentRequests = new HashSet<PayfortPaymentRequest>();
        }

        [Key]
        [StringLength(50)]
        public string ReferenceId { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string IBAN { get; set; }

        public int? ImageRightId { get; set; }

        public int? ImageLeftId { get; set; }

        public int? ImageFrontId { get; set; }

        public int? ImageBackId { get; set; }

        public int? ImageBodyId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid? MainDriverId { get; set; }

        public int? PolicyStatusId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public Guid? SelectedProductId { get; set; }

        public short? SelectedInsuranceTypeCode { get; set; }

       // public int? bankCode { get; set; }


        public int? bankCodeId { get; set; }


        public int? PaymentMethodId { get; set; }

        public virtual AdditionalInfo AdditionalInfo { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutAdditionalDriver> CheckoutAdditionalDrivers { get; set; }

        public virtual CheckoutCarImage CheckoutCarImage { get; set; }

        public virtual CheckoutCarImage CheckoutCarImage1 { get; set; }

        public virtual CheckoutCarImage CheckoutCarImage2 { get; set; }

        public virtual CheckoutCarImage CheckoutCarImage3 { get; set; }

        public virtual CheckoutCarImage CheckoutCarImage4 { get; set; }

        public virtual Driver Driver { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }

        public virtual PolicyStatu PolicyStatu { get; set; }

        public virtual Product Product { get; set; }

        public virtual ProductType ProductType { get; set; }

        public virtual Vehicle Vehicle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Policy> Policies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayfortPaymentRequest> PayfortPaymentRequests { get; set; }
    }
}

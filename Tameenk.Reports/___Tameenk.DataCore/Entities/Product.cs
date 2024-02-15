namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
            OrderItems = new HashSet<OrderItem>();
            PriceDetails = new HashSet<PriceDetail>();
            Product_Benefit = new HashSet<Product_Benefit>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        public Guid Id { get; set; }

        [StringLength(100)]
        public string ExternalProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string QuotaionNo { get; set; }

        public DateTime? QuotationDate { get; set; }

        public DateTime? QuotationExpiryDate { get; set; }

        public int? ProviderId { get; set; }

        public string ProductNameAr { get; set; }

        public string ProductNameEn { get; set; }

        public string ProductDescAr { get; set; }

        public string ProductDescEn { get; set; }

        public decimal ProductPrice { get; set; }

        public int? DeductableValue { get; set; }

        public int? VehicleLimitValue { get; set; }

        public int? QuotationResponseId { get; set; }

        [StringLength(250)]
        public string ProductImage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceDetail> PriceDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Benefit> Product_Benefit { get; set; }

        public virtual QuotationResponse QuotationResponse { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}

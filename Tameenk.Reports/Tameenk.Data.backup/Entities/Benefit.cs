namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Benefit")]
    public partial class Benefit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Benefit()
        {
            InsuaranceCompanyBenefits = new HashSet<InsuaranceCompanyBenefit>();
            Invoice_Benefit = new HashSet<Invoice_Benefit>();
            OrderItemBenefits = new HashSet<OrderItemBenefit>();
            Product_Benefit = new HashSet<Product_Benefit>();
        }

        [Key]
        public short Code { get; set; }

        [StringLength(200)]
        public string EnglishDescription { get; set; }

        [StringLength(200)]
        public string ArabicDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Benefit> Invoice_Benefit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItemBenefit> OrderItemBenefits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Benefit> Product_Benefit { get; set; }
    }
}

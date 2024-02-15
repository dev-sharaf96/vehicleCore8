namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product_Benefit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Benefit()
        {
            ShoppingCartItemBenefits = new HashSet<ShoppingCartItemBenefit>();
        }

        public int Id { get; set; }

        public Guid? ProductId { get; set; }

        public short? BenefitId { get; set; }

        public bool? IsSelected { get; set; }

        public decimal? BenefitPrice { get; set; }

        [StringLength(50)]
        public string BenefitExternalId { get; set; }

        public virtual Benefit Benefit { get; set; }

        public virtual Product Product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCartItemBenefit> ShoppingCartItemBenefits { get; set; }
    }
}

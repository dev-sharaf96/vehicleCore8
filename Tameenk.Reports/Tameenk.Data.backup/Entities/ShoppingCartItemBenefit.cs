namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShoppingCartItemBenefit")]
    public partial class ShoppingCartItemBenefit
    {
        public int Id { get; set; }

        public int ShoppingCartItemId { get; set; }

        public int ProductBenefitId { get; set; }

        public virtual Product_Benefit Product_Benefit { get; set; }
    }
}

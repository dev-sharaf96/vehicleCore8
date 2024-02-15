namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderItemBenefit")]
    public partial class OrderItemBenefit
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }

        public short? BenefitId { get; set; }

        public decimal Price { get; set; }

        [StringLength(50)]
        public string BenefitExternalId { get; set; }

        public virtual Benefit Benefit { get; set; }

        public virtual OrderItem OrderItem { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PriceDetail")]
    public partial class PriceDetail
    {
        [Key]
        public Guid DetailId { get; set; }

        public Guid ProductID { get; set; }

        public byte PriceTypeCode { get; set; }

        public decimal PriceValue { get; set; }

        public decimal? PercentageValue { get; set; }

        public virtual PriceType PriceType { get; set; }

        public virtual Product Product { get; set; }
    }
}

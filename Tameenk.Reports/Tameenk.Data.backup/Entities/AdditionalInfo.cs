namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdditionalInfo")]
    public partial class AdditionalInfo
    {
        [Key]
        [StringLength(50)]
        public string ReferenceId { get; set; }

        public string InfoAsJsonString { get; set; }

        [StringLength(4000)]
        public string DriverAdditionalInfo { get; set; }

        public virtual CheckoutDetail CheckoutDetail { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CheckoutAdditionalDriver
    {
        [Required]
        [StringLength(50)]
        public string CheckoutDetailsId { get; set; }

        public Guid DriverId { get; set; }

        public int Id { get; set; }

        public virtual CheckoutDetail CheckoutDetail { get; set; }

        public virtual Driver Driver { get; set; }
    }
}

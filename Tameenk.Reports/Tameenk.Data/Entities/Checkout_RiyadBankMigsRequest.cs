namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Checkout_RiyadBankMigsRequest
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RiyadBankMigsRequestId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string CheckoutdetailsId { get; set; }

        public virtual RiyadBankMigsRequest RiyadBankMigsRequest { get; set; }

        public virtual RiyadBankMigsRequest RiyadBankMigsRequest1 { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Payment")]
    public partial class Payment
    {
        [Key]
        public int BillNumber { get; set; }

        [StringLength(50)]
        public string ReferenceID { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        [StringLength(25)]
        public string IBNA { get; set; }

        public int? BankCode { get; set; }

        public int? PaymentStatus { get; set; }
    }
}

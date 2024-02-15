namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BankCode")]
    public partial class BankCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Code { get; set; }

        [StringLength(200)]
        public string EnglishDescription { get; set; }

        [StringLength(200)]
        public string ArabicDescription { get; set; }

        public int? ValidationCode { get; set; }
    }
}

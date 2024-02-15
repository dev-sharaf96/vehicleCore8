namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ErrorCode")]
    public partial class ErrorCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Code { get; set; }

        [StringLength(50)]
        public string EnglishText { get; set; }

        [StringLength(200)]
        public string EnglishDescription { get; set; }

        [StringLength(50)]
        public string ArabicText { get; set; }

        [StringLength(200)]
        public string ArabicDescription { get; set; }
    }
}

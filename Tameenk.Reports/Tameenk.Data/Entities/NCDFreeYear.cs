namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NCDFreeYear")]
    public partial class NCDFreeYear
    {
        [Key]
        public byte Code { get; set; }

        [StringLength(400)]
        public string EnglishDescription { get; set; }

        [StringLength(400)]
        public string ArabicDescription { get; set; }
    }
}

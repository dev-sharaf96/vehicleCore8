namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Country")]
    public partial class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Code { get; set; }

        [StringLength(256)]
        public string EnglishDescription { get; set; }

        [StringLength(256)]
        public string ArabicDescription { get; set; }
    }
}

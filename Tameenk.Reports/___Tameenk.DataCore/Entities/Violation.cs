namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Violation")]
    public partial class Violation
    {
        public int Id { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(100)]
        public string DescriptionAr { get; set; }

        [Required]
        [StringLength(100)]
        public string DescriptionEn { get; set; }
    }
}

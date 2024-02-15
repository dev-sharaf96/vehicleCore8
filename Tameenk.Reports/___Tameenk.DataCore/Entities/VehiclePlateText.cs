namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VehiclePlateText")]
    public partial class VehiclePlateText
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Code { get; set; }

        [StringLength(50)]
        public string EnglishDescription { get; set; }

        [StringLength(50)]
        public string ArabicDescription { get; set; }
    }
}

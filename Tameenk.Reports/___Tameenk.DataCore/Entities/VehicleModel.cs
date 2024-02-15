namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VehicleModel")]
    public partial class VehicleModel
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Code { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short VehicleMakerCode { get; set; }

        [StringLength(50)]
        public string EnglishDescription { get; set; }

        [StringLength(50)]
        public string ArabicDescription { get; set; }

        public virtual VehicleMaker VehicleMaker { get; set; }
    }
}

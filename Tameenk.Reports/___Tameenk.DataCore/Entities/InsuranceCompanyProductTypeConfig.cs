namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InsuranceCompanyProductTypeConfig")]
    public partial class InsuranceCompanyProductTypeConfig
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short ProductTypeCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InsuranceCompanyID { get; set; }

        public byte MinDriverAge { get; set; }

        public byte MaxDriverAge { get; set; }

        public byte MaxVehicleAge { get; set; }

        public virtual ProductType ProductType { get; set; }
    }
}

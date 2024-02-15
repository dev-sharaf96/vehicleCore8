namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DriverLicense")]
    public partial class DriverLicense
    {
        [Key]
        public int LicenseId { get; set; }

        public Guid DriverId { get; set; }

        public short? TypeDesc { get; set; }

        [StringLength(20)]
        public string ExpiryDateH { get; set; }

        [StringLength(20)]
        public string IssueDateH { get; set; }

        public virtual Driver Driver { get; set; }

        public virtual LicenseType LicenseType { get; set; }
    }
}

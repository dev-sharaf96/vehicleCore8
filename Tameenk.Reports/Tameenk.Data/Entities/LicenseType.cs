namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LicenseType")]
    public partial class LicenseType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LicenseType()
        {
            DriverLicenses = new HashSet<DriverLicense>();
        }

        [Key]
        public short Code { get; set; }

        [StringLength(400)]
        public string EnglishDescription { get; set; }

        [StringLength(400)]
        public string ArabicDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DriverLicense> DriverLicenses { get; set; }
    }
}

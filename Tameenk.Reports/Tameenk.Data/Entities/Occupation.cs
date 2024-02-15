namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Occupation")]
    public partial class Occupation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Occupation()
        {
            Drivers = new HashSet<Driver>();
            Insureds = new HashSet<Insured>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(200)]
        public string NameAr { get; set; }

        [StringLength(200)]
        public string NameEn { get; set; }

        public bool? IsCitizen { get; set; }

        public bool? IsMale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Driver> Drivers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Insured> Insureds { get; set; }
    }
}

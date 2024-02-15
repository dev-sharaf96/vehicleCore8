namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VehicleSpecification")]
    public partial class VehicleSpecification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VehicleSpecification()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(100)]
        public string DescriptionAr { get; set; }

        [Required]
        [StringLength(100)]
        public string DescriptionEn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Role")]
    public partial class Role
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Role()
        {
            AspNetUsers = new HashSet<AspNetUser>();
        }

        public Guid ID { get; set; }

        public Guid RoleTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleNameAR { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleNameEN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }

        public virtual RoleType RoleType { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Notification()
        {
            NotificationParameters = new HashSet<NotificationParameter>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Group { get; set; }

        [StringLength(512)]
        public string GroupReferenceId { get; set; }

        public int TypeId { get; set; }

        public int StatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationParameter> NotificationParameters { get; set; }
    }
}

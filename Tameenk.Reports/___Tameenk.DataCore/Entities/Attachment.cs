namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Attachment")]
    public partial class Attachment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Attachment()
        {
            PolicyUpdateRequestAttachments = new HashSet<PolicyUpdateRequestAttachment>();
        }

        public int Id { get; set; }

        public Guid Guid { get; set; }

        [Column(TypeName = "image")]
        public byte[] AttachmentFile { get; set; }

        [StringLength(50)]
        public string AttachmentType { get; set; }

        [Required]
        [StringLength(100)]
        public string AttachmentName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolicyUpdateRequestAttachment> PolicyUpdateRequestAttachments { get; set; }
    }
}

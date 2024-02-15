namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyUpdateRequestAttachment")]
    public partial class PolicyUpdateRequestAttachment
    {
        public int Id { get; set; }

        public int PolicyUpdReqId { get; set; }

        public int AttachmentId { get; set; }

        public int AttachmentTypeId { get; set; }

        public virtual Attachment Attachment { get; set; }

        public virtual PolicyUpdateRequest PolicyUpdateRequest { get; set; }
    }
}

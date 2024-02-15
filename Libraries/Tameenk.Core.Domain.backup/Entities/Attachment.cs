using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Core.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public Attachment()
        {
            PolicyUpdateRequestAttachments = new HashSet<PolicyUpdateRequestAttachment>();
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// External reference Id
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Attachment File
        /// </summary>
        public byte[] AttachmentFile { get; set; }

        /// <summary>
        /// Attachment MIME type
        /// </summary>
        public string AttachmentType { get; set; }

        /// <summary>
        /// Attachment Name
        /// </summary>
        public string AttachmentName { get; set; }

        public virtual ICollection<PolicyUpdateRequestAttachment> PolicyUpdateRequestAttachments { get; set; }
    }
}

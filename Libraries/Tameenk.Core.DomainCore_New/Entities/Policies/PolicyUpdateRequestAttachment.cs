namespace Tameenk.Core.Domain.Entities.Policies
{
    /// <summary>
    /// Represent the update request attachment.
    /// </summary>
    public class PolicyUpdateRequestAttachment : BaseEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Policy update request id
        /// </summary>
        public int PolicyUpdReqId { get; set; }

        /// <summary>
        /// Attachment id
        /// </summary>
        public int AttachmentId { get; set; }


        /// <summary>
        /// type of attachment (ex: user id, Driving license..)
        /// </summary>
        public int AttachmentTypeId { get; set; }

        /// <summary>
        /// The attachment.
        /// </summary>
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// The policy update request.
        /// </summary>
        public virtual PolicyUpdateRequest PolicyUpdateRequest { get; set; }

    }
}

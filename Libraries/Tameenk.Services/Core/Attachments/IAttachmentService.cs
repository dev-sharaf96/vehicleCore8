using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.Attachments
{
    public interface IAttachmentService
    {
        /// <summary>
        /// Get Attachment by database Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Attachment that match this id, or null if not exist</returns>
        Attachment GetAttachmentById(int id);

        /// <summary>
        /// Get all attachments
        /// </summary>
        /// <returns>List of all attachments</returns>
        IPagedList<Attachment> GetAttachments(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get attachment by Guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Attachment that match gevin guid, or null if no match</returns>
        Attachment GetAttachmentByGuid(Guid guid);


        /// <summary>
        /// Create attachment
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>Guid of created attachment</returns>
        Guid Create(Attachment attachment);


        /// <summary>
        /// Create list of attachments
        /// </summary>
        /// <param name="attachments">List of attachments</param>
        /// <returns>List of created attachments</returns>
        List<Attachment> Create(List<Attachment> attachments);

        /// <summary>
        /// Update attachment
        /// </summary>
        /// <param name="attachment">Attachment to update</param>
        void Update(Attachment attachment);

        /// <summary>
        /// Delete Attachment
        /// </summary>
        /// <param name="attachment">Attachment</param>
        void Delete(Attachment attachment);

    }
}

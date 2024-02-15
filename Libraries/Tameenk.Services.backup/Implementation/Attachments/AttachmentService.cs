using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Attachments;

namespace Tameenk.Services.Implementation.Attachments
{
    public class AttachmentService : IAttachmentService
    {
        #region Fields
        private readonly IRepository<Attachment> _attachmentRepository;
        #endregion

        #region Ctor
        public AttachmentService(IRepository<Attachment> attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get attachment by Guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Attachment that match gevin guid, or null if no match</returns>
        public Attachment GetAttachmentByGuid(Guid guid)
        {
            return _attachmentRepository.Table.FirstOrDefault(x => x.Guid == guid);
        }

        /// <summary>
        /// Get Attachment by database Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Attachment that match this id, or null if not exist</returns>
        public Attachment GetAttachmentById(int id)
        {
            return _attachmentRepository.Table.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Get all attachments
        /// </summary>
        /// <returns>List of all attachments</returns>
        public IPagedList<Attachment> GetAttachments(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var qry = _attachmentRepository.Table.OrderBy(n => n.Id);
            return new PagedList<Attachment>(qry, pageIndex, pageSize);
        }

        /// <summary>
        /// Update attachment
        /// </summary>
        /// <param name="attachment">Attachment to update</param>
        public void Update(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }
            _attachmentRepository.Update(attachment);
        }


        /// <summary>
        /// Create attachment
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>Guid of created attachment</returns>
        public Guid Create(Attachment attachment)
        {
            if(attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }
            attachment.Guid = Guid.NewGuid();
            _attachmentRepository.Insert(attachment);
            return attachment.Guid;
        }


        /// <summary>
        /// Create list of attachments
        /// </summary>
        /// <param name="attachments">List of attachments</param>
        /// <returns>List of created attachments</returns>
        public List<Attachment> Create(List<Attachment> attachments)
        {
            if(attachments ==null)
                throw new ArgumentNullException(nameof(attachments));
            if (attachments.Count == 0)
                throw new ArgumentNullException("attachments", "Attachments must have items");
            //insert attachments
            //loop on attachments and set Guid prop to new Guid
            foreach (var attachment in attachments)
            {
                attachment.Guid = Guid.NewGuid();
            }
            _attachmentRepository.Insert(attachments);
            //return guid of created attachments
            return attachments;
        }

        /// <summary>
        /// Delete Attachment
        /// </summary>
        /// <param name="attachment">Attachment</param>
        public void Delete(Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            _attachmentRepository.Delete(attachment);
        }
        #endregion
    }
}

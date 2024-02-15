using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain.Dtos
{
    public class TicketModel
    {
        public int? Id { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string CheckedoutEmail { get; set; }
        public string CheckedoutPhone { get; set; }
        public string UserNotes { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }
        public string TicketTypeNameAr { get; set; }
        public string TicketTypeNameEn { get; set; }
        public int? PolicyId { get; set; }
        public string PolicyNo { get; set; }
        public int? InvoiceId { get; set; }
        public int? InvoiceNo { get; set; }
        public string ReferenceId { get; set; }
        public string DriverNin { get; set; }
        public string UserId { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public int StatusId { get; set; }
        public DateTime? OpenedDate { get; set; }        public DateTime? ClosedDate { get; set; }        public string OpenedBy { get; set; }        public string ClosedBy { get; set; }
        public string AdminReply { get; set; }

        //public ICollection<UserTicketAttachment> UserTicketAttachments { get; set; }
        public List<int> UserTicketAttachmentsIds { get; set; }

        public List<string> UserTicketAttachmentsBytes { get; set; }
        public string CreatedBy { get; set; }
    }
}

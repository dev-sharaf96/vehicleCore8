using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    [Table("UserTicketAttachment")]
    public class UserTicketAttachment: BaseEntity
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }

        [StringLength(100)]
        public string AttachmentPath { get; set; }

        public virtual UserTicket UserTicket { get; set; }

    }
}

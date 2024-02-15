using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    [Table("UserTicketHistory")]
    public class UserTicketHistory: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? TicketStatusId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string AdminReply { get; set; }

        public string RepliedBy { get; set; }

    }
}

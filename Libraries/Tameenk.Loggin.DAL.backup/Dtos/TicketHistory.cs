using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? TicketStatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AdminReply { get; set; }
        public string RepliedBy { get; set; }
    }
}

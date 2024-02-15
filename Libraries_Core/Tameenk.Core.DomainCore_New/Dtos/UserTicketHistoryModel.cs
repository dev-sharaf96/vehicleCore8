using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain.Dtos
{
    public class UserTicketHistoryModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int TicketStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AdminReply { get; set; }
        public string RepliedBy { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }

    }
}

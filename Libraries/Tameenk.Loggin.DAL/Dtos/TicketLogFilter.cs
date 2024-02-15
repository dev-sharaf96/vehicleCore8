using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class TicketLogFilter
    {
        public string MethodName { get; set; }
        public int? ChannelId { get; set; }
        public string NationalId { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tameenk.Core.Domain.Dtos
{
    public class CreateUserTicketAPIModel
    {
        public string UserId { get; set; }
        public int TicketTypeId { get; set; }
        public string SequenceOrCustomCardNumber { get; set; }
        public string UserNotes { get; set; }
        public string Language { get; set; }
        public string Channel { get; set; }
        public List<HttpPostedFileBase> postedFiles { get; set; }
        public List<AttachedFiles> AttachedFiles { get; set; }
        public string Nin { get; set; }
    }
}

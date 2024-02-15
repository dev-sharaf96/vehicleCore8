using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tameenk.Core.Domain.Dtos
{
    public class CreateUserTicketModel
    {
        //public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public int TicketTypeId { get; set; }
        public string SequenceOrCustomCardNumber { get; set; }
        public string UserNotes { get; set; }
        public string Language { get; set; }
        public string Channel { get; set; }
        //public List<HttpPostedFileBase> postedFiles { get; set; }
        public List<AttachedFiles> AttachedFiles { get; set; }
        public string NIN { get; set; }
        public string CreatedBy { get; set; }
    }
}

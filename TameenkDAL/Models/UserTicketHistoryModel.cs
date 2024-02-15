using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    public class UserTicketHistoryModel
    {
        public int UserTicketId { get; set; }
        public string UserNotes { get; set; }
        public string PolicyNo { get; set; }
        public int? InvoiceNo { get; set; }

        public string InsuranceCompanyName { get; set; }
        public string VehicleName { get; set; }
        public VehiclePlateModel VehiclePlate { get; set; }
        public string UserTicketStatus { get; set; }
        public string UserTicketAdminReply { get; set; }
        public int TicketTypeId { get; set; }
    }
}

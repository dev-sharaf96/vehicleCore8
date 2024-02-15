using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain.Dtos
{
   public  class ExcelTiketModel
    {
        public int? Id { get; set; }
        public int? TicketTypeId { get; set; }
        public string UserEmail { get; set; }
        public string UserNotes { get; set; }
        public string TicketTypeNameEn { get; set; }
        public DateTime? OpenedDate { get; set; }

        public string Channel { get; set; }
        public string Reporter { get; set; }

        public List<UserTicketHistoryExcelModel> UserTicketHistory { get; set; }
    }

    public class UserTicketHistoryExcelModel
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? TicketStatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AdminReply { get; set; }
        public string RepliedBy { get; set; }
        public string StatusNameEn { get; set; }
    }
}

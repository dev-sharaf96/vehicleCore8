using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using TameenkDAL.Models;

namespace Tameenk.Models
{
    public class UserTicketViewModel
    {
        public List<UserTicketHistoryModel> Tickets { get; set; }
        public List<UserTicketType> UserTicketTypeList { get; set; }
    }
}
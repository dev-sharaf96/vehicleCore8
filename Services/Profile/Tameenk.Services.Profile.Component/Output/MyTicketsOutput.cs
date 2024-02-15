using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class MyTicketsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3,
            NoResultReturned = 4
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public List<UserTicketHistoryModel> Tickets { get; set; }
        public List<UserTicketType> UserTicketTypeList { get; set; }
    }
}

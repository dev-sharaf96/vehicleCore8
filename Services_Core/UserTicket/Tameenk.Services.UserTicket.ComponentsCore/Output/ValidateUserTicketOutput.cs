using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.UserTicket.Components.Output
{
    public class ValidateUserTicketOutput
    {
        public enum ErrorCodes
        {
            Valid = 1,
            NotValid = 2,
            TicketTypeNotAvaliable = 3
        }

        public string ErrorDescription { get; set; }

        public ErrorCodes ErrorCode { get; set; }
    }
}

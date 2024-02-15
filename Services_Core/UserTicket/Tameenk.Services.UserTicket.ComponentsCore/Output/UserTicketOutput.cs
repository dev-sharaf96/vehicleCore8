using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.UserTicket.Components
{
    public class UserTicketOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            PolicyIdNotBelongsToThisUser = 2,
            InvoiceIdNotBelongsToThisUser = 3,
            FailedToCreateUserTicket = 4,
            Exception = 5,
            InvalidInput = 6,
            FailedToUploadAttachment = 7,
            NajmPolicyIssuedBefore24hours = 8,
            VehicleNotFound = 9,
            NotValidToOpenNewTicket = 10,
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        public UserTicketModel UserTicketModel { get; set; }
        public int UserTicketId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public Vehicle Vehicle { get; set; }
        public string PolicyNo { get; set; }
        public int? InvoiceNo { get; set; }
        public int TicketTypeId { get; set; }
        public string UserId { get; set; }
        public string UserTicketIdString { get { return UserTicketId.ToString("000000"); } }
    }
}

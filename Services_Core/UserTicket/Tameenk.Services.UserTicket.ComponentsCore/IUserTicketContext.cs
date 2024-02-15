using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Models;

namespace Tameenk.Services.UserTicket.Components
{
    public interface IUserTicketContext
    {
        UserTicketOutput CreateUserTicket(int userTicketType, string extraData,string sequenceOrCustomCardNumber, string userNotes, string userId, string channel, LanguageTwoLetterIsoCode language, List<HttpPostedFileBase> postedFiles,List<AttachedFiles> attachedFiles, string nin,string createdBy = null);
        List<Policy> GetPoliciesByUserId(string userId);
        List<UserTicketType> GetTicketTypesDB();
        List<Invoice> GetInvoicesByUserId(string userId);
        List<UserTicketsDBModel> GetUserTicketsWithLastHistory(string userId);
        List<Vehicle> GetVehiclesByUserId(string currentUserID);
        UserTicketOutput CreateUserTicketFromDashboard(CreateUserTicketModel createUserTicketModel);
        void SendUpdatedStatusSMS(string userId, string language, int newTicketStatus, int userTicketId, string adminReply);
        UserTicketOutput CreateUserTicketFromAPI(CreateUserTicketAPIModel createUserTicketModel);
        UserTicketOutput DeleteUserTicketHistory(int historyId);
    }
}

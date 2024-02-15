using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Core.Ticket
{
    public interface ITicketService
    {
        List<Channels> GetAllStatus(string lang);
        List<Channels> GetTicketTypes(string lang);
        List<TicketModel> GetAllTicketsWithFilter(TicketFilert filter, out int total, int pageIndex, int pageSize, string lang, bool export = false);
        TicketModel GetTicketDetails(int id, out string exception);
        List<UserTicketHistoryModel> GetTicketHistories(int id, out string exception);
        bool UpdateTicketStatus(UserTicketHistory historyModel, out string ticketUserId, out string exception);
        List<TicketLogModel> GetAllLogsWithFilter(TicketLogFilter filter, out int total, int pageIndex, int pageSize, bool export = false);
        UserTicketAttachment DownloadTicketAttachmentFile(int attachmentId, out string exception);
        //List<TicketModel> ExportAllTicketsWithFilter(TicketFilert filter, out int total, int pageIndex, int pageSize, string lang, bool export = false);
        List<ExcelTiketModel> ExportAllTicketsWithFilter(TicketFilert filter, out int total, out string excep, int pageIndex, int pageSize, string lang, bool export = false);    }
}

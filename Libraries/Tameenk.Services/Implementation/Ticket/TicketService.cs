using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Core.Ticket;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Implementation.Ticket
{
    public class TicketService : ITicketService
    {
        #region fields
        private readonly IRepository<UserTicketStatus> _ticketStatusRepository;
        private readonly IRepository<UserTicketType> _ticketTypeRepository;
        private readonly IRepository<UserTicket> _ticketRepository;
        private readonly IRepository<UserTicketHistory> _ticketHistoryRepository;
        private readonly IRepository<UserTicketAttachment> _ticketAttachmentRepository;
        #endregion

        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="productTypeRepository">product type Repository</param>
        public TicketService(IRepository<UserTicketStatus> ticketStatusRepository,
                             IRepository<UserTicketType> ticketTypeRepository,
                             IRepository<UserTicket> ticketRepository,
                             IRepository<UserTicketHistory> ticketHistoryRepository,
                             IRepository<UserTicketAttachment> ticketAttachmentRepository)
        {
            this._ticketStatusRepository = ticketStatusRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<UserTicketStatus>));
            this._ticketTypeRepository = ticketTypeRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<UserTicketType>));
            this._ticketRepository = ticketRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<UserTicket>));
            this._ticketHistoryRepository = ticketHistoryRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<UserTicketHistory>));
            this._ticketAttachmentRepository = ticketAttachmentRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<UserTicketAttachment>));
        }
        #endregion region 
        public List<Channels> GetAllStatus(string lang)
        {
            var statusList = new List<Channels>();
            var data = _ticketStatusRepository.TableNoTracking.ToList();
            foreach (var status in data)
                statusList.Add(new Channels() { Code = status.Id, Name = (lang == "en") ? status.StatusNameEn : status.StatusNameAr });

            return statusList;
        }

        public List<Channels> GetTicketTypes(string lang)
        {
            var typesList = new List<Channels>();
            Channels otherType = null;
            var data = _ticketTypeRepository.TableNoTracking.Where(a => a.IsActive).ToList();
            foreach (var status in data)
            {
                if (status.Id == (int)EUserTicketTypes.Others)
                {
                    otherType = new Channels() { Code = status.Id, Name = (lang == "en") ? status.TicketTypeNameEn : status.TicketTypeNameAr };
                    continue;
                }
                typesList.Add(new Channels() { Code = status.Id, Name = (lang == "en") ? status.TicketTypeNameEn : status.TicketTypeNameAr });
            }

            if (otherType != null)
                typesList.Add(otherType);

            return typesList;
        }

        public List<TicketModel> GetAllTicketsWithFilter(TicketFilert filter, out int total, int pageIndex, int pageSize, string lang, bool export = false)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllTicketsWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter LangParameter = new SqlParameter() { ParameterName = "lang", Value = lang };
                command.Parameters.Add(LangParameter);

                SqlParameter IdParameter = new SqlParameter() { ParameterName = "id", Value = filter.Id ?? 0 };
                command.Parameters.Add(IdParameter);

                SqlParameter UserEmailParameter = new SqlParameter() { ParameterName = "userEmail", Value = filter.UserEmail ?? "" };
                command.Parameters.Add(UserEmailParameter);

                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateString", Value = (filter.StartDate.HasValue) ? filter.StartDate.Value.ToString("yyyy-MM-dd") : null };
                command.Parameters.Add(startDateParameter);

                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateString", Value = (filter.EndDate.HasValue) ? filter.EndDate.Value.ToString("yyyy-MM-dd") : null };
                command.Parameters.Add(endDateParameter);

                SqlParameter StatusIdParameter = new SqlParameter() { ParameterName = "statusId", Value = filter.StatusId ?? 0 };
                command.Parameters.Add(StatusIdParameter);

                SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = (!string.IsNullOrWhiteSpace(filter.NationalId)) ? filter.NationalId : "" };
                command.Parameters.Add(NationalIdParameter);

                SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = (!string.IsNullOrWhiteSpace(filter.PolicyNo)) ? filter.PolicyNo : "" };
                command.Parameters.Add(PolicyNoParameter);

                SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = filter.InvoiceNo ?? 0 };
                command.Parameters.Add(InvoiceNoParameter);

                SqlParameter CheckedOutEmailParameter = new SqlParameter() { ParameterName = "checkedOutEmail", Value = filter.CheckoutEmail ?? "" };
                command.Parameters.Add(CheckedOutEmailParameter);

                SqlParameter CheckedOutPhoneParameter = new SqlParameter() { ParameterName = "checkedOutPhone", Value = filter.Checkoutphone ?? "" };
                command.Parameters.Add(CheckedOutPhoneParameter);

                SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = (!string.IsNullOrWhiteSpace(filter.ReferenceNo)) ? filter.ReferenceNo : "" };
                command.Parameters.Add(ReferenceNoParameter);

                SqlParameter TicketTypeIdParameter = new SqlParameter() { ParameterName = "ticketTypeId", Value = filter.TicketTypeId ?? 0 };
                command.Parameters.Add(TicketTypeIdParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<TicketModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TicketModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();

                return filteredData;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                total = 0;

                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        public TicketModel GetTicketDetails(int id, out string  exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetTicketDetails";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "id", Value = id });

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                TicketModel userTicket = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TicketModel>(reader).FirstOrDefault();

                if (userTicket != null)
                {
                    reader.NextResult();
                    var attachmentsIds = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).ToList();
                    if (attachmentsIds != null)
                    {
                        userTicket.UserTicketAttachmentsIds = attachmentsIds;
                    }
                }
                dbContext.DatabaseInstance.Connection.Close();

                return userTicket;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception= ex.ToString();
                return null;
            }
        }

        public UserTicketAttachment DownloadTicketAttachmentFile(int attachmentId, out string exception)
        {
            exception = string.Empty;
            try
            {
                return _ticketAttachmentRepository.TableNoTracking.Where(x => x.Id == attachmentId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        public List<UserTicketHistoryModel> GetTicketHistories(int id, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetTicketHistories";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "id", Value = id });

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var userTicketHistory = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<UserTicketHistoryModel>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return userTicketHistory;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        public bool UpdateTicketStatus(UserTicketHistory historyModel, out string ticketUserId, out string exception)
        {
            exception = string.Empty;
            ticketUserId = string.Empty;
            try
            {
                _ticketHistoryRepository.Insert(historyModel);
                var ticket = _ticketRepository.Table.Where(x => x.Id == historyModel.TicketId).FirstOrDefault();
                if (ticket == null)
                {
                    exception = "There is no ticket with id: " + historyModel.TicketId;
                    return false;
                }

                ticketUserId = ticket.UserId;
                ticket.CurrentTicketStatusId = historyModel.TicketStatusId;
                _ticketRepository.Update(ticket);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                ticketUserId = string.Empty;
                return false;
            }
        }

        public List<TicketLogModel> GetAllLogsWithFilter(TicketLogFilter filter, out int total, int pageIndex, int pageSize, bool export = false)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllTicketLogsWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter MethodNameParameter = new SqlParameter() { ParameterName = "methodName", Value = filter.MethodName ?? "" };
                command.Parameters.Add(MethodNameParameter);

                SqlParameter ChannelParameter = new SqlParameter() { ParameterName = "channel", Value = (filter.ChannelId.HasValue) ? Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), filter.ChannelId.Value) : "" };
                command.Parameters.Add(ChannelParameter);

                SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = (!string.IsNullOrWhiteSpace(filter.NationalId)) ? filter.NationalId : "" };
                command.Parameters.Add(NationalIdParameter);

                SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = (!string.IsNullOrWhiteSpace(filter.ReferenceNo)) ? filter.ReferenceNo : "" };
                command.Parameters.Add(ReferenceNoParameter);

                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateString", Value = (filter.StartDate.HasValue) ? filter.StartDate.Value.ToString("yyyy-MM-dd") : null };
                command.Parameters.Add(startDateParameter);

                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateString", Value = (filter.EndDate.HasValue) ? filter.EndDate.Value.ToString("yyyy-MM-dd") : null };
                command.Parameters.Add(endDateParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<TicketLogModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TicketLogModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        //public List<TicketModel> ExportAllTicketsWithFilter(TicketFilert filter, out int total, int pageIndex, int pageSize, string lang, bool export = false)
        //{
        //    var dbContext = EngineContext.Current.Resolve<IDbContext>();
        //    AdminRequestLog log = new AdminRequestLog();
        //    try
        //    {
        //        var command = dbContext.DatabaseInstance.Connection.CreateCommand();
        //        command.CommandText = "GetTicketHistoryForExport";
        //        command.CommandType = CommandType.StoredProcedure;
        //        dbContext.DatabaseInstance.CommandTimeout = 180;
        //        if (!string.IsNullOrEmpty(filter.UserEmail))
        //        {
        //            SqlParameter UserEmailParameter = new SqlParameter() { ParameterName = "userEmail", Value = filter.UserEmail };
        //            command.Parameters.Add(UserEmailParameter);
        //        }
        //        if (filter.StartDate.HasValue)
        //        {
        //            DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
        //            SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = dtStart };
        //            command.Parameters.Add(startDateParameter);
        //        }
        //        if (filter.EndDate.HasValue)
        //        {
        //            DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
        //            SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = dtEnd };
        //            command.Parameters.Add(endDateParameter);
        //        }
        //        if (filter.StatusId.HasValue && filter.StatusId.Value > 0)
        //        {
        //            SqlParameter StatusIdParameter = new SqlParameter() { ParameterName = "statusId", Value = filter.StatusId ?? 0 };
        //            command.Parameters.Add(StatusIdParameter);
        //        }

        //        if (!string.IsNullOrWhiteSpace(filter.NationalId))
        //        {
        //            SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = filter.NationalId };
        //            command.Parameters.Add(NationalIdParameter);
        //        }

        //        if (!string.IsNullOrWhiteSpace(filter.PolicyNo))
        //        {
        //            SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = filter.PolicyNo };
        //            command.Parameters.Add(PolicyNoParameter);
        //        }

        //        if (filter.InvoiceNo.HasValue && filter.InvoiceNo.Value > 0)
        //        {
        //            SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = filter.InvoiceNo };
        //            command.Parameters.Add(InvoiceNoParameter);
        //        }

        //        if (!string.IsNullOrEmpty(filter.CheckoutEmail))
        //        {
        //            SqlParameter CheckedOutEmailParameter = new SqlParameter() { ParameterName = "checkedOutEmail", Value = filter.CheckoutEmail };
        //            command.Parameters.Add(CheckedOutEmailParameter);
        //        }

        //        if (!string.IsNullOrEmpty(filter.Checkoutphone))
        //        {
        //            SqlParameter CheckedOutPhoneParameter = new SqlParameter() { ParameterName = "checkedOutPhone", Value = filter.Checkoutphone };
        //            command.Parameters.Add(CheckedOutPhoneParameter);
        //        }

        //        if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
        //        {
        //            SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = filter.ReferenceNo };
        //            command.Parameters.Add(ReferenceNoParameter);
        //        }

        //        if (filter.TicketTypeId.HasValue && filter.TicketTypeId.Value > 0)
        //        {
        //            SqlParameter TicketTypeIdParameter = new SqlParameter() { ParameterName = "ticketTypeId", Value = filter.TicketTypeId };
        //            command.Parameters.Add(TicketTypeIdParameter);
        //        }
        //        dbContext.DatabaseInstance.Connection.Open();
        //        var reader = command.ExecuteReader();
        //        List<TicketModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TicketModel>(reader).ToList();
        //        total = filteredData.Count;
        //        dbContext.DatabaseInstance.Connection.Close();
        //        return filteredData;
        //    }
        //    catch (Exception ex)
        //    {
        //        dbContext.DatabaseInstance.Connection.Close();
        //        total = 0;
        //        ErrorLogger.LogError(ex.Message, ex, false);
        //        return null;
        //    }
        //}

        public List<ExcelTiketModel> ExportAllTicketsWithFilter(TicketFilert filter, out int total, out string excep, int pageIndex, int pageSize, string lang, bool export = false)
        {
            excep = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            AdminRequestLog log = new AdminRequestLog();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetTicketHistoryForExport";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                if (!string.IsNullOrEmpty(filter.UserEmail))
                {
                    SqlParameter UserEmailParameter = new SqlParameter() { ParameterName = "userEmail", Value = filter.UserEmail };
                    command.Parameters.Add(UserEmailParameter);
                }
                if (filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = dtStart };
                    log.ServiceRequest = filter.StartDate.Value.ToString("yyyy-MM-dd");
                    command.Parameters.Add(startDateParameter);
                }
                if (filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = dtEnd };
                    command.Parameters.Add(endDateParameter);
                }
                if (filter.StatusId.HasValue && filter.StatusId.Value > 0)
                {
                    SqlParameter StatusIdParameter = new SqlParameter() { ParameterName = "statusId", Value = filter.StatusId ?? 0 };
                    command.Parameters.Add(StatusIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(filter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = filter.NationalId };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(filter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = filter.PolicyNo };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (filter.InvoiceNo.HasValue && filter.InvoiceNo.Value > 0)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = filter.InvoiceNo };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (!string.IsNullOrEmpty(filter.CheckoutEmail))
                {
                    SqlParameter CheckedOutEmailParameter = new SqlParameter() { ParameterName = "checkedOutEmail", Value = filter.CheckoutEmail };
                    command.Parameters.Add(CheckedOutEmailParameter);
                }

                if (!string.IsNullOrEmpty(filter.Checkoutphone))
                {
                    SqlParameter CheckedOutPhoneParameter = new SqlParameter() { ParameterName = "checkedOutPhone", Value = filter.Checkoutphone };
                    command.Parameters.Add(CheckedOutPhoneParameter);
                }

                if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = filter.ReferenceNo };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (filter.TicketTypeId.HasValue && filter.TicketTypeId.Value > 0)
                {
                    SqlParameter TicketTypeIdParameter = new SqlParameter() { ParameterName = "ticketTypeId", Value = filter.TicketTypeId };
                    command.Parameters.Add(TicketTypeIdParameter);
                }
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<ExcelTiketModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ExcelTiketModel>(reader).ToList();
                reader.NextResult();
                List<UserTicketHistoryExcelModel> histories = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<UserTicketHistoryExcelModel>(reader).ToList();
                var newlist = new List<ExcelTiketModel>();
                foreach (var item in filteredData)
                {
                    var ticket = new ExcelTiketModel();
                    ticket = item;
                    ticket.UserTicketHistory = new List<UserTicketHistoryExcelModel>();
                    ticket.UserTicketHistory = histories.Where(a => a.TicketId == item.Id).ToList();
                    newlist.Add(ticket);
                }
                total = newlist.Count();

                return newlist;
            }
            catch (Exception ex)
            {
                excep = ex.ToString();
                total = 0;
                //ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
    }
}

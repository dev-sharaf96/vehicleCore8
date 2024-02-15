using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;using System.Linq.Dynamic;

namespace Tameenk.Loggin.DAL
{
   public class InquiryRequestLogDataAccess
    {

        public static bool AddInquiryRequestLog(InquiryRequestLog inquiryRequestLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    inquiryRequestLog.CreatedDate = DateTime.Now;
                    context.InquiryRequestLogs.Add(inquiryRequestLog);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                    }
                }
                return false;
            }
        }
        public static List<InquiryRequestLog> GetAllInquiryRequestLogBasedOnFilter(string connectionString, RequestLogFilter requestLogFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)        {            try            {                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))                    context = new TameenkLog();                else                    context = new TameenkLog(connectionString);                using (context)                {                    context.Database.CommandTimeout = 240;                    context.Configuration.AutoDetectChangesEnabled = false;                    var query = (from d in context.InquiryRequestLogs select d);                    if (requestLogFilter.EndDate.HasValue && requestLogFilter.StartDate.HasValue)                    {                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);                        if (requestLogFilter.EndDate.Value == requestLogFilter.StartDate.Value)                        {                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);                        }                        else                        {                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);                        }                    }                    else if (requestLogFilter.EndDate.HasValue)                    {                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);                        query = query.Where(e => e.CreatedDate <= requestLogFilter.EndDate.Value.Date);                    }                    else if (requestLogFilter.StartDate.HasValue)                    {                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);                        query = query.Where(e => e.CreatedDate >= dtStart);                    }                    if (!string.IsNullOrEmpty(requestLogFilter.NIN))                        query = query.Where(q => q.NIN.Equals(requestLogFilter.NIN));                    if (!string.IsNullOrEmpty(requestLogFilter.VehicleId))                        query = query.Where(q => q.VehicleId.Equals(requestLogFilter.VehicleId));                    if (!string.IsNullOrEmpty(requestLogFilter.ExternalId))                        query = query.Where(q => q.ExternalId.Equals(requestLogFilter.ExternalId));

                    if (requestLogFilter.Channel != null)
                    {
                        var stringChannel = (Channel)requestLogFilter.Channel;
                        query = query.Where(q => q.Channel.Equals(stringChannel.ToString()));
                    }

                    if (requestLogFilter.StartDate == null && requestLogFilter.EndDate == null
                      && string.IsNullOrEmpty(requestLogFilter.ExternalId) && string.IsNullOrEmpty(requestLogFilter.NIN)
                       && string.IsNullOrEmpty(requestLogFilter.VehicleId) && requestLogFilter.Channel == null)
                    {
                        DateTime dtEnd = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 23, 59, 59);                        DateTime dtStart = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                    }                    total = query.Count();                    if (total == 0)                        return null;                    int TotalCount = total;                    int TotalPages = total / pageSize;                    if (total % pageSize > 0)                        TotalPages++;                    if (!string.IsNullOrEmpty(sortField))                    {                        if (sortOrder.HasValue)                        {                            if (sortOrder.GetValueOrDefault())                            {                                query = query.OrderBy("ID DESC");                            }                            else                            {                                query = query.OrderBy("ID");                            }                        }                    }                    query = query.Skip(pageIndex * pageSize).Take(pageSize);                    return query.ToList();                }            }            catch (Exception ex)            {                total = 0;                ErrorLogger.LogError(ex.Message, ex, false);                return null;            }        }        public static InquiryRequestLog GetInquiryRequestLogDetails(string connectionString, int id)        {            try            {                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))                    context = new TameenkLog();                else                    context = new TameenkLog(connectionString);                using (context)                {                    context.Database.CommandTimeout = 240;                    context.Configuration.AutoDetectChangesEnabled = false;                    var query = (from d in context.InquiryRequestLogs                                 where d.Id == id                                 select d).FirstOrDefault();                    return query;                }            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return null;            }        }
    }
}

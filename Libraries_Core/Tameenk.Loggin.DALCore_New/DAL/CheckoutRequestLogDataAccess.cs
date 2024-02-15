using System;
using System.Collections.Generic;

namespace Tameenk.Loggin.DAL
{
    public class CheckoutRequestLogDataAccess
    {
        public static bool AddCheckoutRequestLog(CheckoutRequestLog checkoutRequestLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    context.Database.CommandTimeout = 30;
                    checkoutRequestLog.CreatedDate = DateTime.Now;
                    context.CheckoutRequestLogs.Add(checkoutRequestLog);
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
            catch (Exception)
            {
                return false;
            }
        }
        public static List<CheckoutRequestLog> GetAllCheckoutRequestLogBasedOnFilter(string connectionString, RequestLogFilter requestLogFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)
        {
            try
            {
                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))
                    context = new TameenkLog();
                else
                    context = new TameenkLog(connectionString);

                using (context)
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = (from d in context.CheckoutRequestLogs select d);

                    if (requestLogFilter.EndDate.HasValue && requestLogFilter.StartDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);
                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);

                        if (requestLogFilter.EndDate.Value == requestLogFilter.StartDate.Value)
                        {
                            // DateTime dtEnd = serviceRequestFilter.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                        }
                        else
                        {
                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                        }
                    }
                    else if (requestLogFilter.EndDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);
                        query = query.Where(e => e.CreatedDate <= requestLogFilter.EndDate.Value.Date);

                    }
                    else if (requestLogFilter.StartDate.HasValue)
                    {
                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart);
                    }

                    if (!string.IsNullOrEmpty(requestLogFilter.ReferenceId))
                        query = query.Where(q => q.ReferenceId.Equals(requestLogFilter.ReferenceId));

                    if (!string.IsNullOrEmpty(requestLogFilter.NIN))
                        query = query.Where(q => q.DriverNin.Equals(requestLogFilter.NIN));

                    if (!string.IsNullOrEmpty(requestLogFilter.VehicleId))
                        query = query.Where(q => q.VehicleId.Equals(requestLogFilter.VehicleId));

                    if (!string.IsNullOrEmpty(requestLogFilter.MethodName))
                        query = query.Where(q => q.MethodName.Equals(requestLogFilter.MethodName));

                    if (requestLogFilter.StartDate == null && requestLogFilter.EndDate == null
                        && string.IsNullOrEmpty(requestLogFilter.ReferenceId) && string.IsNullOrEmpty(requestLogFilter.NIN)
                         && string.IsNullOrEmpty(requestLogFilter.VehicleId))
                    {
                        DateTime dtEnd = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 23, 59, 59);
                        DateTime dtStart = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                    }

                    total = query.Count();

                    if (total == 0)
                        return null;
                    int TotalCount = total;
                    int TotalPages = total / pageSize;

                    if (total % pageSize > 0)
                        TotalPages++;

                    if (!string.IsNullOrEmpty(sortField))
                    {
                        if (sortOrder.HasValue)
                        {
                            if (sortOrder.GetValueOrDefault())
                            {
                                query = query.OrderBy("ID DESC");
                            }
                            else
                            {
                                query = query.OrderBy("ID");
                            }
                        }
                    }
                    query = query.Skip(pageIndex * pageSize).Take(pageSize);
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        public static CheckoutRequestLog GetCheckoutRequestLogDetails(string connectionString, int Id)
        {
            try
            {
                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))
                    context = new TameenkLog();
                else
                    context = new TameenkLog(connectionString);

                using (context)
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = (from d in context.CheckoutRequestLogs
                                 where d.Id == Id
                                 select d).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }
    }
}

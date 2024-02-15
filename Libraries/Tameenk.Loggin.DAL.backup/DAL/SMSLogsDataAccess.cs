using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Linq.Dynamic;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Loggin.DAL.Dtos;

namespace Tameenk.Loggin.DAL
{
    public class SMSLogsDataAccess
    {
        public static bool AddToSMSLogsDataAccess(SMSLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.SMSLogs.Add(toSaveLog);
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

        public static List<SMSLog> GetAllSMSLogBasedOnFilter(string connectionString, SMSLogFilter filter, out int total, out string exception, bool export, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)        {


            #region code with stored procedure

            //TameenkLog context;//= new TameenkLog();

            //if (string.IsNullOrEmpty(connectionString))
            //    context = new TameenkLog();
            //else
            //    context = new TameenkLog(connectionString);

            //using (context)
            //{
            //    context.Database.CommandTimeout = 240;
            //    context.Configuration.AutoDetectChangesEnabled = false;
            //    var query = (from d in context.SMSLogs select d);

            //    if (!string.IsNullOrEmpty(filter.MobileNumber))
            //    {
            //        string mobileNumber = Utilities.ValidatePhoneNumber(filter.MobileNumber);
            //        query = query.Where(q => q.MobileNumber.Equals(mobileNumber));
            //    }

            //    if (filter.StatusCode.HasValue)
            //    {
            //        if (filter.StatusCode.Value == 1)
            //            query = query.Where(q => q.ErrorCode == 0 || q.ErrorCode == 100);
            //        else
            //            query = query.Where(q => q.ErrorCode != 0 || q.ErrorCode != 100);
            //    }

            //    if (filter.EndDate.HasValue && filter.StartDate.HasValue)
            //    {
            //        DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
            //        DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
            //        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
            //    }
            //    else if (filter.StartDate.HasValue)
            //    {
            //        DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
            //        query = query.Where(e => e.CreatedDate >= dtStart);
            //    }
            //    else if (filter.EndDate.HasValue)
            //    {
            //        DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
            //        query = query.Where(e => e.CreatedDate <= filter.EndDate.Value.Date);
            //    }

            //    if (!export)
            //    {
            //        total = query.Count();

            //        if (total == 0)
            //            return null;
            //        int TotalCount = total;
            //        int TotalPages = total / pageSize;

            //        if (total % pageSize > 0)
            //            TotalPages++;

            //        if (!string.IsNullOrEmpty(sortField))
            //        {
            //            if (sortOrder.HasValue)
            //            {
            //                if (sortOrder.GetValueOrDefault())
            //                {
            //                    query = query.OrderBy("ID DESC");
            //                }
            //                else
            //                {
            //                    query = query.OrderBy("ID");
            //                }
            //            }
            //        }
            //        query = query.Skip(pageIndex * pageSize).Take(pageSize);
            //    }

            //    return filteredData;
            //}

            

            #endregion
            total = 0;            exception = string.Empty;            List<SMSLog> filteredData = new List<SMSLog>();            try            {
                using (TameenkLog context = new TameenkLog(connectionString))
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetAllSmsLogsFromDBWithFilter";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 60 * 60 * 60;

                    if (filter.Channel.HasValue)
                    {
                        string channel = filter.Channel != null ? Enum.GetName(typeof(Channel), filter.Channel) : string.Empty;
                        SqlParameter StatusCodeParameter = new SqlParameter() { ParameterName = "Channel", Value = channel };
                        command.Parameters.Add(StatusCodeParameter);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.MobileNumber))
                    {
                        filter.MobileNumber = Utilities.ValidatePhoneNumber(filter.MobileNumber);
                        SqlParameter MobileNoParameter = new SqlParameter() { ParameterName = "MobileNo", Value = filter.MobileNumber };
                        command.Parameters.Add(MobileNoParameter);
                    }

                    if (filter.StatusCode.HasValue)
                    {
                        SqlParameter StatusCodeParameter = new SqlParameter() { ParameterName = "StatusCode", Value = filter.StatusCode.Value };
                        command.Parameters.Add(StatusCodeParameter);
                    }

                    if (filter.StartDate.HasValue)
                    {
                        DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                        SqlParameter DateFromParameter = new SqlParameter() { ParameterName = "DateFrom", Value = dtStart.ToString("yyyy-MM-dd HH:mm:ss") };
                        command.Parameters.Add(DateFromParameter);
                    }

                    if (filter.EndDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                        SqlParameter DateToParameter = new SqlParameter() { ParameterName = "DateTo", Value = dtEnd.ToString("yyyy-MM-dd HH:mm:ss") };
                        command.Parameters.Add(DateToParameter);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Method))
                    {
                        SqlParameter MethodParameter = new SqlParameter() { ParameterName = "Method", Value = filter.Method };
                        command.Parameters.Add(MethodParameter);
                    }
                    if (filter.SMSProvider.HasValue)
                    {
                        if (filter.SMSProvider==1)
                        {
                            SqlParameter MethodParameter = new SqlParameter() { ParameterName = "SMSProvider", Value = "STC" };
                            command.Parameters.Add(MethodParameter);
                        }
                        else
                        {
                            SqlParameter MethodParameter = new SqlParameter() { ParameterName = "SMSProvider", Value = "MobiShastra" };
                            command.Parameters.Add(MethodParameter);
                        }
                        
                    }

                    SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex + 1 };
                    command.Parameters.Add(pageNumberParameter);

                    SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                    command.Parameters.Add(pageSizeParameter);

                    SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = (export == true) ? 1 : 0 };
                    command.Parameters.Add(ExportParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSLog>(reader).ToList();

                    if (filteredData != null && filteredData.Count > 0 && !export)
                    {
                        //get data count
                        reader.NextResult();
                        total = ((IObjectContextAdapter)context).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    }

                    return filteredData;
                }
            }            catch (Exception ex)            {                total = 0;                exception = ex.ToString();                ErrorLogger.LogError(ex.Message, ex, false);                return null;            }        }

        public static AllTypeSMSRenewalLogModel GetAllSMSRenewalLogBasedOnFilter(string connectionString, SMSRenewalLogFilter filter, out string exception)
        {
            exception = string.Empty;
            AllTypeSMSRenewalLogModel allTypeSMSRenewalLogModel = new AllTypeSMSRenewalLogModel();
            List<SMSNotification> filteredData = new List<SMSNotification>();
            try
            {
                using (TameenkLog context = new TameenkLog(connectionString))
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetAllSMSRenewalLogsFromDBWithFilter";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 60 * 60 * 60;
                    if (filter.StartDate.HasValue)
                    {
                        DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                        SqlParameter DateFromParameter = new SqlParameter() { ParameterName = "DateFrom", Value = dtStart.ToString("yyyy-MM-dd HH:mm:ss") };
                        command.Parameters.Add(DateFromParameter);
                    }
                    if (filter.EndDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                        SqlParameter DateToParameter = new SqlParameter() { ParameterName = "DateTo", Value = dtEnd.ToString("yyyy-MM-dd HH:mm:ss") };
                        command.Parameters.Add(DateToParameter);
                    }     
                    var reader = command.ExecuteReader();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSNotification>(reader).ToList();
                    allTypeSMSRenewalLogModel.Sent28daysBefore.AddRange(filteredData);
                    reader.NextResult();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSNotification>(reader).ToList();
                    allTypeSMSRenewalLogModel.Sent14daysBefore.AddRange(filteredData);
                    reader.NextResult();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSNotification>(reader).ToList();
                    allTypeSMSRenewalLogModel.SentDayBefore.AddRange(filteredData);
                    reader.NextResult();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSNotification>(reader).ToList();
                    allTypeSMSRenewalLogModel.SentInsuranceExpire.AddRange(filteredData);
                    reader.NextResult();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<SMSNotification>(reader).ToList();
                    allTypeSMSRenewalLogModel.NotSentDueTransferOfCarOwnership.AddRange(filteredData);
                    return allTypeSMSRenewalLogModel;
                }
            }
            catch (Exception ex) 
            {
                exception = ex.ToString(); ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }
    }
}

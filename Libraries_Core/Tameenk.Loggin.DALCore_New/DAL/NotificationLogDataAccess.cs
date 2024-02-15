using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Tameenk.Common.Utilities;

namespace Tameenk.Loggin.DAL
{
    public class NotificationLogDataAccess
    {
        public static bool AddToNotificationLog(NotificationLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.NotificationLogs.Add(toSaveLog);
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
        public static bool CheckIfMessageSentBefore(string phone, string companyName,string method,int insuranceTypeCode, out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    DateTime start = DateTime.Now.AddMinutes(-30);
                    DateTime end = DateTime.Now;
                    int count = 0;
                    if (string.IsNullOrEmpty(companyName))
                    {
                        count = (from d in context.NotificationLogs
                                 where d.Phone == phone && d.ErrorCode == 0 
                                 && d.Method == method && d.CreatedDate >=start  && d.CreatedDate <= end
                                 select d).OrderByDescending(d => d.ID).Count();
                    }
                    else
                    {
                        count = (from d in context.NotificationLogs
                                 where d.Phone == phone && d.CompanyName == companyName
                                 && d.ErrorCode == 0 && d.Method == method&&d.InsuranceTypeCode== insuranceTypeCode && d.CreatedDate >= start && d.CreatedDate <= end
                                 select d).OrderByDescending(d => d.ID).Count();
                    }
                    if (count >0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public static List<QuotationStatus> GetQuotationStatus(string startDate, string endDate, out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetQuotationStatus";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 300;
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateTime", Value = startDate };
                    command.Parameters.Add(startDateParameter);
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateTime", Value = endDate };
                    command.Parameters.Add(endDateParameter);
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<QuotationStatus> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<QuotationStatus>(reader).ToList();
                    return filteredData;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Linq.Dynamic;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Data;

namespace Tameenk.Loggin.DAL
{

    public class QuotationRequestLogDataAccess
    {

        /// <summary>
        /// Add QuotationRequestLog to tameenkLog context.
        /// </summary>
        /// <param name="quotationRequestLog"></param>
        /// <returns></returns>   
        public static bool AddQuotationRequestLog(QuotationRequestLog quotationRequestLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    quotationRequestLog.CreatedDate = DateTime.Now;
                    context.QuotationRequestLogs.Add(quotationRequestLog);
                    context.SaveChanges();
                    return true;
                }
            }
            //catch (Exception ex)
            //{
            //    //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\StagingQuotationApi\logs\log1.txt", ex.ToString());
            //    return false;
            //}
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\StagingQuotationApi\logs\log1.txt", ex.ToString());
                    }
                }
                return false;
            }

        }

        public static List<QuotationRequestLog> GetAllQuotationRequestLogBasedOnFilter(string connectionString, RequestLogFilter requestLogFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)        {            try            {                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))                    context = new TameenkLog();                else                    context = new TameenkLog(connectionString);                using (context)                {                    context.Database.CommandTimeout = 240;                    context.Configuration.AutoDetectChangesEnabled = false;                    var query = (from d in context.QuotationRequestLogs select d);                    if (requestLogFilter.EndDate.HasValue && requestLogFilter.StartDate.HasValue)                    {                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);                        if (requestLogFilter.EndDate.Value == requestLogFilter.StartDate.Value)                        {                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);                        }                        else                        {                            query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);                        }                    }                    else if (requestLogFilter.EndDate.HasValue)                    {                        DateTime dtEnd = new DateTime(requestLogFilter.EndDate.Value.Year, requestLogFilter.EndDate.Value.Month, requestLogFilter.EndDate.Value.Day, 23, 59, 59);                        query = query.Where(e => e.CreatedDate <= requestLogFilter.EndDate.Value.Date);                    }                    else if (requestLogFilter.StartDate.HasValue)                    {                        DateTime dtStart = new DateTime(requestLogFilter.StartDate.Value.Year, requestLogFilter.StartDate.Value.Month, requestLogFilter.StartDate.Value.Day, 0, 0, 0);                        query = query.Where(e => e.CreatedDate >= dtStart);                    }                    if (!string.IsNullOrEmpty(requestLogFilter.NIN))                        query = query.Where(q => q.NIN.Equals(requestLogFilter.NIN));                    if (!string.IsNullOrEmpty(requestLogFilter.VehicleId))                        query = query.Where(q => q.VehicleId.Equals(requestLogFilter.VehicleId));                    if (!string.IsNullOrEmpty(requestLogFilter.ExternalId))                        query = query.Where(q => q.ExtrnlId.Equals(requestLogFilter.ExternalId));                    if (!string.IsNullOrEmpty(requestLogFilter.ReferenceId))                        query = query.Where(q => q.RefrenceId.Equals(requestLogFilter.ReferenceId));
                    if (requestLogFilter.StartDate == null && requestLogFilter.EndDate == null
                    && string.IsNullOrEmpty(requestLogFilter.ReferenceId) && string.IsNullOrEmpty(requestLogFilter.ExternalId) && string.IsNullOrEmpty(requestLogFilter.NIN)
                     && string.IsNullOrEmpty(requestLogFilter.VehicleId))
                    {
                        DateTime dtEnd = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 23, 59, 59);                        DateTime dtStart = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                    }                    total = query.Count();                    if (total == 0)                        return null;                    int TotalCount = total;                    int TotalPages = total / pageSize;                    if (total % pageSize > 0)                        TotalPages++;                    if (!string.IsNullOrEmpty(sortField))                    {                        if (sortOrder.HasValue)                        {                            if (sortOrder.GetValueOrDefault())                            {                                query = query.OrderBy("ID DESC");                            }                            else                            {                                query = query.OrderBy("ID");                            }                        }                    }                    query = query.Skip(pageIndex * pageSize).Take(pageSize);                    return query.ToList();                }            }            catch (Exception ex)            {                total = 0;                ErrorLogger.LogError(ex.Message, ex, false);                return null;            }        }        public static QuotationRequestLog GetQuotationRequestLogDetails(string connectionString, int id)        {            try            {                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))                    context = new TameenkLog();                else                    context = new TameenkLog(connectionString);                using (context)                {                    context.Database.CommandTimeout = 240;                    context.Configuration.AutoDetectChangesEnabled = false;                    var query = (from d in context.QuotationRequestLogs                                 where d.ID == id                                 select d).FirstOrDefault();                    return query;                }            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return null;            }        }


        public static List<RequestsPerCompanyModel> ServiceRequestsFromDBWithFilter(ServiceRequestsPerCompany requestFilter, out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetServiceRequests";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 10 * 60;

                    if (requestFilter.StartDate.HasValue)
                    {
                        SqlParameter StartDate = new SqlParameter() { ParameterName = "@StartDate", Value = requestFilter.StartDate?.ToString("yyyy-MM-dd 00:00:00.000") };
                        command.Parameters.Add(StartDate);
                    }
                    if (requestFilter.EndDate.HasValue)
                    {
                        SqlParameter EndDate = new SqlParameter() { ParameterName = "@EndDate", Value = requestFilter.EndDate?.ToString("yyyy-MM-dd 23:59:59.999") };
                        command.Parameters.Add(EndDate);
                    }
                    if (requestFilter.StatusCode.HasValue)
                    {
                        if (requestFilter.StatusCode == 2)
                        {
                            SqlParameter StatusCode = new SqlParameter() { ParameterName = "@ErrorCode", Value = requestFilter.StatusCode };
                            command.Parameters.Add(StatusCode);
                        }
                        else
                        {
                            SqlParameter StatusCode = new SqlParameter() { ParameterName = "@SuccessCode", Value = requestFilter.StatusCode };
                            command.Parameters.Add(StatusCode);
                        }
                    }
                    if (requestFilter.InsuranceTypeId.HasValue)
                    {
                        SqlParameter InsuranceTypeId = new SqlParameter() { ParameterName = "@InsuranceType", Value = requestFilter.InsuranceTypeId };
                        command.Parameters.Add(InsuranceTypeId);
                    }
                    if (requestFilter.ModuleId.HasValue)
                    {
                        SqlParameter ModuleId = new SqlParameter() { ParameterName = "@module", Value = requestFilter.ModuleId };
                        command.Parameters.Add(ModuleId);
                    }

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<RequestsPerCompanyModel> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<RequestsPerCompanyModel>(reader).ToList();

                    return filteredData;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }
        public static List<PolicyStatisticsDataModel> GetPolicyStatisticsData(PolicyStatisticsFilterModel requestFilter, out string exception, out int totalCount)
        {
            exception = string.Empty;
            totalCount = 0;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {

                List<PolicyStatisticsDataModel> PolicyStatistics = null;                dbContext.DatabaseInstance.CommandTimeout = 60;                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetPolicyStatisticsData";                command.CommandType = CommandType.StoredProcedure;
                if (requestFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(requestFilter.StartDate.Value.Year, requestFilter.StartDate.Value.Month, requestFilter.StartDate.Value.Day, 0, 0, 0);
                    SqlParameter sDate = new SqlParameter() { ParameterName = "@StartDate", Value = dtStart };
                    command.Parameters.Add(sDate);
                }
                if (requestFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(requestFilter.EndDate.Value.Year, requestFilter.EndDate.Value.Month, requestFilter.EndDate.Value.Day, 23, 59, 59);
                    SqlParameter eDate = new SqlParameter() { ParameterName = "@EndDate", Value = dtEnd };
                    command.Parameters.Add(eDate);
                }
                if (requestFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyId = new SqlParameter() { ParameterName = "@InsuranceCompanyId", Value = requestFilter.InsuranceCompanyId };
                    command.Parameters.Add(InsuranceCompanyId);
                }
                if (requestFilter.PageNumber.HasValue)
                {
                    SqlParameter PageNumber = new SqlParameter() { ParameterName = "@PageNumber", Value = requestFilter.PageNumber };
                    command.Parameters.Add(PageNumber);
                }
                if (requestFilter.PageSize.HasValue)
                {
                    SqlParameter PageSize = new SqlParameter() { ParameterName = "@PageSize", Value = requestFilter.PageSize };
                    command.Parameters.Add(PageSize);
                }
                if (requestFilter.PageSize.HasValue)
                {
                    SqlParameter IsExcel = new SqlParameter() { ParameterName = "@IsExcel", Value = requestFilter.IsExcel };
                    command.Parameters.Add(IsExcel);
                }
                if (requestFilter.IsExcel != 1)
                {
                    dbContext.DatabaseInstance.Connection.Open();
                    var reader = command.ExecuteReader();
                    PolicyStatistics = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyStatisticsDataModel>(reader).ToList();
                    if (PolicyStatistics != null)
                    {
                        reader.NextResult();
                        totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                        dbContext.DatabaseInstance.Connection.Close();

                    }

                    dbContext.DatabaseInstance.Connection.Close();
                    return PolicyStatistics;
                }

                else
                {

                    dbContext.DatabaseInstance.Connection.Open();
                    var reader = command.ExecuteReader();
                    PolicyStatistics = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyStatisticsDataModel>(reader).ToList();
                    dbContext.DatabaseInstance.Connection.Close();
                    return PolicyStatistics;
                }


            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }

        public static List<CompanyAvgResponseTime> GetInsuranceCompanyAvgResponseTime(out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetInsuranceCompanyAvgResponseTime";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 90;
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<CompanyAvgResponseTime> companies = ((IObjectContextAdapter)context).ObjectContext.Translate<CompanyAvgResponseTime>(reader).ToList();
                    context.Database.Connection.Close();
                    return companies;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }


    }
}

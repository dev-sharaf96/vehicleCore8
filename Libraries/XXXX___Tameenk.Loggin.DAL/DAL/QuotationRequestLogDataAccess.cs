﻿using System;
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

        public static List<QuotationRequestLog> GetAllQuotationRequestLogBasedOnFilter(string connectionString, RequestLogFilter requestLogFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)

                if (string.IsNullOrEmpty(connectionString))
                    if (requestLogFilter.StartDate == null && requestLogFilter.EndDate == null
                    && string.IsNullOrEmpty(requestLogFilter.ReferenceId) && string.IsNullOrEmpty(requestLogFilter.ExternalId) && string.IsNullOrEmpty(requestLogFilter.NIN)
                     && string.IsNullOrEmpty(requestLogFilter.VehicleId))
                    {
                        DateTime dtEnd = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 23, 59, 59);
                        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                    }

                if (string.IsNullOrEmpty(connectionString))


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

                List<PolicyStatisticsDataModel> PolicyStatistics = null;
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
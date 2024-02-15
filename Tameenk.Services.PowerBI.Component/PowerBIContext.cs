using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.PowerBI.Component
{
    public class PowerBIContext : IPowerBIContext
    {
        public BIServiceLogsModel GetAllServiceLog(string method, string companyKey, DateTime startDate, DateTime endDate, out string exception)
        {
            exception = string.Empty;
            BIServiceLogsModel data = new BIServiceLogsModel();
            using (TameenkLog context = new TameenkLog())
            {
                try
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetServicesLogs";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 60 * 60 * 60;

                    SqlParameter methodParameter = new SqlParameter() { ParameterName = "@Method", Value = method.ToLower() };
                    command.Parameters.Add(methodParameter);

                    SqlParameter companyKeyParameter = new SqlParameter() { ParameterName = "@companyKey", Value = companyKey.ToLower() };
                    command.Parameters.Add(companyKeyParameter);


                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "@dateFrom", Value = startDate.ToString("yyyy-MM-dd 00:00:00") };
                    command.Parameters.Add(startDateParameter);

                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "@dateTo", Value = endDate.ToString("yyyy-MM-dd 23:59:59") };
                    command.Parameters.Add(endDateParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    if (method.ToLower().Trim() == "quotation")
                    {
                        data.QuotationserviceLogs = ((IObjectContextAdapter)context).ObjectContext.Translate<BIQuotationServiceLogsModel>(reader).ToList();
                        return data;
                    }
                    else if (method.ToLower().Trim() == "policy")
                    {
                        data.PolicyserviceLogs = ((IObjectContextAdapter)context).ObjectContext.Translate<BIPolicyServiceLogsModel>(reader).ToList();
                        return data;
                    }
                    else
                    {
                        exception = "Method is not defined";
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\PowerBiAPI\logs\" + method + "_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());

                    exception = ex.ToString();
                    ErrorLogger.LogError(ex.Message, ex, false);
                    return null;
                }
                finally
                {
                    if (context.Database.Connection.State == ConnectionState.Open)
                        context.Database.Connection.Close();
                }
            }
        }


        public PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>> GetAvgResponse(int InsuranceTypeId, int ModuleId, int StatusCode)
        {
            PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>> output = new PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>();
            PowerBIServicesLog log = new PowerBIServicesLog();
            DateTime dtBeforeCalling = DateTime.Now;

            log.Method = "AVGResponse";
            log.CompanyKey = "All";
            log.Channel = "PowerPI";
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.StartDate = DateTime.Now.Date.AddDays(-7);
            log.EndDate = DateTime.Now.Date.AddDays(-1);

            try
            {
                string exception = string.Empty;
                if (!Enum.IsDefined(typeof(ProuductTypes), InsuranceTypeId))
                {
                    output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Invalid InsuranceTypeId";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PowerBILogDataAccess.AddToPowerBILogDataAccess(log);
                    return output;
                }
                if (!Enum.IsDefined(typeof(Modules), ModuleId))
                {
                    output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Invalid ModuleId ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorDescription = "Invalid ModuleId : " + ModuleId;
                    PowerBILogDataAccess.AddToPowerBILogDataAccess(log);
                    return output;
                }
                if (!Enum.IsDefined(typeof(StatusCode), StatusCode))
                {
                    output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Invalid StatusCode";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorDescription = "invalid statusCode  :" + StatusCode;
                    PowerBILogDataAccess.AddToPowerBILogDataAccess(log);
                    return output;
                }
                output.Result = new List<ServiceRequestResponseTimeFromDBModel>();
                output.Result = GetAvgResponseSP(log.StartDate, log.EndDate, InsuranceTypeId, ModuleId, StatusCode, out exception);
                if (!string.IsNullOrEmpty(exception) | output.Result == null || output.Result.Count < 1)
                {
                    output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = "Generic Error";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorDescription = exception.ToString();
                    PowerBILogDataAccess.AddToPowerBILogDataAccess(log);
                    return output;
                }
                output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.Success;
                output.ErrorDescription = "success";
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>>.ErrorCodes.ExceptionError;
                output.ErrorDescription = "Generic Error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PowerBILogDataAccess.AddToPowerBILogDataAccess(log);
                return output;
            }
        }

        public List<ServiceRequestResponseTimeFromDBModel> GetAvgResponseSP(DateTime startDate, DateTime endDate, int InsuranceTypeId, int ModuleId, int StatusCode, out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetAVGServiceRequestResponseTimePowerPI";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 60 * 60 * 60;
                    if (StatusCode == 1 || StatusCode == 2)
                    {
                        if (StatusCode == 2)
                        {
                            SqlParameter StatusCodeParameter = new SqlParameter() { ParameterName = "@ErrorCode", Value = StatusCode };
                            command.Parameters.Add(StatusCodeParameter);
                        }
                        else
                        {
                            SqlParameter StatusCodeParameter = new SqlParameter() { ParameterName = "@SuccessCode", Value = StatusCode };
                            command.Parameters.Add(StatusCodeParameter);
                        }
                    }

                    if (ModuleId == 1 || ModuleId == 2)
                    {
                        if (ModuleId == 1)
                        {
                            SqlParameter ModuleIdParameter = new SqlParameter() { ParameterName = "@Vehicle", Value = 1 };
                            command.Parameters.Add(ModuleIdParameter);
                        }
                        else if (ModuleId == 2)
                        {
                            SqlParameter ModuleIdParameter = new SqlParameter() { ParameterName = "@Autolease", Value = 2 };
                            command.Parameters.Add(ModuleIdParameter);
                        }
                    }
                    if (!string.IsNullOrEmpty(startDate.ToString()))
                    {
                        SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "@StartDate", Value = startDate.ToString("yyyy-MM-dd 00:00:00") };
                        command.Parameters.Add(StartDateParameter);
                    }
                    if (!string.IsNullOrEmpty(endDate.ToString()))
                    {
                        SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "@EndDate", Value = endDate.ToString("yyyy-MM-dd 23:59:59") };
                        command.Parameters.Add(EndDateParameter);
                    }

                    if (InsuranceTypeId > 0)
                    {
                        SqlParameter InsuranceTypeIdParameter = new SqlParameter() { ParameterName = "@InsuranceTypeId", Value = InsuranceTypeId };
                        command.Parameters.Add(InsuranceTypeIdParameter);
                    }
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<ServiceRequestResponseTimeFromDBModel> filteredData = new List<ServiceRequestResponseTimeFromDBModel>();
                    filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<ServiceRequestResponseTimeFromDBModel>(reader)?.ToList();
                    filteredData = filteredData.OrderBy(x => x.AvgInSec).ToList();
                    return filteredData;
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
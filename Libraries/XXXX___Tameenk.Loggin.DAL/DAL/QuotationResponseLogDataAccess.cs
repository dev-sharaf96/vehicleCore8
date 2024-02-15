using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;

namespace Tameenk.Loggin.DAL.DAL
{
    public class QuotationResponseLogDataAccess
    {
        public static List<ServiceRequestResponseTimeFromDBModel> ServiceRequestResponseTimeFromDBFilter(ServiceResponseTimeFilterModel responsetimefilter, int commandTimeout, out int total, out string exception, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Company Name", bool? sortOrder = false)        {            exception = string.Empty;            try            {                using (TameenkLog context = new TameenkLog())                {                    var command = context.Database.Connection.CreateCommand();                    command.CommandText = "GetAVGServiceRequestResponseTime";                    command.CommandType = CommandType.StoredProcedure;                    command.CommandTimeout = 10 * 60;                    if (responsetimefilter.StatusCode.HasValue)                    {                        if (responsetimefilter.StatusCode == 2)                        {                            SqlParameter StatusCode = new SqlParameter() { ParameterName = "ErrorCode", Value = responsetimefilter.StatusCode };                            command.Parameters.Add(StatusCode);                        }                        else                        {                            SqlParameter StatusCode = new SqlParameter() { ParameterName = "SuccessCode", Value = responsetimefilter.StatusCode };                            command.Parameters.Add(StatusCode);                        }                    }                    if (responsetimefilter.ModuleId.HasValue)                    {                        if (responsetimefilter.ModuleId == 1)                        {                            SqlParameter ModuleId = new SqlParameter() { ParameterName = "Vehicle", Value = 1 };                            command.Parameters.Add(ModuleId);                        }                        else if (responsetimefilter.ModuleId == 2)                        {                            SqlParameter ModuleId = new SqlParameter() { ParameterName = "Autolease", Value = 2 };                            command.Parameters.Add(ModuleId);                        }                    }                    if (responsetimefilter.StartDate.HasValue)                    {                        SqlParameter StartDate = new SqlParameter() { ParameterName = "StartDate", Value = responsetimefilter.StartDate?.ToString() };                        command.Parameters.Add(StartDate);                    }                    if (responsetimefilter.EndDate.HasValue)                    {                        SqlParameter EndDate = new SqlParameter() { ParameterName = "EndDate", Value = responsetimefilter.EndDate?.ToString() };                        command.Parameters.Add(EndDate);                    }

                    if (!string.IsNullOrEmpty(responsetimefilter.InsuranceTypeId))
                    {
                        SqlParameter InsuranceTypeId = new SqlParameter() { ParameterName = "InsuranceTypeId", Value = responsetimefilter.InsuranceTypeId };
                        command.Parameters.Add(InsuranceTypeId);
                    }

                    if (!string.IsNullOrEmpty(responsetimefilter.InsuranceCompanyId))
                    {
                        SqlParameter InsuranceCompanyId = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = responsetimefilter.InsuranceCompanyId };
                        command.Parameters.Add(InsuranceCompanyId);
                    }
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<ServiceRequestResponseTimeFromDBModel> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<ServiceRequestResponseTimeFromDBModel>(reader).ToList();
                    total = filteredData.Count();
                    filteredData = filteredData.OrderBy(x => x.AvgInSec).ToList();
                    return filteredData;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                total = 0;
                return null;
            }
        }
    }
}

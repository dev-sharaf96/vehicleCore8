using System;
using System.Collections.Generic;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.PowerBI.Component
{
    public interface IPowerBIContext
    {
        BIServiceLogsModel GetAllServiceLog(string method, string companyKey, DateTime startDate, DateTime endDate, out string exception);
        PowerPIOutputModel<List<ServiceRequestResponseTimeFromDBModel>> GetAvgResponse(int InsuranceTypeId, int ModuleId, int StatusCode);
    }
}

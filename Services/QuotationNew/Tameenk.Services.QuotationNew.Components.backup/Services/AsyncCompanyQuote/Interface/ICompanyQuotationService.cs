using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components
{
    public interface ICompanyQuotationService
    {
        Task<QuotationResponseModel> HandleGetQuote(InsuranceCompany insuranceCompany, QuotationNewRequestDetails quoteRequest, int insuranceTypeCode, ServiceRequestLog predefinedLogInfo, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
    }
}

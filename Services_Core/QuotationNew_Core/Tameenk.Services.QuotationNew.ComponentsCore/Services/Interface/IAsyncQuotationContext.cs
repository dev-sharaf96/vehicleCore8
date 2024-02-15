using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using QuotationIntegrationDTO = Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components
{
    public interface IAsyncQuotationContext
    {
        //Task<string> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        Task<QuotationIntegrationDTO.QuotationResponseModel> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        Task<QuotationNewOutput> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
        //Task<QuotationNewOutput> GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
        //bool InsertIntoQuotationResponseCache(QuotationResponseCache quotationResponseCache, out string exception);
        //void InsertQuotationResponseIntoInmemoryCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId, string jsonResponse);
        void InsertQuotationResponseIntoInmemoryCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId, QuotationIntegrationDTO.QuotationResponseModel quotation);
    }
}

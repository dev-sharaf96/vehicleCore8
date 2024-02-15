using System;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;
using QuotationIntegrationDTO = Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components
{
  public  interface IQuotationService
    {
        Task<QuotationOutPut> GetQuotation(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair, string currentUserId, string currentUserName, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false);
        Task<QuotationIntegrationDTO.QuotationResponseModel> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        Task<QuotationNewOutput> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
        void InsertQuotationResponseIntoInmemoryCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId, QuotationIntegrationDTO.QuotationResponseModel quotation);
    }
}

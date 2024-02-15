using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.QuotationApi.Services
{
    public interface IRequestInitializer
    {
        QuotationServiceRequest GetQuotationRequestData(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue);
    }
}

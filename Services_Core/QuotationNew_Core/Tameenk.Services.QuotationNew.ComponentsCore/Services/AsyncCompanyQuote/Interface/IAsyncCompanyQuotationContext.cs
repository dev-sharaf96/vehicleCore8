using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components
{
    public interface IAsyncCompanyQuotationContext
    {
        Task<List<QuotationResponseModel>> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, DateTime excutionStartDate, Guid? parentRequestId = null, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
    }
}

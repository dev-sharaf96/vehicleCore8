using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.QuotationNew.Components
{
    public interface IQuotationNewContext
    {
        //QuotationResponseCache GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        QuotationResponseCache GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        QuotationNewOutput GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);
        bool InsertIntoQuotationResponseCache(QuotationResponseCache quotationResponseCache, out string exception);
    }
}

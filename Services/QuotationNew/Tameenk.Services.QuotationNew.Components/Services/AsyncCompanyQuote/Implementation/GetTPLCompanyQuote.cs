//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Tameenk.Loggin.DAL;
//using Tameenk.Core.Domain.Entities;

//namespace Tameenk.Services.QuotationNew.Components
//{
//    public class GetTPLCompanyQuote : GetCompanyQuote
//    {
//        private readonly ICompanyQuotationService _companyQuotationService;

//        public GetTPLCompanyQuote()
//        {
//        }

//        public GetTPLCompanyQuote(ICompanyQuotationService companyQuotationService)
//        {
//            _companyQuotationService = companyQuotationService;
//        }

//        public override async Task<QuotationNewResponseModel> HandleGetQuote(InsuranceCompany insuranceCompany, QuotationNewRequestDetails quoteRequest, ServiceRequestLog predefinedLogInfo, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
//        {
//            return await _companyQuotationService.HandleGetQuote(insuranceCompany, quoteRequest, 1, predefinedLogInfo, channel, userId, userName, log, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
//        }
//    }
//}

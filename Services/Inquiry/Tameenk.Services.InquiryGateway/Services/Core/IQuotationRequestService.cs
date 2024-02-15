using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Loggin.DAL;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Models.YakeenMissingFields;

namespace Tameenk.Services.InquiryGateway.Services.Core
{
    public interface IQuotationRequestService
    {
        QuotationRequest GetQuotationRequest(string quotationExternalId);
        InquiryResponseModel HandleQuotationRequest(InquiryRequestModel requestModel, string authorizationToken, ServiceRequestLog predefinedLogInfo);

        InquiryResponseModel UpdateQuotationRequestWithYakeenMissingFields(YakeenMissingInfoRequestModel model);


        /// <summary>
        /// Check if all info from yakeen are returned, if not then user should enter all the missing fields.
        /// </summary>
        /// <param name="result">Inquiry response model</param>
        /// <returns></returns>

        InquiryResponseModel HandleYakeenMissingFields(InquiryResponseModel result);

        InitInquiryResponseModel InitInquiryRequest(InitInquiryRequestModel requestModel);

        InquiryResponseModel HandleQuotationRequestNew(InquiryRequestModel requestModel, string authorizationToken, ServiceRequestLog predefinedLogInfo);


        InitInquiryResponseModel InitInquiryRequestNew(InitInquiryRequestModel requestModel);
    }
}
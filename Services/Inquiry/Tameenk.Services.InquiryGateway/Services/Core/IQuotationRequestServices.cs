using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.InquiryGateway.Models;

namespace Tameenk.Services.InquiryGateway.Services.Core
{
    public interface IQuotationRequestServices
    {
        InquiryResponseModel HandleQuotationRequest(InquiryRequestModel requestModel, string userId = null);
    }
}
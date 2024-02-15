using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;

namespace Tameenk.Services.Inquiry.Components
{
    public interface IInquiryContext
    {
        InquiryOutput InitInquiryRequest(InitInquiryRequestModel model, InquiryRequestLog log);
        InquiryOutput SubmitInquiryRequest(InquiryRequestModel requestModel, string channel, Guid userId, string userName, Guid? parentRequestId = null);

        //NationalAddressOutput GetNationalAddress(string driverNin, string birthDate, string channel, bool fromYakeen);
        InquiryOutput SubmitMissingFeilds(YakeenMissingInfoRequestModel model, string userId, string userName);
        Tameenk.Services.Inquiry.Components.AddDriverOutput AddVechileDriver(AddDriverModel model, string UserId, string userName, bool automatedTest = false);
        AddDriverOutput PurchaseVechileDriver(PurchaseDriverModel model, string UserId, string userName);
        AddressInfoOutput GetAddressByNationalId(string nationalId, string birthDate, string externalId, string channel);
    }
}

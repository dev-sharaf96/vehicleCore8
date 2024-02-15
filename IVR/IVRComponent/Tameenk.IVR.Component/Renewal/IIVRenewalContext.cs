using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;

namespace Tameenk.IVR.Component
{
    public interface IIVRenewalContext
    {
        IVRRenewalOutput<RenewalQuotationResponseModel> ProcessRenewalRequest(RenewalInquiryRequestModel quotationModel);
        IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> SendSadadNumberSMS(RenewalSendLowestLinkSMSRequestModel request);
        IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> SendLowestPriceLinkBySMS(RenewalSendLowestLinkSMSRequestModel request);

        void AddBasicLog(IVRServicesLog log, string methodName, IVRModuleEnum module);
    }
}

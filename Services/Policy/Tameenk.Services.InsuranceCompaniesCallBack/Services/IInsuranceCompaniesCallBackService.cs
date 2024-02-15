using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Services
{
    public interface IInsuranceCompaniesCallBackService
    {
        CommonResponseModel NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel);
        PolicyAttachmentResponseModel GetPolicyAttachments(CommonPolicyRequestModel policyAttachmentModel);
        PolicyRequestAdditionalInfoResponseModel GetPolicyRequestAdditionalInfo(CommonPolicyRequestModel policyRequestModel);
        PolicyAttachmentResponseModel GetPolicyAttachmentsWithURL(CommonPolicyRequestModel policyAttachmentModel);
        WataniyaCallBackOutput UpdateWataniyaPolicyinfo(WataniyaPolicyinfoCallbackRequest callBackRequest);
        WataniyaCallBackOutput UpdateWataniyaVehicleId(WataniyaUpdateVehicleIdCallbackRequest callBackRequest);
        RenewedPoliciesServiceResponse RenewedPoliciesServiceByCompany(RenewedPoliciesServiceRequest callBackRequest);
        void AddRequestLog(PolicyNotificationLog log);
    }
}

using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Repository
{
    public interface IPolicyRepository
    {
        void NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel);
       Tameenk.Core.Domain.Entities.Policy GetPolicyWithReferenceIdAndPolicyNumber(PolicyUploadNotificationModel policyUploadNotificationModel);
        void SavePolicyWithNajmStatus(PolicyUploadNotificationModel policyUploadNotificationModel);
        bool IsPolicyExist(string referenceId, string policyNo);
        bool CheckPolicyExistenceByReferenceIdOrPolicyNo(string referenceId, string policyNo);
        WataniyaMotorPolicyInfo CheckTplWataniyaInitialPolicyExistenceByReferenceId(string referenceId);
        WataniyaMotorPolicyInfo CheckCompWataniyaInitialPolicyExistenceByReferenceId(string referenceId);
        bool UpdateWataniyaPolicyInfoCallback(WataniyaMotorPolicyInfo policyInfo, out string exception);
    }
}
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.PolicyApi.Models;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.PolicyApi.Services
{
    public interface IPolicyRequestService
    {
        Task<bool> GeneratePolicyAsync(string referenceId, LanguageTwoLetterIsoCode userLanguage, ServiceRequestLog predefinedLogInfo, bool showErrors = false);
        Task<PolicyOutput> SubmitPolicyAsync(string referenceId, LanguageTwoLetterIsoCode userLanguage, ServiceRequestLog predefinedLogInfo, bool showErrors = false);
        void GetFailedPolicyFile(string referenceId, string channel);

        /// <summary>
        /// Send Policy File to Client
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <param name="policyResponse">policy Response</param>
        /// <param name="email">Eamil</param>
        /// <param name="userLanguage">user Language</param>
        /// <returns></returns>
        Task<bool> SendPolicyFileToClient(string referenceId, PolicyResponse policyResponse, string email, LanguageTwoLetterIsoCode userLanguage);

        PolicyOutput GeneratePolicyManually(PolicyData policyInfo);

    }
}

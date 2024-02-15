using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Services
{
    public interface IPolicyEmailService
    {
        Task SendPolicyViaMail(SendPolicyViaMailDto sendPolicyViaMailDto);
        string GetPolicySuccessEmailBody(LanguageTwoLetterIsoCode userLanguage);
        string GetPolicyFailedEmailBody(SendPolicyViaMailDto sendPolicyViaMailDto);

    }
}
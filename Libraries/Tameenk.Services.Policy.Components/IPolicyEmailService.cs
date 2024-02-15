using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Policy.Components
{
    public interface IPolicyEmailService
    {
        EmailOutput SendPolicyByMail(SendPolicyViaMailDto sendPolicyViaMailDto, string companyName);
    }
}
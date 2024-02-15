using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    public interface IAutoleaseContext
    {
        AutoLeaseOutput SendClaimRegistrationRequest(ClaimRegistrationRequest claim, int companyId);
        AutoLeaseOutput SendClaimNotificationRequest(ClaimNotificationRequest claimNotification);
        AutoLeaseOutput SendCancellationRequest(CancelPolicyRequestDto cancelRequest, int bankId);
    }
}

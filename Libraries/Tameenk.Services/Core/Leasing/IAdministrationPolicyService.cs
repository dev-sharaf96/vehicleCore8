using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Leasing.Models;

namespace Tameenk.Services.Core
{
    public interface IAdministrationPolicyService
    {
        LeaseOutput<UserClaimModel> GetUserClaimsData(ClaimsFilter filterModel, string currentUserId, string lang);
        LeaseOutput<List<ClaimStatusOutput>> GetAllClaimsStatus(string currentUserId, string lang);
        LeaseOutput<UserClaimListingModel> GetUserClaimDetails(ClaimsFilter filterModel, string currentUserId, string lang);
        LeaseOutput<DownloadClaimFileModel> DownloadClaimFilebyFileId(int fileId, string currentUserId, string lang);

        LeaseOutput<bool> UpdateClaim(ClaimsUpdateModel model, string userId, string lang);
    }
}

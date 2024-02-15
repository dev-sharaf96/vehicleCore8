using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Policies.leasingportal;

namespace Tameenk.Services.Core
{
    public interface ILeasingPolicyService
    {
        LeaseOutput<bool> AddClaimRegestration(ClaimRegestrationModel claim, string userId, Tameenk.Core.Domain.Enums.ClaimRequesterType requesterType, ClaimModule claimModule, string lang);
        LeaseOutput<UserClaimModel> GetUserClaimsData(ClaimsFilter filterModel, string currentUserId, string lang);
        LeaseOutput<List<ClaimStatusOutput>> GetAllClaimsStatus(string currentUserId, string lang);
        LeaseOutput<UserClaimListingModel> GetUserClaimDetails(ClaimsFilter filterModel, string currentUserId, string lang);
        LeaseOutput<DownloadClaimFileModel> DownloadClaimFilebyFileId(int fileId, string currentUserId, string lang);

        LeaseOutput<bool> UpdateClaim(ClaimsUpdateModel model, string userId, string lang);
    }
}

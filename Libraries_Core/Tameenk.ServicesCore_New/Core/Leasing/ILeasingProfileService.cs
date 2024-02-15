using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Policies.leasingportal;

namespace Tameenk.Services.Core.Leasing
{
    public interface ILeasingProfileService
    {
        List<policyStatistics> GetLeasingProfilePolicies(string currentUserId, string nationalId, out string exception);

        //AllPolicyDetailsModel GetClientPolicyLogDetailsService(string referenceId, string externalId, out string exception);

        DriverInsuranceRecord GetDriverInsuranceRecordService(ClientDataModel clientDataModel, ref string exception);

        ProfileBaiscData GetProfileBaiscDataSerivce(string ReferenceId, ref string exception);

        List<DriverModel> GetAdditionalDriversService(string ReferenceId, ref string exception);

         List<AllPolicyDetailsModel> GetClientPolicyLogDetailsService(ClientDataModel clientDataModel, out string exception);
    }
}

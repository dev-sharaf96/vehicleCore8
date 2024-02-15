using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Services.Profile.Component.Output;

namespace Tameenk.Services.Profile.Component
{
    public interface ICorporateContext
    {
        Task<ProfileOutput<LoginOutput>> CorporateUserSignIn(AspNetUser user, string password, string lang, LoginRequestsLog predefinedLog);
        CorporateUsers GetCorporateUserByUserId(string userId);
        CorporateAccount GetCorporateAccountById(int accountId);
        Task<ProfileOutput<bool>> AddCorporateUser(AddCorporateUserModel model);
        CorporatePoliciesOutput GetAllCorporatePolicies(int corporateAccountId, CorporatePoliciesFilter policyFilter, string from, string to, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        ProfileOutput<bool> ManageLockCorporateUser(ManageLockCorporateUserModel model);
        Task<ProfileOutput<VerifyCorporateOTPOutput>> VerifyCorporateOneTimePassword(string email, string otpCode, Channel channel, string lang);
    }
}

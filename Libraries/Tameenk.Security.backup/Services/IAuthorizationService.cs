using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;

namespace Tameenk.Security.Services
{
    public interface IAuthorizationService
    {
        IList<Role> GetAllRoles();

        Role GetRoleByName(string roleName);

        Client GetClientById(string id);

        Task<AspNetUser> FindUser(string userName, string password);

        Task<AspNetUser> GetUser(string id);

        Task<List<string>> CreateUser(AspNetUser user, string password);

        Task<bool> SendTwoFactorCodeSmsAsync(string userId, string phoneNumber);

        //Task<bool> ChangePhoneNumber(string userId, string phoneNumber, string code);
        bool ChangePhoneNumber(string userId, string phoneNumber, string code, out string errors);

        /// <summary>
        /// Get user id from user associated with the request.
        /// </summary>
        /// <param name="user">User principal</param>
        /// <returns></returns>
        string GetUserId(IPrincipal user);

        /// <summary>
        /// Get Access token to be used for communication between APIs
        /// </summary>
        /// <returns></returns>
        AccessTokenResult GetAccessToken(string userId);
        List<Core.Domain.Entities.AspNetUser> GetUsers(int PageIndex = 0, int PageSize = 10);

        int Update(UpdateCustomertModel entity);
        Core.Domain.Entities.AspNetUser GetUserInfoByEmail(string email, string userId , string mobile);        bool DeleteUser(string mobile);        bool ManageUserLock(string userId, bool toLock,string lockedby,string lockedreason);
        AspNetUser IsEmailOrPhoneExist(string email, string mobile);
        AspNetUser GetUserDBByID(string userId);        int ConfirmUserPhoneNumberDB(string userId);        int ConfirmUserEmailDB(string userId);
        bool CheckEmailExist(string email);
        bool CheckPhoneExist(string phone);
        bool UpdateUserEmail(string email, string userId,string phone);
        bool IsUserAutoleasingAuthorized(string userId);
        bool UpdateUserInfo(AspNetUser aspNetUser, out string exception);
        List<AspNetUser> GetSuperAdmins();
        List<AspNetUser> GetSuperAdminsRelated(string adminId);
        AspNetUser GetUserByEmail(string email);
        CorporateUsers GetCorporateUseByEmail(string email);

        string GenerateTokenJWT(string ID, string Email, string userName,string phone, string fullNameAr, string fullNameEn, out string key);
        void GetAllUsersByPhoneAndUpdate(string phone, string currentUserId, out string exception);
        bool CheckUserWithNationalAndDifferentEmail(string nationalId, string email);
        bool ManageUserMobileVerification(string email, string userId, bool isVerified, out string exception);
        AspNetUser GetUserInfoByNationalId(string nationalId);
    }
}

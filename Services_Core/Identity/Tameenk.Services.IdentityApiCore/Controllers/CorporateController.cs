using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.CustomAttributes;
using Tameenk.Security.Services;
using Tameenk.Services.Core;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.IdentityApi.Output;
using Tameenk.Services.Profile.Component;
using Tamkeen.bll.Model;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.IdentityApi.Controllers
{
    [SingleSessionAuthorizeAttribute]
    public class CorporateController : IdentityBaseController
    {
        private readonly ICorporateContext _corporateContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICorporateUserService _corporateUserService;
        private readonly IRepository<CorporateAccount> _corporateAccountRepository;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;

        #region The Ctro
        public CorporateController(ICorporateContext corporateContext,
            IAuthorizationService authorizationService,
            IRepository<CorporateAccount> corporateAccountRepository,
            IRepository<CorporateUsers> corporateUsersRepository,
            ICorporateUserService corporateUserService)
        {
            _corporateContext = corporateContext;
            _authorizationService = authorizationService;
            _corporateAccountRepository = corporateAccountRepository;
            _corporateUsersRepository = corporateUsersRepository;
            _corporateUserService = corporateUserService;
        }
        #endregion

        #region Action Methods
        [HttpGet]
        [Route("api/corporateProfile/GetCorporateAccountInfo")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IActionResult> GetCorporateAccountInfo(string channel)
        {
            var userId = User.Identity.GetUserId();
            userId = "9e85f4f1-9007-4cd4-8b51-fbb5fc88cda6";
            var profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetCorporateAccountInfo";
            profileLog.Channel = channel;

            try
            {
                var userData = await _authorizationService.GetUser(userId);
                if (userData == null)
                {

                }

                if (!userData.IsCorporateUser)
                {

                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                }

                if (corporateUser.IsSuperAdmin != true)
                {
                }

                var corporateAccount = _corporateContext.GetCorporateAccountById(corporateUser.CorporateAccountId);
                if (corporateAccount == null)
                {

                }

                var output = new CorporateAccountInfoModel();
                output.NameAr = corporateAccount.NameAr;
                output.NameEn = corporateAccount.NameEn;
                output.Balance = corporateAccount.Balance;

                return Ok(output, 1);

            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }
        }

        /// <summary>
        /// Get all corporate users with filter
        /// </summary>
        /// <param name="userFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/corporateProfile/all-users-with-Filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CorporateUserModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IActionResult> GetCorporateUsersWithFilter([FromBody]CorporateFilterModel userFilter, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            var log = new ProfileRequestsLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Method = "GetCorporateUsersWithFilter";
            log.Channel = userFilter.Channel.ToString();
            log.UserID = new Guid("9e85f4f1-9007-4cd4-8b51-fbb5fc88cda6");
            var userId = "9e85f4f1-9007-4cd4-8b51-fbb5fc88cda6";
            try
            {
                if (userFilter == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Model is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Error(log.ErrorDescription);
                }

                var userData = await _authorizationService.GetUser(userId);
                if (userData == null)
                {

                }

                if (!userData.IsCorporateUser)
                {

                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                }

                if (corporateUser.IsSuperAdmin != true)
                {
                }
                userFilter.AccountId = corporateUser.CorporateAccountId;

                int totalCount = 0;                string exception = string.Empty;                var result = _corporateUserService.GetCorporateUsersWithFilter(userFilter, pageIndex, pageSize, 240, false, out totalCount, out exception);

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Error("Result Error --> " + exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Error(log.ErrorDescription);
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }


        [HttpPost]
        [Route("api/corporateProfile/addCorporateUser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CorporateUserModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IActionResult> AddCorporateUser([FromBody] CorporateNewUserModel model)
        {
            Output<bool> output = new Output<bool>();
            output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "CorporateAddUser";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            try
            {
                var userId = "bec1c8f6-f042-40fa-913f-cd4b0387b76a";
                var userData = await _authorizationService.GetUser(userId);
                if (userData == null)
                {

                }

                if (!userData.IsCorporateUser)
                {

                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                }

                if (corporateUser.IsSuperAdmin != true)
                {
                }
                model.AccountId = corporateUser.CorporateAccountId;

                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output<bool>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Email is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Password is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "PhoneNumber is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                var userVal = _authorizationService.IsEmailOrPhoneExist(model.Email, model.PhoneNumber);
                if (userVal != null && userVal.Email.ToLower() == model.Email.ToLower())
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "email already exist";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (userVal != null && userVal.PhoneNumber == model.PhoneNumber)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "mobile already exist";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                var user = new Tameenk.Core.Domain.Entities.AspNetUser();
                user.CreatedDate = DateTime.Now;
                user.LastModifiedDate = DateTime.Now;
                user.LockoutEndDateUtc = DateTime.UtcNow;
                user.LockoutEnabled = false;
                user.LastModifiedDate = DateTime.Now;
                user.LastLoginDate = DateTime.Now;
                user.DeviceToken = "";
                user.Email = model.Email;
                user.EmailConfirmed = true; //TODO
                user.RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"); //_db.Roles.ToList()[0].ID,
                user.LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"); //_db.Languages.Where(l => l.isDefault).Select(l => l.Id).SingleOrDefault(),
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumberConfirmed = true; //TODO
                user.UserName = model.Email;
                user.FullName = model.FullName;
                user.TwoFactorEnabled = false;
                user.Channel = model.Channel.ToString();
                user.IsCorporateUser = true;

                var result = await _authorizationService.CreateUser(user, model.Password);
                if (result.Any())
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.CanNotCreate;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "can not create user due to " + string.Join(",", result);
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                CorporateUsers newCorporateUser = new CorporateUsers();
                newCorporateUser.UserId = user.Id;
                newCorporateUser.UserName = user.Email;
                newCorporateUser.PhoneNumber = user.PhoneNumber;
                newCorporateUser.PasswordHash = SecurityUtilities.HashData(model.Password, null);
                newCorporateUser.IsActive = true;
                newCorporateUser.CreatedDate = DateTime.Now;
                newCorporateUser.CorporateAccountId = model.AccountId;
                newCorporateUser.IsSuperAdmin = false;
                _corporateUsersRepository.Insert(newCorporateUser);

                output.ErrorCode = Output<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result = true;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<bool>.ErrorCodes.ExceptionError; ;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)); ;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Error("an error has occured");
            }
        }
        #endregion
    }
}
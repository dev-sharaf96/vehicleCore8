using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.AdministrationApi.AutoleasingOutput;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Payments;
using Tamkeen.bll.Model;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class CorporateController : AdminBaseApiController
    {
        private readonly ICorporateUserService _corporateUserService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;
        private readonly IRepository<CorporateAccount> _corporateAccountRepository;
        private readonly ICorporateAccountService _corporateAccountService;
        private readonly IUserPageService _userPageService;

        public CorporateController(ICorporateUserService corporateUserService, IAuthorizationService authorizationService, IRepository<CorporateUsers> corporateUsersRepository,
            IRepository<CorporateAccount> corporateAccountRepository, ICorporateAccountService corporateAccountService, IUserPageService userPageService)
        {
            _corporateUserService = corporateUserService;
            _authorizationService = authorizationService;
            _corporateUsersRepository = corporateUsersRepository;
            _corporateAccountRepository = corporateAccountRepository;
            _corporateAccountService = corporateAccountService;
            _userPageService = userPageService;
        }

        #region Users

        /// <summary>
        /// geet all corporate users from data base with filter
        /// </summary>
        /// <param name="userFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/corporate/all-users-with-Filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CorporateUserModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCorporateUsersWithFilter([FromBody]CorporateFilterModel userFilter, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Corporate Users";
            log.PageURL = "/admin/corporateUsers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetCorporateUsersWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(userFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (userFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                var result = _corporateUserService.GetCorporateUsersWithFilter(userFilter, pageIndex, pageSize, 240, false, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error --> " + exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        [HttpPost]
        [Route("api/corporate/Adduser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CorporateUserModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IActionResult> RegisterUserAsync([FromBody] CorporateNewUserModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "CorporateAddUser";
            //log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)), Field = "Email" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Email is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)), Field = "Password" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Password is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)), Field = "Mobile" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Phone Number is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                var userVal = _authorizationService.IsEmailOrPhoneExist(model.Email, model.PhoneNumber);
                if (userVal != null && userVal.Email.ToLower() == model.Email.ToLower())
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Lang)), Field = "Email" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData;
                    log.ErrorDescription = "email already exist";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                if (userVal != null && userVal.PhoneNumber == model.PhoneNumber)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(model.Lang)), Field = "Mobile" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData; ;
                    log.ErrorDescription = "mobile already exist";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                //if (Output.Result.errors.Count != 0)
                //{
                //    return Single(Output);
                //}

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
                user.Channel = Channel.autoleasing.ToString();
                user.IsCorporateUser = true;

                var result = await _authorizationService.CreateUser(user, model.Password);
                if (result.Any())
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.CanNotCreate; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(model.Lang)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "can not create user due to " + string.Join(",", result);
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                CorporateUsers corporateUser = new CorporateUsers();
                corporateUser.UserId = user.Id;
                corporateUser.UserName = user.Email;
                corporateUser.PhoneNumber = user.PhoneNumber;
                corporateUser.PasswordHash = SecurityUtilities.HashData(model.Password, null);
                corporateUser.IsActive = true;
                corporateUser.CreatedDate = DateTime.Now;
                corporateUser.CorporateAccountId = model.AccountId;
                _corporateUsersRepository.Insert(corporateUser);

                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Single(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/corporate/manageLockUser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AdministrationOutput<bool>>))]
        public IActionResult LockUser([FromBody] CorporateNewUserModel user)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Corporate Users";
            log.PageURL = "/admin/CorporateAddUser";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "CorporateLockUser";
            log.ServiceRequest = JsonConvert.SerializeObject(user);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(user.UserId))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(user.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "user id is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var userInfo = _corporateUsersRepository.Table.Where(x => x.UserId == user.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.UserNotFound;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("UserNotExist", CultureInfo.GetCultureInfo(user.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "user not found in CorporateUsers with this id " + user.UserId;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (!userInfo.IsActive)
                {
                    var corporateAccount = _corporateAccountService.GetCorporateAccountById(userInfo.CorporateAccountId);
                    if (corporateAccount == null)
                    {
                        Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotFound;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.Lang));
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "this user's account not found in CorporateAccounts with this account id " + userInfo.CorporateAccountId;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(Output);
                    }
                    if (!corporateAccount.IsActive.HasValue || !corporateAccount.IsActive.Value)
                    {
                        Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.Lang));
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "can not activate this user because his account is deActivate";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(Output);
                    }
                }

                userInfo.IsActive = !userInfo.IsActive;
                _corporateUsersRepository.Update(userInfo);

                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException; ;
                Output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/corporate/corporateAccounts")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        public IActionResult GetCorporateAccounts(string lang = "ar")
        {
            try
            {
                List<CorporateAccount> result = _corporateAccountService.GetCorporateAccounts();
                IEnumerable<IdNamePairModel> dataModel = result.Select(e => new IdNamePairModel()
                {
                    Id = e.Id,
                    Name = (lang == "en") ? e.NameEn : e.NameAr
                });

                return Ok(dataModel, result.Count());
            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }
        }

        #endregion

        #region Accounts

        /// <summary>
        /// geet all corporate accounts from data base with filter
        /// </summary>
        /// <param name="userFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/corporate/all-corporate-account-with-Filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CorporateUserModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCorporateAccountsWithFilter([FromBody]CorporateAccountFilter filterModel, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Corporate Accounts";
            log.PageURL = "/admin/corporate/account";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetCorporateAccountsWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (filterModel == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                var result = _corporateAccountService.GetCorporateAccountWithFilter(filterModel, pageIndex, pageSize, 240, false, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error --> " + exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        [HttpPost]
        [Route("api/corporate/AddAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CorporateAccountModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult AddCorporateAccount([FromBody] CorporateNewAccountModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "AddCorporateAccount";
            //log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(model.NameAr) && string.IsNullOrEmpty(model.NameEn))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "Name" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "NameEn or NameAr is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                if (model.Balance < 0)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "Balance" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Balance is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                CorporateAccount corporateAccount = new CorporateAccount();
                corporateAccount.Id = model.Id;
                corporateAccount.NameAr = model.NameAr;
                corporateAccount.NameEn = model.NameEn;
                corporateAccount.Balance = model.Balance;
                corporateAccount.CreatedBy = model.CreatedBy;
                corporateAccount.CreatedDate = DateTime.Now;
                corporateAccount.IsActive = true;

                string exception = string.Empty;
                _corporateAccountService.AddOrupdateCororateAccount(corporateAccount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "ErrorGeneric" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.ExceptionError;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Single(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/corporate/getCorporateAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CorporateAccountModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCorporateAccount(int id)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Corporate Accounts";
            log.PageURL = "/admin/corporate/account";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetCorporateAccountsWithFilter";
            log.ServiceRequest = $"id={id}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (id <= 0)
                    throw new TameenkArgumentNullException("id");

                var corporateAccount = _corporateAccountService.GetCorporateAccount(id);
                if (corporateAccount == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }

                var corporateAccountModel = new CorporateAccountModel() {
                    Id = corporateAccount.Id,
                    NameAr = corporateAccount.NameAr,
                    NameEn = corporateAccount.NameEn,
                    Balance = corporateAccount.Balance,
                    IsActive = (corporateAccount.IsActive.HasValue) ? corporateAccount.IsActive.Value : false
                };

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(corporateAccountModel);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }


        [HttpPost]
        [Route("api/corporate/EditAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CorporateAccountModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult EditCorporateAccount([FromBody] CorporateNewAccountModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "EditCorporateAccount";
            //log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(model.NameAr) && string.IsNullOrEmpty(model.NameEn))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "Name" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "NameEn or NameAr is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                if (model.Balance < 0)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "Balance" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "Balance is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                var corporateAccount = _corporateAccountService.GetCorporateAccount(model.Id);
                if (corporateAccount == null)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "Balance" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "account not found in CorporateUsers with this id " + model.Id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                corporateAccount.NameAr = model.NameAr;
                corporateAccount.NameEn = model.NameEn;
                corporateAccount.Balance = model.Balance;
                corporateAccount.ModifiedBy = model.CreatedBy;
                corporateAccount.ModifiedDate = DateTime.Now;
                corporateAccount.IsActive = model.IsActive;

                string exception = string.Empty;
                _corporateAccountService.AddOrupdateCororateAccount(corporateAccount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)), Field = "ErrorGeneric" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.ExceptionError;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Single(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/corporate/manageLockAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AdministrationOutput<bool>>))]
        public IActionResult LockCorporateAccount([FromBody] CorporateAccountModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Corporate Accounts";
            log.PageURL = "/admin/Corporate/account";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "CorporateLockUser";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (model.Id <= 0)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "account id is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var accountInfo = _corporateAccountService.GetCorporateAccount(model.Id);
                if (accountInfo == null)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.UserNotFound;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("UserNotExist", CultureInfo.GetCultureInfo(model.lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "account not found in CorporateUsers with this id " + model.Id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                accountInfo.IsActive = !accountInfo.IsActive;

                string exception = string.Empty;
                _corporateAccountService.AddOrupdateCororateAccount(accountInfo, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang));
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.ExceptionError;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                var accountUsers = _corporateUserService.GetAccountUsers(model.Id);
                if (accountUsers != null && accountUsers.Count > 0)
                {
                    foreach (var user in accountUsers)
                    {
                        user.IsActive = false;
                    }

                    exception = string.Empty;
                    _corporateUserService.UpdateBulkusers(accountUsers, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang));
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.ExceptionError;
                        log.ErrorDescription = "error happend when try to deActivate account users, and the exception is --> " + exception;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                }

                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = Output.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Add Balance To Wallet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/corporate/addBalanceToWallet")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<WalletOutput>))]
        //[AdminAuthorizeAttribute(pageNumber: 11)]
        public IActionResult AddBalanceToWallet(WalletAddBalanceModel walletAddBalanceModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "WalletPayments";
            log.PageURL = "/admin/corporate/account";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddBalanceToWallet";
            log.ServiceRequest = JsonConvert.SerializeObject(walletAddBalanceModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(11, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                walletAddBalanceModel.BalanceAddedBy = User.Identity.GetUserName();
                WalletOutput output = _corporateAccountService.AddBalanceToWallet(walletAddBalanceModel);

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        #endregion
    }
}

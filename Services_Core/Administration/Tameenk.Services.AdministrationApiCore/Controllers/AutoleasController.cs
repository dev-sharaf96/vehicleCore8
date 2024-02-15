using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
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
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Policy.Components;
using Tamkeen.bll.Model;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class AutoleasController : AdminBaseApiController
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IBankService _banktService;
        private readonly IBankNinService _bankNinService;
        private readonly IBankCompanyService _bankCompanyService;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IUserPageService _userPageService;
        private readonly IAutoleasingUserService _autoleaseUserService;

        public AutoleasController(IAuthorizationService authorizationService , IBankCompanyService bankCompanyService, IBankService bankService, 
            IBankNinService bankNinService, IInsuranceCompanyService insuranceCompanyService, IUserPageService userPageService, IAutoleasingUserService autoleaseUserService)
        {

            this._authorizationService = authorizationService;
            this._bankCompanyService = bankCompanyService;
            this._banktService = bankService;
            this._bankNinService = bankNinService;
            this._insuranceCompanyService = insuranceCompanyService;
            this._userPageService = userPageService;
            this._autoleaseUserService = autoleaseUserService;
        }

        /// <summary>
        /// geet all autolease users from data base with filter
        /// </summary>
        /// <param name="userFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/autoleas/all-users-with-Filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<AutoleaseUserModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetAutoleaseUsersWithFilter([FromBody]AutoleaseFilterModel userFilter, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Autolease Users";
            log.PageURL = "/admin/usersAutolease";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AutoleasingGetAutoleaseUsersWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(userFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (userFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                var result = _autoleaseUserService.GetAutoleaseUsersWithFilter(userFilter, pageIndex, pageSize, 240, out totalCount, out exception);
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
        [Route("api/autoleas/Adduser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AutoleaseUserModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "AutoleasingAddAdminUser";
            //log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                foreach (var user in model.users)
                {
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.Language)), Field = "Email" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        log.ErrorDescription = "Email is empty";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.Language)), Field = "Password" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        log.ErrorDescription = "Password is empty";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (string.IsNullOrEmpty(user.Mobile))
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.Language)), Field = "Mobile" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        log.ErrorDescription = "Mobile is empty";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    var userVal = _authorizationService.IsEmailOrPhoneExist(user.Email, user.Mobile);
                    var userInListSameMobile = model.users.Count(x => x.Mobile == user.Mobile);
                    var userInListSameEmail = model.users.Count(x => x.Email == user.Email);
                    if (userInListSameMobile > 1) // check if mobile duplicate in the list recieved 
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_phone_signup_error_inList", CultureInfo.GetCultureInfo(user.Language)), Field = "Mobile" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData;
                        log.ErrorDescription = "Mobile is used for more than one user in list you enter now";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (userInListSameEmail > 1) // check if email duplicate in the list recieved
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_email_signup_error_inList", CultureInfo.GetCultureInfo(user.Language)), Field = "Email" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData;
                        log.ErrorDescription = "Email is used for more than one user in list you enter now";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (userVal != null && userVal.Email.ToLower() == user.Email.ToLower())
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(user.Language)), Field = "Email" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData;
                        log.ErrorDescription = "email already exist";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (userVal != null && userVal.PhoneNumber == user.Mobile)
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(user.Language)), Field = "Mobile" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData; ;
                        log.ErrorDescription = "mobile already exist";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Single(Output);
                    }
                    if (user.IsSuperAdmin)
                    {
                        var bank = _banktService.GetBank(user.BankId);

                        //if bank id not exist in lookup table
                        if (bank == null)
                        {
                            Error error = new Error() { Message = WebResources.ResourceManager.GetString("bank_not_exist", CultureInfo.GetCultureInfo(user.Language)), Field = "BankId" };
                            Output.Result.errors.Add(error);
                            log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData; ;
                            log.ErrorDescription = "bank id not exist";
                            AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return Single(Output);
                        }
                    }
                }
                if (Output.Result.errors.Count != 0)
                {
                    return Single(Output);
                }
                foreach (var _user in model.users)
                {
                    if (_user.IsSuperAdmin)
                    {
                        var user = new Tameenk.Core.Domain.Entities.AspNetUser();
                        user.CreatedDate = DateTime.Now;
                        user.LastModifiedDate = DateTime.Now;
                        user.LockoutEndDateUtc = DateTime.UtcNow;
                        user.LockoutEnabled = false;
                        user.LastModifiedDate = DateTime.Now;
                        user.LastLoginDate = DateTime.Now;
                        user.DeviceToken = "";
                        user.Email = _user.Email;
                        user.EmailConfirmed = true; //TODO
                        user.RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"); //_db.Roles.ToList()[0].ID,
                        user.LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"); //_db.Languages.Where(l => l.isDefault).Select(l => l.Id).SingleOrDefault(),
                        user.PhoneNumber = _user.Mobile;
                        user.PhoneNumberConfirmed = true; //TODO
                        user.UserName = _user.Email;
                        user.FullName = _user.FullName;
                        user.TwoFactorEnabled = false;
                        user.Channel = Channel.autoleasing.ToString();
                        user.IsAutoLeasing = true;
                        user.AutoLeasingBankId = _user.BankId;
                        user.AutoLeasingAdminId = null;
                        user.IsAutoLeasingSuperAdmin = true;

                        var result = await _authorizationService.CreateUser(user, _user.Password);

                        if (result.Any())
                        {
                            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.CanNotCreate; ;
                            Output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(_user.Language)); ;
                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "can not create user due to " + string.Join(",", result);
                            AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return Single(Output);
                        }
                    }

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
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.users.Select(x => x.Language).FirstOrDefault())); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/autoleas/get")]
        public async Task<IActionResult> GetUser(string id)
        {
            AutoleaseUserModel output = new AutoleaseUserModel();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Autolease Users";
            log.PageURL = "/admin/usersAutolease";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AutoleasingGetUser";
            log.ServiceRequest = $"userId = " + id;
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Empty Input Paramter, parameter userId";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(log.ErrorDescription);
                }

                AspNetUser user = await _authorizationService.GetUser(id);
                if (user == null)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "Can not find this user";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(log.ErrorDescription);
                }

                var outputuser = new AutoleaseUserModel() {
                    UserId = user.Id,
                    FullName = user.FullName,
                    BankId = user.AutoLeasingBankId.Value,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                return Single(outputuser);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/autoleas/getUsers")]
        public IActionResult GetUsers()
        {
            Output<AspNetUser> output = new Output<AspNetUser>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "AutoleasingGetAdminUsers";
            //log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            try
            {
                output.ResultList = _authorizationService.GetSuperAdmins();
                output.ErrorCode = Output<AspNetUser>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Single(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/autoleas/Edituser")]
        public IActionResult Edituser([FromBody] UserUpdateData user)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();

            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Autolease Users";
            log.PageURL = "/admin/usersAutolease";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AutoleasingEditAdminUser";
            log.ServiceRequest = JsonConvert.SerializeObject(user);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
             log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (user.Mobile != null)
                {
                    var IsUserExist = _authorizationService.GetUsers().Any(x => x.PhoneNumber == user.Mobile);
                    if (IsUserExist)
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(user.lang)), Field = "Mobile" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.InvalidData; ;
                        log.ErrorDescription = "mobile already exist";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(Output);
                    }
                }
                var userInfo = _authorizationService.GetUsers().Where(x => x.Id == user.UserId).FirstOrDefault();
                userInfo.FullName = user.FullName == null ? userInfo.FullName : user.FullName;
                userInfo.PhoneNumber = user.Mobile == null ? userInfo.PhoneNumber : user.Mobile;

                string exception = string.Empty;
                var result = _authorizationService.UpdateUserInfo(userInfo, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.lang)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(user.lang)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/autoleas/DeleteUser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PdfGenerationOutput>))]
        public IActionResult DeleteUser([FromBody] AutoleaseUserModel user)
        {
            PdfGenerationOutput Output = new PdfGenerationOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Autolease Users";
            log.PageURL = "/admin/usersAutolease";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AutoleasingDeleteAdminUser";
            log.ServiceRequest = JsonConvert.SerializeObject(user);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(user.UserId))
                {
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                    log.ErrorDescription = "user id is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var userInfo = _authorizationService.GetUsers().Where(x => x.Id == user.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    Output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse; ;
                    Output.ErrorDescription = "User not found";
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not found";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                userInfo.LockoutEnabled = user.IsLock;
                userInfo.LockoutEndDateUtc = user.IsLock ? new DateTime(3000, 12, 31) : DateTime.Now;

                string exception = string.Empty;
                var result = _authorizationService.UpdateUserInfo(userInfo, out exception);
                if (!string.IsNullOrEmpty(exception) || !result)
                {
                    Output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException; ;
                    Output.ErrorDescription = exception;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (userInfo.IsAutoLeasingSuperAdmin.HasValue && userInfo.IsAutoLeasingSuperAdmin.Value)
                {
                    var relatedUsers = _authorizationService.GetSuperAdminsRelated(userInfo.AutoLeasingAdminId);
                    if (relatedUsers != null && relatedUsers.Count > 0)
                    {
                        exception = string.Empty;
                        foreach (var _user in relatedUsers)
                        {
                            _user.LockoutEnabled = user.IsLock;
                            _user.LockoutEndDateUtc = user.IsLock ? new DateTime(3000, 12, 31) : DateTime.Now;
                            var lockuser = _authorizationService.UpdateUserInfo(_user, out exception);
                            if (!string.IsNullOrEmpty(exception) || !lockuser)
                            {
                                Output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException; ;
                                Output.ErrorDescription = exception;
                                log.ErrorCode = (int)Output.ErrorCode;
                                log.ErrorDescription = exception;
                                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                                return Error(Output);
                            }
                        }
                    }
                }

                Output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException; ;
                Output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Delete Driver Address (Physical Delete)
        /// </summary>
        /// <param name="id">driver ID</param>
        /// <returns></returns>
        // [Authorize]
        [HttpPost]
        [Route("api/autoleas/addBank")]
        public IActionResult AddBank([FromBody] Models.BankModel bank)
        {

            Output<BankOutput> Output = new Output<BankOutput>();
            Output.Result = new BankOutput();
            Output.Result.errors = new List<Error>();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "AddBank";
            log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            try
            {
                if (string.IsNullOrEmpty(bank.NameAr))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("EmptyNameAr", CultureInfo.GetCultureInfo(bank.Language)), Field = "NameAr" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Insured ID value is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                }
                if (string.IsNullOrEmpty(bank.NameEn))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("EmptyNameEn", CultureInfo.GetCultureInfo(bank.Language)), Field = "NameEn" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Insured ID value is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);

                }
                if (string.IsNullOrEmpty(bank.IBAN))
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("EmptyIBAN", CultureInfo.GetCultureInfo(bank.Language)), Field = "NameEn" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "IBAN value is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                }
                if (bank.bankInsuranceCompanies == null && bank.bankInsuranceCompanies.Count == 0)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("EmptyInsuranceCompany", CultureInfo.GetCultureInfo(bank.Language)), Field = "InsuranceCompany" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Insured ID value is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                }
                if (bank.bankNins == null && bank.bankNins.Count == 0)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("EmptyNin", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankNin" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Insured ID value is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                }

                //validation for Nins 
                foreach (var nin in bank.bankNins)
                {
                    // check if InsuredID is empty
                    if (string.IsNullOrEmpty(nin.ToString()))
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "Insured ID value is empty";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        //return Single(Output);
                    }

                    // check if InsuredID not number
                    foreach (char c in nin.ToString())
                    {
                        if (!char.IsDigit(c))
                        {
                            Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                            Output.Result.errors.Add(error);
                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "Insured ID value contains character";
                            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                            //return Single(Output);
                        }
                    }

                    // check if InsuredID begin with 7
                    if (!nin.ToString().StartsWith("7"))
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "Insured ID is invalid";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        //return Single(Output);
                    }

                    // check if InsuredID length not = 10 number
                    if (nin.ToString().Length != 10)
                    {
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                        Output.Result.errors.Add(error);

                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "Insured ID length is invalid";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        //return Single(Output);
                    }
                }

                var bankAdded = _banktService.AddBank(bank.NameAr, bank.NameEn, bank.IBAN, bank.NationalAddress, bank.PhoneNumber, bank.Email);
                if (bankAdded == null)
                {
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("InvalidBank", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankId" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "bank not added";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
                if (!_bankNinService.AddBankNin(bankAdded.Id, bank.bankNins))
                {
                    _banktService.DeleteBank(bankAdded);
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("InvalidBank", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankId" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "nins not added";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
                if (!_bankCompanyService.AddBankCompany(bankAdded.Id, bank.bankInsuranceCompanies))
                {
                    _bankNinService.DeleteBankNin(bankAdded.Id, bank.bankNins);
                    Error error = new Error() { Message = WebResources.ResourceManager.GetString("InvalidBank", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankId" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "nins not added";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }

                Output.ErrorCode = Output<BankOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<BankOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<BankOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(bank.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Error("an error has occured");
            }

        }

        [HttpPost]
        [Route("api/autoleas/editBank")]
        public IActionResult EditBank([FromBody] Models.BankModel bank)
        {

            Output<BankOutput> Output = new Output<BankOutput>();
            Output.Result = new BankOutput();
            Output.Result.errors = new List<Error>();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "EditBank";
            log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            try
            {
                var bankAdded = _banktService.GetBank(bank.Id);
                if (bank.bankNins != null && bank.bankNins.Count != 0)
                {
                    //validation for Nins 
                    foreach (var nin in bank.bankNins)
                    {
                        // check if InsuredID is empty
                        if (string.IsNullOrEmpty(nin.ToString()))
                        {
                            Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                            Output.Result.errors.Add(error);
                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "Insured ID value is empty";
                            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                            //return Single(Output);
                        }

                        // check if InsuredID not number
                        foreach (char c in nin.ToString())
                        {
                            if (!char.IsDigit(c))
                            {
                                Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                                Output.Result.errors.Add(error);
                                log.ErrorCode = (int)Output.ErrorCode;
                                log.ErrorDescription = "Insured ID value contains character";
                                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                                //return Single(Output);
                            }
                        }

                        // check if InsuredID begin with 7
                        if (!nin.ToString().StartsWith("7"))
                        {
                            Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                            Output.Result.errors.Add(error);
                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "Insured ID is invalid";
                            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                            //return Single(Output);
                        }

                        // check if InsuredID length not = 10 number
                        if (nin.ToString().Length != 10)
                        {
                            Error error = new Error() { Message = WebResources.ResourceManager.GetString("nin_not_valid", CultureInfo.GetCultureInfo(bank.Language)), Field = "NIN" };
                            Output.Result.errors.Add(error);

                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "Insured ID length is invalid";
                            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                            //return Single(Output);
                        }
                    }
                    var banNins = _bankNinService.GetBankNin(bank.Id);
                    if (!_bankNinService.DeleteBankNin(bank.Id, banNins))
                    {
                        // _banktService.DeleteBank(bankAdded);
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("InvalidBank", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankId" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "nins not added";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    }
                    _bankNinService.AddBankNin(bank.Id, bank.bankNins);
                }
                if (bank.bankInsuranceCompanies != null && bank.bankInsuranceCompanies.Count != 0)
                {
                    var bankCompanies = _bankCompanyService.GetBankCompanyIds(bank.Id);
                    if (!_bankCompanyService.DeleteBankCompany(bank.Id, bankCompanies))
                    {
                        //_bankNinService.DeleteBankNin(bankAdded.Id, bank.bankNins);
                        Error error = new Error() { Message = WebResources.ResourceManager.GetString("InvalidBank", CultureInfo.GetCultureInfo(bank.Language)), Field = "BankId" };
                        Output.Result.errors.Add(error);
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "nins not added";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    }
                    _bankCompanyService.AddBankCompany(bank.Id, bank.bankInsuranceCompanies);
                }
                Bank bankUpdated = new Bank();
                bankUpdated.NameAr = bank.NameEn;
                bankUpdated.NameEn = bank.NameEn;
                bankUpdated.IBAN = bank.IBAN;
                bankUpdated.PhoneNumber = bank.PhoneNumber;
                bankUpdated.Email = bank.Email;
                bankUpdated.NationalAddress = bank.NationalAddress;
                bankUpdated.Id = bank.Id;
                _banktService.EditBank(bankUpdated);

                Output.ErrorCode = Output<BankOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<BankOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<BankOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(bank.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Error("an error has occured");
            }

        }

        [HttpGet]
        [Route("api/autoleas/banks")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        public IActionResult GetBanks(string lang = "ar")
        {
            try
            {
                List<Bank> result = _banktService.GetBanks();
                IEnumerable<IdNamePairModel> dataModel = result.Select(e => new IdNamePairModel()
                {
                    Id = e.Id,
                    Name = (lang == "en") ? e.NameEn : e.NameAr
                });

                return Ok(dataModel, result.Count());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/autoleas/insuranceCompany")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        public IActionResult GetInsuranceCompany(string lang = "ar")
        {
            try
            {
                List<Tameenk.Core.Domain.Entities.InsuranceCompany> result = _insuranceCompanyService.GetAllinsuranceCompany().ToList();
                IEnumerable<IdNamePairModel> dataModel = result.Select(e => new IdNamePairModel()
                {
                    Id = e.InsuranceCompanyID,
                    Name = (lang == "en") ? e.NameEN : e.NameAR
                });

                return Ok(dataModel, result.Count());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }

        }

        public class LookUpTemp
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class UserUpdateData
        {
            [JsonProperty("id")]
            public string UserId { get; set; }

            [JsonProperty("lang")]
            public string lang { get; set; } = "en";

            [JsonProperty("fullName")]
            public string FullName { get; set; }

            [JsonProperty("phoneNumber")]
            public string Mobile { get; set; }

            [JsonProperty("isLock")]
            public bool IsLock { get; set; }
        }
    }
}

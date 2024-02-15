using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Corporate;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Orders;
using Tameenk.Services.Profile.Component.Membership;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Services.Profile.Component.Output;
using TameenkDAL.Models;
using Tamkeen.bll.Model;

namespace Tameenk.Services.Profile.Component
{
    public class CorporateContext : ICorporateContext
    {
        private ClientSignInManager _signInManager;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRepository<CorporateVerifyUsers> _corporateVerifyUsersRepository;
        private readonly INotificationService _notificationService;
        private readonly IRepository<CorporateAccount> _corporateAccountRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IVehicleService _vehicleService;

        public CorporateContext(IRepository<CorporateUsers> corporateUsersRepository, IShoppingCartService shoppingCartService,
            IRepository<CorporateVerifyUsers> corporateVerifyUsersRepository, INotificationService notificationService,
            IRepository<CorporateAccount> corporateAccountRepository,
            IAuthorizationService authorizationService, IVehicleService vehicleService)
        {
            _corporateUsersRepository = corporateUsersRepository;
            _shoppingCartService = shoppingCartService;
            _corporateVerifyUsersRepository = corporateVerifyUsersRepository;
            _notificationService = notificationService;
            _corporateAccountRepository = corporateAccountRepository;
            _authorizationService = authorizationService;
            _vehicleService = vehicleService;
        }

        public async Task<ProfileOutput<LoginOutput>> CorporateUserSignIn(AspNetUser user, string password, string lang, LoginRequestsLog predefinedLog)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            DateTime startTime = DateTime.Now;

            try
            {
                var corporateUser = _corporateUsersRepository.TableNoTracking.Where(a => a.UserName == user.Email).FirstOrDefault();
                if (corporateUser == null)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(lang)); ;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = "CorporateUser is null for email " + user.Email;
                    predefinedLog.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(predefinedLog);
                    return output;
                }

                if (!SecurityUtilities.VerifyHashedData(corporateUser.PasswordHash, password))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(lang)); ;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = "incorrect username and password asp net user " + password + " and hashed password is " + corporateUser.PasswordHash;
                    predefinedLog.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(predefinedLog);
                    return output;
                }

                string exception;
                var Otp = GenerateRendomCode();
                CorporateVerifyUsers verifyData = new CorporateVerifyUsers();
                verifyData.CreatedDate = DateTime.Now;
                verifyData.UserId = corporateUser.UserId;
                verifyData.VerificationCode = Otp.ToString();
                verifyData.ExpiryDate = DateTime.Now.AddMinutes(15);
                verifyData.MethodName = "CorporatePortalLogin";
                _corporateVerifyUsersRepository.Insert(verifyData);
                string smsMessage = CorporateResources.ResourceManager.GetString("OtpSmsBody_Portal", CultureInfo.GetCultureInfo(lang)) + Otp.ToString();
                var smsModel = new SMSModel()
                {
                    PhoneNumber = corporateUser.PhoneNumber,
                    MessageBody = smsMessage,
                    Method = SMSMethod.WebsiteOTP.ToString(),
                    Module = Module.Corporate.ToString(),
                    Channel = predefinedLog.Channel
                };
                var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);

                if (smsOutput.ErrorCode != 0)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.CanNotSendSMS;
                    output.ErrorDescription = WebResources.ErrorGeneric;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = "Can Not Send SMS due to" + smsOutput.ErrorDescription;
                    predefinedLog.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(predefinedLog);
                }

                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = "Success";
                predefinedLog.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                LoginRequestsLogDataAccess.AddLoginRequestsLog(predefinedLog);

                output.Result = new LoginOutput() {
                    UserId = corporateUser.UserId,
                    IsCorporateUser = true,
                    IsCorporateAdmin = (corporateUser.IsSuperAdmin.HasValue && corporateUser.IsSuperAdmin.Value) ? true : false,
                    DisplayNameAr = user.FullNameAr?.Split(' ')?[0],
                    DisplayNameEn = user.FullName?.Split(' ')?[0],
                };
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = CorporateResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang)); ;
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = exp.ToString();
                LoginRequestsLogDataAccess.AddLoginRequestsLog(predefinedLog);
                return output;
            }
        }

        public CorporateUsers GetCorporateUserByUserId(string userId)
        {
            return _corporateUsersRepository.TableNoTracking.FirstOrDefault(u => u.UserId == userId && u.IsActive);
        }

        public CorporateAccount GetCorporateAccountById(int accountId)
        {
            return _corporateAccountRepository.TableNoTracking.FirstOrDefault(c => c.Id == accountId && c.IsActive == true);
        }

        public async Task<ProfileOutput<bool>> AddCorporateUser(AddCorporateUserModel model)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>();
            output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "AddCorporateUser";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("EmptyEmail", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Email is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.FullName))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("EmptyName", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Password is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("EmptyPhoneNumber", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Phone Number is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("EmptyPassword", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Password is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.RePassword))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("EmptyRePassword", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "RePassword is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (model.Password != model.RePassword)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CorporateResources.ResourceManager.GetString("PasswordNotMatched", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "RePassword is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var userVal = _authorizationService.IsEmailOrPhoneExist(model.Email, model.PhoneNumber);
                if (userVal != null && userVal.Email.ToLower() == model.Email.ToLower())
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "email already exist";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (userVal != null && userVal.PhoneNumber == model.PhoneNumber)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "mobile already exist";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
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
                user.RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB");
                user.LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB");
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
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.CanNotCreate;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "can not create user due to " + string.Join(",", result);
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                //log.ErrorCode = 0;
                //log.ErrorDescription = $"user.Id: {user.Id}, model.AccountId: {model.AccountId}, model.Password:{model.Password}";
                //ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);

                CorporateUsers corporateUser = new CorporateUsers();
                corporateUser.UserId = user.Id;
                corporateUser.UserName = user.Email;
                corporateUser.PhoneNumber = user.PhoneNumber;
                corporateUser.PasswordHash = SecurityUtilities.HashData(model.Password, null);
                corporateUser.IsActive = true;
                corporateUser.CreatedDate = DateTime.Now;
                corporateUser.CorporateAccountId = model.AccountId;
                corporateUser.IsSuperAdmin = false;
                _corporateUsersRepository.Insert(corporateUser);

                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        public CorporatePoliciesOutput GetAllCorporatePolicies(int corporateAccountId, CorporatePoliciesFilter policyFilter, string from, string to,string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            var output = new CorporatePoliciesOutput();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                //if (string.IsNullOrEmpty(userId))
                //{
                //    output.ErrorCode = CorporatePoliciesOutput.ErrorCodes.InvalidInput;
                //    output.ErrorDescription = "Invalid Input";
                //    return output;
                //}
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllCorporatePolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "corporateAccountId", Value = corporateAccountId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "from", Value = from });
                command.Parameters.Add(new SqlParameter() { ParameterName = "to", Value = to });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageNumber", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });
                if (policyFilter != null)
                {
                    if (!string.IsNullOrEmpty(policyFilter.PolicyNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "policyNumber", Value = policyFilter.PolicyNumber });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.InsuredNIN))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@insuredNIN", Value = policyFilter.InsuredNIN });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.SequenceOrCustomCardNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "sequenceOrCustomCardNumber", Value = policyFilter.SequenceOrCustomCardNumber });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<MyPoliciesDB> myPoliciesDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MyPoliciesDB>(reader).ToList();

                if (myPoliciesDB != null)
                {
                    reader.NextResult();
                    var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    output.PoliciesTotalCount = totalCount;
                    dbContext.DatabaseInstance.Connection.Close();

                    var Makers = _vehicleService.VehicleMakers();

                    output.PoliciesList = new List<CorporatePolicyModel>();
                    CorporatePolicyModel policyModel;
                    Vehicle vehicle;
                    foreach (var item in myPoliciesDB)
                    {
                        policyModel = new CorporatePolicyModel();
                        policyModel.PolicyFileId = item.PolicyFileId;
                        policyModel.PolicyNo = item.PolicyNo;
                        policyModel.InsuranceCompanyName = lang == "ar" ? item.InsuranceCompanyNameAR : item.InsuranceCompanyNameEN;
                        policyModel.PolicyStatusName = lang == "ar" ? item.PolicyStatusNameAr : item.PolicyStatusNameEn;
                        policyModel.PolicyEffectiveDate = item.PolicyEffectiveDate;
                        policyModel.PolicyExpiryDate = item.PolicyExpiryDate;
                        policyModel.CheckOutDetailsIsEmailVerified = item.CheckOutDetailsIsEmailVerified;
                        policyModel.CheckOutDetailsEmail = item.CheckOutDetailsEmail;
                        policyModel.CheckOutDetailsId = item.CheckOutDetailsId;
                        policyModel.NajimStatus = lang == "ar" ? item.NajmStatusNameAr : item.NajmStatusNameEn;
                        policyModel.PolicyStatusKey = item.PolicyStatusKey;
                        policyModel.ExternalId = item.ExternalId;
                        policyModel.SequenceNumber = item.SequenceNumber;
                        policyModel.CustomCardNumber = item.CustomCardNumber;
                        policyModel.PaymentMethod = item.Name;

                        policyModel.PolicyIssueDate = item.PolicyIssueDate;
                        policyModel.IBAN = item.IBAN;
                        policyModel.InsuredFullNameAr = item.InsuredFullNameAr;
                        policyModel.Phone = item.Phone;
                        policyModel.DriverNIN = item.DriverNIN;
                        policyModel.UserAccountEmail = item.CorporateUserEmail;
                        policyModel.TotalPremium = item.TotalPremium;
                        policyModel.QuotationPrice = item.QuotationPrice;
                        policyModel.PolicyPrice = item.PolicyPrice;
                        policyModel.PaidAmount = item.PaidAmount;
                        policyModel.BenfitsPrice = item.BenfitsPrice;
                        policyModel.InsuranceType = item.InsuranceType;

                        vehicle = new Vehicle()
                        {
                            CarPlateNumber = item.CarPlateNumber,
                            CarPlateText1 = item.CarPlateText1,
                            CarPlateText2 = item.CarPlateText2,
                            CarPlateText3 = item.CarPlateText3,
                            PlateTypeCode = item.PlateTypeCode,
                            VehicleMaker = item.VehicleMaker,
                            VehicleMakerCode = item.VehicleMakerCode,
                            VehicleModel = item.VehicleModel,
                            VehicleModelCode = item.VehicleModelCode,
                            ModelYear = item.ModelYear,

                        };
                        policyModel.VehicleModelName = GetVehicleModelLocalization(lang, vehicle);
                        //policyModel.VehiclePlate = GetVehiclePlateModel(vehicle);

                        output.PoliciesList.Add(policyModel);
                    }
                }

                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();

                output.ErrorCode = CorporatePoliciesOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                output.ErrorCode = CorporatePoliciesOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public ProfileOutput<bool> ManageLockCorporateUser(ManageLockCorporateUserModel model)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>();
            output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "ManageLockCorporateUser";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(model.CorporateUserId))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "user id is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                var userInfo = _corporateUsersRepository.Table.Where(x => x.UserId == model.CorporateUserId && x.CorporateAccountId == model.CorporateAccountId).FirstOrDefault();
                if (userInfo == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.UserNotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("UserNotExist", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "user not found in CorporateUsers with this id " + model.CorporateUserId;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (userInfo.IsActive == model.Lock)
                {
                    output.ErrorCode = model.Lock ? ProfileOutput<bool>.ErrorCodes.AccountAlreadyLocked : ProfileOutput<bool>.ErrorCodes.AccountAlreadyUnLocked;
                    output.ErrorDescription = model.Lock? CorporateResources.ResourceManager.GetString("AccountAlreadyLocked", CultureInfo.GetCultureInfo(model.Language)):
                        CorporateResources.ResourceManager.GetString("AccountAlreadyUnLocked", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                if (!userInfo.IsActive)
                {
                    var corporateAccount = GetCorporateAccountById(model.CorporateAccountId);
                    if (corporateAccount == null)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NotFound;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "this user's account not found in CorporateAccounts with this account id " + userInfo.CorporateAccountId;
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                    if (!corporateAccount.IsActive.HasValue || !corporateAccount.IsActive.Value)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NotAuthorized;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "can not activate this user because his account is deActivate";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                }

                userInfo.IsActive = model.Lock;
                _corporateUsersRepository.Update(userInfo);

                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = model.Lock ? CorporateResources.ResourceManager.GetString("AccountLockedSuccessfully", CultureInfo.GetCultureInfo(model.Language)) :
                    CorporateResources.ResourceManager.GetString("AccountUnLockedSuccessfully", CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        public async Task<ProfileOutput<VerifyCorporateOTPOutput>> VerifyCorporateOneTimePassword(string email, string otpCode, Channel channel, string lang)
        {
            ProfileOutput<VerifyCorporateOTPOutput> output = new ProfileOutput<VerifyCorporateOTPOutput>();
            output.Result = new VerifyCorporateOTPOutput();
            DateTime startTime = DateTime.Now;
            LoginRequestsLog log = new LoginRequestsLog();
            log.Email = email;
            log.Channel = channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "Email is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(otpCode))
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "OTP is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }

                //var encryptedEmail = Convert.FromBase64String(email.Trim());
                //var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                //email = plainEmail;

                var user = _authorizationService.GetUserByEmail(email);
                if (user == null)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "Login data not correct";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                var corporateUser = _corporateUsersRepository.TableNoTracking.Where(x => x.UserName == email).FirstOrDefault();
                if (corporateUser == null)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "user not exist in corporate users ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }

                output.Result.CorporateUser = corporateUser;

                var otp = _corporateVerifyUsersRepository.Table.Where(x => x.UserId == user.Id && x.MethodName == "CorporatePortalLogin").OrderByDescending(x => x.Id).FirstOrDefault();
                if (otp == null)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "OTP not found";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                if (otpCode.Trim() != otp.VerificationCode)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "OTP doesn't match as we recieved " + otpCode.Trim() + " and otp code is " + otp.VerificationCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                if (otp.ExpiryDate < DateTime.Now)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "OneTime Password Expired";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }

                otp.ModifiedDate = DateTime.Now;
                otp.IsVerified = true;
                _corporateVerifyUsersRepository.Update(otp);

                output.Result.AccessTokenResult = _authorizationService.GetAccessToken(user.Id);
                if (output.Result == null)
                {
                    output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.InValidResponse;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "Failed to generate access token";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                
                output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                MigrateUser(corporateUser.UserId);
                SignInManager.SignIn(user, true, true);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang)); ;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                return output;
            }
        }
        private int GenerateRendomCode()
        {
            Random rnd = new Random();
            int code = rnd.Next(1000, 9999);
            return code;
        }

        #region Helper

        public ClientSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ClientSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private void MigrateUser(string userId)
        {
            var anonymousId = HttpContext.Current.Request.AnonymousID;
            // var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
            if (string.IsNullOrWhiteSpace(anonymousId)) return;

            if (string.IsNullOrWhiteSpace(userId)) return;

            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
        }

        private string GetVehicleModelLocalization(string lang, Tameenk.Core.Domain.Entities.VehicleInsurance.Vehicle vehicle)
        {
            var Makers = _vehicleService.VehicleMakers();

            var maker = vehicle.VehicleMakerCode.HasValue ?
                Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
                 Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);

            string modelName = string.Empty;
            string makerName = string.Empty;

            if (maker != null)
            {
                var Models = _vehicleService.VehicleModels(maker.Code);

                if (Models != null)
                {
                    var model = vehicle.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == vehicle.VehicleModelCode) :
                        Models.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleModel || m.EnglishDescription == vehicle.VehicleModel);

                    if (model != null)
                        modelName = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
                }

                makerName = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);

            }
            return modelName + " " + makerName + " " + vehicle.ModelYear;
        }
        private VehiclePlateModel GetVehiclePlateModel(Vehicle vehicle)
        {

            VehiclePlateModel plateModel = new VehiclePlateModel()
            {
                CarPlateNumber = vehicle.CarPlateNumber.HasValue ? vehicle.CarPlateNumber : 0,
                CarPlateText1 = (string.IsNullOrEmpty(vehicle.CarPlateText1) || string.IsNullOrWhiteSpace(vehicle.CarPlateText1))
                    ? "" : vehicle.CarPlateText1,
                CarPlateText2 = (string.IsNullOrEmpty(vehicle.CarPlateText2) || string.IsNullOrWhiteSpace(vehicle.CarPlateText2))
                    ? "" : vehicle.CarPlateText2,
                CarPlateText3 = (string.IsNullOrEmpty(vehicle.CarPlateText3) || string.IsNullOrWhiteSpace(vehicle.CarPlateText3))
                    ? "" : vehicle.CarPlateText3
            };
            CarPlateInfo carPlateInfo = new CarPlateInfo(plateModel.CarPlateText1, plateModel.CarPlateText2, plateModel.CarPlateText3, (int)plateModel.CarPlateNumber);
            plateModel.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
            plateModel.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
            plateModel.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
            plateModel.CarPlateTextEn = carPlateInfo.CarPlateTextEn;
            plateModel.PlateColor = _vehicleService.GetPlateColor(vehicle.PlateTypeCode);
            return plateModel;
        }

        #endregion
    }
}

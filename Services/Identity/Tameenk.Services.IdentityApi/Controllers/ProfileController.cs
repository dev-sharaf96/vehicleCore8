using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using TameenkDAL.Models;
using TameenkDAL.Models.Notifications;
using TameenkDAL.UoW;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services;
using System.Data.Entity;
using Tamkeen.bll.Business;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Extensions;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Core.Promotions;
using Tameenk.Core.Domain.Enums;
using Tameenk.Api.Core;
using Tameenk.Loggin.DAL;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Tameenk.Services.IdentityApi.Output;
using Tameenk.Api.Core.Models;
using Tameenk.Security.Services;
using Tameenk.Models;
using System.Reflection;
using Tameenk.Common.Utilities;
using TameenkDAL;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Security.Encryption;
using Newtonsoft.Json;
using Tameenk.Core.Configuration;
using System.Globalization;
using System.Web;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.UserTicket.Components;
using Tameenk.Resources.UserTicket;
using Tameenk.Services.Profile.Component;
using System.Net.Http;
using Tameenk.Services.Profile.Component.Output;
using Tameenk.Resources.Promotions;
using System.IO;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core.Addresses;
using Tameenk.Resources.Profile;
using Tameenk.Services.Core;
using Tameenk.Resources.WebResources;
using Microsoft.AspNet.Identity.Owin;
using Tameenk.Services.IdentityApi.App_Start;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Security.CustomAttributes;
using Tameenk.Redis;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.IdentityApi.Controllers
{
    enum ProfileType
    {
        Info = 0,
        Statistics = 1,
        Notifications = 2,
        Purchases = 3,
        Addresses = 4,
        BankAccounts = 5,
        Policies = 6,
        Vehicles = 7,
    }
    //[TameenkAuthorizeAttribute]
    [SingleSessionAuthorizeAttribute]
    public class ProfileController : IdentityBaseController
    {
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";
        private ProfileRequestsLog profileLog;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserProfileService policySer;
        private readonly CheckoutBusiness _checkoutBusiness;
        private readonly IPolicyService _policyService;
        private readonly IRepository<PolicyUpdatePayment> _policyUpdPaymentRepository;
        private readonly ITameenkUoW _tameenkUoW;
        private readonly INotificationService _notificationService;
        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IRepository<PolicyUpdateRequest> _policyUpdReqRepository;
        private readonly IRepository<NCDFreeYear> nCDFreeYearRepository;
        private readonly Random _rnd;
        private readonly IPromotionService _promotionService;
        private readonly IVehicleService _vehicleService;
        TameenkDbContext _db = new TameenkDbContext();
        private readonly TameenkConfig _tameenkConfig;
        private readonly IProfileContext _profileContext;
        private readonly IUserTicketContext _userTicketContext;
        private readonly IPromotionContext _promotionContext;
        private readonly ICheckoutContext _checkoutContext;        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";        private readonly IAddressService _addressService;

        private readonly ICorporateContext _corporateContext;
        private readonly ICorporateUserService _corporateUserService;
        private readonly IRepository<CorporateUsers> _corporateUserRepository;
        private readonly Tameenk.Services.Profile.Component.IAuthenticationContext _authenticationContext;
        private readonly IExcelService _excelService;

        private readonly string base_KEY = "iDeNtItY_cAcH";
        private readonly string SHARED_PARTIAL_LOCK_SECRET = "(hrd8Af#qtuZ!93myA%5t^4uDU$A+zx!";

        #region The Ctro
        public ProfileController(ITameenkUoW tameenkUoW, ILogger logger, INotificationService notificationService
            , IPolicyService policyService
            , IRepository<PolicyUpdatePayment> policyUpdPaymentRepository
            , IPayfortPaymentService payfortPaymentService
            , IRepository<PolicyUpdateRequest> policyUpdReqRepository
            , IRepository<Tameenk.Core.Domain.Entities.NCDFreeYear> NCDFreeYearRepository
            , IVehicleService vehicleService
            , IPromotionService promotionService,
            IAuthorizationService authorizationService,
            TameenkConfig tameenkConfig,
            IProfileContext profileContext,
            IUserTicketContext userTicketContext,
            IPromotionContext promotionContext,
            ICheckoutContext checkoutContext,
            IAddressService addressService,
            ICorporateContext corporateContext, ICorporateUserService corporateUserService,
            IRepository<CorporateUsers> corporateUserRepository,
            Tameenk.Services.Profile.Component.IAuthenticationContext authenticationContext,
            IExcelService excelService
            )
        {
            _tameenkUoW = tameenkUoW;
            _notificationService = notificationService;
            _vehicleService = vehicleService;
            policySer = new UserProfileService(tameenkUoW, _vehicleService);
            _policyService = policyService;
            _policyUpdPaymentRepository = policyUpdPaymentRepository;
            _checkoutBusiness = new CheckoutBusiness(tameenkUoW);
            _payfortPaymentService = payfortPaymentService;
            _policyUpdReqRepository = policyUpdReqRepository;
            nCDFreeYearRepository = NCDFreeYearRepository;
            _rnd = new Random(System.Environment.TickCount);
            _promotionService = promotionService;
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _userTicketContext = userTicketContext;
            _profileContext = profileContext;
            _promotionContext = promotionContext;
            _checkoutContext = checkoutContext;
            _addressService = addressService;

            _corporateContext = corporateContext;
            _corporateUserService = corporateUserService;
            _corporateUserRepository = corporateUserRepository;
            _authenticationContext = authenticationContext;
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
        }
        #endregion



        #region Action Methods

        // GET: Profile
        [HttpGet]
        [Route("api/profile/index")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetProfile(string userId, string channel, string language)
        {
            Output<TameenkDAL.Models.UserProfileData> output = new Output<TameenkDAL.Models.UserProfileData>();
            output.Result = new TameenkDAL.Models.UserProfileData();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetProfile";
            profileLog.Channel = channel;
            // check if user exist 
            //  var user = await _userManager.FindAsync(userName, password);
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);

                }
                //if (string.IsNullOrEmpty(password))
                //{
                //    return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);

                //}
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new TameenkDAL.Models.UserProfileData();
                    output.Result = policySer.GetUserProfileData(userId, 0, true);
                    output.Result.Notifications = GetUserNotifications(userId, false);
                    //output.Result.PromotionProgramName = GetUserJoinedPromotionProgramName(userId);
                    output.Result.PromotionProgramName = _promotionService.GetPromotionProgramUserNew(userId);
                    output.ErrorCode = Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.Success;
                    if (output.Result != null && output.Result.VehiclesList.Count > 0)
                    {
                        for (int i = 0; i < output.Result.VehiclesList.Count; i++)
                        {
                            var VehicleMakerCode = output.Result.VehiclesList[i].VehicleMakerCode.Value.ToString("D4");
                            output.Result.VehiclesList[i].CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg";
                        }
                    }
                    return Single(output);
                }
                else
                {
                    return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.ExceptionError, "ExceptionError", language);
            }
            #endregion

        }


        [HttpGet]
        [Route("api/profile/profileByType")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetProfileByType(string userId, int profileTypeId, string channel, string language)
        {
            Output<TameenkDAL.Models.UserProfileData> output = new Output<TameenkDAL.Models.UserProfileData>();
            output.Result = new TameenkDAL.Models.UserProfileData();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetProfile";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new TameenkDAL.Models.UserProfileData();
                    output.Result = policySer.GetUserProfileData(userId, profileTypeId, true);
                    if (profileTypeId == (int)ProfileType.Notifications)
                    {
                        output.Result.Notifications = GetUserNotifications(userId, false);
                    }
                    if (profileTypeId == (int)ProfileType.Info)
                    {
                        //output.Result.PromotionProgramName = GetUserJoinedPromotionProgramName(userId);
                        output.Result.PromotionProgramName = _promotionService.GetPromotionProgramUserNew(userId);
                    }
                    if (profileTypeId == (int)ProfileType.Vehicles)
                    {
                        if (output.Result != null && output.Result.VehiclesList.Count > 0)
                        {
                            for (int i = 0; i < output.Result.VehiclesList.Count; i++)
                            {
                                var VehicleMakerCode = output.Result.VehiclesList[i].VehicleMakerCode.Value.ToString("D4");
                                output.Result.VehiclesList[i].CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg";
                            }
                        }
                    }
                    output.ErrorCode = Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return Single(output);
                }
                else
                {
                    return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                return OutputHandler<UserProfileData>(output, profileLog, Output<TameenkDAL.Models.UserProfileData>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }

        }


        // GET: Profile
        [HttpGet]
        [Route("api/profile/delete-vehicle")]
        public async Task<IHttpActionResult> DeleteVehicle(string userId, string vehicleId, string channel, string language)
        {
            Output<bool> output = new Output<bool>();
            output.Result = false;
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "DeleteVehicle";
            profileLog.Channel = channel;
            profileLog.CreatedDate = DateTime.Now;

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    profileLog.ErrorDescription = "EmptyInputParameter userId is null";
                    return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                if (string.IsNullOrEmpty(vehicleId))
                {
                    profileLog.ErrorDescription = "EmptyInputParameter vehicleID is null";
                    return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }

                DateTime dateTime = DateTime.Now;

                int canDeleteVehicle = (from p in _db.Policies
                                        join ch in _db.CheckoutDetails on p.CheckOutDetailsId equals ch.ReferenceId
                                        join v in _db.Vehicles on ch.VehicleId equals v.ID
                                        where p.PolicyExpiryDate <= dateTime && v.ID.ToString() == vehicleId
                                        select ch.Vehicle).Count();

                if (canDeleteVehicle == 0)
                {
                    //get the obj from db and if it exist then delete it
                    var vehicle = _db.Vehicles.Where(x => x.ID == new Guid(vehicleId)).FirstOrDefault();
                    if (vehicle != null)
                    {
                        try
                        {
                            vehicle.IsDeleted = true;
                            // _db.Entry(vehicle).State = EntityState.Modified;
                            _db.SaveChanges();
                            output.ErrorCode = Output<bool>.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.Result = true;
                            return Single(output);
                        }
                        catch (Exception exp)
                        {
                            profileLog.ErrorDescription = "ExceptionError : " + exp.ToString();
                            return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.ExceptionError, "ExceptionError", language);
                        }
                    }
                    else
                    {
                        profileLog.ErrorDescription = "EmptyInputParamter vehicle object is null";
                        return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.EmptyInputParamter, "EmptyInputParamter", language);
                    }

                }
                else
                {
                    profileLog.ErrorDescription = "Not Allowed user can't delete a vehicle ";
                    return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                profileLog.ErrorDescription = ex.ToString();
                return OutputHandler<bool>(output, profileLog, Output<bool>.ErrorCodes.ExceptionError, "ExceptionError", language);
            }

        }


        [HttpGet]
        [Route("api/profile/DownloadPolicyFile")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> DownloadPolicyFile(string fileId, string language, string channel = "Web")
        {
            Output<string> output = new Output<string>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "DownloadPolicyFile";
            profileLog.Channel = channel;
            try
            {
                if (string.IsNullOrEmpty(fileId))
                {
                    return OutputHandler<string>(output, profileLog, Output<string>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", "en");
                }
                var policyFile = _policyService.DownloadPolicyFile(fileId);
                if (policyFile != null)
                {
                    if (policyFile.PolicyFileByte != null)
                    {
                        output.Result = Convert.ToBase64String(policyFile.PolicyFileByte);
                        output.ErrorCode = Output<string>.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        return Single(output);
                    }
                    else if (!string.IsNullOrEmpty(policyFile.FilePath))
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();
                        string exception = string.Empty;
                        if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            var file = fileShare.GetFile(policyFile.FilePath, out exception);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            if (file != null)
                            {
                                output.Result = Convert.ToBase64String(file);
                                output.ErrorCode = Output<string>.ErrorCodes.Success;
                                output.ErrorDescription = "Success";
                                return Single(output);
                            }
                        }
                        else
                        {
                            exception = string.Empty;
                            var file = File.ReadAllBytes(policyFile.FilePath);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            if (file != null)
                            {
                                output.Result = Convert.ToBase64String(file);
                                output.ErrorCode = Output<string>.ErrorCodes.Success;
                                output.ErrorDescription = "Success";
                                return Single(output);
                            }
                        }
                    }
                }
                return OutputHandler<string>(output, profileLog, Output<string>.ErrorCodes.NotFound, "NotFound", "en");

            }
            catch (Exception ex)
            {
                return OutputHandler<string>(output, profileLog, Output<string>.ErrorCodes.ExceptionError, "ExceptionError", "en", ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/profile/DownloadInvoiceFilePDF")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> DownloadInvoiceFilePDF(string fileId, string language, string channel = "Web")
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            var output = _profileContext.DownloadInvoiceFilePDF(fileId, language, userId, channel);
            return Single(output);
        }

        [HttpGet]
        [Route("api/profile/GetQuotationRequestDetail")]
        public IHttpActionResult GetQuotationRequestDetail(string userId, string culture, string channel = "Web")
        {
            Output<List<QuotationRequestDetailsModel>> output = new Output<List<QuotationRequestDetailsModel>>();
            output.Result = new List<QuotationRequestDetailsModel>();

            profileLog = new ProfileRequestsLog();
            profileLog.Method = "GetQuotationRequestDetail";
            profileLog.Channel = channel;
            profileLog.ServerIP = Utilities.GetInternalServerIP();
            profileLog.UserIP = Utilities.GetUserIPAddress();
            profileLog.UserAgent = Utilities.GetUserAgent();

            Guid currentUserId = Guid.Empty;
            Guid.TryParse(userId, out currentUserId);
            profileLog.UserID = currentUserId;

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = Output<List<QuotationRequestDetailsModel>>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "userId can't be null";
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = output.ErrorDescription;
                    return OutputHandler(output, profileLog, Output<List<QuotationRequestDetailsModel>>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", culture, output.ErrorDescription);

                }
                //get the quotation requests from db
                List<QuotationRequest> quotationRequestList = _tameenkUoW.PolicyRepository.GetQuotationRequestsByUSerId(userId);
                if (quotationRequestList == null)
                {
                    output.ErrorCode = Output<List<QuotationRequestDetailsModel>>.ErrorCodes.NotFound;
                    output.ErrorDescription = "Result is null";
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = output.ErrorDescription;
                    return OutputHandler(output, profileLog, Output<List<QuotationRequestDetailsModel>>.ErrorCodes.NotFound, "NotFound", culture, output.ErrorDescription);
                }

                foreach (var item in quotationRequestList)
                {
                    var model = new QuotationRequestDetailsModel()
                    {
                        ExternalId = item.ExternalId,
                        VehicleMakerCode = item.Vehicle.VehicleMakerCode.ToString(),
                        VehicleMaker = item.Vehicle.VehicleMaker,
                        VehicleModel = item.Vehicle.VehicleModel,
                        DriverFirstName = item.Driver.FirstName,
                        VehicleModelYear = item.Vehicle.ModelYear,
                        CityArabicDescription = item.City.ArabicDescription,
                        CreatedDateTime = item.CreatedDateTime,
                        VehiclePlate = GetVehiclePlateModel(item.Vehicle),
                        RemainingTimeToExpireInSeconds = item.CreatedDateTime.AddHours(16).Subtract(DateTime.Now).TotalSeconds,
                        CarImage = item.Vehicle.VehicleMakerCode.Value.ToString("D4")
                    };
                    output.Result.Add(model);
                }

                output.ErrorCode = Output<List<QuotationRequestDetailsModel>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                profileLog.ErrorCode = (int)output.ErrorCode;
                profileLog.ErrorDescription = output.ErrorDescription;
                return OutputHandler(output, profileLog, Output<List<QuotationRequestDetailsModel>>.ErrorCodes.Success, "Success", culture, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<List<QuotationRequestDetailsModel>>.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                profileLog.ErrorCode = (int)output.ErrorCode;
                profileLog.ErrorDescription = output.ErrorDescription;
                return OutputHandler<List<QuotationRequestDetailsModel>>(output, profileLog, Output<List<QuotationRequestDetailsModel>>.ErrorCodes.ExceptionError, "ExceptionError", culture, ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/profile/PayPolicyUpdRequestUsingPayfort")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public IHttpActionResult PayPolicyUpdRequestUsingPayfort(string policyUpdReqReferenceId, string culture, string channel = "Web")
        {
            Output<string> output = new Output<string>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "PayPolicyUpdRequestUsingPayfort";
            profileLog.Channel = channel;
            if (string.IsNullOrEmpty(policyUpdReqReferenceId))
            {
                return OutputHandler(output, profileLog, Output<string>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", culture);

            }
            try
            {
                //get policy update payment form db
                //also include checkoutDetails of this policy to get some info Ex: userName,userId, driver data
                var policyUpdPayment = _policyUpdPaymentRepository.Table
                    .Include(e => e.PolicyUpdateRequest.Policy.CheckoutDetail)
                    .Include(e => e.PolicyUpdateRequest.Policy.CheckoutDetail.Driver)
                    .FirstOrDefault(x => x.PolicyUpdateRequest.Guid == policyUpdReqReferenceId);
                if (policyUpdPayment == null)
                {
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.NotFound, "PolicyUpdPay", culture);
                }
                //before process payment check if the policy update request within 16 hours, if no then don't process
                //if user didnt pay the payment within 16 hour then the status become Expired
                if (policyUpdPayment.PolicyUpdateRequest.Status == PolicyUpdateRequestStatus.Expired)
                {
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.NotFound, "PolicyReqExp", culture);
                }
                var checkoutDetail = GetCheckoutDetailFromPolicyUpdPayment(policyUpdPayment);
                if (checkoutDetail == null)
                {
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.NotFound, "EmptyPolicyCheckDetail", culture);
                }
                var paymentRequestModel = _checkoutBusiness.GetPolicyUpdReqPayment(policyUpdReqReferenceId, checkoutDetail, policyUpdPayment.Amount);
                paymentRequestModel.RequestId = "04-" + paymentRequestModel.InvoiceNumber.ToString() + "-" + _rnd.Next(111111, 999999); ;

                //craete payfort payment request
                int paymentRequestId = CreatePolicyUpdPayfortRequest(paymentRequestModel, policyUpdReqReferenceId);
                output.Result = paymentRequestId.ToString();
                return OutputHandler(output, profileLog, Output<string>.ErrorCodes.Success, "Success", culture);
            }
            catch (Exception ex)
            {
                return OutputHandler(output, profileLog, Output<string>.ErrorCodes.ExceptionError, "ExceptionError", culture, ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/profile/PolicyUpdPayfortResult")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public IHttpActionResult PolicyUpdPayfortResult(PayfortResponse payfortResponse)
        {
            Output<string> output = new Output<string>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "PolicyUpdPayfortResult";
            try
            {
                var res = payfortResponse.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
              .ToDictionary(prop => prop.Name, prop => prop.GetValue(payfortResponse, null));

                var result = res.ToDictionary(k => k.Key, k => k.Value == null ? "" : k.Value.ToString());
                if (!_payfortPaymentService.ValidatedResponse(result))
                {
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.InValidResponse, "InValidResponse", "en");

                }
                var response = _payfortPaymentService.BuildPayfortPaymentResponse(payfortResponse);
                if (response != null)
                {
                    _payfortPaymentService.ProcessPolicyUpdPayment(payfortResponse.merchant_reference, response);
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.Success, "Success", "en");
                }
                else
                {
                    return OutputHandler(output, profileLog, Output<string>.ErrorCodes.ServiceException, "SerivceIsCurrentlyDown", "en");
                }

            }
            catch (Exception ex)
            {
                return OutputHandler(output, profileLog, Output<string>.ErrorCodes.ExceptionError, "ExceptionError", "en", ex.ToString());
            }
        }



        /// <summary>
        /// Get promotion programs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Promotions/promotion-programs")]
        public IHttpActionResult GetPromotionPrograms(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                Output<List<PromotionProgram>> output = new Output<List<PromotionProgram>>();
                var programs = _promotionService.GetPromotionPrograms();
                // output.Result = programs.;
                return Single(programs);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/profile/userpromotion-programs")]
        public IHttpActionResult GetUserPromotionPrograms(string useremail)
        {
            Output<List<Tameenk.Core.Domain.Dtos.PromotionProgramDTO>> output = new Output<List<Tameenk.Core.Domain.Dtos.PromotionProgramDTO>>();
            try
            {
                output.Result = _promotionService.GetUserPromotionPrograms(useremail);
                output.ErrorCode = Output<List<Tameenk.Core.Domain.Dtos.PromotionProgramDTO>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<List<Tameenk.Core.Domain.Dtos.PromotionProgramDTO>>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.Message;
                return Single(output);
            }
        }

        [HttpGet]
        [Route("api/profile/join-programs")]
        public async Task<IHttpActionResult> JoinProgram(int promotionProgramId, string currentUserId, string userEmail, string lang = "ar")
        {
            Output<List<bool>> output = new Output<List<bool>>();
            try
            {
                // string currentUserId = User.Identity.GetUserId<string>();
                try
                {
                    if (!IsValidEmail(userEmail))
                    {
                        output.ErrorCode = Output<List<bool>>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = "Email is't in a valid format.";
                        return Single(output);
                    }
                    if (promotionProgramId < 1)
                    {
                        output.ErrorCode = Output<List<bool>>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = "Promotion program id can't be less than 1";
                        return Single(output);
                    }
                    var validationError = _promotionService.ValidateBeforeJoinProgram(userEmail, promotionProgramId);
                    if (string.IsNullOrWhiteSpace(validationError))
                    {
                        var enrollmentResponse = _promotionService.EnrollUSerToPromotionProgram(currentUserId, promotionProgramId, userEmail);
                        //if user joined the program then send confirmation email 
                        if (enrollmentResponse.UserEndrollerd)
                        {
                            SendJoinProgramConfirmationEmail(userEmail, promotionProgramId, currentUserId, lang);
                            output.ErrorCode = Output<List<bool>>.ErrorCodes.Success;
                            output.ErrorDescription = LangText.ResourceManager.GetString("JoinProgramMailSent", CultureInfo.GetCultureInfo(lang));
                            return Single(output);
                        }
                        else
                        {
                            //show error that userd couldn't be added to this program.
                            var enrollErrorMessagesAsString = string.Join(Environment.NewLine, enrollmentResponse.Errors.Select(e => e.Description));
                            var errorMsg = LangText.ResourceManager.GetString("ErrorWhileJoiningPromotionProgram", CultureInfo.GetCultureInfo(lang)) + Environment.NewLine + enrollErrorMessagesAsString;
                            output.ErrorCode = Output<List<bool>>.ErrorCodes.NotSuccess;
                            output.ErrorDescription = errorMsg;
                            return Single(output);

                        }
                    }
                    //user domain doesnt exist in program's domains
                    else
                    {
                        output.ErrorCode = Output<List<bool>>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = validationError;
                        return Single(output);
                    }
                }
                catch (Exception ex)
                {
                    output.ErrorCode = Output<List<bool>>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ex.ToString();
                    return Single(output);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<List<bool>>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return Single(output);
            }
        }

        [HttpGet]
        [Route("api/profile/exit-promotion")]
        public IHttpActionResult ExitPromotion(string userId)
        {
            Output<bool> output = new Output<bool>();
            try
            {
                _promotionService.DisenrollUserFromPromotionProgram(userId);
                output.ErrorCode = Output<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<bool>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.Message;
                return Single(output);
            }
        }


        [HttpGet]
        [Route("api/profile/getUserOffers")]
        public async Task<IHttpActionResult> GetUserOffers(string userId, string lang = "ar", string channel = "Web")
        {
            Output<List<QuotationRequestDetailsViewModel>> output = new Output<List<QuotationRequestDetailsViewModel>>();
            output.Result = GetQuotationRequestViewModel(userId);
            output.ErrorCode = Output<List<QuotationRequestDetailsViewModel>>.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            return Single(output);
        }

        /// <summary>
        /// Create Ticket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/profile/createTicket")]
        public IHttpActionResult CreateTicket()
        {
            string userId = _authorizationService.GetUserId(User);

            Output<List<TicketModel>> output = new Output<List<TicketModel>>();
            output.Result = new List<TicketModel>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "CreateTicket";
            profileLog.Channel = HttpContext.Current.Request.Form[2].ToString().ToLower();
            try
            {
                var data = HttpContext.Current.Request.Form[0];
                var createTicketModel = JsonConvert.DeserializeObject<CreateTicketModel>(data);
                if (createTicketModel == null)
                    return Error("createTicketModel is null");

                Tameenk.Core.Domain.Dtos.CreateUserTicketAPIModel createUserTicketModel = new Tameenk.Core.Domain.Dtos.CreateUserTicketAPIModel();
                createUserTicketModel.Language = HttpContext.Current.Request.Form[1].ToString().ToLower();
                createUserTicketModel.Channel = HttpContext.Current.Request.Form[2].ToString().ToLower();
                createUserTicketModel.SequenceOrCustomCardNumber = createTicketModel.SequenceOrCustomCardNumber;
                createUserTicketModel.TicketTypeId = createTicketModel.TicketTypeId;
                createUserTicketModel.UserId = createTicketModel.UserId;
                createUserTicketModel.UserNotes = createTicketModel.UserNotes;
                createUserTicketModel.postedFiles = null;
                createUserTicketModel.Nin = createTicketModel.NIN;
                createUserTicketModel.AttachedFiles = new List<Tameenk.Core.Domain.Dtos.AttachedFiles>();

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    //createUserTicketModel.postedFiles = new List<HttpPostedFileBase>();
                    if (createUserTicketModel.TicketTypeId == (int)EUserTicketTypes.ProofIDAndIBANAndEmail && HttpContext.Current.Request.Files.Count < 3)
                    {
                        output.ErrorCode = Output<List<TicketModel>>.ErrorCodes.InvalidData;
                        output.ErrorDescription = UserTicketResources.ErrorGeneric;
                        profileLog.ErrorCode = (int)output.ErrorCode;
                        profileLog.ErrorDescription = $"attachedFiles are {HttpContext.Current.Request.Files.Count}, it should be 3";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                        return Single(output);
                    }

                    foreach (string key in HttpContext.Current.Request.Files)
                    {
                        //createUserTicketModel.postedFiles.Add(new HttpPostedFileWrapper(HttpContext.Current.Request.Files[key]));
                        var file = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[key]);
                        var attFile = new Tameenk.Core.Domain.Dtos.AttachedFiles()
                        {
                            File = Utilities.GetFileByte(file),
                            Extension = Path.GetExtension(file.FileName)
                        };

                        if (createUserTicketModel.TicketTypeId == (int)EUserTicketTypes.UpdateCustomToSequence)
                        {
                            attFile.TicketTypeFileNameId = (int)TicketTypeFileNameEnum.VehicleRegistration;
                        }
                        else if (createUserTicketModel.TicketTypeId == (int)EUserTicketTypes.ProofIDAndIBANAndEmail)
                        {
                            attFile.TicketTypeFileNameId = key == "0" ? (int)TicketTypeFileNameEnum.PhoneNumber
                                                         : key == "1" ? (int)TicketTypeFileNameEnum.IBANCertificate
                                                         : (int)TicketTypeFileNameEnum.Email;
                        }
                        else if (createUserTicketModel.TicketTypeId == (int)EUserTicketTypes.ReachMaximumPolicyIssuance)
                        {
                            attFile.TicketTypeFileNameId = (int)TicketTypeFileNameEnum.ProofofOwnershipTransfer;
                        }
                        else
                        {
                            attFile.TicketTypeFileNameId = (int)TicketTypeFileNameEnum.Attachment;
                        }

                        createUserTicketModel.AttachedFiles.Add(attFile);
                    }
                }

                var userTicketOutput = _userTicketContext.CreateUserTicketFromAPI(createUserTicketModel);

                if (userTicketOutput == null)
                {
                    output.ErrorCode = Output<List<TicketModel>>.ErrorCodes.NullResult;
                    output.ErrorDescription = "user Ticket Output is NULL";

                    return Single(output);
                }

                if (userTicketOutput.ErrorCode != UserTicketOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = Output<List<TicketModel>>.ErrorCodes.ServiceException;
                    output.ErrorDescription = userTicketOutput.ErrorDescription;

                    return Single(output);
                }

                ProfileNotification profileNotification = new ProfileNotification();
                profileNotification.CreatedDate = DateTime.Now;
                profileNotification.DescriptionAr = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo("ar")), userTicketOutput.UserTicketId.ToString("0000000000"));
                profileNotification.DescriptionEn = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo("en")), userTicketOutput.UserTicketId.ToString("0000000000"));
                profileNotification.TypeId = (int)EProfileNotificationType.Success;
                profileNotification.UserId = userTicketOutput.UserId;
                profileNotification.TicketStatusId = (int)EUserTicketStatus.TicketOpened;
                profileNotification.ModuleId = (int)EProfileNotificationModule.UserTicket;
                _profileContext.CreateProfileNotification(profileNotification);

                //SMS
                //_userTicketContext.SendUpdatedStatusSMS(userTicketOutput.UserId, "ar", (int)EUserTicketStatus.TicketOpened, userTicketOutput.UserTicketId);

                TicketModel ticketModel = new TicketModel();
                ticketModel.TicketId = userTicketOutput.UserTicketId;
                ticketModel.PolicyNo = userTicketOutput.PolicyNo;
                ticketModel.InsuranceCompanyName = userTicketOutput.InsuranceCompanyName;
                ticketModel.StatusName = UserTicketResources.PopupTicketCreated;

                if (userTicketOutput.TicketTypeId == (int)EUserTicketTypes.LinkWithNajm || userTicketOutput.TicketTypeId == (int)EUserTicketTypes.ChangePolicyData
                || userTicketOutput.TicketTypeId == (int)EUserTicketTypes.CouldnotPrintPolicy || userTicketOutput.TicketTypeId == (int)EUserTicketTypes.PolicyGeneration)
                {
                    ticketModel.VehicleName = _tameenkUoW.PolicyRepository.getVehicleModelLocalization(createTicketModel.Language, userTicketOutput.Vehicle);
                    ticketModel.VehiclePlate = GetVehiclePlateModel(userTicketOutput.Vehicle);
                }
                output.Result.Add(ticketModel);
                output.ErrorCode = Output<List<TicketModel>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return OutputHandler(output, profileLog, Output<List<TicketModel>>.ErrorCodes.Success, "Success", createUserTicketModel.Language);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<List<TicketModel>>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return Single(output);
            }
        }

        /// <summary>
        /// Get All User Tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/profile/getAllUserTickets")]
        public IHttpActionResult GetAllUserTickets(string lang = "ar", string channel = "Web")
        {
            Output<List<TicketModel>> output = new Output<List<TicketModel>>();
            output.Result = new List<TicketModel>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "GetAllUserTickets";
            profileLog.Channel = channel;
            string userId = _authorizationService.GetUserId(User);

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler(output, profileLog, Output<List<TicketModel>>.ErrorCodes.EmptyInputParamter, "EmptyUserId", "en");
                }

                var result = _userTicketContext.GetUserTicketsWithLastHistory(userId);
                if (result == null)
                {
                    return OutputHandler(output, profileLog, Output<List<TicketModel>>.ErrorCodes.NullResult, $"NullResult for userId:{userId}", "en");
                }
                TicketModel ticketModel = null;
                foreach (var item in result)
                {
                    ticketModel = new TicketModel();
                    ticketModel.TicketId = item.TicketId;
                    ticketModel.UserNotes = item.UserNotes;
                    ticketModel.PolicyNo = item.PolicyNo;
                    ticketModel.InsuranceCompanyName = lang == "en" ? item.InsuranceCompanyNameEn : item.InsuranceCompanyNameAr;
                    ticketModel.StatusName = lang == "en" ? item.StatusNameEn : item.StatusNameAr;
                    ticketModel.AdminReply = item.AdminReply;
                    ticketModel.TicketStatusId = item.StatusId;
                    //output.Result.vehicle = item.;
                    if (item.TicketTypeId == (int)EUserTicketTypes.LinkWithNajm || item.TicketTypeId == (int)EUserTicketTypes.ChangePolicyData
                    || item.TicketTypeId == (int)EUserTicketTypes.CouldnotPrintPolicy || item.TicketTypeId == (int)EUserTicketTypes.PolicyGeneration)
                    {
                        var vehicle = new Vehicle()
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
                            ModelYear = item.ModelYear
                        };
                        ticketModel.VehicleName = _tameenkUoW.PolicyRepository.getVehicleModelLocalization(lang, vehicle);
                        ticketModel.VehiclePlate = GetVehiclePlateModel(vehicle);
                    }
                    output.Result.Add(ticketModel);
                }
                return OutputHandler(output, profileLog, Output<List<TicketModel>>.ErrorCodes.Success, "Success", "en");

            }
            catch (Exception ex)
            {
                return OutputHandler(output, profileLog, Output<List<TicketModel>>.ErrorCodes.ExceptionError, "ExceptionError", "en", ex.ToString());
            }

        }

        /// <summary>
        /// Get All User Notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/profile/getAllUserNotifications")]
        public IHttpActionResult GetAllUserNotifications(string lang = "ar", string channel = "Web")
        {
            Output<List<ProfileNotificationOutput>> output = new Output<List<ProfileNotificationOutput>>();
            output.Result = new List<ProfileNotificationOutput>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "GetAllUserNotifications";
            profileLog.Channel = channel;
            string userId = _authorizationService.GetUserId(User);
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler(output, profileLog, Output<List<ProfileNotificationOutput>>.ErrorCodes.EmptyInputParamter, "EmptyUserId", "en");
                }

                var result = _profileContext.GetProfileNotifications(userId);
                if (result == null)
                {
                    return OutputHandler(output, profileLog, Output<List<ProfileNotificationOutput>>.ErrorCodes.NullResult, $"NullResult for userId:{userId}", "en");
                }
                ProfileNotificationOutput profileNotificationOutput = null;
                foreach (var item in result)
                {
                    profileNotificationOutput = new ProfileNotificationOutput();
                    profileNotificationOutput.TypeId = item.TypeId;
                    profileNotificationOutput.ModuleId = item.ModuleId;
                    profileNotificationOutput.CreatedDate = item.CreatedDate;
                    profileNotificationOutput.Description = lang == "en" ? item.DescriptionEn : item.DescriptionAr;
                    profileNotificationOutput.TicketStatusId = item.TicketStatusId;

                    output.Result.Add(profileNotificationOutput);
                }
                return OutputHandler(output, profileLog, Output<List<ProfileNotificationOutput>>.ErrorCodes.Success, "Success", "en");

            }
            catch (Exception ex)
            {
                return OutputHandler(output, profileLog, Output<List<ProfileNotificationOutput>>.ErrorCodes.ExceptionError, "ExceptionError", "en", ex.ToString());
            }

        }

        /// <summary>
        /// Get All Ticket Types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/profile/getAllTicketTypes")]
        public IHttpActionResult GetAllTicketTypes(string lang = "ar", string channel = "Web")
        {
            Output<List<TicketTypeOutput>> output = new Output<List<TicketTypeOutput>>();
            output.Result = new List<TicketTypeOutput>();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "GetAllTicketTypes";
            profileLog.Channel = channel;

            try
            {
                var result = _userTicketContext.GetTicketTypesDB();
                if (result == null)
                {
                    return OutputHandler(output, profileLog, Output<List<TicketTypeOutput>>.ErrorCodes.NullResult, "NullResult", "en");
                }
                TicketTypeOutput ticketTypeOutput = null;
                TicketTypeOutput ticketTypeOthers = null;
                foreach (var item in result)
                {
                    ticketTypeOutput = new TicketTypeOutput();
                    ticketTypeOutput.TypeId = item.Id;
                    ticketTypeOutput.TicketTypeName = lang == "en" ? item.TicketTypeNameEn : item.TicketTypeNameAr;
                    if (item.Id == (int)EUserTicketTypes.Others)
                    {
                        ticketTypeOthers = ticketTypeOutput;
                        continue;
                    }
                    output.Result.Add(ticketTypeOutput);
                }
                output.Result.Add(ticketTypeOthers);
                return OutputHandler(output, profileLog, Output<List<TicketTypeOutput>>.ErrorCodes.Success, "Success", "en");

            }
            catch (Exception ex)
            {
                return OutputHandler(output, profileLog, Output<List<TicketTypeOutput>>.ErrorCodes.ExceptionError, "ExceptionError", "en", ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/profile/sadadBills")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetAllSadadBills(string userId, string channel, string language, int pageNumber = 1)
        {
            Output<MySadadBillsOutput> output = new Output<MySadadBillsOutput>();
            output.Result = new MySadadBillsOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetAllSadadBills";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<MySadadBillsOutput>(output, profileLog, Output<MySadadBillsOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new MySadadBillsOutput();
                    var mySadadBillsOutput = _profileContext.GetAllMySadadBills(userId, language, pageNumber);
                    if (mySadadBillsOutput.ErrorCode != Profile.Component.Output.MySadadBillsOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = Output<MySadadBillsOutput>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = mySadadBillsOutput.ErrorDescription;
                        return OutputHandler<MySadadBillsOutput>(output, profileLog, Output<MySadadBillsOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
                    }

                    output.ErrorCode = Output<MySadadBillsOutput>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result.ErrorCode = mySadadBillsOutput.ErrorCode;
                    output.Result.ErrorDescription = output.ErrorDescription;
                    output.Result.SadadBillsList = mySadadBillsOutput.SadadBillsList;
                    output.Result.SadadBillsCount = mySadadBillsOutput.SadadBillsCount;
                    output.Result.CurrentPage = mySadadBillsOutput.CurrentPage;
                    output.Result.Lang = mySadadBillsOutput.Lang;
                    return Single(output);
                }
                else
                {
                    output.ErrorCode = Output<MySadadBillsOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "NotAuthorized";
                    return OutputHandler<MySadadBillsOutput>(output, profileLog, Output<MySadadBillsOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<MySadadBillsOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return OutputHandler<MySadadBillsOutput>(output, profileLog, Output<MySadadBillsOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }

        }


        [HttpPost]
        [Route("api/profile/vehicles")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetVehicles([FromBody]Tameenk.Core.Domain.Dtos.MyVehicleFilter vehicleFilter, string userId, string channel, string language, int pageNumber = 1)
        {
            Output<MyVehiclesOutput> output = new Output<MyVehiclesOutput>();
            output.Result = new MyVehiclesOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetAllVehicles";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<MyVehiclesOutput>(output, profileLog, Output<MyVehiclesOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new MyVehiclesOutput();
                    var myVehiclesOutput = _profileContext.GetAllMyVehicles(userId, vehicleFilter, language, pageNumber,12);
                    if (myVehiclesOutput.ErrorCode != Profile.Component.Output.MyVehiclesOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = Output<MyVehiclesOutput>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = myVehiclesOutput.ErrorDescription;
                        return OutputHandler<MyVehiclesOutput>(output, profileLog, Output<MyVehiclesOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
                    }

                    output.ErrorCode = Output<MyVehiclesOutput>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result.ErrorCode = myVehiclesOutput.ErrorCode;
                    output.Result.ErrorDescription = output.ErrorDescription;
                    output.Result.VehiclesList = myVehiclesOutput.VehiclesList;
                    output.Result.VehiclesTotalCount = myVehiclesOutput.VehiclesTotalCount;
                    return Single(output);
                }
                else
                {
                    output.ErrorCode = Output<MyVehiclesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "NotAuthorized";
                    return OutputHandler<MyVehiclesOutput>(output, profileLog, Output<MyVehiclesOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<MyVehiclesOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return OutputHandler<MyVehiclesOutput>(output, profileLog, Output<MyVehiclesOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/profile/statistics")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetStatistics(string userId, string channel, string language)
        {
            Output<MyStatisticsOutput> output = new Output<MyStatisticsOutput>();
            output.Result = new MyStatisticsOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetStatistics";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<MyStatisticsOutput>(output, profileLog, Output<MyStatisticsOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new MyStatisticsOutput();
                    var myStatisticsOutput = _profileContext.GetMyStatistics(userId);
                    if (myStatisticsOutput.ErrorCode != Profile.Component.Output.MyStatisticsOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = Output<MyStatisticsOutput>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = myStatisticsOutput.ErrorDescription;
                        return Single(output);
                    }

                    output.ErrorCode = Output<MyStatisticsOutput>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result.ErrorCode = myStatisticsOutput.ErrorCode;
                    output.Result.ErrorDescription = output.ErrorDescription;
                    output.Result.Statistics = myStatisticsOutput.Statistics;
                    return Single(output);
                }
                else
                {
                    output.ErrorCode = Output<MyStatisticsOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "NotAuthorized";
                    return OutputHandler<MyStatisticsOutput>(output, profileLog, Output<MyStatisticsOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<MyStatisticsOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return OutputHandler<MyStatisticsOutput>(output, profileLog, Output<MyStatisticsOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }
        }

        #region may to use it later
        //[HttpGet]
        //[Route("api/profile/tickets")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        //public async Task<IHttpActionResult> GetTickets(string userId, string channel, string language)
        //{
        //    Output<MyTicketsOutput> output = new Output<MyTicketsOutput>();
        //    output.Result = new MyTicketsOutput();
        //    profileLog = new ProfileRequestsLog();
        //    profileLog.UserID = new Guid(userId);
        //    profileLog.Method = "GetTickets";
        //    profileLog.Channel = channel;
        //    // check if user exist 
        //    try
        //    {
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return OutputHandler<MyTicketsOutput>(output, profileLog, Output<MyTicketsOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
        //        }
        //        var result = await _authorizationService.GetUser(userId);
        //        if (result != null)
        //        {
        //            output.Result = new MyTicketsOutput();
        //            var myTicketsOutput = _profileContext.GetMyTickets(userId);
        //            if (myTicketsOutput.ErrorCode != Profile.Component.Output.MyTicketsOutput.ErrorCodes.Success)
        //            {
        //                output.ErrorCode = Output<MyTicketsOutput>.ErrorCodes.NotSuccess;
        //                output.ErrorDescription = myTicketsOutput.ErrorDescription;
        //                return OutputHandler<MyTicketsOutput>(output, profileLog, Output<MyTicketsOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
        //            }

        //            output.ErrorCode = Output<MyTicketsOutput>.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";
        //            output.Result.ErrorCode = myTicketsOutput.ErrorCode;
        //            output.Result.ErrorDescription = output.ErrorDescription;
        //            output.Result.Tickets = myTicketsOutput.Tickets;
        //            return Single(output);
        //        }
        //        else
        //        {
        //            output.ErrorCode = Output<MyTicketsOutput>.ErrorCodes.NotAuthorized;
        //            output.ErrorDescription = "NotAuthorized";
        //            return OutputHandler<MyTicketsOutput>(output, profileLog, Output<MyTicketsOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = Output<MyTicketsOutput>.ErrorCodes.ExceptionError;
        //        output.ErrorDescription = ex.ToString();
        //        return OutputHandler<MyTicketsOutput>(output, profileLog, Output<MyTicketsOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
        //    }
        //}

        //[HttpGet]
        //[Route("api/profile/notifications")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        //public async Task<IHttpActionResult> GetNotifications(string userId, string channel, string language)
        //{
        //    Output<MyNotificationsOutput> output = new Output<MyNotificationsOutput>();
        //    output.Result = new MyNotificationsOutput();
        //    profileLog = new ProfileRequestsLog();
        //    profileLog.UserID = new Guid(userId);
        //    profileLog.Method = "GetNotifications";
        //    profileLog.Channel = channel;
        //    // check if user exist 
        //    try
        //    {
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return OutputHandler<MyNotificationsOutput>(output, profileLog, Output<MyNotificationsOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
        //        }
        //        var result = await _authorizationService.GetUser(userId);
        //        if (result != null)
        //        {
        //            output.Result = new MyNotificationsOutput();
        //            var myNotificationsOutput = _profileContext.GetMyNotifications(userId);
        //            if (myNotificationsOutput.ErrorCode != Profile.Component.Output.MyNotificationsOutput.ErrorCodes.Success)
        //            {
        //                output.ErrorCode = Output<MyNotificationsOutput>.ErrorCodes.NotSuccess;
        //                output.ErrorDescription = myNotificationsOutput.ErrorDescription;
        //                return OutputHandler<MyNotificationsOutput>(output, profileLog, Output<MyNotificationsOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
        //            }

        //            output.ErrorCode = Output<MyNotificationsOutput>.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";
        //            output.Result.ErrorCode = myNotificationsOutput.ErrorCode;
        //            output.Result.ErrorDescription = output.ErrorDescription;
        //            output.Result.Notifications = myNotificationsOutput.Notifications;
        //            return Single(output);
        //        }
        //        else
        //        {
        //            output.ErrorCode = Output<MyNotificationsOutput>.ErrorCodes.NotAuthorized;
        //            output.ErrorDescription = "NotAuthorized";
        //            return OutputHandler<MyNotificationsOutput>(output, profileLog, Output<MyNotificationsOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = Output<MyNotificationsOutput>.ErrorCodes.ExceptionError;
        //        output.ErrorDescription = ex.ToString();
        //        return OutputHandler<MyNotificationsOutput>(output, profileLog, Output<MyNotificationsOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
        //    }
        //} 
        #endregion

        [HttpPost]
        [Route("api/profile/invoices")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetInvoices([FromBody] Tameenk.Core.Domain.Dtos.MyInvoicesFilter invoicefilter, string userId, string channel, string language, int pageNumber = 1)
        {
            Output<MyInvoicesOutput> output = new Output<MyInvoicesOutput>();
            output.Result = new MyInvoicesOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetInvoices";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<MyInvoicesOutput>(output, profileLog, Output<MyInvoicesOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new MyInvoicesOutput();
                    var myNotificationsOutput = _profileContext.GetAllMyInvoices(userId, invoicefilter, language, pageNumber);
                    if (myNotificationsOutput.ErrorCode != Profile.Component.Output.MyInvoicesOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = Output<MyInvoicesOutput>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = myNotificationsOutput.ErrorDescription;
                        return OutputHandler<MyInvoicesOutput>(output, profileLog, Output<MyInvoicesOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
                    }

                    output.ErrorCode = Output<MyInvoicesOutput>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result.ErrorCode = myNotificationsOutput.ErrorCode;
                    output.Result.ErrorDescription = output.ErrorDescription;
                    output.Result.InvoicesList = myNotificationsOutput.InvoicesList;
                    output.Result.InvoicesTotalCount = myNotificationsOutput.InvoicesTotalCount;
                    return Single(output);
                }
                else
                {
                    output.ErrorCode = Output<MyInvoicesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "NotAuthorized";
                    return OutputHandler<MyInvoicesOutput>(output, profileLog, Output<MyInvoicesOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<MyInvoicesOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return OutputHandler<MyInvoicesOutput>(output, profileLog, Output<MyInvoicesOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/profile/policies")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetPolicies([FromBody] Tameenk.Core.Domain.Dtos.MyPoliciesFilter policiesFilter, string userId, string channel, string language, int pageNumber = 1)
        {
            Output<MyPoliciesOutput> output = new Output<MyPoliciesOutput>();
            output.Result = new MyPoliciesOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(userId);
            profileLog.Method = "GetPolicies";
            profileLog.Channel = channel;
            // check if user exist 
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return OutputHandler<MyPoliciesOutput>(output, profileLog, Output<MyPoliciesOutput>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", language);
                }
                var result = await _authorizationService.GetUser(userId);
                if (result != null)
                {
                    output.Result = new MyPoliciesOutput();
                    var myPoliciesOutput = _profileContext.GetAllMyPolicies(userId, policiesFilter, language, pageNumber);
                    if (myPoliciesOutput.ErrorCode != Profile.Component.Output.MyPoliciesOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = Output<MyPoliciesOutput>.ErrorCodes.NotSuccess;
                        output.ErrorDescription = myPoliciesOutput.ErrorDescription;
                        return OutputHandler<MyPoliciesOutput>(output, profileLog, Output<MyPoliciesOutput>.ErrorCodes.NotSuccess, "NotSuccess", language);
                    }

                    output.ErrorCode = Output<MyPoliciesOutput>.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result.ErrorCode = myPoliciesOutput.ErrorCode;
                    output.Result.ErrorDescription = output.ErrorDescription;
                    output.Result.PoliciesList = myPoliciesOutput.PoliciesList;
                    output.Result.PoliciesTotalCount = myPoliciesOutput.PoliciesTotalCount;
                    return Single(output);
                }
                else
                {
                    output.ErrorCode = Output<MyPoliciesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "NotAuthorized";
                    return OutputHandler<MyPoliciesOutput>(output, profileLog, Output<MyPoliciesOutput>.ErrorCodes.NotAuthorized, "NotAuthorized", language);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<MyPoliciesOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                return OutputHandler<MyPoliciesOutput>(output, profileLog, Output<MyPoliciesOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
            }
        }
        #region Private Methods

        private List<QuotationRequestDetailsViewModel> GetQuotationRequestViewModel(string userId)
        {
            //get the quotation requests from db
            List<QuotationRequest> quotationRequestList = _tameenkUoW.PolicyRepository.GetQuotationRequestsByUSerId(userId);
            //convert the list to view model
            List<QuotationRequestDetailsViewModel> model = new List<QuotationRequestDetailsViewModel>();
            var ncdList = nCDFreeYearRepository.TableNoTracking.ToList();
            foreach (var item in quotationRequestList)
            {
                var VehicleMakerCode = item.Vehicle.VehicleMakerCode.Value.ToString("D4");
                model.Add(new QuotationRequestDetailsViewModel
                {
                    ExternalId = item.ExternalId,
                    VehicleMakerCode = item.Vehicle.VehicleMakerCode.ToString(),
                    VehicleMaker = item.Vehicle.VehicleMaker,
                    VehicleModel = item.Vehicle.VehicleModel,
                    DriverFirstName = item.Driver.FirstName,
                    VehicleModelYear = item.Vehicle.ModelYear,
                    CityArabicDescription = item.City.ArabicDescription,
                    CreatedDateTime = item.CreatedDateTime,
                    VehiclePlate = GetVehiclePlateModel(item.Vehicle),
                    NcdFreeYears = item.NajmNcdFreeYears,
                    Ncd = ncdList.Where(x => x.Code == item.NajmNcdFreeYears).FirstOrDefault(),
                    RemainingTimeToExpireInSeconds = item.CreatedDateTime.AddHours(16).Subtract(DateTime.Now).TotalSeconds,
                    CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg"
                });
            }
            return model;
        }


        private bool SendJoinProgramConfirmationEmail(string userEmail, int programId, string currentUserId, string lang)
        {
            JoinProgramConfirmEmailModel model = new JoinProgramConfirmEmailModel()
            {
                UserId = currentUserId,
                JoinRequestedDate = DateTime.Now,
                PromotionProgramId = programId,
                UserEmail = userEmail
            };
            var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), JOIN_PROMOTION_PROGRAM_SHARED_KEY);
            //token = HttpUtility.UrlEncode(token);
            string hashed = SecurityUtilities.HashData(token, null);
            var emailSubject = LangText.JoinProgramConfirmationSubject;
            // string url = Url.Action("ConfirmJoinProgram", "Promotion", new { token }, protocol: Utilities.SiteURL);
            string url = Utilities.SiteURL + "/promotionPrograms/confirmJoinProgram/?token=" + token + "&hashed=" + hashed;
            string emailBody = string.Format(LangText.JoinProgramConfirmationBody, url);
            MessageBodyModel messageBodyModel = new MessageBodyModel();
            messageBodyModel.Image = Utilities.SiteURL + "/assets/imgs/PromoActivation.png";
            messageBodyModel.Language = lang;
            messageBodyModel.MessageBody = emailBody;
            return MailUtilities.SendMail(messageBodyModel, emailSubject, _tameenkConfig.SMTP.SenderEmailAddress, userEmail);
            //await _notificationService.SendEmailAsync(userEmail, emailSubject, emailBody, null, true).ConfigureAwait(false);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private List<NotificationModel> GetUserNotifications(string userId, bool getReadedNotifications = false)
        {
            List<NotificationModel> notificationModels = new List<NotificationModel>();
            var notifications = _notificationService.GetUserNotifications(userId, getReadedNotifications);
            foreach (var notification in notifications)
            {
                switch (notification.Type)
                {
                    case NotificationType.NewPolicyUpdateRequest:
                    case NotificationType.PolicyUpdateRequestApproved:
                        notificationModels.Add(CreateNewPolicyUpdNotification(notification));
                        break;
                    case NotificationType.PolicyUpdateRequestAwaitingPayment:
                        notificationModels.Add(CreateAwaitingPaymentPolicyNotification(notification));
                        break;
                    case NotificationType.PolicyUpdateRequestRejected:
                        notificationModels.Add(CreateRejectedPolicyUpdNotification(notification));
                        break;
                }

            }

            return notificationModels;
        }
        private NewPolicyUpdReqtNotificationModel CreateNewPolicyUpdNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new NewPolicyUpdReqtNotificationModel(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

            return policyUpdNotification;
        }
        private AwaitingPaymentPolicyUpdNotificationModel CreateAwaitingPaymentPolicyNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new AwaitingPaymentPolicyUpdNotificationModel(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;
            policyUpdNotification.EnablePaymentButton = policyUpdNotification.CreatedAt.GivenDateWithinGivenHours(16);

            return policyUpdNotification;
        }
        private RejectedPolicyUpdReqNotification CreateRejectedPolicyUpdNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new RejectedPolicyUpdReqNotification(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

            return policyUpdNotification;
        }
        private string GetUserJoinedPromotionProgramName(string userId)
        {
            var progUser = _promotionService.GetPromotionProgramUser(userId);
            if (progUser != null && progUser.EmailVerified)
            {
                return progUser.PromotionProgram.Name;
            }

            return string.Empty;
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
        /// <summary>
        /// Get Checkout details from Policy update payment obj
        /// </summary>
        /// <param name="policyUpdPayment"></param>
        /// <returns>Checkout details</returns>
        private CheckoutDetail GetCheckoutDetailFromPolicyUpdPayment(PolicyUpdatePayment policyUpdPayment)
        {
            return policyUpdPayment.PolicyUpdateRequest?.Policy?.CheckoutDetail;
        }
        private int CreatePolicyUpdPayfortRequest(PaymentRequestModel paymentRequest, string policyUpdReqReferenceId)
        {
            var request = new PayfortPaymentRequest()
            {
                UserId = paymentRequest.UserId,
                Amount = paymentRequest.PaymentAmount,
                ReferenceNumber = paymentRequest.RequestId
            };
            var policyUpdateRequest = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqReferenceId);
            policyUpdateRequest.PayfortPaymentRequests.Add(request);
            _policyUpdReqRepository.Update(policyUpdateRequest);
            return request.ID;
        }
        #endregion

        [HttpGet]
        [Route("api/profile/promotion-programsByTypeId")]
        public IHttpActionResult GetPromotionProgramsByTypeId(int typeId, string channel, string lang)
        {
            Output<List<IdNameModel>> output = new Output<List<IdNameModel>>();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserID = (string.IsNullOrEmpty(User.Identity.GetUserId())) ? new Guid().ToString() : User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.MethodName = "GetPromotionProgramsByTypeId";
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"typeId: { typeId } , channel: { channel }, lang: { lang }";

            try
            {
                if (typeId < 0)
                {
                    output.ErrorCode = Output<List<IdNameModel>>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Type Id value is invalid";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    return OutputHandler<List<IdNameModel>>(output, log, Output<List<IdNameModel>>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", lang, output.ErrorDescription);
                }

                output.Result = new List<IdNameModel>();
                output.Result = _promotionContext.GetPromotionProgramsByTypeId(typeId, lang);

                output.ErrorCode = Output<List<IdNameModel>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<List<IdNameModel>>(output, log, Output<List<IdNameModel>>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<List<IdNameModel>>.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<List<IdNameModel>>(output, log, Output<List<IdNameModel>>.ErrorCodes.ServiceException, "ServiceException", lang, output.ErrorDescription);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/profile/SearchPolicy")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> SearchPolicy([FromBody] SearchPolicyModel model)
        {
            Output<PolicyOutput> output = new Output<PolicyOutput>();
            output.Result = new PolicyOutput();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "SearchPolicy";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            Guid userId = Guid.Empty;
            if (Guid.TryParse(User.Identity.GetUserId(), out userId))
                log.UserID = userId;

            string exp = string.Empty;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyData", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (model.Captcha == null)
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CaptchaEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Captcha is null";
                    return OutputHandler<PolicyOutput>(output, log, Output<PolicyOutput>.ErrorCodes.ExceptionError, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }
                if (!ValidateCaptcha(model.Captcha, out exp))
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.InvalidCaptcha;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CaptchaInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid Captcha";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (string.IsNullOrWhiteSpace(model.NIN))
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National Id is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (!model.NIN.StartsWith("1") && !model.NIN.StartsWith("2"))
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("InvalidNationalId", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National Id is invalid as it's not start with (1 or 2)";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (!Enum.IsDefined(typeof(VehicleIdType), model.VehicleTypeId))
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("VehicleTypeIdInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"model.VehicleTypeId is not valid as passed value is: {model.VehicleTypeId}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (model.VehicleTypeId == (int)VehicleIdType.CustomCard)
                {
                    if (string.IsNullOrWhiteSpace(model.CustomCardNumber))
                    {
                        output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CustomNumberEmpty", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Custom Number is empty";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return Single(output);
                    }
                    if (model.CustomCardNumber.Length > 10)
                    {
                        output.ErrorCode = Output<PolicyOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("InvalidCustomNumber", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Custom Number is invalid as it's excees the max length";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return Single(output);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.SequenceNumber))
                    {
                        output.ErrorCode = Output<PolicyOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SequenceNumberEmpty", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Sequence Number is empty";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return Single(output);
                    }
                    if (model.SequenceNumber.Length > 10)
                    {
                        output.ErrorCode = Output<PolicyOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("InvalidSequenceNumber", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Sequence Number is invalid as it's excees the max length";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return Single(output);
                    }
                }

                var cachKey = $"{model.Channel}_PrintPolicy_{base_KEY}_{log.UserIP}";
                UserPartialLockModel partialLock = await ValidateIfUserPartiallyLocked(model.Hashed, log.UserIP, cachKey, model.Lang);
                if (partialLock.IsLocked)
                {
                    output.ErrorCode = Output<PolicyOutput>.ErrorCodes.AccountLocked;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("LoginUserPartialLock", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"user is partially locked as he exceeds the max tries: {partialLock.ErrorTimesUserTries}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                var result = _checkoutContext.GetUserPolicies(model.NIN, model.SequenceNumber, model.CustomCardNumber, out exp);
                if (result == null || result.ErrorCode != PolicyOutput.ErrorCodes.Success || result.Policies == null || result.Policies.Count <= 0)
                {
                    partialLock.ErrorTimesUserTries += 1;
                    if (partialLock.ErrorTimesUserTries >= 10)
                    {
                        partialLock.IsLocked = true;
                        partialLock.LockDueDate = DateTime.Now.AddMinutes(5);
                        await RedisCacheManager.Instance.SetAsync(cachKey, partialLock, 5 * 60);
                    }
                    else
                        result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);
                }
                if (!string.IsNullOrEmpty(exp) && result.ErrorCode != PolicyOutput.ErrorCodes.Success)
                    return OutputHandler<PolicyOutput>(output, profileLog, Output<PolicyOutput>.ErrorCodes.ServiceException, "ExceptionError", model.Lang, exp);

                output.Result = result;
                output.ErrorCode = Output<PolicyOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<PolicyOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ExceptionError", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Single(output);
            }
        }

        private bool ValidateCaptcha(Tameenk.Core.Domain.Dtos.ValidateCaptchaModel model, out string exp)
        {
            exp = "";
            var encryptedCaptcha = AESEncryption.DecryptString(model.Token, SHARED_SECRET);
            exp = encryptedCaptcha;
            try
            {
                var captchaToken = JsonConvert.DeserializeObject<CaptchaResponseModel>(encryptedCaptcha);
                if (captchaToken.ExpiryDate.Value.CompareTo(DateTime.Now.AddSeconds(-captchaToken.ExpiredInSeconds)) < 0)
                {
                    return false;
                }
                if (captchaToken.Captcha.Equals(model.Input, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        [HttpGet]
        [Route("api/profile/getNationalAddressesByUserId")]
        public IHttpActionResult GetNationalAddressesByUserId(string channel, string lang, int pageIndx = 0, int pageSize = 10)
        {
            Output<NationalAddressesOutput> output = new Output<NationalAddressesOutput>();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetNationalAddressesByUserId";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = Output<NationalAddressesOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Id is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Ok(output);
                }
                output.Result = new NationalAddressesOutput();
                output.Result.Addresses = new List<AddressInfoModel>();
                int totlaCount = 0;
                string exception = string.Empty;
                var addresses = _addressService.GetProfileAddressesForUser(userId, out totlaCount, out exception, 1, 10);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = Output<NationalAddressesOutput>.ErrorCodes.ServiceException;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed due to " + exception;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Ok(output);
                }
                if (addresses == null || addresses.Count() == 0)
                {
                    output.ErrorCode = Output<NationalAddressesOutput>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoAddressFound", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "addresses is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Ok(output);
                }
                output.Result.TotalCount = totlaCount;
                output.Result.Addresses = addresses;

                output.ErrorCode = Output<NationalAddressesOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<NationalAddressesOutput>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.Message;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Error(output);
            }
        }

        [HttpPost]
        [Route("api/profile/checkForOffers")]
        public async Task<IHttpActionResult> CheckOffersbyNin(CheckForOffersModel model)
        {
            Output<OffersOutputModel> output = new Output<OffersOutputModel>();
            output.Result = new OffersOutputModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "CheckOffersbyNin";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            Guid userId = Guid.Empty;
            if(Guid.TryParse(User.Identity.GetUserId(), out userId))
                log.UserID = userId;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyData", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (model.Captcha == null)
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CaptchaEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Captcha is null";
                    return OutputHandler<OffersOutputModel>(output, log, Output<OffersOutputModel>.ErrorCodes.ExceptionError, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }
                string error;
                if (!ValidateCaptcha(model.Captcha, out error))
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.InvalidCaptcha;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CaptchaInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid Captcha";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (string.IsNullOrWhiteSpace(model.NIN))
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National Id is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }
                if (!model.NIN.StartsWith("1") && !model.NIN.StartsWith("2"))
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.InvalidData;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("InvalidNationalId", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National Id is invalid as it's not start with (1 or 2)";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                var cachKey = $"{model.Channel}_Wareef_{base_KEY}_{log.UserIP}";
                UserPartialLockModel partialLock = await ValidateIfUserPartiallyLocked(model.Hashed, log.UserIP, cachKey, model.Lang);
                if (partialLock.IsLocked)
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.AccountLocked;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("LoginUserPartialLock", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"user is partially locked as he exceeds the max tries: {partialLock.ErrorTimesUserTries}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                var result = _checkoutContext.UserHasActivePolicy(model.NIN);
                if (result == null || result.ErrorCode != CheckoutOutput.ErrorCodes.Success || result.ActivePolicyData == null || result.ActivePolicyData.HasActivePolicy == 0)
                {
                    partialLock.ErrorTimesUserTries += 1;
                    if (partialLock.ErrorTimesUserTries >= 10)
                    {
                        partialLock.IsLocked = true;
                        partialLock.LockDueDate = DateTime.Now.AddMinutes(5);
                        await RedisCacheManager.Instance.SetAsync(cachKey, partialLock, 5 * 60);
                    }
                    else
                        output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);
                }
                if (result.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ResultEmptyForWareefDiscount", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Error happend while check for offers in --> _checkoutContext.UserHasActivePolicy";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Single(output);
                }

                //output.Result = new OffersOutputModel()
                //{
                //    ExpiryDate = result.ActivePolicyData.ExpiryDate,
                //    HasActivePolicy = (result.ActivePolicyData.HasActivePolicy == 1) ? true : false
                //};

                output.Result.ExpiryDate = result.ActivePolicyData.ExpiryDate;
                output.Result.HasActivePolicy = (result.ActivePolicyData.HasActivePolicy == 1) ? true : false;

                output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<OffersOutputModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ServiceException", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Single(output);
            }
        }

        //[HttpPost]
        //[Route("api/profile/updateNationalAddress")]
        //public IHttpActionResult UpdateNationalAddress(UpdateAddressFromProfileModel model)
        //{
        //    Output<bool> output = new Output<bool>();
        //    ProfileRequestsLog log = new ProfileRequestsLog();
        //    log.CreatedDate = DateTime.Now;
        //    log.Method = "UpdateNationalAddress";
        //    log.Channel = model.Channel;
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.CreatedDate = DateTime.Now;
        //    string userId = _authorizationService.GetUserId(User);
        //    if (string.IsNullOrEmpty(userId))
        //        userId = User.Identity.GetUserId();
        //    log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyUserId", CultureInfo.GetCultureInfo(model.Lang));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "User Id is empty";
        //            return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
        //        }
        //        if (model == null)
        //        {
        //            output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyData", CultureInfo.GetCultureInfo(model.Lang));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "model is null";
        //            return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.ExceptionError, log.ErrorDescription, model.Lang, output.ErrorDescription);
        //        }
        //        if (model.Address == null)
        //        {
        //            output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyData", CultureInfo.GetCultureInfo(model.Lang));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Address is null";
        //            return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.ExceptionError, log.ErrorDescription, model.Lang, output.ErrorDescription);
        //        }
        //        if (model.Address.Id < 0)
        //        {
        //            output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmptyData", CultureInfo.GetCultureInfo(model.Lang));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "address is is invalid";
        //            return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.ExceptionError, log.ErrorDescription, model.Lang, output.ErrorDescription);
        //        }

        //        int totlaCount = 0;
        //        var addresses = _addressService.GetProfileAddressesForUser(userId, out totlaCount, 1, int.MaxValue);

        //        var address = addresses.Where(a => a.Id == model.Address.Id).FirstOrDefault();
        //        if (address == null)
        //        {
        //            output.ErrorCode = Output<bool>.ErrorCodes.NullResult;
        //            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("AddressNotExist", CultureInfo.GetCultureInfo(model.Lang));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "No address for this user: " + userId + " with this address id :" + model.Address.Id;
        //            return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
        //        }

        //        address.Title = model.Address.Title;
        //        address.Address1 = model.Address.Address1;
        //        address.Address2 = model.Address.Address2;
        //        address.BuildingNumber = model.Address.BuildingNumber;
        //        address.Street = model.Address.Street;
        //        address.District = model.Address.District;
        //        address.UnitNumber = model.Address.UnitNumber;
        //        address.PostCode = model.Address.PostCode;
        //        address.AdditionalNumber = model.Address.AdditionalNumber;
        //        address.CityId = model.Address.CityId;
        //        address.City = model.Address.City;
        //        address.RegionId = model.Address.RegionId;
        //        address.RegionName = model.Address.RegionName;
        //        address.Latitude = model.Address.Latitude;
        //        address.Longitude = model.Address.Longitude;
        //        address.PolygonString = model.Address.PolygonString;
        //        address.AddressLoction = model.Address.AddressLoction;
        //        address.Restriction = model.Address.Restriction;
        //        address.IsPrimaryAddress = model.Address.IsPrimaryAddress;
        //        _addressService.UpdateAddress(address);

        //        output.ErrorCode = Output<bool>.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = output.ErrorDescription;
        //        return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.Success, "Success", model.Lang, output.ErrorDescription);
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = Output<bool>.ErrorCodes.ServiceException;
        //        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ServiceException", CultureInfo.GetCultureInfo(model.Lang));
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.Message;
        //        return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.ServiceException, ex.Message, model.Lang, output.ErrorDescription);
        //    }
        //}

        /// <summary>
        /// Change Password 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/profile/change-Password")]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            model.UserId = _authorizationService.GetUserId(User);
            var result = await _authenticationContext.ChangePassword(model);
            return Single(result);
        }

        /// <summary>
        /// Change Password Confirmation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/profile/change-Password-confirm")]
        public async Task<IHttpActionResult> ConfirmChangePassword([FromBody] ChangePasswordViewModel model)
        {
            model.UserId = _authorizationService.GetUserId(User);
            var result = await _authenticationContext.ChangePasswordConfirm(model);
            return Single(result);
        }

        /// <summary>
        /// Change Password Resend OTP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/profile/change-Password-resendotp")]
        public async Task<IHttpActionResult> ChangePasswordReSendOTP([FromBody] ChangePasswordViewModel model)
        {
            model.UserId = _authorizationService.GetUserId(User);
            var result = await _authenticationContext.ChangePasswordReSendOTP(model);
            return Single(result);
        }

        [HttpGet]
        [Route("api/profile/corporate-account-info")]
        public IHttpActionResult GetCorporateAccountInfo(string lang, string channel)
        {
            Output<Models.CorporateAccountInfoModel> output = new Output<Models.CorporateAccountInfoModel>();
            output.Result = new Models.CorporateAccountInfoModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetCorporateAccountInfo";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateAccount = _corporateContext.GetCorporateAccountById(corporateUser.CorporateAccountId);
                if (corporateAccount == null)
                {
                    output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("No corporate account found with thid id {0}", corporateUser.CorporateAccountId);
                    return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                output.Result.NameAr = corporateAccount.NameAr;
                output.Result.NameEn = corporateAccount.NameEn;
                output.Result.Balance = corporateAccount.Balance;
                output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(lang));

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<Models.CorporateAccountInfoModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return OutputHandler<Models.CorporateAccountInfoModel>(output, log, Output<Models.CorporateAccountInfoModel>.ErrorCodes.ExceptionError, "ExceptionError", lang, output.ErrorDescription);
            }
        }

        [HttpGet]
        [Route("api/profile/corporate-account-users")]
        public IHttpActionResult GetCorporateUsersInfo(string lang, string channel)
        {
            Output<CorporateAccountUsersViewModel> output = new Output<CorporateAccountUsersViewModel>();
            output.Result = new CorporateAccountUsersViewModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetCorporateUsersInfo";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var userFilter = new CorporateFilterModel();
                userFilter.AccountId = corporateUser.CorporateAccountId;
                int totalCount = 0;
                string exception = string.Empty;
                var result = _corporateUserService.GetCorporateUsersWithFilter(userFilter, 1, 10, 240, false, out totalCount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = exception;
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                output.Result.CorporateUsersList = result;
                output.Result.CorporateUsersTotalCount = totalCount;
                output.Result.CurrentPage = 1;
                output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError, "ExceptionError", lang, output.ErrorDescription);
            }
        }

        [HttpGet]
        [Route("api/profile/corporate-account-users-filter")]
        public IHttpActionResult GetCorporateUsersInfoByFilter(string lang, string channel, int pageIndex, string email = null, string mobile = null)
        {
            Output<CorporateAccountUsersViewModel> output = new Output<CorporateAccountUsersViewModel>();
            output.Result = new CorporateAccountUsersViewModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetCorporateUsersInfoByFilter";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var filter = new CorporateFilterModel();
                filter.Email = email;
                filter.PhoneNumber = mobile;
                filter.AccountId = corporateUser.CorporateAccountId;
                int totalCount = 0;
                string exception = string.Empty;
                var result = _corporateUserService.GetCorporateUsersWithFilter(filter, 1, 10, 240, false, out totalCount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = exception;
                    return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                output.Result.CorporateUsersList = result;
                output.Result.CorporateUsersTotalCount = totalCount;
                output.Result.CurrentPage = pageIndex;
                output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return OutputHandler<CorporateAccountUsersViewModel>(output, log, Output<CorporateAccountUsersViewModel>.ErrorCodes.ExceptionError, "ExceptionError", lang, output.ErrorDescription);
            }
        }

        [HttpPost]
        [Route("api/profile/corporate-account-add-user")]
        public async Task<IHttpActionResult> AddNewCorporateAccountUser(NewCoroprateUserModel model)
        {
            Output<bool> output = new Output<bool>();
            output.Result = new bool();
            output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "AddNewCorporateAccountUser";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }

                Profile.Component.Models.AddCorporateUserModel newUserModel = new Profile.Component.Models.AddCorporateUserModel();
                newUserModel.FullName = model.FullName;
                newUserModel.Email = model.Email;
                newUserModel.PhoneNumber = model.PhoneNumber;
                newUserModel.Password = model.Password;
                newUserModel.RePassword = model.ConfirmPassword;
                newUserModel.Channel = Channel.Portal;
                newUserModel.Language = model.Lang;
                newUserModel.AccountId = corporateUser.CorporateAccountId;
                var result = await _corporateContext.AddCorporateUser(newUserModel);
                if ((int)result.ErrorCode != 1)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, model.Lang, output.ErrorDescription);
                }

                output.Result = true;
                output.ErrorCode = Output<bool>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.Success, "Success", model.Lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<bool>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<bool>(output, log, Output<bool>.ErrorCodes.ExceptionError, "ExceptionError", model.Lang, output.ErrorDescription);
            }
        }

        [HttpGet]
        [Route("api/profile/corporate-account-policies")]
        public IHttpActionResult GetCorporateAccountPoliciesInfo(string lang, string channel)
        {
            Output<CorporateAccountPoliciesViewModel> output = new Output<CorporateAccountPoliciesViewModel>();
            output.Result = new CorporateAccountPoliciesViewModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetCorporateAccountPoliciesInfo";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                CorporatePoliciesOutput result = _corporateContext.GetAllCorporatePolicies(corporateUser.CorporateAccountId,null,null, null, lang);
                if ((int)result.ErrorCode != 1)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                output.Result.PoliciesList = result.PoliciesList;
                output.Result.PoliciesTotalCount = result.PoliciesTotalCount;
                output.Result.Lang = lang;
                output.Result.CurrentPage = 1;
                output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError, "ExceptionError", lang, output.ErrorDescription);
            }
        }

        [HttpGet]
        [Route("api/profile/corporate-account-policies-filter")]
        public IHttpActionResult GetCorporateAccountPoliciesFilterInfo(string lang, string channel, int pageIndex, string policyNumber = null, string insuredNIN = null, string sequenceOrCustomCardNumber = null, string dateFrom = null, string dateTo = null)
        {
            Output<CorporateAccountPoliciesViewModel> output = new Output<CorporateAccountPoliciesViewModel>();
            output.Result = new CorporateAccountPoliciesViewModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "GetCorporateAccountPoliciesFilterInfo";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var filter = new Tameenk.Core.Domain.Dtos.CorporatePoliciesFilter();
                filter.PolicyNumber = policyNumber;
                filter.InsuredNIN = insuredNIN;
                filter.SequenceOrCustomCardNumber = sequenceOrCustomCardNumber;
                CorporatePoliciesOutput result = _corporateContext.GetAllCorporatePolicies(corporateUser.CorporateAccountId, filter,dateFrom,dateTo, lang, pageIndex);
                if ((int)result.ErrorCode != 1)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.EmptyInputParamter, log.ErrorDescription, lang, output.ErrorDescription);
                }

                output.Result.PoliciesList = result.PoliciesList;
                output.Result.PoliciesTotalCount = result.PoliciesTotalCount;
                output.Result.Lang = lang;
                output.Result.CurrentPage = pageIndex;
                output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.Success, "Success", lang, output.ErrorDescription);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError, "ExceptionError", lang, output.ErrorDescription);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("api/profile/DownloadPolicy")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> DownloadPolicy(string r)
        {
            Output<string> output = new Output<string>();
            try
            {
                if (string.IsNullOrEmpty(r))
                {
                    output.ErrorCode = Output<string>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo("ar"));
                    return Single(output);
                }
                var policy = _policyService.GetPolicyByReferenceId(r);
                if (policy == null || !policy.PolicyFileId.HasValue)
                {
                    output.ErrorCode = Output<string>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo("ar"));
                    return Single(output);
                }
                var policyFile = _policyService.DownloadPolicyFile(policy.PolicyFileId.ToString());
                if (policyFile != null)
                {
                    if (policyFile.PolicyFileByte != null)
                    {
                        output.Result = Convert.ToBase64String(policyFile.PolicyFileByte);
                        output.ErrorCode = Output<string>.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        return Single(output);
                    }
                    else if (!string.IsNullOrEmpty(policyFile.FilePath))
                    {
                        if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            FileNetworkShare fileShare = new FileNetworkShare();
                            string exception = string.Empty;
                            var file = fileShare.GetFile(policyFile.FilePath, out exception);
                            if (file != null)
                            {
                                output.Result = Convert.ToBase64String(file);
                                output.ErrorCode = Output<string>.ErrorCodes.Success;
                                output.ErrorDescription = "Success";
                                return Single(output);
                            }
                        }
                        else
                        {
                            output.Result = Convert.ToBase64String(System.IO.File.ReadAllBytes(policyFile.FilePath));
                            output.ErrorCode = Output<string>.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            return Single(output);
                        }
                    }
                }
                output.ErrorCode = Output<string>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo("ar"));
                return Single(output);

            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<string>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ExceptionError", CultureInfo.GetCultureInfo("ar"));
                return Single(output);

            }
        }

        [HttpPost]
        [Route("api/profile/updateNationalAddress")]
        public IHttpActionResult UpdateNationalAddress(UpdateAddressFromProfileModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _profileContext.UpdateNationalAddress(model, userId);
            if (output.ErrorCode == NationalAddressesOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        [HttpGet]
        [Route("api/profile/corporate-account-policies-export")]
        public IHttpActionResult ExportCorporateAccountPolicies(string lang, string channel, string policyNumber = null, string insuredNIN = null, string sequenceOrCustomCardNumber = null)
        {
            Output<CorporateAccountPoliciesViewModel> output = new Output<CorporateAccountPoliciesViewModel>();
            output.Result = new CorporateAccountPoliciesViewModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "ExportCorporateAccountPolicies";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();

            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                var userData = _authorizationService.GetUser(userId);
                if (userData.Result == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in ASP.net users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!userData.Result.IsCorporateUser)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Corporate";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var corporateUser = _corporateContext.GetCorporateUserByUserId(userId);
                if (corporateUser == null)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("User Not Found in corporate users table with this id {0}", userId);
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NullResult, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (!corporateUser.IsActive)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Not Active";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized, log.ErrorDescription, lang, output.ErrorDescription);
                }
                if (corporateUser.IsSuperAdmin != true)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is not supper admin";
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.NotAuthorized, log.ErrorDescription, lang, output.ErrorDescription);
                }

                var filter = new Tameenk.Core.Domain.Dtos.CorporatePoliciesFilter();
                filter.PolicyNumber = policyNumber;
                filter.InsuredNIN = insuredNIN;
                filter.SequenceOrCustomCardNumber = sequenceOrCustomCardNumber;
                CorporatePoliciesOutput result = _corporateContext.GetAllCorporatePolicies(corporateUser.CorporateAccountId, filter,null,null, lang, 1,int.MaxValue);
                if ((int)result.ErrorCode != 1)
                {
                    output.ErrorCode = Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    return OutputHandler<CorporateAccountPoliciesViewModel>(output, log, Output<CorporateAccountPoliciesViewModel>.ErrorCodes.ExceptionError, log.ErrorDescription, lang, output.ErrorDescription);
                }
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);

                byte[] file = _excelService.GenerateExcelCorporatePolicies(result.PoliciesList);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Error("an error has occured");
            }
        }
        [HttpGet]        [Route("api/profile/DownloadEInvoiceFilePDF")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]        public async Task<IHttpActionResult> DownloadEInvoiceFilePDF(string fileId, string language, string channel = "Web")        {            string userId = _authorizationService.GetUserId(User);            if (string.IsNullOrEmpty(userId))                userId = User.Identity.GetUserId();            var output = _profileContext.DownloadEInvoiceFilePDF(fileId, language, userId, channel);            return Single(output);        }

        [HttpPost]
        [Route("api/profile/odpolicies")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> GetODPolicies([FromBody] Tameenk.Core.Domain.Dtos.MyPoliciesFilter policiesFilter, string channel, string language, int pageNumber = 1)
        {
            Output<ODPoliciesOutput> output = new Output<ODPoliciesOutput>();
            output.Result = new ODPoliciesOutput();
            profileLog = new ProfileRequestsLog();
            profileLog.Method = "GetODPolicies";
            profileLog.Channel = channel;
            profileLog.UserIP = Utilities.GetUserIPAddress();
            profileLog.ServerIP = Utilities.GetInternalServerIP();
            profileLog.UserAgent = Utilities.GetUserAgent();
            try
            {
                string userId = _authorizationService.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    userId = User.Identity.GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language));
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = "userId is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                    return Error(output);
                }
                if (userId == new Guid().ToString())
                {
                    output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language));
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = "User is anonymous";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                    return Error(output);
                }
                profileLog.UserID = new Guid(userId);
                var result = await _authorizationService.GetUser(userId);
                if (result == null)
                {
                    output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language));
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = $"No user with Id: {userId}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                    return Error(output);
                }
                profileLog.Email = result.Email;

                string exception = string.Empty;
                var odPoliciesOutput = _profileContext.GetAllODPolicies(userId, policiesFilter, out exception, language, pageNumber);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.ServiceException;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language));
                    profileLog.ErrorCode = (int)output.ErrorCode;
                    profileLog.ErrorDescription = $"Error happend, and error is: {exception}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                    return Error(output);
                }

                output.Result = odPoliciesOutput;
                output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(language));
                profileLog.ErrorCode = (int)output.ErrorCode;
                profileLog.ErrorDescription = output.ErrorDescription;
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<ODPoliciesOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language));
                profileLog.ErrorCode = (int)output.ErrorCode;
                profileLog.ErrorDescription = $"ExceptionError, and error is: {ex.ToString()}";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(profileLog);
                return Error(output);
            }
        }

        #region Update User Profile Data

        [HttpPost]
        [Route("api/profile/sendOTP")]
        public async Task<IHttpActionResult> SendOTP(UpdateUserProfileDataModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            var output = await _profileContext.SendOTPAsync(model, userId);
            return Single(output);
        }

        [HttpPost]
        [Route("api/profile/reSendOTP")]
        public async Task<IHttpActionResult> ReSendOTP(UpdateUserProfileDataModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            var output = await _profileContext.ReSendOTPAsync(model, userId);
            return Single(output);
        }

        [HttpPost]
        [Route("api/profile/updateUserProfileData")]
        public async Task<IHttpActionResult> UpdateUserProfileBasicData(UpdateUserProfileDataModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            var output = await _profileContext.UpdateUserProfileData(model, userId);
            return Single(output);
        }

        [HttpPost]
        [Route("api/profile/confirmEmail")]
        public async Task<IHttpActionResult> EmailConfirmation(UpdateUserProfileDataModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            var output = await _profileContext.EmailConfirmation(model, userId);
            return Single(output);
        }

        #endregion


        #region Private Methods

        #region Manage User Partially Lock

        private async Task<UserPartialLockModel> ValidateIfUserPartiallyLocked(string hashedValue, string userIP, string cachKey, string lang)
        {
            UserPartialLockModel partialLock = null;
            var sessionId = Guid.NewGuid().ToString();

            try
            {
                var lockedFromCach = await RedisCacheManager.Instance.GetAsync<UserPartialLockModel>(cachKey);
                if (lockedFromCach != null && lockedFromCach.IsLocked)
                    return lockedFromCach;

                if (string.IsNullOrEmpty(hashedValue))
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "hashedValue is empty", lang);

                var encryptedCaptcha = AESEncryption.DecryptString(hashedValue, SHARED_PARTIAL_LOCK_SECRET);
                if (string.IsNullOrEmpty(encryptedCaptcha))
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "encryptedCaptcha after DecryptString is empty", lang);

                partialLock = JsonConvert.DeserializeObject<UserPartialLockModel>(encryptedCaptcha);
                if (partialLock == null)
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "partialLock model after DeserializeObject is null", lang);

                if (partialLock.LockDueDate >= DateTime.Now)
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "LoginUserPartialLock", $"user is still locked as partialLock.LockDueDate = {partialLock.LockDueDate}", lang);

                if (partialLock.ErrorTimesUserTries >= 10)
                    return HandleUserPartialLockReturnModel(partialLock.SessionId, true, DateTime.Now.AddMinutes(5), partialLock.ErrorTimesUserTries, "LoginUserPartialLock", $"user is partially locked as error tries times is: {partialLock.ErrorTimesUserTries}", lang);

                var clearText = $"{partialLock.SessionId}_{SHARED_PARTIAL_LOCK_SECRET}_{userIP}";
                if (!SecurityUtilities.VerifyHashedData(partialLock.Hashed, clearText))
                    return HandleUserPartialLockReturnModel(partialLock.SessionId, false, DateTime.Now, partialLock.ErrorTimesUserTries, "ErrorGeneric", $"Hashed Not Matched as we received hashed:{partialLock.Hashed}, clearText is: {clearText} and hash is {SecurityUtilities.HashData(clearText, null)}", lang);

                return partialLock;
            }
            catch (Exception ex)
            {
                return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 1, "ErrorGeneric", $"Exception error when ValidateIfUserPartiallyLocked: {ex.ToString()}", lang);
            }
        }

        private UserPartialLockModel HandleUserPartialLockReturnModel(string sessionId, bool isLocked, DateTime lockedDueDate, int errorTimesUserTries, string errorDescriptionKey, string logDescription, string lang)
        {
            UserPartialLockModel partialLock = new UserPartialLockModel()
            {
                LogDescription = logDescription,
                ErrorDescription = WebResources.ResourceManager.GetString(errorDescriptionKey, CultureInfo.GetCultureInfo(lang)),
                ErrorTimesUserTries = errorTimesUserTries,
                IsLocked = isLocked,
                LockDueDate = lockedDueDate,
                SessionId = sessionId,
            };
            return partialLock;
        }

        private string GeneratePartiallyLockHashedKey(string userIP, UserPartialLockModel lockModel)
        {
            string clearText = $"{lockModel.SessionId}_{SHARED_PARTIAL_LOCK_SECRET}_{userIP}";
            lockModel.Hashed = SecurityUtilities.HashData(clearText, null);

            return AESEncryption.EncryptString(JsonConvert.SerializeObject(lockModel), SHARED_PARTIAL_LOCK_SECRET);
        }

        #endregion

        #endregion
    }
}





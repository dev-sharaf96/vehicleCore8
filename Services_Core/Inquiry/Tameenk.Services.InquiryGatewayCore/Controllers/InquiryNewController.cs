using MoreLinq;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Resources.Inquiry;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.InquiryGateway.Services.Core;
using VehicleModel = Tameenk.Services.Inquiry.Components.VehicleModel;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.Vehicles;
using Tameenk.Core.Domain.Entities;
using static Tameenk.Services.InquiryGateway.Controllers.InquiryController;
using Tameenk.Services.Core.Payments;
using Tameenk.Security.Encryption;
using Tameenk.Resources.Checkout;
using Tameenk.Core.Domain.Entities.Payments;
using DriverModel = Tameenk.Services.Inquiry.Components.DriverModel;
using DriverExtraLicenseModel = Tameenk.Services.Inquiry.Components.DriverExtraLicenseModel;
using Tameenk.Resources.WebResources;
using Tameenk.Security.CustomAttributes;
using Tameenk.Core.Data;
using System.Data.Entity;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Redis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.InquiryGateway.Controllers
{
    public class InquiryNewController : BaseApiController
    {
        #region Fields
        private readonly IAddressService _addressService;
        private readonly IVehicleService _vehicleService;
        private readonly IWebApiContext _webApiContext;
        private readonly IQuotationService _quotationService;
        private readonly IHttpClient _httpClient;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IInquiryContext inquiryContext;
        private readonly IPaymentService _paymentService;
        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";
        private const string SHARED_Key = "bC@nAtIoNaL_2020_AdDrEsS_hAsHKEy";
        private readonly HashSet<string> allowedLanguage = new HashSet<string>() { "ar", "en" };

        private const string InquiryNinVehiclesCach_Base_KEY = "iNqUiRy_NiN_vEhIcLeS_cAcH";
        private const int cach_TiMe = 24 * 60 * 60;

        public IRepository<Driver> _driverRepository { get; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="quotationRequestService"></param>
        /// <param name="addressService">Address Service</param>
        /// <param name="vehicleService">Vehicle Service</param>
        /// <param name="webApiContext">Web Api Context</param>
        /// <param name="quotationService">quotation Service</param>
        /// <param name="httpClient">Http client</param>
        /// <param name="authorizationService">Authentication Service</param>
        /// <param name="logger">I Logger</param>
        /// <param name="tameenkConfig"></param>
        public InquiryNewController(
            IQuotationRequestService quotationRequestService, IAddressService addressService
            , IVehicleService vehicleService, IWebApiContext webApiContext,
            IQuotationService quotationService,
            IHttpClient httpClient,
            ILogger logger,
            IAuthorizationService authorizationService,
            TameenkConfig tameenkConfig,
            IInquiryContext inquiryContext,
             IPaymentService paymentService,
             IRepository<Vehicle> vehRepository,
             IRepository<Driver> driverRepository
            )
        {
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));
            _vehicleService = vehicleService ?? throw new TameenkArgumentNullException(nameof(IVehicleService));
            _webApiContext = webApiContext ?? throw new TameenkArgumentNullException(nameof(IWebApiContext));
            _httpClient = httpClient ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
            _quotationRequestService = quotationRequestService ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
            _authorizationService = authorizationService ?? throw new TameenkArgumentNullException(nameof(IAuthorizationService));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));
            _config = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            this.inquiryContext = inquiryContext;
            _quotationService = quotationService ?? throw new TameenkArgumentNullException(nameof(IQuotationService));
            _paymentService = paymentService ?? throw new TameenkArgumentNullException(nameof(IPaymentService));
           // VehRepository = vehRepository;
            _driverRepository = driverRepository;
        }

        #endregion


        #region Methods 
        //[HttpPost]
        //[Route("api/InquiryNew/init-inquiry-request")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryOutputModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<InquiryOutputModel>))]
        //public IHttpActionResult InitInquiryRequest([FromBody]InitInquiryRequestModel model, Guid? parentRequestId = null)
        //{
        //    model.InquiryOutputModel = new InquiryOutputModel();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.NationalId))
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.NationalIdRequired, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }
        //        if (string.IsNullOrEmpty(model.SequenceNumber))
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.SequenceNumberRequired, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }
        //        if (model.VehicleIdTypeId == 0)
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.VehicleIdTypeIdRequired, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }
        //        if (model.PolicyEffectiveDate == null)
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.PolicyEffectiveDateRequired, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }
        //        if (!ValidateCaptcha(model.CaptchaToken, model.CaptchaInput))
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvalidCaptcha, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }
        //        if (model.OwnerTransfer)
        //        {
        //            if (!_vehicleService.ValidateOwnerTransfer(model.OwnerNationalId, model.NationalId))
        //            {
        //                SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvaildOldOwnerNin, "InitInquiryRequest", model.InquiryOutputModel);
        //                return Error(model.InquiryOutputModel);
        //            }
        //        }
        //        if (!ValidPolicyEffectiveDate(model.PolicyEffectiveDate))
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvalidPolicyEffectiveDate, "InitInquiryRequest", model.InquiryOutputModel);
        //            return Error(model.InquiryOutputModel);
        //        }

        //        //  var userId = _authorizationService.GetUserId(User);
        //        var result = _quotationRequestService.InitInquiryRequest(model);
        //        if (!(string.IsNullOrEmpty(result.InquiryOutputModel.Description)) && result.InquiryOutputModel.StatusCode != 0)
        //        {
        //            return Error(result.InquiryOutputModel);
        //        }
        //        else
        //        {
        //            if (result.IsVehicleExist && result.IsMainDriverExist)
        //            {
        //                // Init Complete Submit to get Quations without redirect to UI
        //                InquiryRequestModel inquiryRequestModel = MapToInquiryRequestModel(model);
        //                inquiryRequestModel.Drivers = result.Drivers;
        //                inquiryRequestModel.Insured = result.Insured;
        //                inquiryRequestModel.Vehicle = result.Vehicle;
        //                inquiryRequestModel.CityCode = result.CityCode.Value;
        //                InquiryOutputModel resultObj = SubmitInquiryAsync(inquiryRequestModel).Result;
        //                return Ok(resultObj);
        //            }
        //            else
        //            {
        //                model.InquiryOutputModel.InitInquiryResponseModel = result;
        //                SetInquiryOutputModel((int)HttpStatusCode.OK, SubmitInquiryResource.Success, "InitInquiryRequest", model.InquiryOutputModel);
        //                return Ok(model.InquiryOutputModel);
        //            }

        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        SetInquiryOutputModel((int)HttpStatusCode.InternalServerError, exp.GetBaseException().Message, "InitInquiryRequest", model.InquiryOutputModel);
        //        model.InquiryOutputModel.Description = SubmitInquiryResource.InvalidData;
        //        return Error(model.InquiryOutputModel);
        //    }
        //}


        [HttpPost]
        [Route("api/InquiryNew/init-inquiry-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(InquiryOutput))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InquiryOutput))]
        public InquiryOutput InitInquiryRequest([FromBody] Tameenk.Services.Inquiry.Components.InitInquiryRequestModel model)
        {
            var output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.MethodName = "InitInquiryRequest";
            log.Channel = model.Channel.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequestId = model.ParentRequestId;
            log.NIN = model.NationalId;
            log.VehicleId = model.SequenceNumber;
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            if (model.Channel != Channel.Mobile &&
                model.Channel != Channel.android &&
                model.Channel != Channel.ios)
            {
                log.PolicyEffectiveDate = model.PolicyEffectiveDate;
            }
            log.MobileVersion = model.MobileVersion;
            try
            {
                var userId = User.Identity.GetUserId() == null ? Guid.Empty : new Guid(User.Identity.GetUserId());
                var userName = User.Identity.GetUserName();
                log.UserId = userId.ToString();
                log.UserName = userName;
                //if (model.Channel != Channel.Mobile &&
                //    model.Channel != Channel.android &&
                //    model.Channel != Channel.ios)
                //{
                    // Validate Captcha
                    var encryptedCaptcha = AESEncryption.DecryptString(model.CaptchaToken, SHARED_SECRET);
                    try
                    {
                        var captchaToken = JsonConvert.DeserializeObject<CaptchaToken>(encryptedCaptcha);
                        if (captchaToken.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
                        {
                            output.ErrorCode = InquiryOutput.ErrorCodes.ExpiredCaptcha;
                            output.ErrorDescription = SubmitInquiryResource.InvalidCaptcha;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Expired Captcha";
                            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                            return output;
                        }
                        if (!captchaToken.Captcha.Equals(model.CaptchaInput, StringComparison.Ordinal))
                        {
                            output.ErrorCode = InquiryOutput.ErrorCodes.WrongInputCaptcha;
                            output.ErrorDescription = SubmitInquiryResource.WrongInputCaptcha;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Wrong Input Captcha";
                            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                            return output;
                        }
                    }
                    catch (Exception)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.InvalidCaptcha;
                        output.ErrorDescription = SubmitInquiryResource.InvalidCaptcha;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Invalid Captcha as we recieve encryptedCaptcha: " + encryptedCaptcha;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
              //  }
                output = inquiryContext.InitInquiryRequest(model,log);
                if(output.ErrorCode==InquiryOutput.ErrorCodes.Success&& model.Channel == Channel.ios)
                {
                   if (output.InitInquiryResponseModel!=null && output.InitInquiryResponseModel.CityCode==null)
                        output.InitInquiryResponseModel.CityCode = 0;
                }
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
               
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "InitInquiryRequest-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }

        [HttpPost]
        [SingleSessionAuthorizeAttribute]
        [Route("api/InquiryNew/submit-inquiry-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(InquiryOutput))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InquiryOutput))]
        public InquiryOutput SubmitInquiryRequest([FromBody]InquiryRequestModel model)
        {
            var result = new InquiryOutput();
            try
            {
                if (string.IsNullOrEmpty(model.Channel))
                    model.Channel = "portal";
                string currentUserId = _authorizationService.GetUserId(User);
                Guid userId = Guid.Empty;
                Guid.TryParse(currentUserId, out userId);
                // var userId = User.Identity.GetUserId() == null ? Guid.Empty : new Guid(User.Identity.GetUserId());
                var userName = User.Identity.GetUserName();
                result = inquiryContext.SubmitInquiryRequest(model, model.Channel,userId, userName);
                if (result.ErrorCode == InquiryOutput.ErrorCodes.Success && result.InquiryResponseModel != null)
                {
                    if (result.InquiryResponseModel.Vehicle.VehicleMakerCode.HasValue)
                    {
                        var VehicleMakerCode = result.InquiryResponseModel.Vehicle.VehicleMakerCode.Value.ToString("D4");
                        result.InquiryResponseModel.Vehicle.CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg";

                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                var lang = model.Language;
                if (string.IsNullOrEmpty(lang) || !allowedLanguage.Contains(lang))
                    lang = "ar";

                result.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                result.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "SubmitInquiryRequest-Exception";
                log.ErrorCode = (int)result.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return result;
            }
        }




        [HttpPost]
        [SingleSessionAuthorizeAttribute]
        [Route("api/InquiryNew/submit-yakeen-missing-fields")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult SubmitYakeenMissingFields([FromBody]YakeenMissingInfoRequestModel model)
        {
            try
            {
                //if (model.YakeenMissingFields.VehicleMakerCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleMaker) && model.YakeenMissingFields.VehicleMaker != "غير متوفر")))
                //{
                //    model.YakeenMissingFields.VehicleMakerCode = int.Parse(model.YakeenMissingFields.VehicleMaker);
                //}
                //if (model.YakeenMissingFields.VehicleModelCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleModel) && model.YakeenMissingFields.VehicleModel != "غير متوفر")))
                //{
                //    model.YakeenMissingFields.VehicleModelCode = int.Parse(model.YakeenMissingFields.VehicleModel);
                //}

                //var result = _quotationRequestService.UpdateQuotationRequestWithYakeenMissingFields(model);
                string currentUserId = _authorizationService.GetUserId(User);
                Guid userId = Guid.Empty;
                Guid.TryParse(currentUserId, out userId);
                var userName = User.Identity.GetUserName();
                var result = inquiryContext.SubmitMissingFeilds(model, userId.ToString(), userName);
                if(result.ErrorCode==InquiryOutput.ErrorCodes.Success&&result.InquiryResponseModel.IsValidInquiryRequest)
                {
                    return Ok(result.InquiryResponseModel);
                }
                else
                    return Error2(result.InquiryResponseModel);
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric; 
                
                InquiryRequestLog log = new InquiryRequestLog();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                if (log.Headers["User-Agent"].ToString().Contains("Tameenak/1"))
                    log.Channel = "ios";
                else if (log.Headers["User-Agent"].ToString().Contains("okhttp/"))
                    log.Channel = "android";
                else
                    log.Channel = "Portal";
                log.MethodName = "SubmitYakeenMissingFields-Exception";
                log.ServiceRequest = JsonConvert.SerializeObject(model);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return Error(output);
            }

        }

        [HttpGet]
        [SingleSessionAuthorizeAttribute]
        [Route("api/InquiryNew/edit-inquiry-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult EditInquiryRequest(string eid, int r=0, string re="")
        {
            try
            {
                if (string.IsNullOrEmpty(eid))
                    throw new TameenkArgumentNullException("externalId");


                if (r == 0)
                {
                    var res = _quotationService.GetQuotationRequestByExternal(eid);
                    return Ok(InitializeEditRequestResponseModel(res, false, re,eid));
                }
                else
                {
                        string exception = string.Empty;
                        var res = _quotationService.GetQuotationRequestByExternalNew(eid, out exception);
                    if (res == null)
                    {
                        var newres = _quotationService.GetQuotationRequestByExternal(eid);
                        return Ok(InitializeEditRequestResponseModel(newres, false, re, eid));
                    }
                    else if (!string.IsNullOrEmpty(exception))
                    {
                       
                            InquiryRequestLog log = new InquiryRequestLog();
                            log.ServerIP = Utilities.GetInternalServerIP();
                            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                            log.UserIP = Utilities.GetUserIPAddress();
                            log.MethodName = "EditInquiryRequest-Exception";
                            log.ErrorDescription = "error get quotatio data error is : " + exception;
                            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                            return Error("Erro Happen");
                    }

                    var response = InitializeEditRequestResponseModelNew(res, true, re, eid, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {

                        InquiryRequestLog log = new InquiryRequestLog();
                        log.ServerIP = Utilities.GetInternalServerIP();
                        log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                        log.UserIP = Utilities.GetUserIPAddress();
                        log.MethodName = "EditInquiryRequest-Exception";
                        log.ErrorDescription = "error InitializeEditRequest : " + exception;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return Error("Erro Happen");
                    }
                        return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var lang = "ar";
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "EditInquiryRequest-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }
        }

        /// <summary>
        /// Return list of all occupatins Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiryNew/all-occipations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetOccupations()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Occupation>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetOccupations-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }
        /// <summary>
        /// Return list of all educations Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/all-educations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetEducations(string lang = "")
        {
            try
            {

                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Education>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(Education))
                           .Cast<Education>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {
                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = EducationResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetEducations-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }


        /// <summary>
        /// Return list of all medical condition Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/all-medical-conditions")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetMedicalConditionss(string lang = "")
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<MedicalCondition>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(MedicalCondition))
                           .Cast<MedicalCondition>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {

                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = MedicalConditionResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetMedicalConditionss-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }

        }


        /// <summary>
        /// Return list of all transimission types Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/all-transimission-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetTransimissionTypes(string lang = "")
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<TransmissionType>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(TransmissionType))
                           .Cast<TransmissionType>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {
                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = TransmissionTypeResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetTransimissionTypes-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }


        /// <summary>
        /// Return list of all cities Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/all-cities")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.Inquiry.Components.CityModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCities(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_addressService.GetAllFromCities().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetCities-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        /// <summary>
        /// Get driving violations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/violations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetViolations(string lang = "")
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Violation>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(Violation))
                           .Cast<Violation>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {

                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = ViolationResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetViolations-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        /// <summary>
        /// Get parking locations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/parking-locations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetParkingLocations(string lang = "")//
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<ParkingLocation>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(ParkingLocation))
                           .Cast<ParkingLocation>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {

                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = ParkingLocationResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetParkingLocations-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        /// <summary>
        /// Get braking systems.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/braking-systems")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetBrakingSystems()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<BrakingSystem>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetBrakingSystems-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }


        /// <summary>
        /// Get cruise control types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/cruise-control-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCruiseControlTypes()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<CruiseControlType>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetCruiseControlTypes-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }

        }

        /// <summary>
        /// Get parking sensors.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/parking-sensors")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetParkingSensors()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<ParkingSensors>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetParkingSensors-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        /// <summary>
        /// Get parking sensors.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/camera-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCameraTypes()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<VehicleCameraType>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetCameraTypes-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        [HttpGet]
        [Route("api/InquiryNew/all-countries")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.Inquiry.Components.CountryModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCountries(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_addressService.GetCountries(pageIndex, pageSize).Where(a=>a.Code!=113).Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetCountries-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }

        }


        /// <summary>
        /// Get Quotation Request by External Id
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/quotation-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult GetQuotationRequest(string externalId)
        {
            try
            {
                if (string.IsNullOrEmpty(externalId))
                    throw new TameenkArgumentNullException("externalId");

                var res = _quotationService.GetQuotationRequest(externalId);
                return Ok(res.ToModel());
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetQuotationRequest-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }
        }

        /// <summary>
        /// Delete Vehicle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [Route("api/InquiryNew/user-delete-vehicle")]
        public IActionResult DeleteVehicle(String id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");

                Guid guidResult = new Guid();
                bool isValid = Guid.TryParse(id, out guidResult);

                if (!isValid)
                    throw new TameenkArgumentException("Not Vaild id", "id");


                if (_vehicleService.CheckVehicleAttachToVaildPolicy(id))
                {
                    throw new ArgumentException("Can\'t delete this vehicle");
                }


                Vehicle vehicle = _vehicleService.GetVehicle(id);

                if (vehicle == null)
                {
                    throw new ArgumentNullException(nameof(vehicle));
                }

                _vehicleService.DeleteVehicle(vehicle);
                return Ok(true);
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "DeleteVehicle-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }


        }

        /// <summary>
        /// Get vehicle for specific user 
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<VehicleModel>>))]
        [Route("api/InquiryNew/user-vehicle")]
        public IActionResult GetVehicleForUser(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");

                var result = _vehicleService.GetVehicleForUser(id, pageIndx, pageSize);

                //then convert to model
                IEnumerable<VehicleModel> dataModel = result.Select(e => e.ToModel());


                dataModel = dataModel.ToList();



                var language = _webApiContext.CurrentLanguage;
                var Makers = _vehicleService.VehicleMakers();

                foreach (var vehicle in dataModel)
                {
                    var maker = vehicle.VehicleMakerCode.HasValue ?
                        Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
                         Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);


                    if (maker != null)
                    {
                        var Models = _vehicleService.VehicleModels(maker.Code);

                        if (Models != null)
                        {
                            var model = vehicle.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == vehicle.VehicleModelCode) :
                                Models.FirstOrDefault(m => m.ArabicDescription == vehicle.Model || m.EnglishDescription == vehicle.Model);

                            if (model != null)
                                vehicle.Model = (language == LanguageTwoLetterIsoCode.Ar ? model.ArabicDescription : model.EnglishDescription);
                        }

                        vehicle.VehicleMaker = (language == LanguageTwoLetterIsoCode.Ar ? maker.ArabicDescription : maker.EnglishDescription);
                    }

                }

                dataModel = dataModel.DistinctBy(d => new
                {
                    d.RegisterationPlace,
                    d.ModelYear,
                    d.VehicleMaker,
                    d.VehicleMakerCode,
                    d.VehicleModelCode,
                    d.Model,
                    d.CarPlateNumber,
                    d.CarPlateText1,
                    d.CarPlateText2,
                    d.CarPlateText3
                });


                return Ok(dataModel, dataModel.Count());

            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetVehicleForUser-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }

        }


        /// <summary>
        /// Get Address for specific user 
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.Inquiry.Components.AddressModel>>))]
        [Route("api/InquiryNew/user-address")]
        public IActionResult GetAddressesForUser(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            try
            {


                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");

                var result = _addressService.GetAddressesForUser(id, pageIndx, pageSize);

                //then convert to model

                IEnumerable<Tameenk.Services.Inquiry.Components.AddressModel> dataModel = result.Select(e => e.ToModel()).Distinct();
                dataModel = dataModel.DistinctBy(p => new { p.City, p.District, p.Street, p.RegionName });

                return Ok(dataModel, dataModel.Count());

            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetAddressesForUser-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }

        }

        /// <summary>
        /// Get all vehicle body types
        /// </summary>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/vehicle-body-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehicleBodyTypes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_vehicleService.VehicleBodyTypes(pageIndex, pageSize)
                    .Select(e => new IdNamePairModel()
                    {
                        Id =e.Code,
                        Name = _webApiContext.CurrentLanguage == LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                    }));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetVehicleBodyTypes-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);

            }
        }

        /// <summary>
        /// Return list of all vehicle maker Id Name pairs
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/vehicle-makers")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehicleMakers(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_vehicleService.VehicleMakers(pageIndex, pageSize)
                    .Select(e => new IdNamePairModel()
                    {
                        Id = e.Code,
                        Name = _webApiContext.CurrentLanguage == LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                    }));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetVehicleMakers-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }
        }

        /// <summary>
        /// Get vehicle models of given maker.
        /// </summary>
        /// <param name="vehicleMakerId">Vehicle Maker</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/vehicle-models")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehicleModels(int vehicleMakerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_vehicleService.VehicleModels(vehicleMakerId, pageIndex, pageSize)
                    .Select(e => new IdNamePairModel()
                    {
                        Id = Convert.ToInt32(e.Code),
                        Name = _webApiContext.CurrentLanguage == LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                    }));
            }
            catch (Exception ex)
            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetVehicleModels-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }
        }

        /// <summary>
        /// Get vehicle models of given maker.
        /// </summary>
        /// <param name="id">Vehicle Maker</param>
        /// <param name="lang">Page index</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/vehcileModels")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<LookUpTemp>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetVehcileModels(int id,string lang = "ar")
        {
            try
            {
                List<LookUpTemp> lookups = new List<LookUpTemp>();
                List<Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel> data = _vehicleService.GetVehicleModels(id);
                foreach (Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel model in data)
                {
                    lookups.Add(new LookUpTemp()
                    {
                        id = Convert.ToInt32(model.Code),
                        name = lang == "ar" ? model.ArabicDescription : model.EnglishDescription

                    });
                }

                return Ok(lookups);

            }
            catch (Exception ex)
            {
              
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetVehcileModels-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        [HttpGet]
        [Route("api/InquiryNew/Kilometers")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetKilometers(string lang = "")
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Mileage>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(Mileage))
                           .Cast<Mileage>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {
                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = MileageResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetKilometers-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }

        /// <summary>
        /// Return list of all relationsShip Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InquiryNew/all-relationsShip")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetRelationsShip(string lang = "")
        {
            try
            {
                var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<RelationShip>().Select(e => e.ToModel());
                if (!string.IsNullOrEmpty(lang))
                {
                    LookupsOutput output = new LookupsOutput();
                    output.Result = new List<Lookup>();
                    var listKeys = Enum.GetValues(typeof(RelationShip))
                           .Cast<RelationShip>()
                           .Select(v => new
                           {
                               id = v,
                               key = v.ToString()
                           })
                           .ToList();
                    foreach (var item in listKeys)
                    {
                        output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = RelationShipResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
                    }
                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return Single(output);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetRelationsShip-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);
            }

        }














        /// <summary>        /// Return list of all Number Of Accident Last 5 Years Range, Id Name Pairs        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/InquiryNew/getNumberOfAccidentLast5YearsRange")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetNumberOfAccidentLast5YearsRange(string lang = "")        {            try            {                if (string.IsNullOrEmpty(lang))                {                    List<IdNamePair> numberOfAccidentLast5YearsRange = new List<IdNamePair>();                    var item = new IdNamePair() { Id = 0, Name = GeneralMessages.None };                    numberOfAccidentLast5YearsRange.Add(item);                    for (int i = 1; i < 11; i++)                    {                        item = new IdNamePair() { Id = i, Name = i.ToString() };                        numberOfAccidentLast5YearsRange.Add(item);                    }                    return Ok(numberOfAccidentLast5YearsRange.Select(e => e.ToModel()));                }                else                {                    LookupsOutput output = new LookupsOutput();                    output.Result = new List<Lookup>();                    var item = new Lookup() { Id = "0", Name = GeneralMessages.ResourceManager.GetString("None", CultureInfo.GetCultureInfo(lang)) };
                    output.Result.Add(item);
                    for (int i = 1; i < 11; i++)                    {                        output.Result.Add(new Lookup() { Id = i.ToString(), Name = i.ToString() });                    }                    output.ErrorCode = LookupsOutput.ErrorCodes.Success;                    output.ErrorDescription = "Success";                    return Single(output);                }            }            catch (Exception ex)            {

                LookupsOutput output = new LookupsOutput();
                output.ErrorCode = LookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetNumberOfAccidentLast5YearsRange-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);            }        }

















        /// <summary>        /// Return list of License Years, Id Name Pairs        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/InquiryNew/getLicenseYearsList")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetLicenseYearsList()        {            try            {                List<IdNamePair> licenseYears = new List<IdNamePair>();                var item = new IdNamePair();                for (int i = 1; i < 21; i++)                {                    item = new IdNamePair() { Id = i, Name = i.ToString() };                    licenseYears.Add(item);                }                return Ok(licenseYears.Select(e => e.ToModel()));            }            catch (Exception ex)            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetLicenseYearsList-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);            }        }

        /// <summary>
        /// return national address for specific nationa id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpPost]        //[Route("api/InquiryNew/getNationalAddress")]        //[AllowAnonymous]        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<NationalAddressOutput>))]        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        //public IHttpActionResult GetNationalAddress([FromBody] Models.NationalAddressModel model)        //{        //    NationalAddressOutput output = new NationalAddressOutput();        //    InquiryRequestLog log = new InquiryRequestLog();        //    log.MethodName = "GetNationalAddress";
        //    log.Channel = model.Channel;
        //    log.NIN = model.NationalId;
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    try        //    {        //        if (string.IsNullOrEmpty(model.NationalId))
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.NationalIdIsNull;
        //            output.ErrorDescription = "National Id is empty";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Ok(output);
        //        }        //        if (string.IsNullOrEmpty(model.BirthDate))
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.DateOfBirthGIsEmpty;
        //            output.ErrorDescription = "Birth date is empty";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Ok(output);
        //        }        //        if (string.IsNullOrEmpty(model.Channel))
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.ChannelIsEmpty;
        //            output.ErrorDescription = "Channel is empty";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Ok(output);
        //        }        //        var hky = Request.Headers.GetValues("_hky");        //        if (hky == null)
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.HashedIsEmpty;
        //            output.ErrorDescription = "Hashed key is empty";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Ok(output);
        //        }        //        var hkyFromChannel = hky.First();        //        var clearText = model.NationalId + "_" + model.Channel + "-" + SHARED_Key;
        //        if (!SecurityUtilities.VerifyHashedData(hkyFromChannel, clearText))
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.HashedNotMatched;
        //            output.ErrorDescription = "Hashed key not match";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Ok(output);
        //        }        //        var result = inquiryContext.GetNationalAddress(model.NationalId, model.BirthDate, model.Channel, model.FromYakeen);        //        if (result == null)
        //        {
        //            output.ErrorCode = NationalAddressOutput.ErrorCodes.ResultIsNull;
        //            output.ErrorDescription = "Service result is null";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Error(output);
        //        }        //        if (result.ErrorCode != NationalAddressOutput.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = result.ErrorCode;
        //            output.ErrorDescription = "Error happend when get addresses";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = result.ErrorDescription;
        //            InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //            return Error(output);
        //        }        //        log.ErrorCode = (int)result.ErrorCode;
        //        log.ErrorDescription = "Success";
        //        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //        output = result;
        //        return Ok(output);        //    }        //    catch (Exception ex)        //    {        //        output.ErrorCode = NationalAddressOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = ex.ToString();
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
        //        return Error(output);        //    }        //}

        [HttpGet]        [Route("api/InquiryNew/getVehiclesByNin")]
        public async Task<IActionResult> GetVehiclesInfo(string Nin)        {            InquiryOutput output = new InquiryOutput();            InquiryRequestLog log = new InquiryRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.MethodName = "GetVehiclesInfoByNin";
            log.NIN = Nin;
            try            {
                if (string.IsNullOrEmpty(Nin))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Nin is empty"; // WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Nin is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return Error(output);
                }

                //NinVehiclesCachingModel cachedVehicles = await ValidateIfUserHasCachedData($"{InquiryNinVehiclesCach_Base_KEY}_{Nin}");
                //if (cachedVehicles.Vehicles != null || cachedVehicles.TriesCount >= 3)
                //    return Ok(cachedVehicles.Vehicles);

                string exception = string.Empty;
                var result = _vehicleService.GetVehicleInfoByNin(Nin, out exception);
                if (!string.IsNullOrEmpty(exception) || result == null || result.Count <= 0)
                {
                    //if (cachedVehicles.TriesCount < 3)
                    //{
                    //    cachedVehicles.TriesCount += 1;
                    //    var remainingTime = GetRemainingExpirationSeconds(cachedVehicles.ExpirationDate);
                    //    await _redisCacheManager.UpdateAsync($"{InquiryNinVehiclesCach_Base_KEY}_{Nin}", cachedVehicles, (remainingTime > 0 ? remainingTime : cach_TiMe));
                    //}

                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception)? $"Get Vehicle Info return exception: {exception}" : $"No result return for {Nin}";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return Error(output);
                }

                //await _redisCacheManager.SetAsync($"{InquiryNinVehiclesCach_Base_KEY}_{Nin}", HandleUserPartialLockReturnModel(result, cachedVehicles.SessionId, cachedVehicles.TriesCount, cachedVehicles.ExpirationDate), cach_TiMe);

                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("ar"));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return Ok(result);
            }            catch (Exception ex)            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return Error(output);            }        }

        //private async Task<NinVehiclesCachingModel> ValidateIfUserHasCachedData(string cachKey)
        //{
        //    var sessionId = Guid.NewGuid().ToString();

        //    try
        //    {
        //        var cachedData = await _redisCacheManager.GetAsync<NinVehiclesCachingModel>($"{cachKey}");
        //        if (cachedData != null && ((cachedData.Vehicles != null && cachedData.Vehicles.Count >= 1) || cachedData.TriesCount >= 3))
        //            return cachedData;

        //        if (cachedData == null)
        //            return HandleUserPartialLockReturnModel(null, sessionId, 1, DateTime.Now.AddDays(1));
                
        //        if (cachedData.TriesCount >= 3)
        //            return HandleUserPartialLockReturnModel(cachedData.Vehicles, cachedData.SessionId, (cachedData.TriesCount + 1), cachedData.ExpirationDate);

        //        return HandleUserPartialLockReturnModel(cachedData.Vehicles, cachedData.SessionId, (cachedData.TriesCount + 1), cachedData.ExpirationDate);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleUserPartialLockReturnModel(null, sessionId, 1, DateTime.Now.AddDays(1));
        //    }
        //}

        //private NinVehiclesCachingModel HandleUserPartialLockReturnModel(List<VehicleInfo> vehicles, string sessionId,int timesUserTries, DateTime expiryDate)
        //{
        //    NinVehiclesCachingModel partialLock = new NinVehiclesCachingModel()
        //    {
        //        SessionId = sessionId,
        //        TriesCount = timesUserTries,
        //        Vehicles = vehicles,
        //        ExpirationDate = expiryDate
        //    };
        //    return partialLock;
        //}

        //private int GetRemainingExpirationSeconds(DateTime expirationDate)
        //{
        //    try
        //    {
        //        if (DateTime.Now >= expirationDate)
        //            return 0;

        //        var _remainingSeconds = (expirationDate - DateTime.Now).TotalSeconds;
        //        int.TryParse(_remainingSeconds.ToString(), out int remainingSeconds);
        //        return remainingSeconds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        #endregion


        #region Private Methods 

        private bool ValidateCaptcha(string token, string input)
        {
            try
            {
                var requestModel = new ValidateCaptchaModel
                {
                    Token = token,
                    Input = input
                };

                var validateTask = _httpClient.PostAsync($"{_config.Identity.Url}api/identity/captcha/validate/", requestModel, authorizationToken: AuthorizationToken);
                validateTask.Wait();
                var result = validateTask.Result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = JsonConvert.DeserializeObject<CommonResponseModel<bool>>(result);
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Inquiry controller -> ValidateCaptcha (token : {token}, input {input})", ex);
            }
            return false;
        }

        //private InitInquiryResponseModel InitializeEditRequestResponseModel(CheckoutDetail quotationRequest, bool isRenual,string referenceId)
        //{
        //    InitInquiryResponseModel responseModel = new InitInquiryResponseModel();
        //    responseModel.IsRenualRequest = isRenual;
        //    responseModel.ExternalId = quotationRequest.ExternalId;
        //    responseModel.ReferenceId = referenceId;
        //    //responseModel.CityCode = quotationRequest.CityCode;
        //    responseModel.PolicyEffectiveDate =  DateTime.Now;
        //    responseModel.IsVehicleUsedCommercially = quotationRequest.Vehicle.IsUsedCommercially ?? false;
        //    responseModel.IsCustomerCurrentOwner = !quotationRequest.Vehicle.OwnerTransfer;

        //    long CarOwnerNINOutput = 0;
        //    long.TryParse(quotationRequest.Vehicle.CarOwnerNIN, out CarOwnerNINOutput);
        //    responseModel.OldOwnerNin = CarOwnerNINOutput;

        //    responseModel.IsCustomerSpecialNeed = quotationRequest.Driver.IsSpecialNeed;
        //    responseModel.Insured = new InsuredModel();
        //    responseModel.Insured.InsuredWorkCityCode = quotationRequest.Insured.WorkCityId;
        //    responseModel.Insured.EducationId = quotationRequest.Insured.EducationId;
        //    responseModel.Insured.ChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years ?? 0;
        //    responseModel.Insured.NationalId = quotationRequest.Insured.NationalId;

        //    responseModel.Vehicle = quotationRequest.Vehicle.ToModel();

        //    responseModel.Drivers = new List<DriverModel>();

        //    if (quotationRequest.Vehicle != null)
        //        responseModel.IsVehicleExist = true;
        //    if (quotationRequest.Driver != null)
        //        responseModel.IsMainDriverExist = true;

        //    string[] mainDriverBirthDateParts = null;

        //    if (quotationRequest.Driver.IsCitizen && !string.IsNullOrWhiteSpace(quotationRequest.Driver.DateOfBirthH))
        //    {
        //        mainDriverBirthDateParts = quotationRequest.Driver.DateOfBirthH.Split('-');
        //    }
        //    else if (!quotationRequest.Driver.IsCitizen)
        //    {
        //        mainDriverBirthDateParts = quotationRequest.Driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
        //    }

        //    if (mainDriverBirthDateParts != null && mainDriverBirthDateParts.Length > 2)
        //    {
        //        responseModel.Insured.BirthDateMonth = Convert.ToByte(mainDriverBirthDateParts[1]);
        //        responseModel.Insured.BirthDateYear = short.Parse(mainDriverBirthDateParts[2]);
        //    }

        //    var drivers = quotationRequest.Drivers.ToList();
        //    //to make main driver on the top of list to be passed to angular in driver[0]
        //    if (quotationRequest.Insured.NationalId.ToString().StartsWith("7"))
        //    {
        //        Driver insured = new Driver();
        //        insured.NIN = quotationRequest.Insured.NationalId;
        //        drivers.Insert(0, insured);
        //        var mainDriver = quotationRequest.Driver;// drivers.FirstOrDefault(d => d.NIN == quotationRequest.Driver.NIN);
        //        mainDriver.DrivingPercentage = 100;
        //        drivers.Remove(mainDriver);
        //        drivers.Insert(1, mainDriver);
        //    }
        //    else
        //    {

        //        var mainDriver = drivers.FirstOrDefault(d => d.NIN == quotationRequest.Insured.NationalId);
        //        drivers.Remove(mainDriver);
        //        drivers.Insert(0, mainDriver);
        //    }
        //    var driversExtraLicenses = quotationRequest.Insured.InsuredExtraLicenses.OrderByDescending(i => i.IsMainDriver); //to make main driver on the top of list to be passed to angular in driver[0]
        //    DriverModel driverModel;
        //    foreach (var driver in drivers)
        //    {
        //        driverModel = new DriverModel();
        //        driverModel.NationalId = driver.NIN;
        //        driverModel.DriverNOALast5Years = driver.NOALast5Years ?? 0;
        //        driverModel.MedicalConditionId = driver.MedicalConditionId ?? 0;
        //        driverModel.EducationId = driver.EducationId;
        //        driverModel.ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0;
        //        driverModel.DriverHomeCityCode = driver.CityId.HasValue ? (int?)driver.CityId.Value : null;
        //        driverModel.DriverWorkCityCode = driver.WorkCityId.HasValue ? (int?)driver.WorkCityId.Value : null;
        //        driverModel.DrivingPercentage = driver.DrivingPercentage ?? 0;        //        driverModel.RelationShipId = driver.RelationShipId ?? 0;        //        if (driver.DriverViolations != null && driver.DriverViolations.Any())
        //        {
        //            //driverModel.ViolationIds = driver.DriverViolations.Select(d => d.ViolationId).ToList();
        //            List<int> violations=  driver.DriverViolations
        //             .Where(dv => dv.NIN == driver.NIN && dv.InsuredId == quotationRequest.InsuredId)
        //             .Select(d => d.ViolationId).Distinct().ToList();
        //            if(violations != null)
        //            {
        //                driverModel.ViolationIds = violations;
        //            }
        //            else
        //            {
        //                driverModel.ViolationIds = new List<int>();
        //            }
        //        }
        //        else
        //        {
        //            driverModel.ViolationIds = new List<int>();
        //        }

        //        var driverExtraLicenses = driversExtraLicenses.Where(d => d.DriverNin == driver.NIN);
        //        if (driverExtraLicenses != null && driverExtraLicenses.Any())
        //        {
        //            driverModel.DriverExtraLicenses = driverExtraLicenses
        //                .Select(d => new DriverExtraLicenseModel() { CountryId = d.LicenseCountryCode, LicenseYearsId = d.LicenseNumberYears }).ToList();
        //        }
        //        else
        //        {
        //            driverModel.DriverExtraLicenses = new List<DriverExtraLicenseModel>();
        //        }

        //        string[] driverDateParts = null;
        //        if (driver.IsCitizen && !string.IsNullOrWhiteSpace(driver.DateOfBirthH))
        //        {
        //            driverDateParts = driver.DateOfBirthH.Split('-');
        //        }
        //        else if (!driver.IsCitizen)
        //        {
        //            driverDateParts = driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
        //        }
        //        if (driverDateParts != null && driverDateParts.Length > 2)
        //        {
        //            driverModel.BirthDateMonth = Convert.ToByte(driverDateParts[1]);
        //            driverModel.BirthDateYear = short.Parse(driverDateParts[2]);
        //        }

        //        if (quotationRequest.Insured.NationalId.ToString().StartsWith("7") && driver.NIN == quotationRequest.Driver.NIN)
        //            driverModel.IsCompanyMainDriver = true;
        //        responseModel.Drivers.Add(driverModel);
        //    }

        //    return responseModel;
        //}

        private InitInquiryResponseModel InitializeEditRequestResponseModel(QuotationRequest quotationRequest, bool isRenual, string referenceId,string eid)
        {
            InitInquiryResponseModel responseModel = new InitInquiryResponseModel();
            responseModel.IsRenualRequest = isRenual;
            responseModel.ExternalId = eid;
            responseModel.ReferenceId = referenceId;
            responseModel.CityCode = quotationRequest.CityCode;
            responseModel.PolicyEffectiveDate = quotationRequest.RequestPolicyEffectiveDate ?? DateTime.Now;
            responseModel.IsVehicleUsedCommercially = quotationRequest.Vehicle.IsUsedCommercially ?? false;
            responseModel.IsCustomerCurrentOwner = !quotationRequest.Vehicle.OwnerTransfer;

            long CarOwnerNINOutput = 0;
            long.TryParse(quotationRequest.Vehicle.CarOwnerNIN, out CarOwnerNINOutput);
            responseModel.OldOwnerNin = CarOwnerNINOutput;

            responseModel.IsCustomerSpecialNeed = quotationRequest.Driver.IsSpecialNeed;
            responseModel.Insured = new InsuredModel();
            responseModel.Insured.InsuredWorkCityCode = quotationRequest.Insured.WorkCityId;
            responseModel.Insured.EducationId = quotationRequest.Insured.EducationId;
            responseModel.Insured.ChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years ?? 0;
            responseModel.Insured.NationalId = quotationRequest.Insured.NationalId;

            responseModel.Vehicle = quotationRequest.Vehicle.ToModel();

            responseModel.Drivers = new List<DriverModel>();

            if (quotationRequest.Vehicle != null)
                responseModel.IsVehicleExist = true;
            if (quotationRequest.Driver != null)
                responseModel.IsMainDriverExist = true;

            string[] mainDriverBirthDateParts = null;

            if (quotationRequest.Driver.IsCitizen && !string.IsNullOrWhiteSpace(quotationRequest.Driver.DateOfBirthH))
            {
                mainDriverBirthDateParts = quotationRequest.Driver.DateOfBirthH.Split('-');
            }
            else if (!quotationRequest.Driver.IsCitizen)
            {
                mainDriverBirthDateParts = quotationRequest.Driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
            }

            if (mainDriverBirthDateParts != null && mainDriverBirthDateParts.Length > 2)
            {
                responseModel.Insured.BirthDateMonth = Convert.ToByte(mainDriverBirthDateParts[1]);
                responseModel.Insured.BirthDateYear = short.Parse(mainDriverBirthDateParts[2]);
            }

            var drivers = quotationRequest.Drivers.ToList();
            //to make main driver on the top of list to be passed to angular in driver[0]
            if (quotationRequest.Insured.NationalId.ToString().StartsWith("7"))
            {
                Driver insured = new Driver();
                insured.NIN = quotationRequest.Insured.NationalId;
                drivers.Insert(0, insured);
                var mainDriver = quotationRequest.Driver;// drivers.FirstOrDefault(d => d.NIN == quotationRequest.Driver.NIN);
                mainDriver.DrivingPercentage = 100;
                drivers.Remove(mainDriver);
                drivers.Insert(1, mainDriver);
            }
            else
            {

                var mainDriver = drivers.FirstOrDefault(d => d.NIN == quotationRequest.Insured.NationalId);
                drivers.Remove(mainDriver);
                drivers.Insert(0, mainDriver);
            }
            var driversExtraLicenses = quotationRequest.Insured.InsuredExtraLicenses.OrderByDescending(i => i.IsMainDriver); //to make main driver on the top of list to be passed to angular in driver[0]
            DriverModel driverModel;
            foreach (var driver in drivers)
            {
                driverModel = new DriverModel();
                driverModel.NationalId = driver.NIN;
                driverModel.DriverNOALast5Years = driver.NOALast5Years ?? 0;
                driverModel.MedicalConditionId = driver.MedicalConditionId ?? 0;
                driverModel.EducationId = driver.EducationId;
                driverModel.ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0;
                driverModel.DriverHomeCityCode = driver.CityId.HasValue ? (int?)driver.CityId.Value : null;
                driverModel.DriverWorkCityCode = driver.WorkCityId.HasValue ? (int?)driver.WorkCityId.Value : null;
                driverModel.DrivingPercentage = driver.DrivingPercentage ?? 0;                driverModel.RelationShipId = driver.RelationShipId ?? 0;                if (driver.DriverViolations != null && driver.DriverViolations.Any())
                {
                    //driverModel.ViolationIds = driver.DriverViolations.Select(d => d.ViolationId).ToList();
                    List<int> violations = driver.DriverViolations
                     .Where(dv => dv.NIN == driver.NIN && dv.InsuredId == quotationRequest.InsuredId)
                     .Select(d => d.ViolationId).Distinct().ToList();
                    if (violations != null)
                    {
                        driverModel.ViolationIds = violations;
                    }
                    else
                    {
                        driverModel.ViolationIds = new List<int>();
                    }
                }
                else
                {
                    driverModel.ViolationIds = new List<int>();
                }

                var driverExtraLicenses = driversExtraLicenses.Where(d => d.DriverNin == driver.NIN);
                if (driverExtraLicenses != null && driverExtraLicenses.Any())
                {
                    driverModel.DriverExtraLicenses = driverExtraLicenses
                        .Select(d => new DriverExtraLicenseModel() { CountryId = d.LicenseCountryCode, LicenseYearsId = d.LicenseNumberYears }).ToList();
                }
                else
                {
                    driverModel.DriverExtraLicenses = new List<DriverExtraLicenseModel>();
                }

                string[] driverDateParts = null;
                if (driver.IsCitizen && !string.IsNullOrWhiteSpace(driver.DateOfBirthH))
                {
                    driverDateParts = driver.DateOfBirthH.Split('-');
                }
                else if (!driver.IsCitizen)
                {
                    driverDateParts = driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                }
                if (driverDateParts != null && driverDateParts.Length > 2)
                {
                    driverModel.BirthDateMonth = Convert.ToByte(driverDateParts[1]);
                    driverModel.BirthDateYear = short.Parse(driverDateParts[2]);
                }
                responseModel.Drivers.Add(driverModel);
            }

            return responseModel;
        }


     

        private bool ValidPolicyEffectiveDate(DateTime policyEffectiveDate)
        {

            if (policyEffectiveDate < DateTime.Now.Date.AddDays(1) || policyEffectiveDate > DateTime.Now.AddDays(14))
            {
                return false;
            }
            return true;
        }
        private InitInquiryResponseModel InitializeEditRequestResponseModelNew(RenewalPolicesData quotationRequest, bool isRenual, string referenceId,string eid ,out string exception)
        {
            exception = string.Empty;
            try
            {
                InitInquiryResponseModel responseModel = new InitInquiryResponseModel();
                responseModel.IsRenualRequest = isRenual;
                responseModel.ExternalId = eid;
                responseModel.ReferenceId = referenceId;
                responseModel.CityCode = quotationRequest.WorkCityId;
                responseModel.PolicyEffectiveDate = DateTime.Now;
                responseModel.IsVehicleUsedCommercially = quotationRequest.IsUsedCommercially ?? false;
                responseModel.IsCustomerCurrentOwner = !quotationRequest.OwnerTransfer;

                long CarOwnerNINOutput = 0;
                long.TryParse(quotationRequest.CarOwnerNIN, out CarOwnerNINOutput);
                responseModel.OldOwnerNin = CarOwnerNINOutput;

                responseModel.IsCustomerSpecialNeed = quotationRequest.IsSpecialNeed;
                responseModel.Insured = new InsuredModel();
                responseModel.Insured.InsuredWorkCityCode = quotationRequest.WorkCityId;
                responseModel.Insured.EducationId = quotationRequest.EducationId.Value;
                responseModel.Insured.ChildrenBelow16Years = quotationRequest.ChildrenBelow16Years ?? 0;
                responseModel.Insured.NationalId = quotationRequest.IsuredNationalId;

                //  responseModel.Vehicle = quotationRequest.Vehicle.ToModel();
                var veh = _vehicleService.GetVehicle(quotationRequest.VehicleId.ToString());
                //var vehicle = new VehicleModel();
                //vehicle.OwnerNationalId = quotationRequest.CarOwnerNIN;
                //vehicle.OwnerTransfer = quotationRequest.OwnerTransfer;
                //vehicle.SequenceNumber = quotationRequest.SequenceNumber;
                //vehicle.VehicleIdTypeId = quotationRequest.VehicleIdTypeId;
                //vehicle.PlateTypeCode = quotationRequest.PlateTypeCode;
                //vehicle.TransmissionTypeId = quotationRequest.TransmissionTypeId.Value;
                responseModel.Vehicle = new VehicleModel();
                responseModel.Vehicle = veh.ToModel();

                responseModel.Drivers = new List<DriverModel>();

                // if (quotationRequest.Vehicle != null)
                responseModel.IsVehicleExist = true;
                // if (quotationRequest.Driver != null)
                responseModel.IsMainDriverExist = true;

                string[] mainDriverBirthDateParts = null;

                if (quotationRequest.IsCitizen && !string.IsNullOrWhiteSpace(quotationRequest.DateOfBirthH))
                {
                    mainDriverBirthDateParts = quotationRequest.DateOfBirthH.Split('-');
                }
                else if (!quotationRequest.IsCitizen)
                {
                    mainDriverBirthDateParts = quotationRequest.DateOfBirthG.Value.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                }

                if (mainDriverBirthDateParts != null && mainDriverBirthDateParts.Length > 2)
                {
                    responseModel.Insured.BirthDateMonth = Convert.ToByte(mainDriverBirthDateParts[1]);
                    responseModel.Insured.BirthDateYear = short.Parse(mainDriverBirthDateParts[2]);
                }

                //var drivers = quotationRequest.Drivers.ToList();
                var drivers = new List<Driver>();
                //to make main driver on the top of list to be passed to angular in driver[0]
                var mainDriver = new Driver();
                if (quotationRequest.MainDriverId != null &&
                    quotationRequest.MainDriverId != Guid.Empty)
                {
                    mainDriver = _driverRepository.Table.Include(a => a.DriverViolations).FirstOrDefault(d => d.DriverId == quotationRequest.MainDriverId);
                    drivers.Add(mainDriver);
                    //drivers.Insert(0, mainDriver);
                }

                if (quotationRequest.AdditionalDriverIdOne != null &&
                    quotationRequest.AdditionalDriverIdOne != Guid.Empty)
                {
                    var additionalDriverIdOne = _driverRepository.Table.Include(a => a.DriverViolations).FirstOrDefault(d => d.DriverId == quotationRequest.AdditionalDriverIdOne);
                    drivers.Add(additionalDriverIdOne);
                    // drivers.Insert(1, additionalDriverIdOne);
                }
                if (quotationRequest.AdditionalDriverIdTwo != null &&
                     quotationRequest.AdditionalDriverIdTwo != Guid.Empty)
                {
                    var additionalDriverIdTwo = _driverRepository.Table.Include(a => a.DriverViolations).FirstOrDefault(d => d.DriverId == quotationRequest.AdditionalDriverIdTwo);
                    drivers.Add(additionalDriverIdTwo);
                    // drivers.Insert(2, additionalDriverIdTwo);
                }


                if (quotationRequest.IsuredNationalId.ToString().StartsWith("7"))
                {
                    Driver insured = new Driver();
                    insured.NIN = quotationRequest.IsuredNationalId;
                    drivers.Insert(0, insured);
                    var _mainDriver = mainDriver;// drivers.FirstOrDefault(d => d.NIN == quotationRequest.Driver.NIN);
                    _mainDriver.DrivingPercentage = 100;
                    drivers.Remove(mainDriver);
                    drivers.Insert(1, _mainDriver);
                }
                else
                {
                    drivers.Remove(mainDriver);
                    drivers.Insert(0, mainDriver);
                }

                //var driversExtraLicenses = quotationRequest.Insured.InsuredExtraLicenses.OrderByDescending(i => i.IsMainDriver);
                var driversExtraLicenses = new List<InsuredExtraLicenses>();
                //to make main driver on the top of list to be passed to angular in driver[0]
                DriverModel driverModel;
                foreach (var driver in drivers)
                {
                    driverModel = new DriverModel();
                    driverModel.NationalId = driver.NIN;
                    driverModel.DriverNOALast5Years = driver.NOALast5Years ?? 0;
                    driverModel.MedicalConditionId = driver.MedicalConditionId ?? 0;
                    driverModel.EducationId = driver.EducationId;
                    driverModel.ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0;
                    driverModel.DriverHomeCityCode = driver.CityId.HasValue ? (int?)driver.CityId.Value : null;
                    driverModel.DriverWorkCityCode = driver.WorkCityId.HasValue ? (int?)driver.WorkCityId.Value : null;
                    driverModel.DrivingPercentage = driver.DrivingPercentage ?? 0;
                    driverModel.RelationShipId = driver.RelationShipId ?? 0;
                    if (driver.DriverViolations != null && driver.DriverViolations.Any())
                    {
                        //driverModel.ViolationIds = driver.DriverViolations.Select(d => d.ViolationId).ToList();
                        List<int> violations = driver.DriverViolations
                         .Where(dv => dv.NIN == driver.NIN && dv.InsuredId == quotationRequest.InsuredId)
                         .Select(d => d.ViolationId).Distinct().ToList();
                        if (violations != null)
                        {
                            driverModel.ViolationIds = violations;
                        }
                        else
                        {
                            driverModel.ViolationIds = new List<int>();
                        }
                    }
                    else
                    {
                        driverModel.ViolationIds = new List<int>();
                    }

                    var driverExtraLicenses = driversExtraLicenses.Where(d => d.DriverNin == driver.NIN);
                    if (driverExtraLicenses != null && driverExtraLicenses.Any())
                    {
                        driverModel.DriverExtraLicenses = driverExtraLicenses
                            .Select(d => new DriverExtraLicenseModel() { CountryId = d.LicenseCountryCode, LicenseYearsId = d.LicenseNumberYears }).ToList();
                    }
                    else
                    {
                        driverModel.DriverExtraLicenses = new List<DriverExtraLicenseModel>();
                    }

                    string[] driverDateParts = null;
                    if (driver.IsCitizen && !string.IsNullOrWhiteSpace(driver.DateOfBirthH))
                    {
                        driverDateParts = driver.DateOfBirthH.Split('-');
                    }
                    else if (!driver.IsCitizen)
                    {
                        driverDateParts = driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                    }
                    if (driverDateParts != null && driverDateParts.Length > 2)
                    {
                        driverModel.BirthDateMonth = Convert.ToByte(driverDateParts[1]);
                        driverModel.BirthDateYear = short.Parse(driverDateParts[2]);
                    }
                    responseModel.Drivers.Add(driverModel);
                }

                return responseModel;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                throw;
            }
        }
        //private InquiryOutputModel SetInquiryOutputModel(int statusCode, string description, string methodName, InquiryOutputModel inquiryOutput)
        //{

        //    inquiryOutput.StatusCode = statusCode;
        //    inquiryOutput.Description = description;
        //    inquiryOutput.MethodName = methodName;
        //    LogInquiry(inquiryOutput);
        //    return inquiryOutput;
        //}
        //private async Task<InquiryOutputModel> SubmitInquiryAsync(InquiryRequestModel requestModel, Guid? parentRequestId = null)
        //{
        //    requestModel.InquiryOutputModel = new InquiryOutputModel();
        //    if (ModelState.IsValid)
        //    {

        //        if (requestModel.Vehicle.OwnerTransfer)
        //        {
        //            if (!_vehicleService.ValidateOwnerTransfer(requestModel.Vehicle.OwnerNationalId, requestModel.Insured.NationalId))
        //            {
        //                SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvaildOldOwnerNin, "SubmitInquiryAsync", requestModel.InquiryOutputModel);
        //                return requestModel.InquiryOutputModel;
        //            }
        //        }

        //        if (!ValidPolicyEffectiveDate(requestModel.PolicyEffectiveDate))
        //        {

        //            SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvalidPolicyEffectiveDate, "SubmitInquiryAsync", requestModel.InquiryOutputModel);
        //            return requestModel.InquiryOutputModel;
        //        }
        //        string currentUserId = _authorizationService.GetUserId(User);
        //        string currentUserName = string.Empty;
        //        if (!string.IsNullOrEmpty(currentUserId))
        //        {
        //            var currentUser = await _authorizationService.GetUser(currentUserId);
        //            currentUserName = currentUser?.FullName;
        //        }
        //        Guid selectedUserId = Guid.Empty;
        //        Guid.TryParse(currentUserId, out selectedUserId);
        //        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
        //        predefinedLogInfo.UserID = selectedUserId;
        //        predefinedLogInfo.UserName = currentUserName;
        //        predefinedLogInfo.RequestId = parentRequestId;
        //        var result = _quotationRequestService.HandleQuotationRequestNew(requestModel, AuthorizationToken, predefinedLogInfo);
        //        if (!(string.IsNullOrEmpty(result.InquiryOutputModel.Description)) && result.InquiryOutputModel.StatusCode != 0)
        //        {
        //            return requestModel.InquiryOutputModel;
        //        }
        //        else
        //        {
        //            SetInquiryOutputModel((int)HttpStatusCode.OK, SubmitInquiryResource.Success, "SubmitInquiryAsync", requestModel.InquiryOutputModel);
        //            requestModel.InquiryOutputModel.InquiryResponseModel = _quotationRequestService.HandleYakeenMissingFields(result);
        //            return requestModel.InquiryOutputModel;

        //        }
        //    }
        //    else
        //    {
        //        SetInquiryOutputModel((int)HttpStatusCode.BadRequest, SubmitInquiryResource.InvalidData, "SubmitInquiryAsync", requestModel.InquiryOutputModel);
        //        return requestModel.InquiryOutputModel;

        //    }
        //}
        private void LogInquiry(InquiryOutputModel model)
        {
            try
            {
                InquiryRequestLog inquiryRequestLog = new InquiryRequestLog()
                {
                    CreatedDate = DateTime.Now,
                    UserIP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.Headers["User-Agent"].ToString(),
                    ErrorCode = model.StatusCode,
                    ErrorDescription = model.Description,
                    //CalledUrl = HttpContext.Current.Request.Url.OriginalString,
                    MethodName = model.MethodName,
                };
                InquiryRequestLogDataAccess.AddInquiryRequestLog(inquiryRequestLog);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
            }
        }

        private InquiryRequestModel MapToInquiryRequestModel(InitInquiryRequestModel model)
        {
            try
            {
                InquiryRequestModel inquiryRequestModel = new InquiryRequestModel();
                if (model != null)
                {
                    inquiryRequestModel.Vehicle = new VehicleModel();
                    inquiryRequestModel.Insured = new InsuredModel();
                    inquiryRequestModel.Vehicle.SequenceNumber = model.SequenceNumber;
                    inquiryRequestModel.Vehicle.OwnerNationalId = model.OwnerNationalId;
                    inquiryRequestModel.Vehicle.OwnerTransfer = model.OwnerTransfer;
                    inquiryRequestModel.Insured.NationalId = model.NationalId;
                    inquiryRequestModel.PolicyEffectiveDate = model.PolicyEffectiveDate;
                    inquiryRequestModel.Vehicle.VehicleIdTypeId = model.VehicleIdTypeId;


                }
                return inquiryRequestModel;
            }
            catch (Exception ex)
            {
                _logger.Log("Issue on Mapping" + ex.Message);
                throw;
            }
        }








        /// <summary>        /// Get All Payment Methods        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/inquiry/payment-methods")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Core.Domain.Dtos.PaymentMethodModel>>))]        public IActionResult GetAllPaymentMethod()        {            try            {                var result = _paymentService.GetAllPaymentMethod().Where(a=>a.Code!=4);
                return Ok(result.Select(e => new Tameenk.Core.Domain.Dtos.PaymentMethodModel
                {
                    Active = e.Active,
                    Code = e.Code,
                    Name = e.Name,
                    Order = e.Order
                }));
            }            catch (Exception ex)            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetAllPaymentMethod-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);            }        }

        [HttpGet]        [Route("api/inquiry/paymentmethods")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Core.Domain.Dtos.PaymentMethodModel>>))]        public IActionResult GetAllPaymentMethodByChannel(string channel)        {            try            {
                if(string.IsNullOrEmpty(channel))
                    return Error("channel is null or empty");
                IEnumerable<PaymentMethod> result = null;                if (channel.ToLower()=="ios")                   result = _paymentService.GetAllPaymentMethod().Where(a => a.IosEnabled==true);
                else if (channel.ToLower() == "android")
                    result = _paymentService.GetAllPaymentMethod().Where(a => a.AndroidEnabled == true);

                if (result!=null)
                {
                    List<PaymentMethodModel> methods = new List<PaymentMethodModel>();
                    foreach(var item in result)
                    {
                        PaymentMethodModel method = new PaymentMethodModel();
                        method.Active = item.Active;
                        method.Code = item.Code;
                        method.Name = item.Name;
                        method.Order = item.Order;
                        method.Brands = item.Brands;
                        method.LogoUrl = item.LogoUrl;
                        method.EnglishDescription = item.EnglishDescription;
                        method.ArabicDescription = item.ArabicDescription;
                        methods.Add(method);
                    }
                    return Ok(methods);
                }
                return Error("No Payment Method Available");
            }            catch (Exception ex)            {
                InquiryOutput output = new InquiryOutput();
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "GetAllPaymentMethodByChannel-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                return Error(output);            }        }
        #endregion

        [HttpGet]
        [Route("api/InquiryNew/all-lookups")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult InquiryLookups()
        {
            try
            {
                InquiryLookupsOutput output = new InquiryLookupsOutput();
                output.TransimissionTypes = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<TransmissionType>();
                output.ParkingLocations = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<ParkingLocation>();
                output.VehicleUsages = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<VehicleUse>();
                output.BrakingSystems = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<BrakingSystem>();
                output.CruiseControlTypes = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<CruiseControlType>();
                output.ParkingSensors = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<ParkingSensors>();
                output.VehicleCameraTypes = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<VehicleCameraType>();
                output.Mileages = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<Mileage>();
                output.MedicalConditions = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<MedicalCondition>();
                output.Violation = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<Violation>();
                output.Education = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<Education>();
                output.RelationShips = Tameenk.Core.Domain.Enums.Extensions.GetLookupAsKeyValuePair<RelationShip>();
                output.LicenseYears = new List<InquiryLookup>();
                output.LastAccident = new List<InquiryLookup>();
                output.LastAccident.Add(new InquiryLookup() { Id = 0, Name = GeneralMessages.ResourceManager.GetString("None", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString())) });

                for (int i = 1; i < 21; i++)
                {
                    if (i < 11)
                        output.LastAccident.Add(new InquiryLookup() { Id = (int)i, Name = i.ToString() });
                    output.LicenseYears.Add(new InquiryLookup() { Id = (int)i, Name = i.ToString() });
                }

                var cities = _addressService.GetAllFromCities().Select(e => e.ToModel());
                output.Cities = cities.ToList();

                var countries = _addressService.GetCountries(0, int.MaxValue).Where(a => a.Code != 113).Select(e => e.ToModel());
                output.Countries = countries.ToList();

                output.ErrorCode = InquiryLookupsOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Ok(output);
            }
            catch (Exception ex)
            {
                InquiryLookupsOutput output = new InquiryLookupsOutput();
                output.ErrorCode = InquiryLookupsOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()));
                InquiryRequestLog log = new InquiryRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.MethodName = "InquiryDependancy-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return Error(output);
            }
        }
    }
}
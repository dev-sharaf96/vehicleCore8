using MoreLinq;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Resources.Vehicles;
using Tameenk.Security.CustomAttributes;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.InquiryGateway.Extensions;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Models.YakeenMissingFields;
using Tameenk.Services.InquiryGateway.Services.Core;
using Tameenk.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.InquiryGateway.Controllers
{
    public class InquiryController : BaseApiController
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
        public InquiryController(
            IQuotationRequestService quotationRequestService, IAddressService addressService
            , IVehicleService vehicleService, IWebApiContext webApiContext,
            IQuotationService quotationService,
            IHttpClient httpClient,
            ILogger logger,
            IAuthorizationService authorizationService,
            TameenkConfig tameenkConfig
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
            _quotationService = quotationService ?? throw new TameenkArgumentNullException(nameof(IQuotationService));

        }

        #endregion

        #region Actions

        #region website profile APIs


        /// <summary>
        /// Delete Vehicle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [Route("api/inquiry/user-delete-vehicle")]
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
                return Error(ex.Message);
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Models.VehicleModel>>))]
        [Route("api/inquiry/user-vehicle")]
        public IActionResult GetVehicleForUser(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");

                var result = _vehicleService.GetVehicleForUser(id, pageIndx, pageSize);

                //then convert to model
                IEnumerable<Models.VehicleModel> dataModel = result.Select(e => e.ToModel());


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
                return Error(ex);
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<AddressModel>>))]
        [Route("api/inquiry/user-address")]
        public IActionResult GetAddressesForUser(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            try
            {


                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");

                var result = _addressService.GetAddressesForUser(id, pageIndx, pageSize);

                //then convert to model
                IEnumerable<AddressModel> dataModel = result.Select(e => e.ToModel()).Distinct();
                dataModel = dataModel.DistinctBy(p => new { p.City, p.District, p.Street, p.RegionName });

                return Ok(dataModel, dataModel.Count());

            }
            catch (Exception ex)
            {
                return Error(ex);
            }

        }
        #endregion

        /// <summary>
        /// Get Quotation Request by External Id
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Inquiry/quotation-request")]
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
                return Error(ex.Message);
            }
        }

        //[HttpPost]
        //[Route("api/Inquiry/init-inquiry-request")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InitInquiryResponseModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        //public IHttpActionResult InitInquiryRequest([FromBody]InitInquiryRequestModel model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if (!ValidateCaptcha(model.CaptchaToken, model.CaptchaInput))
        //        {
        //            return Error(SubmitInquiryResource.InvalidCaptcha);
        //        }
        //        if (model.OwnerTransfer)
        //        {
        //            if (!_vehicleService.ValidateOwnerTransfer(model.OwnerNationalId, model.NationalId))
        //            {
        //                return Error(SubmitInquiryResource.InvaildOldOwnerNin);
        //            }
        //        }
        //        if (!ValidPolicyEffectiveDate(model.PolicyEffectiveDate))
        //        {
        //            return Error(SubmitInquiryResource.InvalidPolicyEffectiveDate);
        //        }
        //        DateTime endDate = new DateTime(2020, 6, 30);
        //        if (DateTime.Now.Date<= endDate.Date)
        //        {
        //            if(model.PolicyEffectiveDate.Date > endDate.Date)
        //            {
        //                return Error(SubmitInquiryResource.InvalidPolicyEffectiveDateToEndOfJune);
        //            }
        //        }
        //        //var userId = _authorizationService.GetUserId(User);
        //        var result = _quotationRequestService.InitInquiryRequest(model);
        //        if (result.Errors != null && result.Errors.Any())
        //        {
        //            return Error(result.Errors);
        //        }
        //        else
        //        {
        //            return Ok(result);
        //        }
        //    }
        //    else
        //    {
        //        return Error(ModelState, "SubmitInquiryRequest.InvalidData");
        //    }
        //}

        ///// <summary>
        ///// Submit Inquiry Request.
        ///// </summary>
        ///// <param name="requestModel">Reqouset Model</param>
        ///// <returns>Qoutation External id.</returns>
        //[HttpPost]
        //[Route("api/Inquiry/submit-inquiry-request")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        //public async Task<IHttpActionResult> SubmitInquiryRequest([FromBody]InquiryRequestModel requestModel, Guid? parentRequestId = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!ValidateCaptcha(requestModel.CaptchaToken, requestModel.CaptchaInput))
        //        {
        //            return Error(SubmitInquiryResource.InvalidCaptcha);
        //        }

        //        if (requestModel.Vehicle.OwnerTransfer)
        //        {
        //            if (!_vehicleService.ValidateOwnerTransfer(requestModel.Vehicle.OwnerNationalId, requestModel.Insured.NationalId))
        //            {
        //                return Error(SubmitInquiryResource.InvaildOldOwnerNin);
        //            }
        //        }

        //        if (!ValidPolicyEffectiveDate(requestModel.PolicyEffectiveDate))
        //        {
        //            return Error(SubmitInquiryResource.InvalidPolicyEffectiveDate);
        //        }
        //        DateTime endDate = new DateTime(2020, 6, 30);
        //        if (DateTime.Now.Date <= endDate.Date)
        //        {
        //            if (requestModel.PolicyEffectiveDate.Date > endDate.Date)
        //            {
        //                return Error(SubmitInquiryResource.InvalidPolicyEffectiveDateToEndOfJune);
        //            }
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
        //        var result = _quotationRequestService.HandleQuotationRequest(requestModel, AuthorizationToken, predefinedLogInfo);
        //        if (result.Errors != null && result.Errors.Any())
        //        {
        //            return Error(result.Errors);
        //        }
        //        else
        //        {
        //            return Ok(_quotationRequestService.HandleYakeenMissingFields(result));
        //        }
        //    }
        //    else
        //    {
        //        return Error(ModelState, "SubmitInquiryRequest.InvalidData");
        //    }
        //}


        //[HttpPost]
        //[Route("api/Inquiry/submit-yakeen-missing-fields")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        //public IHttpActionResult SubmitYakeenMissingFields([FromBody] YakeenMissingInfoRequestModel model)
        //{
        //    try
        //    {
        //        if(model.YakeenMissingFields.VehicleMakerCode == null &&((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleMaker)  && model.YakeenMissingFields.VehicleMaker != "غير متوفر")))
        //        {
        //            model.YakeenMissingFields.VehicleMakerCode = int.Parse(model.YakeenMissingFields.VehicleMaker);
        //        }
        //        if (model.YakeenMissingFields.VehicleModelCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleModel) && model.YakeenMissingFields.VehicleModel != "غير متوفر")))
        //        {
        //            model.YakeenMissingFields.VehicleModelCode = int.Parse(model.YakeenMissingFields.VehicleModel);
        //        }
        //        var result = _quotationRequestService.UpdateQuotationRequestWithYakeenMissingFields(model);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log($"InquiryController -> SubmitYakeenMissingFields an error occurred while updating the missing fields.", ex);
        //        return Error(ex.ToString());
        //    }

        //}


        //[HttpGet]
        //[Route("api/Inquiry/edit-inquiry-request")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        //public IHttpActionResult EditInquiryRequest(string externalId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(externalId))
        //            throw new TameenkArgumentNullException("externalId");

        //        var res = _quotationService.GetQuotationRequest(externalId);
        //        return Ok(InitializeEditRequestResponseModel(res));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log("EditInquiryRequest error occured.", ex);
        //        return Error(ex.Message);
        //    }
        //}

        /// <summary>
        /// Return list of all occupatins Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/all-occipations")]
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
                return Error(ex.ToString());
            }

        }



        /// <summary>
        /// Return list of all educations Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/all-educations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetEducations()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Education>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }

        }

        /// <summary>
        /// Return list of all medical condition Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/all-medical-conditions")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<IdNamePairModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(List<ErrorModel>))]
        public IActionResult GetMedicalConditionss()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<MedicalCondition>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }

        }


        /// <summary>
        /// Return list of all transimission types Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/all-transimission-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetTransimissionTypes()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<TransmissionType>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get parking locations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/parking-locations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetParkingLocations()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<ParkingLocation>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get braking systems.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/braking-systems")]
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
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get cruise control types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/cruise-control-types")]
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
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get parking sensors.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/parking-sensors")]
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
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get parking sensors.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/camera-types")]
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
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// Get driving violations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/violations")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetViolations()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Violation>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }


        /// <summary>
        /// Return list of all cities Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/all-cities")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<CityModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCities(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_addressService.GetCities(pageIndex, pageSize).Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/inquiry/all-countries")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<CountryModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetCountries(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_addressService.GetCountries(pageIndex, pageSize).Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        #region Vehicle


        /// <summary>
        /// Return list of all vehicle maker Id Name pairs
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/vehicle-makers")]
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
                return Error(ex.Message);
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
        [Route("api/inquiry/vehicle-models")]
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
                return Error(ex.Message);
            }
        }


        /// <summary>
        /// Get all vehicle body types
        /// </summary>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/inquiry/vehicle-body-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehicleBodyTypes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                return Ok(_vehicleService.VehicleBodyTypes(pageIndex, pageSize)
                    .Select(e => new IdNamePairModel()
                    {
                        Id = int.Parse(e.YakeenCode),
                        Name = _webApiContext.CurrentLanguage == LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                    }));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        #endregion


        [HttpGet]
        [Route("api/inquiry/vehcileModels")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<LookUpTemp>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetVehcileModels(int id)
        {
            try
            {
                List<LookUpTemp> lookups = new List<LookUpTemp>();
                List<Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel> data = _vehicleService.GetVehicleModels(id);
                foreach(Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel model in data)
                {
                    lookups.Add(new LookUpTemp() {
                        id = Convert.ToInt32(model.Code) ,
                        name = _webApiContext.CurrentLanguage == LanguageTwoLetterIsoCode.Ar ? model.ArabicDescription : model.EnglishDescription

                    });
                }

                return Ok(lookups);
                
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        public class LookUpTemp
        {
            public int id { get; set; }
            public string name { get; set; }
        }



        [HttpGet]        [Route("api/inquiry/Kilometers")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]        public IActionResult GetKilometers(string lang = "")        {

            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Mileage>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }


            //try
            //{
            //    var list = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Mileage>().Select(e => e.ToModel());
            //    if (!string.IsNullOrEmpty(lang))
            //    {
            //        LookupsOutput output = new LookupsOutput();
            //        output.Result = new List<Lookup>();
            //        var listKeys = Enum.GetValues(typeof(Mileage))
            //               .Cast<Mileage>()
            //               .Select(v => new
            //               {
            //                   id = v,
            //                   key = v.ToString()
            //               })
            //               .ToList();
            //        foreach (var item in listKeys)
            //        {
            //            output.Result.Add(new Lookup() { Id = ((int)item.id).ToString(), Name = MileageResource.ResourceManager.GetString(item.key, CultureInfo.GetCultureInfo(lang)) });
            //        }
            //        output.ErrorCode = LookupsOutput.ErrorCodes.Success;
            //        output.ErrorDescription = "Success";
            //        return Single(output);
            //    }
            //    return Ok(list);
            //}
            //catch (Exception ex)
            //{
            //    return Error(ex.Message);
            //}

        }


        #endregion

        #region Private Methods

        //private bool ValidateCaptcha(string token, string input)
        //{
        //    try
        //    {
        //        var requestModel = new ValidateCaptchaModel
        //        {
        //            Token = token,
        //            Input = input
        //        };
        //        //var validateTask = _httpClient.PostAsync($"{_config.Identity.Url}api/identity/captcha/validate/", requestModel, authorizationToken: AuthorizationToken);
        //        //validateTask.Wait();
        //        //var result = validateTask.Result.Content.ReadAsStringAsync().Result;
        //        //if (!string.IsNullOrWhiteSpace(result))
        //        //{
        //        //    var response = JsonConvert.DeserializeObject<CommonResponseModel<bool>>(result);
        //        //    return response.Data;
        //        //}
        //        var encryptedCaptcha = AESEncryption.DecryptString(requestModel.Token, "xYD_3h95?D&*&rTL");
        //        var captchaToken = JsonConvert.DeserializeObject<CaptchaToken>(encryptedCaptcha);
        //        if (captchaToken.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
        //        {
        //            return false;
        //        }
        //        if (captchaToken.Captcha.Equals(requestModel.Input, StringComparison.Ordinal))
        //        {
        //            return true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log($"Inquiry controller -> ValidateCaptcha (token : {token}, input {input})", ex);
        //    }
        //    return false;
        //}


        //private InitInquiryResponseModel InitializeEditRequestResponseModel(QuotationRequest quotationRequest)
        //{

        //    var responseModel = quotationRequest.ToInquiryResponseModel();
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
        //    for (int i = 0; i < drivers.Count; i++)
        //    {
        //        string[] driverDateParts = null;
        //        if (drivers[i].IsCitizen && !string.IsNullOrWhiteSpace(drivers[i].DateOfBirthH))
        //        {
        //            driverDateParts = drivers[i].DateOfBirthH.Split('-');
        //        }
        //        else if (!drivers[i].IsCitizen)
        //        {
        //            driverDateParts = drivers[i].DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
        //        }
        //        if (driverDateParts != null && driverDateParts.Length > 2)
        //        {
        //            responseModel.Drivers[i].BirthDateMonth = Convert.ToByte(driverDateParts[1]);
        //            responseModel.Drivers[i].BirthDateYear = short.Parse(driverDateParts[2]);
        //        }
        //    }

        //    return responseModel;
        //}

        //private bool ValidPolicyEffectiveDate(DateTime policyEffectiveDate)
        //{

        //    if (policyEffectiveDate < DateTime.Now.Date.AddDays(1) || policyEffectiveDate > DateTime.Now.AddDays(14))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        #endregion

        [HttpGet]
        [Route("api/InquiryNew/vehicle-usage")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult VehicleUsage()
        {
            try
            {
                return Ok(Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<VehicleUse>().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

    }
    [JsonObject]
    public class CaptchaToken
    {
        /// <summary>
        /// Captcha value.
        /// </summary>
        [JsonProperty("captcha")]
        public string Captcha { get; set; }

        /// <summary>
        /// Captcha expiration date.
        /// </summary>
        [JsonProperty("expiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}

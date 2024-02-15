using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Resources;
using Tameenk.Api.Core;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegrationApi.Repository;
using Tameenk.Services.YakeenIntegrationApi.Services.Core;

namespace Tameenk.Services.YakeenIntegrationApi.Controllers
{
    public class VehicleController : BaseApiController
    {
        private readonly IYakeenVehicleServices _yakeenVehicleServices;
        private readonly ILogger _logger;

        public VehicleController(IYakeenVehicleServices yakeenVehicleServices, ILogger logger)
        {
            _yakeenVehicleServices = yakeenVehicleServices;
            _logger = logger;
        }

        //[HttpPost]
        //[SwaggerOperation("GetBySequenceNumberOrCustomCardNumber")]
        //[ResponseType(typeof(VehicleYakeenModel))]
        //public IHttpActionResult Post([FromBody]VehicleInfoRequestModel VehicleInfoRequest)
        //{
        //    _logger.Log($"YakeenIntegrationApi -> VehicleController -> Start Getting vehicle info VehicleInfoRequest: {JsonConvert.SerializeObject(VehicleInfoRequest)}");
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
        //            predefinedLogInfo.UserID = VehicleInfoRequest.UserId;
        //            predefinedLogInfo.UserName = VehicleInfoRequest.UserName;
        //            predefinedLogInfo.RequestId = VehicleInfoRequest.ParentRequestId;
        //            var vehicle = _yakeenVehicleServices.GetVehicleByOfficialId(VehicleInfoRequest, predefinedLogInfo);
        //            if (vehicle == null)
        //            {
        //                _logger.Log($"YakeenIntegrationApi -> VehicleController -> Unable to find vehicle with this request");
        //                vehicle = new VehicleYakeenModel();
        //                vehicle.Error = new YakeenInfoErrorModel
        //                {
        //                    ErrorMessage = $"Unable to find vehicle with this request {JsonConvert.SerializeObject(VehicleInfoRequest)}"
        //                };
        //                return Ok(vehicle);
        //            }
        //            else
        //            {
        //                if (vehicle.Success)
        //                {
        //                    _logger.Log($"YakeenIntegrationApi -> VehicleController -> End getting vehicle info, result is: {JsonConvert.SerializeObject(vehicle)}");
        //                    return Ok(vehicle);
        //                }
        //                else
        //                {
        //                    ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
        //                                 typeof(SubmitInquiryResource).Assembly);
        //                    var ErrorMessage = rm.GetString($"YakeenError_{vehicle.Error?.ErrorCode}");
        //                    var GenericErrorMessage = SubmitInquiryResource.YakeenError_100;

        //                    if (vehicle.Error.Type == EErrorType.YakeenError ||
        //                        (vehicle.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //                    {
        //                        _logger.Log($"YakeenIntegrationApi -> VehicleController -> Error happen when getting vehicle info,Error: {JsonConvert.SerializeObject(vehicle.Error.ErrorMessage)}");
        //                        if (vehicle.Error == null)
        //                            vehicle.Error = new YakeenInfoErrorModel();
        //                        vehicle.Error.ErrorMessage = ErrorMessage ?? GenericErrorMessage;
        //                        return Ok(vehicle);
        //                    }
        //                    else
        //                    {
        //                        if (vehicle.Error == null)
        //                            vehicle.Error = new YakeenInfoErrorModel();
        //                        vehicle.Error.ErrorMessage = GenericErrorMessage;
        //                        _logger.Log($"YakeenIntegrationApi -> VehicleController -> Unable to process request");
        //                        return Ok(vehicle);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _logger.Log($"YakeenIntegrationApi -> VehicleController -> Model is not valid, ModelState: {JsonConvert.SerializeObject(ModelState)}");
        //            return BadRequest(ModelState);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log($"YakeenIntegrationApi -> VehicleController -> exception happend while getting vehicle info form yakeen", ex);
        //        return Error(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[SwaggerOperation("GetByTameenkId")]
        //[Route("api/Vehicle/GetByTameenkId/{TameenkId}")]
        //[ResponseType(typeof(VehicleYakeenModel))]
        //public IHttpActionResult GetByTameenkId(Guid TameenkId)
        //{
        //    var vehicle = _yakeenVehicleServices.GetVehicleByTameenkId(TameenkId);
        //    if (vehicle == null)
        //    {
        //        return Error($"Unable to find vehicle with id {TameenkId}");
        //    }
        //    else
        //    {
        //        if (vehicle.Success)
        //        {
        //            return Ok(vehicle);
        //        }
        //        else
        //        {
        //            if (vehicle.Error.Type == EErrorType.YakeenError ||
        //                (vehicle.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //            {
        //                return BadRequest(vehicle.Error.ErrorMessage);
        //            }
        //            else
        //            {
        //                return Error("Unable to process request");
        //            }
        //        }
        //    }
        //}
    }
}

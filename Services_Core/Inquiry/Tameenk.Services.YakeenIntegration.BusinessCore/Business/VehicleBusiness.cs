using Newtonsoft.Json;
using System;
using System.Net;
using System.Resources;
using Tameenk.Api.Core;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class VehicleBusiness
    {
        private readonly IYakeenVehicleServices _yakeenVehicleServices;
        private readonly ILogger _logger;

        public VehicleBusiness(IYakeenVehicleServices yakeenVehicleServices, ILogger logger)
        {
            _yakeenVehicleServices = yakeenVehicleServices;
            _logger = logger;
        }

        public YakeenOutputModel GetBySequenceNumberOrCustomCardNumber(VehicleInfoRequestModel VehicleInfoRequest)
        {
            YakeenOutputModel yakeenOutputModel = new YakeenOutputModel();
            try
            {

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = VehicleInfoRequest.UserId;
                predefinedLogInfo.UserName = VehicleInfoRequest.UserName;
                predefinedLogInfo.RequestId = VehicleInfoRequest.ParentRequestId;
                var vehicle = _yakeenVehicleServices.GetVehicleByOfficialId(VehicleInfoRequest, predefinedLogInfo);
                if (vehicle == null)
                {
                    yakeenOutputModel.StatusCode = 400;
                    yakeenOutputModel.Description = YakeenResource.VehicleNotFound;
                    yakeenOutputModel.ErrorDescription = "Vehicle Not Found";
                    return yakeenOutputModel;
                }
                else
                {
                    string ErrorDescription = vehicle?.Error?.ErrorMessage;
                    if (vehicle.Success)
                    {
                        yakeenOutputModel.VehicleYakeenModel = vehicle;
                        //yakeenOutputModel.StatusCode = 200;
                        //yakeenOutputModel.Description = YakeenResource.Success;
                        //yakeenOutputModel.ErrorDescription = "Success";

                        return yakeenOutputModel;
                    }
                    else
                    {
                        ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
                                     typeof(SubmitInquiryResource).Assembly);
                        var ErrorMessage = rm.GetString($"YakeenError_{vehicle.Error?.ErrorCode}");
                        var GenericErrorMessage = SubmitInquiryResource.YakeenError_100;

                        if (vehicle.Error.Type == EErrorType.YakeenError ||
                            (vehicle.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
                        {
                            if (vehicle.Error == null)
                                vehicle.Error = new YakeenInfoErrorModel();
                            vehicle.Error.ErrorMessage = ErrorMessage ?? GenericErrorMessage;
                            vehicle.Error.ErrorDescription = ErrorDescription ?? rm.GetString($"YakeenError_100", new System.Globalization.CultureInfo("en"));

                            yakeenOutputModel.VehicleYakeenModel = vehicle;
                            //yakeenOutputModel.StatusCode = 400;
                            //yakeenOutputModel.Description = GenericErrorMessage;
                            //yakeenOutputModel.ErrorDescription = GenericErrorMessage;
                            return yakeenOutputModel;
                        }
                        else
                        {
                            if (vehicle.Error == null)
                                vehicle.Error = new YakeenInfoErrorModel();
                            vehicle.Error.ErrorMessage = GenericErrorMessage;
                            vehicle.Error.ErrorDescription = ErrorDescription ?? rm.GetString($"YakeenError_100",new System.Globalization.CultureInfo("en"));
                            yakeenOutputModel.VehicleYakeenModel = vehicle;
                            //yakeenOutputModel.StatusCode = 400;
                            //yakeenOutputModel.Description = GenericErrorMessage;
                            //yakeenOutputModel.ErrorDescription = GenericErrorMessage;
                            return yakeenOutputModel;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // _logger.Log($"YakeenIntegrationApi -> VehicleController -> exception happend while getting vehicle info form yakeen", ex);
                if (yakeenOutputModel.VehicleYakeenModel == null)
                    yakeenOutputModel.VehicleYakeenModel = new VehicleYakeenModel();
                if (yakeenOutputModel.VehicleYakeenModel.Error == null)
                    yakeenOutputModel.VehicleYakeenModel.Error = new YakeenInfoErrorModel();

                yakeenOutputModel.VehicleYakeenModel.Error.ErrorCode = "400";
                yakeenOutputModel.VehicleYakeenModel.Error.ErrorMessage = YakeenResource.VehicleException;
                yakeenOutputModel.VehicleYakeenModel.Error.ErrorDescription = ex.ToString();

                return yakeenOutputModel;
            }
        }

        //public YakeenOutputModel GetByTameenkId(Guid TameenkId)
        //{
        //    YakeenOutputModel yakeenOutputModel = new YakeenOutputModel();
        //    var vehicle = _yakeenVehicleServices.GetVehicleByTameenkId(TameenkId);
        //    if (vehicle == null)
        //    {
        //       // CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, $"Unable to find vehicle with id {TameenkId}", ref yakeenOutputModel);
        //        yakeenOutputModel.StatusCode = 400;
        //        yakeenOutputModel.Description = $"Unable to find vehicle with id {TameenkId}";
        //        yakeenOutputModel.ErrorDescription = $"Unable to find vehicle with id {TameenkId}";
        //        return yakeenOutputModel;

        //    }
        //    else
        //    {
        //        if (vehicle.Success)
        //        {
        //            CommonService.SetYakeenOutput((int)HttpStatusCode.OK, YakeenResource.Success, ref yakeenOutputModel);
        //            yakeenOutputModel.VehicleYakeenModel = vehicle;
        //            yakeenOutputModel.StatusCode = 200;
        //            yakeenOutputModel.Description = YakeenResource.Success;
        //            yakeenOutputModel.ErrorDescription = "Success";
        //            return yakeenOutputModel;
        //        }
        //        else
        //        {
        //            if (vehicle.Error.Type == EErrorType.YakeenError ||
        //                (vehicle.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //            {
        //                //CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, vehicle.Error.ErrorMessage, ref yakeenOutputModel);
        //                yakeenOutputModel.StatusCode = 400;
        //                yakeenOutputModel.Description = vehicle.Error.ErrorMessage;
        //                yakeenOutputModel.ErrorDescription = vehicle.Error.ErrorMessage;
        //                return yakeenOutputModel;

        //            }
        //            else
        //            {
        //                CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, "Unable to process request", ref yakeenOutputModel);
        //                yakeenOutputModel.StatusCode = 400;
        //                yakeenOutputModel.Description = "Unable to process request";
        //                yakeenOutputModel.ErrorDescription = "Unable to process request";
        //                return yakeenOutputModel;
        //            }
        //        }
        //    }
        //}
    }
}

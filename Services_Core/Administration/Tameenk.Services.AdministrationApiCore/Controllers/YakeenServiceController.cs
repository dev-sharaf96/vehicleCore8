using System;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core;
using Tameenk.Core.Exceptions;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Policy.Components;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Tameenk.Api.Core;
using Newtonsoft.Json;
using Tameenk.Security.Services;
using Tameenk.Services.Policy.Components.Morni;
using Tameenk.Services.Administration.Identity;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using Tameenk.Api.Core.Models;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Integration.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class YakeenServiceController : AdminBaseApiController
    {
        #region Fields

        private readonly IYakeenCityCenterService _yakeenCityCenterService;
        private readonly IYakeenClient _yakeenClient;
        private readonly IExcelService _excelService;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<CustomCardInfo> _customCardInfoRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserPageService _userPageService;


        #endregion

        #region CTOR

        public YakeenServiceController(IYakeenCityCenterService yakeenCityCenterService,
            IExcelService excelService,
            IYakeenClient yakeenClient,
            IRepository<CustomCardInfo> customCardInfoRepository,
            IVehicleService vehicleService,
             IAuthorizationService authorizationService,
             IUserPageService userPageService)
        {
            _yakeenCityCenterService = yakeenCityCenterService ?? throw new TameenkArgumentNullException(nameof(IYakeenCityCenterService));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IYakeenCityCenterService));
            _yakeenClient = yakeenClient;
            _customCardInfoRepository = customCardInfoRepository;
            _vehicleService = vehicleService;
            _authorizationService = authorizationService ?? throw new TameenkArgumentNullException(nameof(IAuthorizationService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));

        }

        #endregion


        [HttpPost]
        [Route("api/YakeenService/UpdateCarInfoByCustomTwo")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<ServiceRequestLog>))]
        [AdminAuthorizeAttribute(pageNumber: 101)]
        public IActionResult UpdateCarInfoByCustomTwo(CarInfoCustomTwoDto model)
        {
            AdministrationOutput<ServiceRequestLog> output = new AdministrationOutput<ServiceRequestLog>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "UpdateCarInfoByCustomTwo";
            log.PageURL = "/admin/UpdateCarInfoByCustomTwo";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateCarInfoByCustomTwo";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();

            try
            {
                string currentUserId = _authorizationService.GetUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                    currentUserId = User.Identity.GetUserId();

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
                var isAuthorized = _userPageService.IsAuthorizedUser(101, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (model == null)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "model data is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (string.IsNullOrEmpty(model.CustomCrdNumber))
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Empty CustomCrdNumber";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "EmptyCustomCrdNumber";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (!model.ModelYear.HasValue)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Empty ModelYear";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "EmptyModelYear";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                // check if we get data before
                string exception = string.Empty;
                var result = ServiceRequestLogDataAccess.CheckForGetVehicleInfoByCustomTwoBefore("TameenkLogLive", model.CustomCrdNumber, model.Export, out exception);
                if (result != null && result.ErrorCode == 1)
                {
                    output.Result = new ServiceRequestLog();
                    output.Result = result;
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.Success;
                    output.ErrorDescription = $"Success as data checked before, and Id is: {result.ID}";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.Channel = "Dashboard";
                predefinedLogInfo.ServerIP = Utilities.GetInternalServerIP();
                predefinedLogInfo.VehicleId = model.CustomCrdNumber;
                predefinedLogInfo.VehicleModelYear = model.ModelYear;

                YakeenVehicleOutput yakeenOutput = _yakeenClient.CarInfoByCustomTwo(model, predefinedLogInfo);
                log.ServiceResponse = JsonConvert.SerializeObject(yakeenOutput.result);

                if (yakeenOutput == null)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.NullResult;
                    output.ErrorDescription = "Error happend while getting data from Yakeen, please check the logs in Service Requests page";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "yakeen output is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (yakeenOutput.ErrorCode != YakeenVehicleOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Error happend while getting data from Yakeen, please check the logs Service Requests page";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed from yakeen due to: " + yakeenOutput.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (yakeenOutput.result == null)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.NullResult;
                    output.ErrorDescription = "Error happend while getting data from Yakeen, please check the logs Service Requests page";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "yakeenOutput.result is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (yakeenOutput.result.sequenceNumber == 0)
                {
                    output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.NotFound;
                    output.ErrorDescription = "Custom card still not converted, please check the logs Service Requests page";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Custom card still not converted";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                output.Result = new ServiceRequestLog();
                output.Result = predefinedLogInfo;
                output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = AdministrationOutput<ServiceRequestLog>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Exception error happend, please call Development Team to check the log";
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
        }
    }
}
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Newtonsoft.Json;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// Vehicle controller
    /// </summary>
    public class VehicleController : AdminBaseApiController
    {

        #region Fields
        private readonly IVehicleService _vehicleService;
        private readonly IUserPageService _userPageService;
        private readonly IExcelService _excelService;
        private readonly IYakeenClient _yakeenClient;
        #endregion

        #region Ctor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="vehicleService">The vehicle Service.</param>
        public VehicleController(IVehicleService vehicleService, IUserPageService userPageService, IExcelService excelService, IYakeenClient yakeenClient)
        {
            _vehicleService = vehicleService ?? throw new TameenkArgumentNullException(nameof(IVehicleService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
            _yakeenClient = yakeenClient ?? throw new TameenkArgumentNullException(nameof(IYakeenClient));
        }
        #endregion


        #region Method

        /// <summary>
        /// Edit Vehicle 
        /// </summary>
        /// <param name="vehicleModel">Vehicle Model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/edit")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Models.VehicleModel>))]
        [AdminAuthorizeAttribute(pageNumber: 9)]
        public  IActionResult EditVehicle([FromBody]Models.VehicleModel vehicleModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Vehicles";
            log.PageURL = "/admin/vehicles/edit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditVehicle";
            log.ServiceRequest = JsonConvert.SerializeObject(vehicleModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(9, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (vehicleModel == null)
                {
                    return Error("The Object is Null.");
                }

                if (string.IsNullOrEmpty(vehicleModel.ID))
                    throw new TameenkArgumentNullException("The vehicle Id Can't be Null.");

                Guid guid = new Guid();
                if (!Guid.TryParse(vehicleModel.ID, out guid)){
                    throw new TameenkArgumentException("The vehicle Id is not vaild id.", "VehicleId");
                }


                Vehicle vehicle = vehicleModel.ToEntity();
                
                vehicle = _vehicleService.UpdateVehicleInAdmin(vehicle);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(vehicle.ToModel());
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }


        /// <summary>
        /// delete vehicle ( Mark vehicle as deleted from DB ) 
        /// </summary>
        /// <param name="vehicleId">vehicle ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/delete")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber:9)]
        public IActionResult DeleteVehicle(string vehicleId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Vehicles";
            log.PageURL = "/admin/Vehicles";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteVehicle";
            log.ServiceRequest = $"vehicleId: {vehicleId}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(9, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(vehicleId))
                    throw new TameenkArgumentNullException("Vehicle id can't be null.");

               Vehicle vehicle= _vehicleService.GetVehicle(vehicleId);

                if (vehicle == null)
                {
                    throw new TameenkArgumentException("Vehicle not found ", "vehicleId");
                }

                _vehicleService.DeleteVehicle(vehicle);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok();

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        /// <summary>
        /// get all Vehicle based on filter
        /// </summary>
        /// <param name="vehicleFilterModel">Vehicle filter Model</param>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">Sort Field</param>
        /// <param name="sortOrder">sort Order</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Models.VehicleModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 9)]
        public IActionResult GetAllVehicleBasedOnFilter(VehicleFilterModel vehicleFilterModel,int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Vehicles";
            log.PageURL = "/admin/Vehicles";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllVehicleBasedOnFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(vehicleFilterModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(9, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (vehicleFilterModel == null)
                    throw new TameenkArgumentNullException("policyFilter");

                string exception = string.Empty;
                string vehicleId = string.Empty;
                int totalcount = 0; ;
                if (!string.IsNullOrEmpty(vehicleFilterModel.SequenceNumber))
                    vehicleId = vehicleFilterModel.SequenceNumber;
                else
                    vehicleId = vehicleFilterModel.CustomCardNumber;
                
                List<Vehicle> vehicles = _vehicleService.GetAllVehicleBasedOnFilter(vehicleId, pageIndex, pageSize, out totalcount,out exception);
               if(!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "exception : " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Error:exception");
                }
                //var result = _vehicleService.GetAllVehicleBasedOnFilter(query, pageIndex, pageSize, sortField, sortOrder);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(vehicles.Select(e => e.ToModel()), totalcount);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// get all details to specific Vehicle by id
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Models.VehicleModel>))]
        [AdminAuthorizeAttribute(pageNumber: 9)]
        public IActionResult GetVehicleDetails(string id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Vehicles";
            log.PageURL = "/admin/vehicles/edit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetVehicleDetails";
            log.ServiceRequest = $"id: {id}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(9, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Vehicle id can't be null.");

                Vehicle vehicle = _vehicleService.GetVehicle(id);

                if (vehicle == null)
                {
                    throw new TameenkArgumentException("Vehicle not found ", "vehicleId");
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(vehicle.ToModel());

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Delete Vehicle Requests
        /// </summary>
        /// <param name="sequenceNumber">Sequence Number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/deleteVehicleRequests")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 36)]
        public IActionResult DeleteVehicleRequests(string sequenceNumber)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Release Vehicle Requests Limit";
            log.PageURL = "/admin/vehicleLimit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteVehicleRequests";
            log.ServiceRequest = $"sequenceNumber: {sequenceNumber}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(36, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(sequenceNumber))
                    throw new TameenkArgumentNullException("Sequence Number can't be null.");

                bool result = _vehicleService.DeleteVehicleRequests(sequenceNumber);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        /// <summary>
        /// Get vehicle Breaking Systems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-breakingSystem")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleBreakingSystem()
        {
            try
            {
                List<IdNamePairModel> result = new List<IdNamePairModel>();

                var breakingSystems = _vehicleService.GetBreakingSystems();
                if (breakingSystems == null || breakingSystems.Count == 0)
                    return Ok(new List<IdNamePairModel>());

                foreach (var item in breakingSystems)
                {
                    IdNamePairModel model = new IdNamePairModel();
                    model.Id = item.Code;
                    model.Name = item.NameAr;
                    result.Add(model);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get vehicle Breaking Systems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-parkingSensors")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleParkingSensors()
        {
            try
            {
                List<IdNamePairModel> result = new List<IdNamePairModel>();

                var sensors = _vehicleService.GetSensors();
                if (sensors == null || sensors.Count == 0)
                    return Ok(new List<IdNamePairModel>());

                foreach (var item in sensors)
                {
                    IdNamePairModel model = new IdNamePairModel();
                    model.Id = item.Code;
                    model.Name = item.NameAr;
                    result.Add(model);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get vehicle Breaking Systems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-cameraTypes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleCameraTypes()
        {
            try
            {
                List<IdNamePairModel> result = new List<IdNamePairModel>();

                var cameraTypes = _vehicleService.GetCameraTypes();
                if (cameraTypes == null || cameraTypes.Count == 0)
                    return Ok(new List<IdNamePairModel>());

                foreach (var item in cameraTypes)
                {
                    IdNamePairModel model = new IdNamePairModel();
                    model.Id = item.Code;
                    model.Name = item.NameAr;
                    result.Add(model);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get vehicle colors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-vehicleColors")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleColors()
        {
            try
            {
                var colors = _vehicleService.GetVehicleColors();
                if (colors == null || colors.Count == 0)
                    return Ok(new List<IdNamePairModel>());

                var result = colors.Select(e => new IdNamePairModel()
                {
                    Id = Convert.ToInt32(e.Code),
                    Name = e.ArabicDescription
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        #region Vehicle maker

        /// <summary>
        /// Return list of all vehicle maker Id Name pairs with filter
        /// </summary>
        /// <param name="code">maker code</param>
        /// <param name="description">maker description</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-makers-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleMakers(string code, string description, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = _vehicleService.GetVehiclemakersWithFilter(out int count, false, code, description, pageIndex, pageSize);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToVehicleMakersListingModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Export Tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-makers-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllNewServiceRequest(string code, string description)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = _vehicleService.GetVehiclemakersWithFilter(out int count, true, code, description, 0, 0);

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.ExportVehicleMakers(result, "Vehicl eMakers");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get vehicle maker details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-makers-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleMakerDetailss(int id)
        {
            try
            {
                if (id <= 0)
                    throw new TameenkArgumentNullException("MakerCode");

                var result = _vehicleService.GetMakerDetails(id);
                return Ok(result.ToVehicleMakerDetrailsModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Return list of all vehicle maker models with paging
        /// </summary>
        /// <param name="code">maker code</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-maker-models")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleMakerModels(int code, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var result = _vehicleService.GetVehiclemakermodels(out int count, code.ToString(), pageIndex, pageSize);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToVehicleMakerModelsListingModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// add or update maker model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/vehicle-makers-addOrUpdateModel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult AddNewMakerModel([FromBody]Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel model, string action)
        {
            try
            {
                var makerModel = new Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel()
                {
                    Code = model.Code,
                    MakerCode = model.MakerCode,
                    EnglishDescription = model.EnglishDescription,
                    ArabicDescription = model.ArabicDescription
                };

                var output = _vehicleService.AddorUpdateMakerModel(makerModel, action);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// add new maker
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/vehicle-makers-addNewMaker")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult AddNewMaker([FromBody]Tameenk.Services.AdministrationApi.Models.VehicleMakerModel model)
        {
            try
            {
                var maker = new Tameenk.Loggin.DAL.Dtos.VehicleMakerModel()
                {
                    Code = model.Code,
                    EnglishDescription = model.EnglishDescription,
                    ArabicDescription = model.ArabicDescription
                };

                var output = _vehicleService.AddorNewMaker(maker);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get vehicle maker model details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-makermodel-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetMakerModelDetials(int code, int makerCode)
        {
            try
            {
                if (code <= 0)
                    throw new TameenkArgumentNullException("Code");
                if (code <= 0)
                    throw new TameenkArgumentNullException("makerCode");

                var result = _vehicleService.GetMakerModelDetails(code, makerCode);
                return Ok(result.ToVehicleMakerModelDetrailsModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Return list of all vehicle maker models with filter
        /// </summary>
        /// <param name="code">code</param>
        /// <param name="makerCode">maker code</param>
        /// <param name="description">maker description</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-maker-models-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetVehicleMakerModelsWithFilter(string code, string makerCode, string description, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = _vehicleService.GetVehiclemakerModelsWithFilter(out int count, false, code, makerCode, description, pageIndex, pageSize);

                return Ok(result.Select(res => res.ToVehicleModelsListingModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Export Tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vehicle/vehicle-makerModels-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllMakerModels(string code, string makerCode, string description)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = _vehicleService.GetVehiclemakerModelsWithFilter(out int count, true, code, makerCode, description, 0, 0);

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.ExportVehicleMakerModels(result, "Vehicl eMakers");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// check maker code exist
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/vehicle-makers-checkMakerCode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult CheckMakerCode([FromBody]Tameenk.Services.AdministrationApi.Models.VehicleMakerModel model)
        {
            try
            {
                var output = _vehicleService.CheckMakeCodeExist(model.Code);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// check if model code exist or not
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vehicle/vehicle-makers-checkMakerModelCode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult CheckMakerModelCode([FromBody]Tameenk.Services.AdministrationApi.Models.VehicleMakerModel model)
        {
            try
            {
                var output = _vehicleService.CheckMakeModelCodeExist(model.Code,model.MakerCode);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        #endregion

        [HttpPost]
        [Route("api/vehicle/ownerShip")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 80)]
        public IActionResult GetVehicleOwnerShip([FromBody] ownerShipVehicleFilterModel ownerShipVehicleFilterModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            ServiceRequestLog ServiceRequestLog = new ServiceRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = ServiceRequestLog.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "vehicle ownerShip";
            log.PageURL = "/vehicle/vehicle-ownerShip";
            log.ApiURL = ServiceRequestLog.ServiceURL = Utilities.GetCurrentURL;
            log.MethodName = ServiceRequestLog.Method = "GetVehicleOwnerShip";
            log.ServiceRequest = ServiceRequestLog.ServiceRequest = JsonConvert.SerializeObject(ownerShipVehicleFilterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = ServiceRequestLog.UserName = User.Identity.GetUserName();
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
                var isAuthorized = _userPageService.IsAuthorizedUser(80, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                VehicleYakeenRequestDto VehicleYakeenRequestDto = new VehicleYakeenRequestDto();
                VehicleYakeenRequestDto.VehicleId = ownerShipVehicleFilterModel.VehicleId;
                VehicleYakeenRequestDto.VehicleIdTypeId = (int)VehicleIdType.SequenceNumber;
                VehicleYakeenRequestDto.OwnerNin = ownerShipVehicleFilterModel.NationalId;
                var result = _yakeenClient.GetVehicleInfo(VehicleYakeenRequestDto, ServiceRequestLog);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
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

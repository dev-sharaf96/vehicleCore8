using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Security.Services;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Newtonsoft.Json;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// Common Controller for General Calls (GetCities, GetCountries, ...)
    /// </summary>
    
    public class AddressController : AdminBaseApiController
    {
        #region Fields
        private readonly IAddressService _addressService;
        private readonly IUserPageService _userPageService;
        private readonly IDriverService _driverService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addressService"></param>
        public AddressController( IAddressService addressService, IUserPageService userPageService, IDriverService driverService)
        {
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _driverService = driverService ?? throw new TameenkArgumentNullException(nameof(IDriverService));
        }

        #endregion

        #region Methods
        /// <summary>
        /// Delete Driver Address (Physical Delete)
        /// </summary>
        /// <param name="id">driver ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Address/deleteDriverAddress")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 22)]
        public IActionResult DeleteDriverAddress(string id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Drivers";
            log.PageURL = "/admin/drivers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteDriverAddress";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(22, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(id))
                {
                    log.ErrorCode = 6;
                    log.ErrorDescription = "address id is empty or null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
                int addressId = 0;
                if(!int.TryParse(id,out addressId))
                {
                    log.ErrorCode =7;
                    log.ErrorDescription = "id is not valid formatt as its "+id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
                var address = _addressService.GetAddressDetails(addressId);
                if(address==null)
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "address is null with id: " + id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Address found");
                }
                log.DriverNin = address.NationalId;
                _addressService.DeleteAddress(address);
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
                return Error("an error has occured, please try again later");
            }

        }

        /// <summary>
        /// get all details to specific driver by id
        /// </summary>
        /// <param name="id">driver id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Address/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddressModel>))]
        [AdminAuthorizeAttribute(pageNumber: 22)]
        public IActionResult GetAddressDetails(string id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Driver Info";
            log.PageURL = "/admin/drivers/edit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAddressDetails";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(22, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized");
                }
                if (string.IsNullOrEmpty(id))
                {
                    log.ErrorCode = 6;
                    log.ErrorDescription = "address id is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
                int addressId = 0;
                if (!int.TryParse(id, out addressId))
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "id is not valid formatt as its " + id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
                Address address = _addressService.GetAddressDetailsNoTracking(addressId);
                if (address == null)
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "address is null with id: " + id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Address found");
                }
                log.DriverNin = address.NationalId;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(address.ToModel());
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured, please try again later");
            }
        }
        

        /// <summary>
        /// Edit Driver Address 
        /// </summary>
        /// <param name="addressModel">address Model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Address/editDriverAddress")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddressModel>))]
        [AdminAuthorizeAttribute(pageNumber: 22)]
        public IActionResult EditDriverAddress([FromBody]AddressModel addressModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Driver Address Info";
            log.PageURL = "/drivers/address/edit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditDriverAddress";
            log.ServiceRequest = JsonConvert.SerializeObject(addressModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(22, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized");
                }
                if (addressModel == null)
                {
                    log.ErrorCode = 6;
                    log.ErrorDescription = "address model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("address model is null");
                }
                if (addressModel.Id < 1)
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "address id is not valid as id coming as "+addressModel.Id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
                Address address = _addressService.GetAddressDetails(addressModel.Id);
                if (address == null)
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "address is null with id: " + addressModel.Id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Address found");
                }
                log.DriverNin = address.NationalId;

                address.Title = addressModel.Title;
                address.Address1 = addressModel.Address1;
                address.Address2 = addressModel.Address2;
                address.BuildingNumber = addressModel.BuildingNumber;
                address.Street = addressModel.Street;
                address.District = addressModel.District;
                address.UnitNumber = addressModel.UnitNumber;
                address.PostCode = addressModel.PostCode;
                address.AdditionalNumber = addressModel.AdditionalNumber;
                address.CityId = addressModel.CityId;
                address.City = addressModel.City;
                address.RegionId = addressModel.RegionId;
                address.RegionName = addressModel.RegionName;
                address.Latitude = addressModel.Latitude;
                address.Longitude = addressModel.Longitude;
                address.PolygonString = addressModel.PolygonString;
                address.AddressLoction = addressModel.AddressLoction;
                address.Restriction = addressModel.Restriction;
                address.IsPrimaryAddress = addressModel.IsPrimaryAddress;
                _addressService.UpdateAddress(address);

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(address.ToModel());
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured, please try again later");
            }

        }

        /// <summary>
        /// Return list of all cities Id Name Pairs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Address/all-cities")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<CityModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetCities()
        {
            try
            {
                return Ok(_addressService.GetCities().Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }

        }


        /// <summary>
        /// Add New Driver Address 
        /// </summary>
        /// <param name="addressModel">address Model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Address/addNewDriverAddress")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddressModel>))]
        [AdminAuthorizeAttribute(pageNumber: 22)]
        public IActionResult AddNewDriverAddress([FromBody]AddressModel addressModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Driver Address Info";
            log.PageURL = "/drivers/address/add";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddNewDriverAddress";
            log.ServiceRequest = JsonConvert.SerializeObject(addressModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(22, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (addressModel == null)
                    return Error("The Object is Null.");
                var driver = _driverService.GetDriver(addressModel.DriverId.ToString());
                if (driver == null)
                {
                    log.ErrorCode = 15;
                    log.ErrorDescription = "driver is null as we receive id " + addressModel.DriverId.ToString();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("driver is null");
                }

                Address address = new Address()
                {
                    Title = addressModel.Title,
                    Address1 = addressModel.Address1,
                    Address2 = addressModel.Address2,
                    BuildingNumber = addressModel.BuildingNumber,
                    Street = addressModel.Street,
                    District = addressModel.District,
                    UnitNumber = addressModel.UnitNumber,
                    PostCode = addressModel.PostCode,
                    AdditionalNumber = addressModel.AdditionalNumber,
                    CityId = addressModel.CityId,
                    City = addressModel.City,
                    RegionId = addressModel.RegionId,
                    RegionName = addressModel.RegionName,
                    Latitude = addressModel.Latitude,
                    Longitude = addressModel.Longitude,
                    PolygonString = addressModel.PolygonString,
                    AddressLoction = addressModel.AddressLoction,
                    Restriction = addressModel.Restriction,
                    IsPrimaryAddress = addressModel.IsPrimaryAddress,
                    DriverId = addressModel.DriverId,
                    NationalId = driver.NIN,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                _addressService.AddNewAddress(address);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(address.ToModel());
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

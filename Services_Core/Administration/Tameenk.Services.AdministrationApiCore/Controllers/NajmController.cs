using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Najm;
using Tameenk.Services.Implementation.Najm;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Implementation;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
     [AdminAuthorizeAttribute(pageNumber: 1)]
    public class NajmController : AdminBaseApiController
    {
        #region Fields
        private readonly INajmService _najmService;
        private readonly IUserPageService _userPageService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="najmService"></param>
        public NajmController(INajmService najmService, IUserPageService userPageService)
        {
            _najmService = najmService ?? throw new TameenkArgumentNullException(nameof(INajmService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
        }

        #endregion

        #region Methods
        /// <summary>
        /// Delete Driver Address (Physical Delete)
        /// </summary>
        /// <param name="id">driver ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/najm/deleteNajmResponse")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult DeleteNajmResponse(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Najm Responses";
            log.PageURL = "/admin/najm";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllNajmResponsesBasedOnFilter";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(1, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (id < 0)
                    throw new TameenkArgumentNullException("Najm Response id can't be less than zero");

                NajmResponseEntity najmResponse = _najmService.GetNajmResponseById(id);

                if (najmResponse == null)
                    throw new TameenkArgumentNullException("Najm Response is null");

                _najmService.DeleteNajmResponse(najmResponse);
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
        /// Get Najm Responses With Policy Holder Nin
        /// </summary>
        /// <param name="policyHolderNin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/najm/getNajmResponsesBasedOnFilter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<NajmResponseModel>>))]
        public IActionResult GetAllNajmResponsesBasedOnFilter(NajmResponseFilterModel najmResponseFilterModel, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Najm Responses";
            log.PageURL = "/admin/najm";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllNajmResponsesBasedOnFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(najmResponseFilterModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(1, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                IQueryable<NajmResponseEntity> query = _najmService.GetAllNajmResponsesWithFilter(najmResponseFilterModel.ToServiceModel());
                var result = _najmService.GetAllNajmResponsesBasedOnFilter(query);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result.Select(e => e.ToModel()), query.ToList().Count);
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
        /// Get Najm Responses With Policy Holder Nin
        /// </summary>
        /// <param name="policyHolderNin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/najm/UpdateNajmResponse")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Output>))]
        public IActionResult UpdateNajmResponse(NajmResponseModel model)
        {
            var outPut = new Output();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Najm Responses";
            log.PageURL = "/admin/najm";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateNajmResponse";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (model == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Model is null";
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    return Error(outPut);
                }

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
                var isAuthorized = _userPageService.IsAuthorizedUser(1, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                NajmResponseEntity najmResponse = _najmService.GetNajmResponseById(model.Id);
                if (najmResponse == null)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "No Najm responses for this id " + model.Id;
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    return Error(outPut);
                }

                najmResponse.NCDReference = model.NCDReference;
                if (model.NCDFreeYears.HasValue)
                    najmResponse.NCDFreeYears = model.NCDFreeYears.Value;

                string exception = string.Empty;
                var updateEmail = _najmService.UpdateNajmResponse(najmResponse, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "Error happen when UpdateNajmResponse --> " + exception;
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "success";
                outPut.ErrorCode = (int)log.ErrorCode;
                outPut.ErrorDescription = log.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(outPut);
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

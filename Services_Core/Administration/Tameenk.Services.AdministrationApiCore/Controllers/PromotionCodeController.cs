using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Promotions;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;
using Newtonsoft.Json;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 20)]
    public class PromotionCodeController : AdminBaseApiController
    {
        #region Fields
        private readonly IPromotionService _promotionService;
        private readonly IUserPageService _userPageService;
        #endregion

        #region Ctor

        public PromotionCodeController(IPromotionService promotionService, IUserPageService userPageService)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(IPromotionService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
        }

        #endregion

        #region Actions

        /// <summary>
        /// Get promotion programs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/promotion-code/promotion-codes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramCodeModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionProgramCodes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Program Code";
            log.PageURL = "PromotionProgramCode";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramCodes";
            log.ServiceRequest = $"pageIndex: {pageIndex}, pageSize: {pageSize}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(20, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionCodes().Select(e => e.ToModel()));
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
        /// Get promotion codes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/promotion-code/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramCodeModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionCodeById(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Program Code";
            log.PageURL = "PromotionProgramCode";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionCodeById";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(20, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionCode(id).ToModel());
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
        /// Edit PromotionProgramCode 
        /// </summary>
        /// <param name="PromotionProgramCodeModel">Promotion Program Code Model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/promotion-code/edit")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PromotionProgramCodeModel>))]
        public IActionResult EditPromotionCode([FromBody]PromotionProgramCodeModel promotionProgramCodeModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Program Code";
            log.PageURL = "PromotionProgramCode";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditPromotionCode";
            log.ServiceRequest = JsonConvert.SerializeObject(promotionProgramCodeModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(20, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (promotionProgramCodeModel == null)
                {
                    return Error("The Object is Null.");
                }

                PromotionProgramCode promotionCode = promotionProgramCodeModel.ToEntity();
                promotionCode = _promotionService.UpdatePromotionProgramCode(promotionCode);
                promotionProgramCodeModel = promotionCode.ToModel();
                return Ok(promotionProgramCodeModel);
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
        /// Add new PromotionProgram
        /// </summary>
        /// <param name="promotionProgramModel">Insurance Company Model</param>
        /// <para>(default value (false) not replace and return error)</para> </param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/promotion-code/add")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PromotionProgramCodeModel>))]
        public async System.Threading.Tasks.Task<IActionResult> AddPromotionCode([FromBody]PromotionProgramCodeModel promotionProgramModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Program Code";
            log.PageURL = "PromotionProgramCode";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddPromotionCode";
            log.ServiceRequest = JsonConvert.SerializeObject(promotionProgramModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(20, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (promotionProgramModel == null)
                {
                    return Error("The Object is Null.");
                }
                PromotionProgramCode promotionCode = promotionProgramModel.ToEntity();
                if (promotionProgramModel.IsComperhensive == true)
                {
                    promotionCode.InsuranceTypeCode = 2;
                    promotionCode = _promotionService.AddPromotionProgramCode(promotionCode);
                }
                else
                {
                    promotionCode.InsuranceTypeCode = 1;
                    promotionCode = _promotionService.AddPromotionProgramCode(promotionCode);
                }
                PromotionProgramCodeModel res = promotionCode.ToModel();
                return Ok(res);
                

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        [HttpGet]
        [Route("api/promotion-code/promotionDetails")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramCodeModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionProgramCodesByProgramId(int programId ,int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Program Code";
            log.PageURL = "PromotionProgramCode";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramCodesByProgramId";
            log.ServiceRequest = $"programId: {programId}, pageIndex: {pageIndex}, pageSize: {pageSize}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(20, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionProgramCodesByProgramId(programId).Select(e => e.ToModel()));
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

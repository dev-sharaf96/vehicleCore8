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
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Core;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;
using Newtonsoft.Json;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 19)]
    public class PromotionDomianController : AdminBaseApiController
    {
        #region Fields
        private readonly IPromotionService _promotionService;
        private readonly IUserPageService _userPageService;
        #endregion

        #region Ctor

        public PromotionDomianController(IPromotionService promotionService, IUserPageService userPageService)
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
        [Route("api/PromotionDomian/promotion-Domain")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramDomainModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionProgramDomains(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramDomains";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionProgramDomains().Select(e => e.ToModel()));
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
        /// Get promotion programs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/PromotionDomian/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramDomainModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionProgramDomainById(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramDomainById";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionProgramDomain(id).ToModel());
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
        /// Edit PromotionProgram 
        /// </summary>
        /// <param name="PromotionProgramModel">Promotion Program Model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PromotionDomian/edit")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PromotionProgramModel>))]
        public IActionResult EditPromotion([FromBody]PromotionProgramDomainModel promotionProgramDomainModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion";
            log.PageURL = "promotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditPromotion";
            log.ServiceRequest = JsonConvert.SerializeObject(promotionProgramDomainModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (promotionProgramDomainModel == null)
                {
                    return Error("The Object is Null.");
                }

                PromotionProgramDomain promotionProgramDomain = promotionProgramDomainModel.ToEntity();
                promotionProgramDomain = _promotionService.UpdatePromotionProgramDomain(promotionProgramDomain);
                promotionProgramDomainModel = promotionProgramDomain.ToModel();
                return Ok(promotionProgramDomainModel);
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
        [Route("api/PromotionDomian/add")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PromotionProgramDomainModel>))]
        public async System.Threading.Tasks.Task<IActionResult> AddPromotionDomain([FromBody]PromotionProgramDomainModel promotionProgramDomainModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion";
            log.PageURL = "promotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddPromotionDomain";
            log.ServiceRequest = JsonConvert.SerializeObject(promotionProgramDomainModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (promotionProgramDomainModel == null)
                {
                    return Error("The Object is Null.");
                }

                PromotionProgramDomain promotionProgramDomain = promotionProgramDomainModel.ToEntity();

                List<PromotionProgramDomain> promotionsDomain= GetPromotionDomainList((int)promotionProgramDomain.PromotionProgramId , promotionProgramDomain.Domian);
               _promotionService.AddBulkPromotionProgramDomain(promotionsDomain);
                return Ok(_promotionService.GetPromotionProgramDomainByProgramId((int)promotionProgramDomain.PromotionProgramId).Select(e => e.ToModel()));
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
        [Route("api/PromotionDomian/promotionDomainsByProgramId")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramDomainModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult GetPromotionProgramDomainByProgramId(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion";
            log.PageURL = "Promotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramDomainByProgramId";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.GetPromotionProgramDomainByProgramId(id).Select(e => e.ToModel()));
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
        [Route("api/PromotionDomian/changeStatus")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramDomainModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult ChangeStatus(int id, bool status)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion";
            log.PageURL = "Promotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ChangeStatus";
            log.ServiceRequest = $"id: {id}, status: {status}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                return Ok(_promotionService.ChangePromotionDomainStatus(id, status).Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }


        [HttpPost]
        [Route("api/PromotionDomian/updatePromotionDomain")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramDomainModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public async System.Threading.Tasks.Task<IActionResult> UpdatePromotionDomain([FromBody]PromotionProgramDomainModel promotionProgramDominModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion";
            log.PageURL = "Promotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdatePromotionDomain";
            log.ServiceRequest = JsonConvert.SerializeObject(promotionProgramDominModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                PromotionProgramDomain promotionProgramDomin = promotionProgramDominModel.ToEntity();
                return Ok(_promotionService.UpdatePromotionDomain(promotionProgramDomin));

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

        #region PrivateMethods 
        public List<PromotionProgramDomain> GetPromotionDomainList(int programId ,string domains)
        {
            List<PromotionProgramDomain> promotionDomains = new List<PromotionProgramDomain>();

             
             char[] splitDelimiter = { ',', ';','/'};
             var domainArr = domains.Split(splitDelimiter);
            foreach (var item in domainArr)
            {
                promotionDomains.Add(new PromotionProgramDomain()
                {
                    DomainNameAr = item.ToString(),
                    Domian = item.ToString(),
                    DomainNameEn = item.ToString(),
                    PromotionProgramId = programId,
                    IsActive = true

                });

            }



            return promotionDomains;


        }


        #endregion
    }
}
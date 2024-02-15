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
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Core.Exceptions;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using Tameenk.Core.Configuration;
using OfficeOpenXml;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Implementation;
using Tameenk.Services.Administration.Identity;
using Tameenk.Resources.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class PromotionController : AdminBaseApiController
    {
        #region Fields
        private readonly IPromotionService _promotionService;
        private readonly IUserPageService _userPageService;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IExcelService _excelService;
        #endregion

        #region Ctor

        public PromotionController(IPromotionService promotionService, IUserPageService userPageService
            , TameenkConfig tameenkConfig, IExcelService excelService)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(IPromotionService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
        }

        #endregion

        #region Actions

        /// <summary>
        /// Get promotion programs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Promotions/promotion-programs")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult GetPromotionPrograms(int pageIndex = 0, int pageSize = int.MaxValue)

        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionPrograms";
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
                return Ok(_promotionService.GetPromotionPrograms().Select(e => e.ToModel()));
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
        [Route("api/Promotions/userpromotion-programs")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult GetUserPromotionPrograms(string useremail,int pageIndex = 0, int pageSize = int.MaxValue)

        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetUserPromotionPrograms";
            log.ServiceRequest = $"useremail: {useremail}, pageIndex: {pageIndex}, pageSize:{pageSize}";
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
                return Ok(_promotionService.GetUserPromotionPrograms(useremail, pageIndex, pageSize).Select(e=>e.ToModel()));
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
        [Route("api/Promotions/getAllPromotions")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult GetAllPromotions(int pageIndex = 0, int pageSize = int.MaxValue)

        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllPromotions";
            log.ServiceRequest = $"pageIndex: {pageIndex}, pageSize:{pageSize}";
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
                return Ok(_promotionService.GetAllPromotionPrograms().Select(e => e.ToModel()));
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
        [Route("api/promotions/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult GetPromotionProgramById(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramById";
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
                return Ok(_promotionService.GetPromotionProgram(id).ToModel());
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
        [Route("api/promotions/add")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PromotionProgramModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public async System.Threading.Tasks.Task<IActionResult> AddPromotion([FromBody]PromotionProgramModel promotionProgramModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddPromotion";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
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
                if (promotionProgramModel.EffectiveDate >= promotionProgramModel.DeactivatedDate)
                {
                    return Error("Promotion Effective Date must be greater than Deactivated Date ..!");
                }

                PromotionProgram promotionProgram = promotionProgramModel.ToEntity();
                promotionProgram.ValidationMethodId = 1;
                promotionProgram.IsActive = true;
                promotionProgram.IsPromoByEmail = true;
                promotionProgram = _promotionService.AddPromotionProgram(promotionProgram);
                PromotionProgramModel res = promotionProgram.ToModel();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
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
        [Route("api/Promotions/changeStatus")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult ChangeStatus(int id , bool status)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ChangeStatus";
            log.ServiceRequest = $"id: {id}, status:{status}";
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
                return Ok(_promotionService.ChangePromotionStatus(id, status).Select(e => e.ToModel()));
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
        [Route("api/Promotions/updatePromotion")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public async System.Threading.Tasks.Task<IActionResult> UpdatePromotion([FromBody]PromotionProgramModel promotionProgramModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "add Promotion";
            log.PageURL = "/promotion/addPromotion";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdatePromotion";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(19, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                PromotionProgram promotionProgram = promotionProgramModel.ToEntity();
                if(promotionProgram.EffectiveDate >= promotionProgram.DeactivatedDate)
                {
                    return Error("Promotion Effective Date must be greater than Deactivated Date ..!");
                }

                return Ok(_promotionService.UpdatePromotion(promotionProgram).Select(e => e.ToModel()));

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        #region Promotion Diescounts sheet

        [Authorize]
        [HttpPost]
        [Route("api/Promotions/promotion-discouns-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<PromotionDiscountModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GetPromotionDiscountsWithFilter([FromBody]PromotionDiscountModel model, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Deserving Offers";
            log.PageURL = "/admin/promotion/deservingOffers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionDiscountsWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (model == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                DeservingDiscount modelEntity = model.ToEntity();
                var result = _promotionService.GetAllDeservingDiscountsFromDBWithFilter(modelEntity, pageIndex, pageSize, false, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
                }

                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result.Select(res => res.ToEntity()), totalCount);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Promotions/promotion-discouns-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult ExportPromotionDiscounts([FromBody]PromotionDiscountModel model, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Deserving Offers";
            log.PageURL = "/admin/promotion/deservingOffers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ExportPromotionDiscounts";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (model == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                DeservingDiscount modelEntity = model.ToEntity();
                var result = _promotionService.GetAllDeservingDiscountsFromDBWithFilter(modelEntity, pageIndex, pageSize, true, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
                }

                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.GenerateOffersExcel(result);
                if (file == null && file.Length == 0)
                    return Ok("");

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Convert.ToBase64String(file));            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        /// <summary>
        /// Delete Vehicle Requests
        /// </summary>
        /// <param name="id">row id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Promotions/promotion-discoun-delete")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Policy.Components.PdfGenerationOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult DeleteDeservingOffers(int id)
        {
            Tameenk.Services.Policy.Components.PdfGenerationOutput output = new Policy.Components.PdfGenerationOutput();

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Deserving Offers";
            log.PageURL = "/admin/promotion/deservingOffers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteDeservingOffers";
            log.ServiceRequest = $"row id: {id}";
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
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "User not authorized";
                    return Error(output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "User not authenticated";
                    return Error(output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "Not Authorized : " + log.UserID;
                    return Error(output);
                }
                if (id <= 0)
                    throw new TameenkArgumentNullException("Record id is invalid");

                string exception = string.Empty;
                bool result = _promotionService.DeleteDeservingOffersRecord(id, out exception);

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Result Error";
                    return Error(output);
                }

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Deleted Successfully";
                return Error(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error";
                return Error("an error has occured");
            }

        }

        [HttpPost]
        [Route("api/Promotions/promotion-discouns-upload")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Output>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult UploadOffersSheet([FromBody]FileModel file)
        {
            Output output = new Output();

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Deserving Offers";
            log.PageURL = "/admin/promotion/deservingOffers";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UploadOffersSheet";
            log.ServiceRequest = JsonConvert.SerializeObject(file);
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
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "User not authorized";
                    return Error(output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "User not authenticated";
                    return Error(output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    //output.ErrorCode = Policy.Components.PdfGenerationOutput.ErrorCodes.
                    output.ErrorDescription = "Not Authorized : " + log.UserID;
                    return Error(output);
                }
                if (file == null)
                {
                    //log.ErrorCode = 10;
                    log.ErrorDescription = "File is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Policy.Components.PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                    output.ErrorDescription = "File is empty";
                    return Error(output);
                }
                if (file.Content == null)
                {
                    //log.ErrorCode = 10;
                    log.ErrorDescription = "File content is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Policy.Components.PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                    output.ErrorDescription = "File content is empty";
                    return Error(output);
                }

                // first save file to server
                string exception = string.Empty;
                var isRemoteServer = _tameenkConfig.RemoteServerInfo.UseNetworkDownload;
                var domain = _tameenkConfig.RemoteServerInfo.DomainName;
                var serverIP = _tameenkConfig.RemoteServerInfo.ServerIP;
                var username = _tameenkConfig.RemoteServerInfo.ServerUserName;
                var password = _tameenkConfig.RemoteServerInfo.ServerPassword;

                string path = SaveOffersSheetFile(file.Content, "." + file.Extension, isRemoteServer, domain, serverIP, username, password, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "There is an error occured, please try again later";
                    return Error(output);
                }
                if (string.IsNullOrEmpty(path))
                {
                    //log.ErrorCode = 11;
                    log.ErrorDescription = "File Path is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                    output.ErrorDescription = "File Path is null";
                    return Error(output);
                }

                // initialize db model
                IDictionary<int, string> invalidRows = new Dictionary<int, string>();
                exception = string.Empty;
                List<DeservingDiscount> dataList = InitializeDBModel(path, out exception, out invalidRows);
                if (!string.IsNullOrEmpty(exception))
                {
                    //log.ErrorCode = 11;
                    log.ErrorDescription = "Error when read data from excel file due to "+exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error when read data from excel file";
                    return Error(output);
                }

                if (dataList == null || dataList.Count == 0)
                {
                    //log.ErrorCode = 11;
                    log.ErrorDescription = "Excel is empty, there is no data";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = log.ErrorDescription;
                    return Error(output);
                }
                log.ServiceRequest = JsonConvert.SerializeObject(dataList);
                exception = string.Empty;
                _promotionService.AddBulkOffersDataSheet(dataList, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "an error has occured";
                    return Error(output);
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.ErrorCode = (int)Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "success";

                if (invalidRows != null && invalidRows.Count > 0)
                {
                    output.ErrorDescription = "Data imported succefully, but the blew rows are invalid: \n";
                    foreach (var item in invalidRows)
                    {
                        output.ErrorDescription += "row numer: " + item.Key + ", message: " + item.Value + "\n";
                    }
                }

                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        public string SaveOffersSheetFile(byte[] file, string extension, bool isRemoteServer, string domain, string serverIP, string username, string password, out string exception)
        {
            try
            {
                exception = string.Empty;
                FileNetworkShare fileShare = new FileNetworkShare();
                var currentDate = DateTime.Now;
                var generatedSheetFileName = $"{currentDate.ToString("HHmmss", new CultureInfo("en-US"))}{extension}";
                string offerSheetDirPath = Path.Combine(Utilities.GetAppSetting("OffersSheet"), currentDate.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), currentDate.Hour.ToString(new CultureInfo("en-US")));
                string generatedSheetFilePath = Path.Combine(offerSheetDirPath, generatedSheetFileName);

                if (isRemoteServer)
                {
                    string reportFilePath = generatedSheetFilePath;
                    generatedSheetFilePath = serverIP + "\\" + generatedSheetFilePath;
                    offerSheetDirPath = serverIP + "\\" + offerSheetDirPath;
                    if (fileShare.UploadFileToShare(domain, username, password, offerSheetDirPath, generatedSheetFilePath, file, serverIP, out exception))
                        return reportFilePath;
                    else
                        return string.Empty;
                }
                else
                {
                    if (!Directory.Exists(offerSheetDirPath))
                        Directory.CreateDirectory(offerSheetDirPath);

                    File.WriteAllBytes(generatedSheetFilePath, file);
                    return generatedSheetFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return "an error has occured";
            }
        }

        private List<DeservingDiscount> InitializeDBModel(string path, out string exception, out IDictionary<int, string> invalidRows)
        {
            exception = string.Empty;
            invalidRows = new Dictionary<int, string>();
            string invalidRowException = string.Empty;
            List<DeservingDiscount> dataList = new List<DeservingDiscount>();

            try
            {
                //create a new Excel package in a memorystream
                byte[] dataBytes = null;
                if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload) //server 2
                {
                    FileNetworkShare fileShare = new FileNetworkShare();
                    var file = fileShare.GetFile(path, out exception);
                    if (file != null)
                        dataBytes = file;
                    else
                        return new List<DeservingDiscount>();
                }
                else
                    dataBytes = File.ReadAllBytes(path);

                FileInfo fi = new FileInfo(path);
                using (FileStream fs = fi.Create())
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(fs))
                    {
                        DeservingDiscount obj = new DeservingDiscount();
                        foreach (var worksheet in excelPackage.Workbook.Worksheets)
                        {
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            //loop all rows
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var model = new DeservingDiscount();

                                if (EmptyRow(worksheet, row, ColCount))
                                    break;

                                int emptycol = 0;
                                int probCount = 0;
                                //loop all columns in a row
                                for (int col = 1; col <= ColCount; col++)
                                {
                                    if (probCount == ColCount)
                                        break;

                                    var cellName = worksheet.Cells[1, col, 1, col].Text?.Trim();
                                    var cellValue = worksheet.Cells[row, col].Value?.ToString();
                                    if (worksheet.Cells[row, col].Value != null)
                                    {
                                        if (!ValidateNationalIdAndExpiryDate(cellName, cellValue, out invalidRowException))
                                        {
                                            if (!string.IsNullOrEmpty(invalidRowException))
                                                invalidRows.Add(row, invalidRowException);
                                            break;
                                        }

                                        switch (cellName)
                                        {
                                            case "NationalId":
                                                model.NationalId = cellValue;
                                                break;

                                            case "Name":
                                                model.Name = cellValue;
                                                break;

                                            case "Mobile":
                                                model.Mobile = cellValue;
                                                break;

                                            case "ExpiryDate":
                                                DateTime dDate = new DateTime();
                                                if (DateTime.TryParse(cellValue, out dDate))
                                                {
                                                    //var newData = dDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                                    //model.ExpiryDate = DateTime.ParseExact(newData, "yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US"));
                                                    model.ExpiryDate = dDate;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        invalidRows.Add(row, cellName + " is empty");
                                        emptycol++;
                                    }

                                    probCount++;
                                }
                                if (emptycol < ColCount)
                                {
                                    model.IsDeleted = false;
                                    model.CreatedDate = DateTime.Now;
                                    dataList.Add(model);
                                }
                            }
                        }
                    }
                }

                return dataList;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return new List<DeservingDiscount>();
            }
        }

        private static bool EmptyRow(ExcelWorksheet worksheet, int row, int colcount)
        {
            int emptyCount = 1;
            for (int col = 1; col < colcount; col++)
            {
                if (worksheet.Cells[row, col].Value == null)
                {
                    emptyCount++;
                }
            }
            if (emptyCount == colcount)
                return true;
            return false;
        }

        private static bool ValidateNationalIdAndExpiryDate(string cell, string value, out string invalidRowException)
        {
            invalidRowException = string.Empty;
            if (cell == "NationalId")
            {
                if (string.IsNullOrEmpty(value))
                {
                    invalidRowException = "NationalId value is empty";
                    return false;
                }

                foreach (char c in value)
                {
                    if (!char.IsDigit(c))
                    {
                        invalidRowException = "NationalId value contains character";
                        return false;
                    }
                }
            }

            if (cell == "ExpiryDate")
            {
                if (string.IsNullOrEmpty(value))
                {
                    invalidRowException = "ExpiryDate value is empty";
                    return false;
                }

                DateTime dDate;
                if (!DateTime.TryParse(value, out dDate))
                {
                    invalidRowException = "ExpiryDate value is not valid date";
                    return false;
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// get all national ids linked with specific promotion program by promotion program id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Promotions/promotionNationalIdsByProgramId")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PromotionProgramNinsModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 19)]
        public IActionResult GetPromotionProgramNinsByProgramId(int id, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "PromotionProgramNins";
            log.PageURL = "promotion/addPromotionProgramNins";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPromotionProgramNinsByProgramId";
            log.ServiceRequest = $"promotionProgramId: {id}, pageIndex: {pageIndex}, pageSize: {pageSize}";
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

                int totalCount = 0;
                string exception = string.Empty;
                var result = _promotionService.GetPromotionProgramNationalIdsByProgramId(id, pageIndex, pageSize, out exception, out totalCount);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(log.ErrorDescription);
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result.Select(e => e.ToPromotionProgramNinsModel()), totalCount);
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
        /// mark national id as deleted from promotion program
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Promotions/deleteNinFromPromotinProgram")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 63)]
        public IActionResult DeleteNinFromPromotinProgram(int rowId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "PromotionProgramNins";
            log.PageURL = "promotion/addPromotionProgramNins";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteNinFromPromotinProgram";
            log.ServiceRequest = $"Promotion Program Nins RowId: {rowId}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(63, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (rowId < 1)
                    throw new TameenkArgumentNullException("DB RowId can't be null.");

                string exception = string.Empty;
                bool result = _promotionService.DeleteNinFromPromotionProgram(rowId, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
                }

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

        #region Promotion Program Approvals

        [HttpPost]
        [Route("api/Promotions/all-approvals-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PromotionProgramApprovalsModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 63)]
        public IActionResult GetAllApromotionApprovalsWithFilter([FromBody]PromotionProgramApprovalsFilterModel filterModel, int pageIndex = 1, int pageSize = 10)
        {
            AdministrationOutput<List<PromotionProgramApprovalsModel>> Output = new AdministrationOutput<List<PromotionProgramApprovalsModel>>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Approvals";
            log.PageURL = "/promotion/approvals";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllApromotionApprovalsWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(63, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                int totalCount = 0;                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var allApprovals = _promotionService.GetAllPromotionApprovalsFromDBWithFilter(filterModel, pageIndex, pageSize, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new List<PromotionProgramApprovalsModel>();
                Output.Result = allApprovals.Select(e => e.ToModel()).ToList();

                Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.Success;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output, totalCount);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<List<PromotionProgramApprovalsModel>>.ErrorCodes.ServiceException;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/Promotions/approvePromotionApproval")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        //[AdminAuthorizeAttribute(pageNumber: 36)]
        public IActionResult ApprovePromotionApproval([FromBody]PromotionProgramApprovalActionModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Approvals";
            log.PageURL = "/promotion/approvals";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ApprovePromotionApproval";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(63, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _promotionService.ApprovePromotionProgram(model, log.UserID, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while approve, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = true;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/Promotions/deletePromotionApproval")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        //[AdminAuthorizeAttribute(pageNumber: 36)]
        public IActionResult DeletePromotionApproval([FromBody]PromotionProgramApprovalActionModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Approvals";
            log.PageURL = "/promotion/approvals";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeletePromotionApproval";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(63, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _promotionService.DeletePromotionProgram(model, log.UserID, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while delete, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = true;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpGet]
        [Route("api/Promotions/getImage")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetImage(int id, string lang)
        {
            AdministrationOutput<byte[]> Output = new AdministrationOutput<byte[]>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Promotion Approvals";
            log.PageURL = "/promotion/approvals";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ApprovePromotionApprovalGetImage";
            log.ServiceRequest = $"id={id} & lang={lang}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(63, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var data = _promotionService.GetPromotionUserById(id);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (data == null)
                {
                    Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.NullResult;
                    Output.ErrorDescription = string.Format(PromotionProgramResource.ResourceManager.GetString("PromotionNotFound", CultureInfo.GetCultureInfo(lang)), id);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "No record in PromotionUser table with this id: " + id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (string.IsNullOrEmpty(data.AttachmentPath))
                {
                    Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.NullResult;
                    Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("PromotionAttachmentNotFound", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "data.AttachmentPath is empty with this id: " + id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string fileBytes = string.Empty;
                if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload) //server 2
                {
                    FileNetworkShare fileShare = new FileNetworkShare();
                    exception = string.Empty;
                    var file = fileShare.GetFile(data.AttachmentPath, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.ExceptionError;
                        Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ReadFile_ServiceException", CultureInfo.GetCultureInfo(lang));
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "error happend while get file from remote server, and the error is: " + exception;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(Output);
                    }

                    Output.Result = file;
                    fileBytes = Convert.ToBase64String(file);
                }
                else
                    Output.Result = File.ReadAllBytes(data.AttachmentPath);
                //fileBytes = Convert.ToBase64String(File.ReadAllBytes(data.AttachmentPath));

                Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.Success;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<byte[]>.ErrorCodes.ServiceException;
                Output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        #endregion

        #endregion
    }
}

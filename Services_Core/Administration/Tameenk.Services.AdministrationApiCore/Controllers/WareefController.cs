using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.AdministrationApi.Models.OutPut;
using Tameenk.Services;
using Tameenk.Services.Implementation;
using Tameenk.Services.Administration.Identity;
using Tameenk.Core.Configuration;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Promotions;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using System.Globalization;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class WareefController : AdminBaseApiController
    {
        private readonly IUserPageService _userPageService;
        private readonly IWareefService _wareefService;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IExcelService _excelService;
        private readonly IPromotionService _promotionService;
        public WareefController(IUserPageService userPageService, IWareefService wareefService, TameenkConfig tameenkConfig, IExcelService excelService, IPromotionService promotionService)
        {
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _wareefService = wareefService;
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(IPromotionService));
        }
        #region Wareef Item
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/Add")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AddWareef([FromBody] WareefModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "wareefAdd";
            log.PageURL = "/Wareef/Add";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddWareef";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(88, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameAr))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "partner Arabic Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameEn))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "partner  English Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.ImageData?.ImageData))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = " partner Image Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.category == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Select partner Category";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.category?.id==null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Select partner Category";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output); 
                }
                string exption = string.Empty;
                var res = _wareefService.Save(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "error Insert To partner the ex : "+ exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Fail to save partner";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log); 
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription=ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/Edit")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult EditWareef([FromBody] WareefModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "wareefEdit";
            log.PageURL = "/Wareef/Edit";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "wareefEdit";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(88, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "partner edit model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.Id == null || model.Id <= 0)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "partner edit id is invalid ";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameAr))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "partner Arabic Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameEn))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "partner  English Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.ImageData?.ImageData) && string.IsNullOrEmpty(model.ImageData?.NewImageData))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = " partner Image Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.category== null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Select partner Category";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.category?.id == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Select partner Category";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                string exption = string.Empty;
                var res = _wareefService.Edit(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "false error";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }

                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/Delete")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult DeleteWareef([FromBody] WareefModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "wareefDelete";
            log.PageURL = "/Wareef/Delete";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "wareefDelete";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(88, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Delete model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (model.Id == null || model.Id <= 0)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Invalid selected item for  Delete";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                string exption = string.Empty;
                var res = _wareefService.Delete(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "error when delete  "+ exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "fail to delete Partner";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }

                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/All")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult GetAll()
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "wareefGetAll";
            log.PageURL = "/Wareef/All";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetWareef";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(88, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.GetAll(out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "error when Get Data ex is :" + exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "No data Found ";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.Data = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }
        #endregion
        #region Wareef Category
        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/AllCategory")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AllCategory()
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "AllCategory";
            log.PageURL = "/Wareef/AllCategory";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AllCategory";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(87, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.GetAllCategory(out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription ="error when Get Categories EX is :"+ exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "Categories is NUll ";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.CategoryData = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/AddCategory")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AddCategory([FromBody] WareefCategoryModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "AddCategory";
            log.PageURL = "/Wareef/AddCategory";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddCategory";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(87, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Category Model Is Null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (string.IsNullOrEmpty(model.NameAr))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "category Arabic Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameEn))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "category  English Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                //if (string.IsNullOrEmpty(model.Icon))
                //{
                //    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                //    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                //    log.ErrorDescription = " Category Icon Is required";
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error(output);
                //}
                string exption = string.Empty;
                var res = _wareefService.SaveCategory(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    output.ErrorDescription = " fail to add Category EX is: "+exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    output.ErrorDescription = " fail to add Category" ;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("error saving");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success; 
                output.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/EditCategory")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult EditCategory([FromBody] WareefCategoryModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "EditCategory";
            log.PageURL = "/Wareef/EditCategory";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditCategory";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(87, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "EditCategory model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.Id == null || model.Id <= 0)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "EditCategory model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameAr))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "category Arabic Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.NameEn))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "category  English Name Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.Icon))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = " Category Icon Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                string exption = string.Empty;
                var res = _wareefService.EditCategory(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "fail to edit Category";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/DeleteCategory")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult DeleteCategory([FromBody] DeleteCategoryModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "DeleteCategory";
            log.PageURL = "/Wareef/DeleteCategory";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteCategory";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(87, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "DeleteCategory model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model.categoryId <= 0)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "fail to delete category ";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                string exption = string.Empty;
                var res = _wareefService.DeleteCategory(model.categoryId, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription ="fail to delete Ex is : "+ exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (!res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "error delete category";
                    return Error("delete error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }
        #endregion

        #region discounts 
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/AddDiscount")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AddWareefDiscount([FromBody] WareefDiscountBenefits model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "wareefDiscount";
            log.PageURL = "/Wareef/AddDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddWareefDiscount";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(89, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Discount Model Is Null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Discount Model Is Null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (model.item == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "item is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.DiscountValue))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "Descount Value Is required";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                string exption = string.Empty;
                var res = _wareefService.SaveDiscount(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption) || !res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "error Add descount";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/EditDiscount")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult EditWareefDiscount([FromBody] WareefDiscountBenefits model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "EditDiscount";
            log.PageURL = "/Wareef/EditDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditDiscount";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(89, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                string exption = string.Empty;
                var res = _wareefService.EditDiscount(model, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption) || !res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/DeleteDiscount")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult DeleteDiscount(DeleteDiscountBenefitModel model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "DeleteDiscount";
            log.PageURL = "/Wareef/DeleteDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteDiscount";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(89, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (model.discountId < 1)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    log.ErrorDescription = "model is null";
                    return Ok(output);
                }
                string exption = string.Empty;
                var res = _wareefService.DeleteDiscount(model.discountId, log.UserName, out exption);
                if (!string.IsNullOrEmpty(exption) || !res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/AllWareefDiscounts")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AllDiscounts()
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "AllDiscount";
            log.PageURL = "/Wareef/AllDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AllDiscount";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(89, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.GetAllDiscounts(out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "result is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("false error");
                }
                // get benifits
                var disDetails = _wareefService.GetAllDiscountDetails(out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
               

                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.DiscountData = res;
                output.customDiscountBenefits = disDetails;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/AllWareefDiscountDetails")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult AllDiscountDetails(int discountId)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "AllDiscountDetails";
            log.PageURL = "/Wareef/AllDiscountDetails";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AllDiscountDetails";
            log.ServiceRequest = JsonConvert.SerializeObject(discountId);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(90, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.GetAllDiscountDetails(discountId , out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("exption error");
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    log.ErrorDescription = "result is null";
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.DiscountBenefits = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }

        #endregion
        #region discount details benfits
          [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/GetCategoryPartners")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult GetCategoryPartners(int categoryId)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "GetCategoryPartners";
            log.PageURL = "/Wareef/GetCategoryPartners";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetWareef";
            // log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(91, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }



                string exption = string.Empty;
                var res = _wareefService.wareefParnersPerCategory(categoryId, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = exption;
                    return Error("exption error");
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "false error";
                    return Error("false error");
                }

                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.WarefParteners = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/PartnerDiscounts")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult PartnerDiscounts(int partnerId)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "GetPartnerDiscounts";
            log.PageURL = "/Wareef/GetPartnerDiscounts";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPartnerDiscounts";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(91, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.wareefParnerDiscounts(partnerId, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = exption;
                    log.ErrorDescription = exption;
                    return Error(output);
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "false error";
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.DiscountData = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("api/Wareef/DiscountBenfitsDescription")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult DiscountBenfitsDescription(int DiscountId)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "DiscountBenfitsDescription";
            log.PageURL = "/Wareef/DiscountBenfitsDescription";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DiscountBenfitsDescription";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(91, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exption = string.Empty;
                var res = _wareefService.wareefDiscountBenfitsDiscription(DiscountId, out exption);
                if (!string.IsNullOrEmpty(exption))
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = exption;
                    log.ErrorDescription = exption;
                    return Error(output);
                }
                if (res == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    log.ErrorDescription = "false error";
                    return Error("false error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                output.DiscountBenefits = res;
                log.ErrorDescription = "success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.Message);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("api/Wareef/EditDiscountBenefitDescription")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        public IActionResult EditDiscountBenefitDescription([FromBody] List<WareefDiscountBenefit> model)
        {
            var output = new WareefOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "EditDiscountBenefitDescription";
            log.PageURL = "/Wareef/EditDiscountBenefitDescription";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "EditDiscountBenefitDescription";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {

                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(91, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (model == null)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    return Ok(output);
                }
                if (model.Count < 0)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    return Ok(output);
                }

                string exption = string.Empty;
                var res = _wareefService.EditDiscountBenefitDiscriptions(model, out exption);
                if (!string.IsNullOrEmpty(exption) || !res)
                {
                    output.ErrorCode = WareefOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                    return Error("exption error");
                }
                output.ErrorCode = WareefOutput.ErrorCodes.Success;
                output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        #endregion


        #region Upload Wareef Discount Client
        [HttpPost]
        [Route("api/Wareef/UploadWareefDiscount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Tameenk.Api.Core.Models.ErrorModel>))]
        [AdminAuthorizeAttribute(pageNumber: 92)]
        public IActionResult UploadWareefDiscountSheet([FromBody] FileModel file)
        {
            Output output = new Output();

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Deserving Offers";
            log.PageURL = "api/Wareef/UploadWareefDiscount";
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
                    output.ErrorDescription = "User not authorized";
                    return Error(output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorDescription = "User not authenticated";
                    return Error(output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(92, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorDescription = "Not Authorized : " + log.UserID;
                    return Error(output);
                }
                if (file == null)
                {
                    log.ErrorDescription = "File is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Policy.Components.PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                    output.ErrorDescription = "File is empty";
                    return Error(output);
                }
                if (file.Content == null)
                {
                    log.ErrorDescription = "File content is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Policy.Components.PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                    output.ErrorDescription = "File content is empty";
                    return Error(output);
                }
                if (file.Date == null)
                {
                    log.ErrorDescription = "Date is empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    output.ErrorCode = (int)Policy.Components.PdfGenerationOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Date is empty";
                    return Error(output);
                }

                IDictionary<int, string> invalidRows = new Dictionary<int, string>();
               string exception = string.Empty;
                List<DeservingDiscount> dataList = InitializeDBModel(file.Content, file.Date, out exception, out invalidRows);
                if (dataList == null || dataList.Count == 0)
                {
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

        private List<DeservingDiscount> InitializeDBModel(byte[] file, DateTime date, out string exception, out IDictionary<int, string> invalidRows)
        {
            exception = string.Empty;
            invalidRows = new Dictionary<int, string>();
            string invalidRowException = string.Empty;
           // date = date.AddHours(23).AddMinutes(59).AddSeconds(58);  
            List<DeservingDiscount> dataList = new List<DeservingDiscount>();
            try
            {
                using (MemoryStream fs = new MemoryStream(file))
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(fs))
                    {
                        DeservingDiscount obj = new DeservingDiscount();
                        foreach (var worksheet in excelPackage.Workbook.Worksheets)
                        {
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var model = new DeservingDiscount();
                                if (EmptyRow(worksheet, row, ColCount))
                                    break;
                                int emptycol = 0;
                                int probCount = 0;
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
                                    model.ExpiryDate = date;
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
    }
}

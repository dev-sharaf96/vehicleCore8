using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Resources.Checkout;
using Tameenk.Loggin.DAL;
using Newtonsoft.Json;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Implementation;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class CheckoutsController : AdminBaseApiController
    {
        #region Fields
        private readonly ICheckoutsService _checkoutsService;
        private readonly IUserPageService _userPageService;
        #endregion


        #region The Constructor 

        /// <summary>
        /// The Constructor  .
        /// </summary>
        /// <param name="checkoutsService">checkouts Service</param>
        public CheckoutsController(ICheckoutsService checkoutsService, IUserPageService userPageService)
        {
            _checkoutsService = checkoutsService ?? throw new TameenkArgumentNullException(nameof(ICheckoutsService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));

        }
        #endregion

        #region Methods 


        /// <summary>
        /// Get Checkouts With Filter
        /// </summary>
        /// <param name="checkoutsFilterModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/checkouts/getCheckoutsWithFilter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CheckoutsModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetCheckoutsWithFilter(CheckoutsFilterModel checkoutsFilterModel, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ReferenceId", bool sortOrder = false)
        {
            try
            {
                if (checkoutsFilterModel == null)
                    throw new TameenkArgumentNullException("checkoutsFilterModel");

                IQueryable<CheckoutDetail> query = _checkoutsService.PrepareCheckoutDetailsQueryWithFilter(checkoutsFilterModel.ToServiceModel());

                var result = _checkoutsService.GetCheckoutsWithFilter(query, pageIndex, pageSize, sortField, sortOrder);
                return Ok(result.Select(e => e.ToModel()), query.ToList().Count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// get all details to specific checkout by referenceId
        /// </summary>
        /// <param name="referenceId">reference Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/checkouts/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CheckoutsModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetCheckoutDetails(string referenceId)
        {
            try
            {
                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("reference id can't be null.");

                CheckoutDetail checkoutDetail = _checkoutsService.GetCheckoutDetailsByReferenceId(referenceId);

                if (checkoutDetail == null)
                {
                    throw new TameenkArgumentException("Checkout not found ", "referenceId");
                }

                return Ok(checkoutDetail.ToModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }
        [HttpPost]        [Route("api/checkouts/getAllCheckedoutPoliciesBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CheckoutDriverInfo>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetAllCheckedoutPoliciesBasedOnFilter([FromBody]PolicyCheckoutFilterModel policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ID", bool sortOrder = false)        {            try            {                if (policyFilter == null)                    throw new TameenkArgumentNullException("policyFilter");                CheckoutDriverInfo result = _checkoutsService.GetAllCheckedoutPoliciesBasedOnFilter(policyFilter.ToModel());                return result != null ? Ok(result.ToServiceModel()) : Ok(new PolicyCheckoutModel() { ID = 0 });            }            catch (Exception ex)            {                return Error("an error has occured");            }        }        [HttpPost]        [Route("api/checkouts/updateCheckedoutPolicy")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 7)]        public IActionResult UpdateCheckedoutPolicy([FromBody]PolicyCheckoutModel policyCheckoutModel)        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateCheckedoutPolicy";
            log.ServiceRequest = JsonConvert.SerializeObject(policyCheckoutModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.DriverNin = policyCheckoutModel.Nin;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try            {
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (policyCheckoutModel == null)                    throw new TameenkArgumentNullException("policyCheckoutModel");                bool isValidPhone = Utilities.IsValidPhoneNo(policyCheckoutModel.Phone);                bool isValidEmail = Utilities.IsValidMail(policyCheckoutModel.Email);                bool isValidIBAN = Utilities.IsValidIBAN(policyCheckoutModel.IBAN);                List<ErrorModel> errors = new List<ErrorModel>();                if (!isValidPhone)                {                    errors.Add(new ErrorModel(description: CheckoutResources.checkout_error_phone));                }                if (!isValidEmail)                {                    errors.Add(new ErrorModel(description: CheckoutResources.checkout_error_email));                }                if (!isValidIBAN)                {                    errors.Add(new ErrorModel(description: CheckoutResources.Checkout_InvalidIBAN));                }                if (errors.Any())                {                    return Error(errors);                }                CheckoutDriverInfo checkoutDriverInfo = policyCheckoutModel.ToModel();                checkoutDriverInfo.ModifiedBy = User.Identity.GetUserName();                checkoutDriverInfo.ModifiedDate = DateTime.Now;                if (!checkoutDriverInfo.IBAN.ToLower().StartsWith("sa"))                {                    checkoutDriverInfo.IBAN = "sa" + checkoutDriverInfo.IBAN;                }                bool result = _checkoutsService.UpdateCheckedoutPolicy(checkoutDriverInfo);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result);            }            catch (Exception ex)            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");            }        }        [HttpPost]        [Route("api/checkouts/addCheckedoutPolicy")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 33)]        public IActionResult addCheckedoutPolicy([FromBody]PolicyCheckoutModel policyCheckoutModel)        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "checkout";
            log.PageURL = "admin/policies/checkout";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "addCheckedoutPolicy";
            log.ServiceRequest = JsonConvert.SerializeObject(policyCheckoutModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.DriverNin = policyCheckoutModel.Nin;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try            {
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
                var isAuthorized = _userPageService.IsAuthorizedUser(33, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (policyCheckoutModel == null)                    throw new TameenkArgumentNullException("policyCheckoutModel");                bool isValidPhone = Utilities.IsValidPhoneNo(policyCheckoutModel.Phone);                bool isValidEmail = Utilities.IsValidMail(policyCheckoutModel.Email);                bool isValidIBAN = Utilities.IsValidIBAN(policyCheckoutModel.IBAN);                List<ErrorModel> errors = new List<ErrorModel>();                if (!isValidPhone)                {                    errors.Add(new ErrorModel(description: CheckoutResources.checkout_error_phone));                }                if (!isValidEmail)                {                    errors.Add(new ErrorModel(description: CheckoutResources.checkout_error_email));                }                if (!isValidIBAN)                {                    errors.Add(new ErrorModel(description: CheckoutResources.Checkout_InvalidIBAN));                }                if (errors.Any())                {                    return Error(errors);                }                CheckoutDriverInfo checkoutDriverInfo = policyCheckoutModel.ToModel();                checkoutDriverInfo.CreatedBy = User.Identity.GetUserName();                checkoutDriverInfo.CreatedDate = DateTime.Now;                if (!checkoutDriverInfo.IBAN.ToLower().StartsWith("sa"))                {                    checkoutDriverInfo.IBAN = "sa" + checkoutDriverInfo.IBAN;                }                bool result = _checkoutsService.AddCheckedoutPolicy(checkoutDriverInfo);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result);            }            catch (Exception ex)            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");            }        }        [HttpGet]        [Route("api/checkouts/deleteCheckedoutPolicy")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 33)]        public IActionResult DeleteCheckedoutPolicy(int Id)        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "checkout";
            log.PageURL = "admin/policies/checkout";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteCheckedoutPolicy";
            log.ServiceRequest = $"Id: {Id}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try            {                if (Id <= 0)                    throw new TameenkArgumentNullException("Id");
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
                var isAuthorized = _userPageService.IsAuthorizedUser(33, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }                CheckoutDriverInfo checkoutDriverInfo = new CheckoutDriverInfo();                checkoutDriverInfo.ID = Id;                checkoutDriverInfo.DeletedBy = User.Identity.GetUserName();                checkoutDriverInfo.DeletedDate = DateTime.Now;                string nin = string.Empty;                string request = string.Empty;                bool result = _checkoutsService.DeleteCheckedoutPolicy(checkoutDriverInfo, out nin, out request);
                log.DriverNin = nin;
                log.ServiceRequest = request;

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result);            }            catch (Exception ex)            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");            }        }

        /// <summary>
        /// Get All Channels Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/checkouts/getChannels")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ChannelModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllChannel()
        {
            try
            {
                var result = _checkoutsService.GetAllChannel();

                return Ok(result.Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// upload checkout new images
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]        [Route("api/checkouts/UploadImages")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Output>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Output>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult UploadCheckOutImages([FromBody]CheckoutsModel model)        {
            var outPut = new Output();
            string exception = string.Empty;
            try
            {
                if (model == null)
                {
                    outPut.ErrorCode = 2;
                    outPut.ErrorDescription = "model is null";
                    return Error(outPut);
                }
                var checkoutDetails = _checkoutsService.GetComprehansiveCheckoutDetail(model.ReferenceId);
                if (checkoutDetails != null)
                {
                    bool isImagesChanged = false;
                    if (model.ImageBack.NewImageData != null)
                    {
                        var imageBack = new CheckoutCarImage() { ImageData = model.ImageBack.NewImageData };
                        checkoutDetails.ImageBack = imageBack;
                        checkoutDetails.ImageBackId = imageBack.ID;
                        _checkoutsService.AddChekoutCarImages(imageBack, out exception);
                        isImagesChanged = true;
                    }
                    if (model.ImageBody.NewImageData != null)
                    {
                        var imageBody = new CheckoutCarImage() { ImageData = model.ImageBody.NewImageData };
                        checkoutDetails.ImageBody = imageBody;
                        checkoutDetails.ImageBodyId = imageBody.ID;
                        _checkoutsService.AddChekoutCarImages(imageBody, out exception);
                        isImagesChanged = true;
                    }
                    if (model.ImageFront.NewImageData != null)
                    {
                        var imageFront = new CheckoutCarImage() { ImageData = model.ImageFront.NewImageData };
                        checkoutDetails.ImageFront = imageFront;
                        checkoutDetails.ImageFrontId = imageFront.ID;
                        _checkoutsService.AddChekoutCarImages(imageFront, out exception);
                        isImagesChanged = true;
                    }
                    if (model.ImageLeft.NewImageData != null)
                    {
                        var imageLeft = new CheckoutCarImage() { ImageData = model.ImageLeft.NewImageData };
                        checkoutDetails.ImageLeft = imageLeft;
                        checkoutDetails.ImageLeftId = imageLeft.ID;
                        _checkoutsService.AddChekoutCarImages(imageLeft, out exception);
                        isImagesChanged = true;
                    }
                    if (model.ImageRight.NewImageData != null)
                    {
                        var imageRight = new CheckoutCarImage() { ImageData = model.ImageRight.NewImageData };
                        checkoutDetails.ImageRight = imageRight;
                        checkoutDetails.ImageRightId = imageRight.ID;
                        _checkoutsService.AddChekoutCarImages(imageRight, out exception);
                        isImagesChanged = true;
                    }

                    if (string.IsNullOrEmpty(exception) && isImagesChanged)
                    {
                        _checkoutsService.UpdateCheckOut(checkoutDetails, out exception);
                        if (string.IsNullOrEmpty(exception))
                        {
                            outPut.ErrorCode = 1;
                            outPut.ErrorDescription = "success";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.ErrorCode = 2;
                            outPut.ErrorDescription = exception;
                            return Error(outPut);
                        }
                    }
                    else
                    {
                        outPut.ErrorCode = 2;
                        outPut.ErrorDescription = exception;
                        return Error(outPut);
                    }
                }
                else
                {
                    outPut.ErrorCode = 2;
                    outPut.ErrorDescription = "no checkout details for this refeence " + model.ReferenceId;
                    return Error(outPut);
                }
            }
            catch (Exception ex)
            {
                outPut.ErrorCode = 2;
                outPut.ErrorDescription = "an error has occured";
                return Error(outPut);
            }        }        /// <summary>
        /// Update checkout email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]        [Route("api/checkouts/UpdateCheckoutEmail")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Output>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Output>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult UpdateCheckoutEmail([FromBody]CheckoutsModel model)        {
            var outPut = new Output();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Autolease Users";
            log.PageURL = "/admin/checkouts";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateCheckoutEmail";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ReferenceId = model.ReferenceId;

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

                if (string.IsNullOrEmpty(model.Email))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "Email is null";
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    return Error(outPut);
                }

                if (string.IsNullOrEmpty(model.ReferenceId))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "Reference is null";
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    return Error(outPut);
                }

                string exception = string.Empty;
                var assignedDriverNinsByEmail = _checkoutsService.GetAssignedDriverNinsByEmail(model.Email, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = "Error when GetAssignedDriverNinsByEmail --> " + exception;
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }
                if (assignedDriverNinsByEmail != null && assignedDriverNinsByEmail.Count > 0 && !assignedDriverNinsByEmail.Contains(model.Driver.NIN))
                {
                    log.ErrorCode = 6;
                    log.ErrorDescription = "Entered email is assigned to another user";
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }

                var checkoutDetails = _checkoutsService.GetCheckoutDetails(model.ReferenceId);
                if (checkoutDetails == null)
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "No checkout details for this reference " + model.ReferenceId;
                    outPut.ErrorCode = (int)log.ErrorCode;
                    outPut.ErrorDescription = log.ErrorDescription;
                    return Error(outPut);
                }

                checkoutDetails.Email = model.Email;
                checkoutDetails.ModifiedDate = DateTime.Now;
                var updateEmail = _checkoutsService.UpdateCheckOut(checkoutDetails, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "Error happen when UpdateCheckOut --> " + exception;
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
                outPut.ErrorCode = 2;
                outPut.ErrorDescription = "an error has occured";
                return Error(outPut);
            }        }
        #endregion 
    }
}

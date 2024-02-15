using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Ticket;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Core.Excel;
using Tameenk.Core.Configuration;
using System.IO;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Profile.Component;
using Tameenk.Resources;
using System.Globalization;
using Tameenk.Services.Implementation;
using Tameenk.Resources.UserTicket;
using Tameenk.Services.UserTicket.Components;
using System.Web;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Newtonsoft.Json;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class TicketController : AdminBaseApiController
    {
        #region Fields
        private readonly ITicketService _ticketService;
        private readonly IExcelService _excelService;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IProfileContext _profileContext;
        private readonly IUserTicketContext _userTicketContext;
        private readonly IUserPageService _userPageService;
        #endregion

        #region The Constructor 

        /// <summary>
        /// The Constructor  .
        /// </summary>
        /// <param name="ticketService">Ticket Service</param>
        /// <param name="excelService">Excel Service</param>
        /// <param name="tameenkConfig">Tameenk Config</param>
        /// <param name="profileContext">Profile Context</param>
        /// <param name="userTicketContext">User Ticket Context</param>
        public TicketController(ITicketService ticketService, IExcelService excelService, TameenkConfig tameenkConfig, IProfileContext profileContext, IUserTicketContext userTicketContext
            , IUserPageService userPageService)
        {
            _ticketService = ticketService ?? throw new TameenkArgumentNullException(nameof(ITicketService));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
            _tameenkConfig = tameenkConfig;
            _profileContext = profileContext;
            _userTicketContext = userTicketContext;
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Get All Ticket Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/all-status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ChannelModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllStatus(string lang)
        {
            try
            {
                var result = _ticketService.GetAllStatus(lang);
                List<ChannelModel> modelList = new List<ChannelModel>();
                foreach (var item in result)
                {
                    ChannelModel model = new ChannelModel();
                    model.Name = item.Name;
                    model.Code = item.Code;
                    modelList.Add(model);

                }
                return Ok(modelList);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All Ticket Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/all-types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ChannelModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllTypes(string lang)
        {
            try
            {
                var result = _ticketService.GetTicketTypes(lang);
                return Ok(result.Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All based on filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/all")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketListingModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 43)]
        public IActionResult GetAllTicketsWithFilter(TicketFilterModel filter, int pageIndex = 0, int pageSize = 10, string lang = "En")
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            //log.CompanyID = insuranceCompanyId;
            log.PageName = "Tickets";
            log.PageURL = "/admin/tickets";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllTicketsWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(43, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                List<TicketModel> results = new List<TicketModel>();

                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("");
                }

                results = _ticketService.GetAllTicketsWithFilter(filter.ToTicketModel(), out int count, pageIndex, pageSize, lang, false);

                if (results == null)
                    return Error("no result found");
                List<TicketListingModel> listItems = new List<TicketListingModel>();
                foreach (var result in results)
                {
                    TicketListingModel item = new TicketListingModel();
                    item.CheckedoutEmail = result.CheckedoutEmail;
                    item.CheckedoutPhone = result.CheckedoutPhone;
                    item.DriverNin = result.DriverNin;
                    item.Id = result.Id;
                    item.InvoiceId = result.InvoiceId;
                    item.InvoiceNo = result.InvoiceNo;
                    item.PolicyId = result.PolicyId.HasValue? result.PolicyId.Value.ToString():string.Empty;
                    item.PolicyNo = result.PolicyNo;
                    item.ReferenceId = result.ReferenceId;
                    item.StatusNameAr = result.StatusNameAr;
                    item.StatusNameEn = result.StatusNameEn;
                   
                    item.TicketTypeNameAr = result.TicketTypeNameAr;
                    item.TicketTypeNameEn = result.TicketTypeNameEn;
                    item.UserEmail = result.UserEmail;
                    item.UserNotes = result.UserNotes;
                    item.UserPhone = result.UserPhone;
                    item.SequenceNumber = result.SequenceNumber;
                    item.CustomCardNumber = result.CustomCardNumber;


                    if(!string.IsNullOrEmpty(result.StatusNameEn))
                        item.Status = result.StatusNameEn;
                    else
                        item.Status = result.StatusNameAr;
                    if (!string.IsNullOrEmpty(result.TicketTypeNameEn))
                        item.TicketType = result.TicketTypeNameEn;
                    else
                        item.TicketType = result.TicketTypeNameAr;
                    //item.UserTicketAttachments = item.UserTicketAttachments;
                    //  item.UserTicketAttachmentsBytes = item.UserTicketAttachmentsBytes;
                    listItems.Add(item);
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(listItems, count);
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
        /// Export Tickets
        /// </summary>
        /// <returns></returns>
        [HttpPost]        [Route("api/ticket/excel")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketListingModel>>))]        [AdminAuthorizeAttribute(pageNumber: 43)]        public IActionResult ExportAllNewServiceRequest(TicketFilterModel filter)        {            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Tickets";
            log.PageURL = "/admin/tickets";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ExportTicket";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(43, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }                if (filter.StartDate != null && filter.EndDate != null)                {                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);                    if (dtStart > dtEnd)
                    {
                        log.ErrorCode = 5;
                        log.ErrorDescription = $"ExportTicket dtStart > dtEnd";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                        return Ok("StartDate must be before end date");
                    }                }

                string exception = string.Empty;                List<ExcelTiketModel> result = _ticketService.ExportAllTicketsWithFilter(filter.ToTicketModel(), out int count, out exception, 0, 0, "En", true);
                if (result == null)
                {                    log.ErrorCode = 6;
                    log.ErrorDescription = "ExportAllTicketsWithFilter return null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("Error happend while getting tickets data from DB, pleae check the log");                }                if (!string.IsNullOrEmpty(exception))
                {                    log.ErrorCode = 7;
                    log.ErrorDescription = $"exception is not null, and the error is: {exception}";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("Error happend while getting tickets data from DB, pleae check the log");                }                byte[] file = _excelService.ExportAllTicket(result, "Ticket Logs");                if (file != null && file.Length > 0)                    return Ok(Convert.ToBase64String(file));                else
                {                    log.ErrorCode = 8;
                    log.ErrorDescription = $"_excelService.ExportAllTicket return file empty or file length is <= 0";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("Error happend while adding tickets data to excell, please check the log");                }            }            catch (Exception ex)            {                log.ErrorCode = 8;
                log.ErrorDescription = $"Exception error, and the error is: {ex.ToString()}";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error("an error has occured");            }        }

        /// <summary>
        /// Get details to Fail policy by reference Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<TicketListingModel>))]
        [AdminAuthorizeAttribute(pageNumber: 43)]
        public IActionResult GetDetailsToFailPolicyByReferenceId(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            //log.CompanyID = insuranceCompanyId;
            log.PageName = "Tickets";
            log.PageURL = "/tickets/details/";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetDetailsToFailPolicyByReferenceId";
            log.ServiceRequest = id.ToString();
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if(Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
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
                var isAuthorized = _userPageService.IsAuthorizedUser(43, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (id <= 0)
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "this id is not founc";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("no result found");
                }
                string exception = string.Empty;
                var result = _ticketService.GetTicketDetails(id,out exception);
                if(!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured");
                }
                if (result == null)
                {
                    log.ErrorCode = 13;
                    log.ErrorDescription = "result is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured");
                }
                var resultModel = new TicketListingModel();// result.ToTicketDetrailsModel();
                resultModel.CheckedoutEmail = result.CheckedoutEmail;
                resultModel.CheckedoutPhone = result.CheckedoutPhone;
                resultModel.DriverNin = result.DriverNin;
                resultModel.Id = result.Id;
                resultModel.InvoiceId = result.InvoiceId;
                resultModel.InvoiceNo = result.InvoiceNo;
                resultModel.PolicyId = result.PolicyId.HasValue ? result.PolicyId.Value.ToString() : string.Empty;
                resultModel.PolicyNo = result.PolicyNo;
                resultModel.ReferenceId = result.ReferenceId;
                resultModel.StatusNameAr = result.StatusNameAr;
                resultModel.StatusNameEn = result.StatusNameEn;
                resultModel.TicketTypeNameAr = result.TicketTypeNameAr;
                resultModel.TicketTypeNameEn = result.TicketTypeNameEn;
                resultModel.UserEmail = result.UserEmail;
                resultModel.UserNotes = result.UserNotes;
                resultModel.UserPhone = result.UserPhone;
                resultModel.UserId = result.UserId;
                resultModel.UserTicketAttachmentsIds = result.UserTicketAttachmentsIds;
                resultModel.SequenceNumber = result.SequenceNumber;
                resultModel.CustomCardNumber = result.CustomCardNumber;
                resultModel.CreatedBy = result.CreatedBy;
                //if (result.UserTicketAttachments != null)
                //{
                //    resultModel.UserTicketAttachmentsBytes = new List<string>();
                //    foreach (var item in result.UserTicketAttachments)
                //    {
                //        if (!string.IsNullOrEmpty(item.AttachmentPath))
                //        {
                //            //server 2
                //            if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                //            {
                //                FileNetworkShare fileShare = new FileNetworkShare();
                //                //string exception = string.Empty;
                //                var file = fileShare.GetFile(item.AttachmentPath, out exception);
                //                if (file != null)
                //                {
                //                    resultModel.UserTicketAttachmentsBytes.Add("data:image/jpg;base64," + Convert.ToBase64String(file));
                //                }
                //            }
                //            else
                //            {
                //                resultModel.UserTicketAttachmentsBytes.Add("data:image/jpg;base64," + Convert.ToBase64String(File.ReadAllBytes(item.AttachmentPath)));
                //            }
                //        }
                //    }
                //}
                return Ok(resultModel);
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
        /// Download Ticket Attachment File By Id
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("api/ticket/downloadTicketAttachmentFile")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<TicketListingModel>))]
        //[AdminAuthorizeAttribute(pageNumber: 0)]
        //public IHttpActionResult DownloadTicketAttachmentFile(int attachmentId)
        //{
        //    try
        //    {
        //        if (attachmentId <= 0)
        //            return Error("Not Valid Attachment Id");
        //        string exception = string.Empty;
        //        var resultAttachment = _ticketService.DownloadTicketAttachmentFile(attachmentId, out exception);
        //        if (!string.IsNullOrEmpty(exception))
        //        {
        //            return Error(exception);
        //        }
        //        if (resultAttachment == null)
        //        {
        //            return Error("No Result");
        //        }
        //        var output = new TicketAttachedFileToDownloadModel();

        //        if (!string.IsNullOrEmpty(resultAttachment.AttachmentPath))
        //        {
        //            //server 2
        //            if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
        //            {
        //                FileNetworkShare fileShare = new FileNetworkShare();
        //                //string exception = string.Empty;
        //                var file = fileShare.GetFile(resultAttachment.AttachmentPath, out exception);
        //                if (file != null)
        //                {
        //                    output.File = Convert.ToBase64String(file);
        //                    var indexOfLastdot = resultAttachment.AttachmentPath.LastIndexOf('.');
        //                    output.Extension = resultAttachment.AttachmentPath.Substring(indexOfLastdot + 1);
        //                }
        //            }
        //            else
        //            {
        //                output.File = Convert.ToBase64String(File.ReadAllBytes(resultAttachment.AttachmentPath));
        //                var indexOfLastdot = resultAttachment.AttachmentPath.LastIndexOf('.');
        //                output.Extension = resultAttachment.AttachmentPath.Substring(indexOfLastdot + 1);
        //            }
        //        }
                
        //        return Ok(output);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("an error has occured");
        //    }
        //}
        [HttpGet]
        [Route("api/ticket/downloadTicketAttachmentFile")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<TicketListingModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult DownloadTicketAttachmentFile(int attachmentId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Success policies";
            log.PageURL = "/admin/tickets/details/";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DownloadTicketAttachmentFile";
            log.ServiceRequest = $"attachmentId: {attachmentId}";
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
                if (attachmentId <= 0)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "attachmentId is less than zero its " + attachmentId;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("invalid id");
                }
                string exception = string.Empty;
                var resultAttachment = _ticketService.DownloadTicketAttachmentFile(attachmentId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured");
                }
                if (resultAttachment == null)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "No Result";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Result");
                }
                if (string.IsNullOrEmpty(resultAttachment.AttachmentPath))
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = "AttachmentPath is null or empty";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Result");
                }
                
                var output = new TicketAttachedFileToDownloadModel();
                FileNetworkShare fileShare = new FileNetworkShare();
                exception = string.Empty;
                if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                {
                    var file = fileShare.GetFile(resultAttachment.AttachmentPath, out exception);
                    if (file == null)
                        file = fileShare.GetFileFromNewServer(resultAttachment.AttachmentPath, out exception);
                    if (file != null)
                    {
                        output.File = Convert.ToBase64String(file);
                        var indexOfLastdot = resultAttachment.AttachmentPath.LastIndexOf('.');
                        output.Extension = resultAttachment.AttachmentPath.Substring(indexOfLastdot + 1);
                    }
                }
                else
                {
                    var file = fileShare.GetFile(resultAttachment.AttachmentPath, out exception);
                    if (file == null)
                        file = fileShare.GetFileFromNewServer(resultAttachment.AttachmentPath, out exception);
                    output.File = Convert.ToBase64String(file);
                    var indexOfLastdot = resultAttachment.AttachmentPath.LastIndexOf('.');
                    output.Extension = resultAttachment.AttachmentPath.Substring(indexOfLastdot + 1);
                }
                return Ok(output);
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
        /// Get All Ticket History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/all-histories")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketHistoryModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllTicketHistories(int id)
        {
            try
            {
                string exception = string.Empty;
                var result = _ticketService.GetTicketHistories(id,out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    return Error(exception);
                }
                if (result == null)
                {
                    return Error("No Result");
                }
                List<TicketHistoryModel> list = new List<TicketHistoryModel>();
                foreach (var item in result)
                {
                    TicketHistoryModel listItem = new TicketHistoryModel();
                    listItem.Id = item.Id;
                    listItem.AdminReply = item.AdminReply;
                    listItem.RepliedBy = item.RepliedBy;
                    listItem.StatusNameAr = item.StatusNameAr;
                    listItem.StatusNameEn = item.StatusNameEn;
                    listItem.StatusId = item.TicketStatusId;
                    listItem.TicketId = item.TicketId;
                    listItem.CreatedDate = item.CreatedDate;
                    list.Add(listItem);
                }
                if (result == null)
                {
                    return Error("No Resultin list");
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// update ticket status
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isClosed"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/UpdateStatus")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Output>))]
        [AdminAuthorizeAttribute(pageNumber: 43)]
        public IActionResult UpdateTicketStatus([FromBody]List<TicketHistoryModel> model, bool isClosed)
        {
            var outPut = new Output();

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            //log.CompanyID = insuranceCompanyId;
            log.PageName = "Tickets";
            log.PageURL = "/admin/tickets/details/";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateTicketStatus";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(43, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string repliedBy = User.Identity.GetUserName();
                string exception;
                string ticketUserId;
                int ticketsWithIssuesCount = 0;
                foreach (var ticket in model)
                {
                    UserTicketHistory history = new UserTicketHistory()
                    {
                        AdminReply = ticket.AdminReply,
                        RepliedBy = repliedBy,
                        CreatedDate = DateTime.Now,
                        TicketId = ticket.Id,
                        TicketStatusId = (isClosed == true) ? (int)EUserTicketStatus.TicketClosed : (int)EUserTicketStatus.UnderProcessing
                    };

                    exception = string.Empty;
                    ticketUserId = string.Empty;
                    var result = _ticketService.UpdateTicketStatus(history, out ticketUserId, out exception);
                    if (!string.IsNullOrEmpty(exception) || !result)
                    {
                        outPut.ErrorCode = 5;
                        outPut.ErrorDescription = "an error has occured while updating ticket status";
                        log.ErrorCode = outPut.ErrorCode;
                        log.ErrorDescription = exception;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        ++ticketsWithIssuesCount;
                        continue;
                    }

                    ProfileNotification profileNotification = new ProfileNotification();
                    profileNotification.CreatedDate = DateTime.Now;
                    if (history.TicketStatusId == (int)EUserTicketStatus.TicketClosed)
                    {
                        profileNotification.DescriptionAr = string.Format(UserTicketResources.ResourceManager.GetString("TicketClosed", CultureInfo.GetCultureInfo("ar")), history.TicketId.Value.ToString("0000000000"));
                        profileNotification.DescriptionEn = string.Format(UserTicketResources.ResourceManager.GetString("TicketClosed", CultureInfo.GetCultureInfo("en")), history.TicketId.Value.ToString("0000000000"));
                        profileNotification.TypeId = (int)EProfileNotificationType.Success;
                        profileNotification.TicketStatusId = (int)EUserTicketStatus.TicketClosed;
                    }
                    else
                    {
                        profileNotification.DescriptionAr = string.Format(UserTicketResources.ResourceManager.GetString("NewReplyAdded", CultureInfo.GetCultureInfo("ar")), history.TicketId.Value.ToString("0000000000"));
                        profileNotification.DescriptionEn = string.Format(UserTicketResources.ResourceManager.GetString("NewReplyAdded", CultureInfo.GetCultureInfo("en")), history.TicketId.Value.ToString("0000000000"));
                        profileNotification.TypeId = (int)EProfileNotificationType.UnderProcessing;
                        profileNotification.TicketStatusId = (int)EUserTicketStatus.UnderProcessing;
                    }

                    profileNotification.UserId = ticketUserId;
                    profileNotification.ModuleId = (int)EProfileNotificationModule.UserTicket;
                    _profileContext.CreateProfileNotification(profileNotification);

                    //SMS
                    if (isClosed)
                    {
                        _userTicketContext.SendUpdatedStatusSMS(ticket.UserId, "ar", history.TicketStatusId.Value, history.TicketId.Value, ticket.AdminReply);
                    }
                }

                if (ticketsWithIssuesCount == model.Count)
                {
                    outPut.ErrorCode = 6;
                    outPut.ErrorDescription = "an error has occured, please contact with technical team";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = "Failed to update all tickets status";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }

                if (ticketsWithIssuesCount > 0 && ticketsWithIssuesCount < model.Count)
                {
                    outPut.ErrorCode = 7;
                    outPut.ErrorDescription = "an error has occured, please contact with technical team";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = "Failed to update some tickets status";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }

                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "Comment added successfully";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = outPut.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All ticket log based on filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/all-log")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketListingModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllLogWithFilterNew(TicketLogFilterModel filter, int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                List<TicketLogModel> result = new List<TicketLogModel>();

                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("");
                }

                result = _ticketService.GetAllLogsWithFilter(filter.ToTicketLogModel(), out int count, pageIndex, pageSize);

                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToTicketLogListingModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Export ticket log
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/excel-log")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketListingModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportTicketLog(TicketLogFilterModel filter)
        {
            try
            {
                List<TicketLogModel> result = new List<TicketLogModel>();

                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("");
                }

                result = _ticketService.GetAllLogsWithFilter(filter.ToTicketLogModel(), out int count, 0, 0, true);

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.ExportTicketLog(result, "TicketLogs");

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
        /// Create Ticket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/createTicket")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<TicketListingModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 44)]
        public IActionResult CreateTicket([FromBody]CreateTicketModel createTicketModel)
        {
            var output = new CreatedTicketOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            //log.CompanyID = insuranceCompanyId;
            log.PageName = "createTicket";
            log.PageURL = "/admin/tickets/createTicket";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "CreateTicket";
            log.ServiceRequest = JsonConvert.SerializeObject(createTicketModel);
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
                var isAuthorized = _userPageService.IsAuthorizedUser(44, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CreateUserTicketModel createUserTicketModel = new CreateUserTicketModel();
                createUserTicketModel.Channel = Channel.Dashboard.ToString();
                createUserTicketModel.Language = createTicketModel.Language;
                createUserTicketModel.SequenceOrCustomCardNumber = createTicketModel.SequenceOrCustomCardNumber;
                createUserTicketModel.TicketTypeId = createTicketModel.TicketTypeId;
                createUserTicketModel.UserPhone = createTicketModel.UserPhone;
                createUserTicketModel.UserNotes = createTicketModel.UserNotes;
                createUserTicketModel.NIN = createTicketModel.NIN;
                createUserTicketModel.CreatedBy = User.Identity.GetUserName();

                if (createTicketModel.AttachedFiles != null)
                {
                    createUserTicketModel.AttachedFiles = new List<Tameenk.Core.Domain.Dtos.AttachedFiles>();
                    foreach (var item in createTicketModel.AttachedFiles)
                    {
                        createUserTicketModel.AttachedFiles.Add(new Tameenk.Core.Domain.Dtos.AttachedFiles() { Extension = item.Extension, File = item.File, TicketTypeFileNameId = item.TicketTypeFileId });

                    }
                }

                var userTicketOutput = _userTicketContext.CreateUserTicketFromDashboard(createUserTicketModel);

                if (userTicketOutput == null)
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "userTicketOutput is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured");
                }
                if (userTicketOutput.ErrorCode == UserTicketOutput.ErrorCodes.NotValidToOpenNewTicket || userTicketOutput.ErrorCode != UserTicketOutput.ErrorCodes.Success)
                {
                    output = new CreatedTicketOutput();
                    output.ErrorCode = (int)userTicketOutput.ErrorCode;
                    output.ErrorDescription = userTicketOutput.ErrorDescription;
                    output.TicketId = userTicketOutput.UserTicketId;

                    log.ErrorCode = 11;
                    log.ErrorDescription = "an error has occured during create ticket due to " + userTicketOutput.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                //if (userTicketOutput.ErrorCode != UserTicketOutput.ErrorCodes.Success)
                //{
                //    log.ErrorCode = 11;
                //    log.ErrorDescription = "an error has occured during create ticket due to " + userTicketOutput.ErrorDescription;
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error("an error has occured");
                //}
                ProfileNotification profileNotification = new ProfileNotification();
                profileNotification.CreatedDate = DateTime.Now;
                profileNotification.DescriptionAr = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreatedByCustomerService", CultureInfo.GetCultureInfo("ar")), userTicketOutput.UserTicketId.ToString("0000000000"));
                profileNotification.DescriptionEn = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreatedByCustomerService", CultureInfo.GetCultureInfo("en")), userTicketOutput.UserTicketId.ToString("0000000000"));
                profileNotification.TypeId = (int)EProfileNotificationType.Success;
                profileNotification.TicketStatusId = (int)EUserTicketStatus.TicketOpened;

                profileNotification.UserId = userTicketOutput.UserId;
                profileNotification.ModuleId = (int)EProfileNotificationModule.UserTicket;
                _profileContext.CreateProfileNotification(profileNotification);

                //SMS
                //_userTicketContext.SendUpdatedStatusSMS(userTicketOutput.UserId, "ar", (int)EUserTicketStatus.TicketOpened, userTicketOutput.UserTicketId,string.Empty);

                output = new CreatedTicketOutput();
                output.ErrorCode = (int)userTicketOutput.ErrorCode;
                output.ErrorDescription = userTicketOutput.ErrorDescription;
                output.TicketId = userTicketOutput.UserTicketId;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Delete User Ticket History
        /// </summary>
        /// <param name="historyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/deleteUserTicketHistory")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 43)]
        public IActionResult DeleteUserTicketHistory(int historyId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Ticket Details";
            log.PageURL = "/admin/tickets/details";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteUserTicketHistory";
            log.ServiceRequest = $"historyId: {historyId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            var outPut = new Output();
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
                var isAuthorized = _userPageService.IsAuthorizedUser(43, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (historyId <= 0)
                    throw new TameenkArgumentNullException("History Id must be postive number");

                string exception = string.Empty;
                var output = _userTicketContext.DeleteUserTicketHistory(historyId);
                if (output.ErrorCode != UserTicketOutput.ErrorCodes.Success)
                {
                    outPut.ErrorCode = (int)output.ErrorCode;
                    outPut.ErrorDescription = output.ErrorDescription;
                    return Ok(outPut);
                }

                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "Comment deleted successfully";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(outPut);
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
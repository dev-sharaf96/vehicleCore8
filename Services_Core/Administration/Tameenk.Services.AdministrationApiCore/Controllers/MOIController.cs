using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Encryption;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 28)]
    public class MOIController : AdminBaseApiController
    {
        private readonly IRepository<MOIDetail> _moiDetailRepository;
        private readonly IPromotionService promotionService;
        private readonly TameenkConfig tameenkConfig;
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";
        private readonly IUserPageService _userPageService;
        public MOIController(IRepository<MOIDetail> moiDetailRepository,
            IPromotionService promotionService,
            TameenkConfig tameenkConfig, IUserPageService userPageService)
        {
            this._moiDetailRepository = moiDetailRepository;
            this.promotionService = promotionService;
            this.tameenkConfig = tameenkConfig;
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
        }

        enum MOIFilter
        {
            All = 1,
            Approved,
            NoneApproved
        };
        /// <summary>
        /// get all insuarance companies
        /// </summary>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">Sort Field</param>
        /// <param name="sortOrder">sort Order</param>
        /// <param name="clientEmail">client Email</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Moi/all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<MOIDetail>>))]
        public IActionResult GetAll(int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = true, int filter = 1, string clientEmail = null)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "MOI";
            log.PageURL = "admin/approve-moi";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAll";
            log.ServiceRequest = $"pageIndex: {pageIndex}, pageSize: {pageSize}, sortField: {sortField}, sortOrder: {sortOrder}, filter: {filter}, clientEmail: {clientEmail}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(28, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                IQueryable<MOIDetail> query = _moiDetailRepository.TableNoTracking;

                if (filter == (int)MOIFilter.Approved)
                {
                    query = query.Where(x => x.Approved == true);
                }
                else if (filter == (int)MOIFilter.NoneApproved)
                {
                    query = query.Where(x => x.Approved == false);
                }

                if (!string.IsNullOrEmpty(clientEmail))
                    query = query.Where(x => x.Email.Contains(clientEmail));

                query = query.ToList().Select(x => new MOIDetail
                {
                    Approved = x.Approved,
                    CreatedAt = x.CreatedAt,
                    Email = x.Email,
                    Id = x.Id,
                    UserId = x.UserId
                }).AsQueryable();

                var result = new PagedList<MOIDetail>(query, pageIndex, pageSize, sortField, sortOrder);

                return Ok(result, result.TotalCount);
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
        [Route("api/Moi/change-status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<MOIDetail>>))]
        public IActionResult ChangeStatus(int id, bool approved)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "MOI";
            log.PageURL = "admin/approve-moi";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ChangeStatus";
            log.ServiceRequest = $"id: {id}, approved: {approved}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(28, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var moiDetail = _moiDetailRepository.Table.FirstOrDefault(x => x.Id == id);
                if (moiDetail != null)
                {
                    moiDetail.Approved = approved;
                    _moiDetailRepository.Update(moiDetail);
                    if (approved)
                    {
                        var moiProgram = promotionService.GetPromotionProgramByKey("MOI");
                        var enrollmentResponse = promotionService.EnrollAndApproveUSerToPromotionProgram(moiDetail.UserId, moiProgram.Id, moiDetail.Email);
                        if (enrollmentResponse.UserEndrollerd)
                        {
                            SendJoinProgramConfirmationEmail(moiDetail.Email, moiProgram.Id, moiDetail.UserId);
                        }
                    }
                    else
                    {
                        promotionService.DisenrollUserFromPromotionProgram(moiDetail.UserId);
                    }
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                }
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

        [HttpGet]
        [Route("api/moi/delete")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult Delete(int id, bool unSubscribe)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "MOI";
            log.PageURL = "admin/approve-moi";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "Delete";
            log.ServiceRequest = $"id: {id}, unSubscribe: {unSubscribe}";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(28, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var moiDetail = _moiDetailRepository.Table.FirstOrDefault(x => x.Id == id);
                if (moiDetail != null)
                {
                    if (unSubscribe)
                    {
                        promotionService.DisenrollUserFromPromotionProgram(moiDetail.UserId);
                    }
                    _moiDetailRepository.Delete(moiDetail);
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(true);
                }
                return Ok(false);
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
        [Route("api/moi/getImage")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetImage(int id)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "MOI";
            log.PageURL = "admin/approve-moi";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetImage";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(28, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var moiDetail = _moiDetailRepository.TableNoTracking.FirstOrDefault(x => x.Id == id);

                if (moiDetail == null)
                {
                    return Ok(string.Empty);
                }

                return Ok(moiDetail.FileByteArray);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        private bool SendJoinProgramConfirmationEmail(string userEmail, int programId, string currentUserId)
        {
            JoinProgramEmailConfirmModel model = new JoinProgramEmailConfirmModel()
            {
                UserId = currentUserId,
                JoinRequestedDate = DateTime.Now,
                PromotionProgramId = programId,
                UserEmail = userEmail
            };
            var emailSubject = "بى كير برنامج نظام وزارة الداخلية";
            string url = Utilities.SiteURL + "/moi";
            string emailBody = string.Format(@"<table class=""m_bg_white"" width=""50%"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #F5F9FD;"">
        <tr>
            <td width=""850"" align=""center"" >
                <table width=""100%""  border=""0"" cellpadding=""0"" cellspacing=""0""
                    style=""border-collapse:collapse"">
                    <tr>
                        <td class=""m_overflow135 m_fs16"" align=""center""
                            style=""font-family:helvetica neue,helvetica,arial,sans-serif;font-size:20px;font-weight:bold;color:#444444;max-width:150px;overflow:hidden;text-overflow:ellipsis;padding-left:8px; padding-top:30px;"">
                            <span>
                                <h2> تفعيل إشتراك</h2>
                            </span>
                        </td>
                    </tr>
                </table>
                <table class=""m_wb"" width=""850"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                    style=""background-color:#ffffff"">
                    <tr>
                        <td width=""850"" align=""center"" style=""margin-bottom:20px"">
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                style=""background-color:#F5F9FD"">

                                <tr>
                                    <td class=""m_wc m_pl m_pr"" width=""850"" align=""center""
                                        style=""padding:0px 20px 0px 20px; background-color:#F5F9FD; "">
                                        <table class=""m_wc"" width=""100%""  border=""0"" cellpadding=""0""
                                            cellspacing=""0"" style=""border-collapse:collapse;"">
                                            <tr>
                                                <td class=""m_plr0""  >
                                                    <a href=""#""
                                                        style=""text-decoration:none;display:block;"" >

                                                        <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                                            style=""border-collapse:collapse;"">
                                                            <tr>
                                                                <td class=""m_fs14 m_lh20 "" valign=""bottom""
                                                                    style=""font-family:helvetica neue,helvetica,arial,sans-serif;font-size:16px;line-height:20px;color:#444444"">
                                                                <h4>    عزيزنا العميل، </h4>
																
																<p>تم تأكيد الاشتراك في نظام وزارة الداخلية يُرجى الضغط على الرابط التالي:</p>
														<br/>
                                                                </td>
                                                            </tr>
                                                        </table>

                                                       
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                        <table class=""m_wc"" width=""450"" align=""center"" border=""0"" cellpadding=""0""
                                            cellspacing=""0"" style=""border-collapse:collapse;"">
                                            <tr>
                                                <td class=""m_plr0""  style=""padding:0 100px 0 100px"">
                                                    <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                                        <tr>
                                                            <td align=""center""
                                                                style=""padding:14px 0px 14px 0px;background-color:#186a9e;border-radius:30px !important;-webkit-border-radius: 50px;
                                                                -ms-border-radius: 50px;
                                                                -moz-border-radius: 50px;"" border-radius=""20"">
                                                                <a class=""m_overflow240""
                                                                    href=""{0}""
                                                                    style=""font-family:helvetica neue,helvetica,arial,sans-serif;font-weight:bold;font-size:18px;line-height:1.5;color:#ffffff;text-decoration:none;display:block;text-align:center;max-width:400px;overflow:hidden;text-overflow:ellipsis"">
                                                                 تفعيل
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>

                                     
                                    </td>
                                </tr>
                                <tr>
                                    <td class=""m_wc m_pl m_pr"" width=""850"" align=""center""
                                        style=""padding:20 20px 20 20px;"">
                                        <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                            style=""border-collapse:collapse; margin-bottom: 30px;"">
                                            <tr>
                                                <td>
                                                    
                                                    <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        style=""border-collapse:collapse;"">
                                                        <tr>
                                                            <td class=""m_fs14 m_lh20""  valign=""bottom""
                                                                style="" background-color:#F5F9FD;  padding:0px 20px 40px 20px;font-family:helvetica neue,helvetica,arial,sans-serif;font-size:16px;line-height:20px;color:#444444"">
                                                            <h4>  شكرًا لإعطائنا الفرصة لخدمتك،،،<br>
																	فريق <b>#بي_كير</b>  </h4>
                                                            </td>
                                                        </tr>
                                                    </table>


                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>", url);
            MessageBodyModel messageBodyModel = new MessageBodyModel();
            messageBodyModel.MessageBody = emailBody;
            messageBodyModel.Language = "ar";
            messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PromoActivation.png";
            return MailUtilities.SendMail(messageBodyModel, emailSubject, tameenkConfig.SMTP.SenderEmailAddress, userEmail);
        }

    }

    public class JoinProgramEmailConfirmModel
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public DateTime JoinRequestedDate { get; set; }

        public int PromotionProgramId { get; set; }
    }
}

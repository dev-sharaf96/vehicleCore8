using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Models.Promotion;
using Tameenk.Security.Encryption;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Extensions;

namespace Tameenk.Controllers
{
    [TameenkAuthorizeAttribute]
    public class MOIController : Controller
    {
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";

        IRepository<MOIDetail> _moiDetailRepository;
        private readonly IPromotionService promotionService;
        private readonly TameenkConfig tameenkConfig;

        public MOIController(IRepository<MOIDetail> moiDetailRepository,
            IPromotionService promotionService, TameenkConfig tameenkConfig)
        {
            this._moiDetailRepository = moiDetailRepository;
            this.promotionService = promotionService;
            this.tameenkConfig = tameenkConfig;
        }

        private string CurrentUserID
        {
            get
            {
                return User.Identity.GetUserId<string>();
            }
        }

        // GET: MOI
        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace(CurrentUserID))
            {
                if (!IsUserJoinedToMOIProgram())
                {
                    return RedirectToAction("MoiJoin", "MOI");
                }
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadImage()
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                int maxSize = 5; //MegaByte
                List<string> validFileExtensions = new List<string>() { "jpeg", "jpg", "png", "gif", "bmp", "tif", "tiff", "webp", "jfif" };
                string fileExtension = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
                if (!validFileExtensions.Contains(fileExtension))
                {
                    return Json(new { status = false, message = LangText.invalidFileExtension + string.Join(",", validFileExtensions) }, JsonRequestBehavior.AllowGet);
                }

                if (file.ContentLength > maxSize * 1048576) // 1048576 = 1024 * 1024
                {
                    return Json(new { status = false, message = string.Format(LangText.fileSizeLimitExceeded, maxSize) }, JsonRequestBehavior.AllowGet);
                }

                string userId = User.Identity.GetUserId();
                string userEmail = User.Identity.GetUserName();

                var moiDetail = _moiDetailRepository.TableNoTracking
                                .FirstOrDefault(x => x.Email == userEmail
                                                && x.UserId == userId);

                if (moiDetail != null && moiDetail.Approved == false)
                {
                    return Json(new { status = false, message = LangText.alreadyRegistered }, JsonRequestBehavior.AllowGet);
                }
                else if (moiDetail != null&& moiDetail.Approved == true)
                {
                    return Json(new { status = false, message = LangText.alreadyRegisteredAndApproved }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    MOIDetail detail = new MOIDetail();
                    detail.UserId = userId;
                    detail.Email = userEmail;
                    detail.FileByteArray = file.ConvertToByteArray();
                    detail.FileName = file.FileName;
                    detail.FileMimeType = file.ContentType;
                    _moiDetailRepository.Insert(detail);
                    return Json(new { status = true, message = LangText.MoiRequestRecieved }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Join(string email)
        {
            try
            {
                MOIDetail detail = new MOIDetail();
                detail.UserId = CurrentUserID;
                detail.Email = email;
                _moiDetailRepository.Insert(detail);
                var promotionProgram = promotionService.GetPromotionProgramBydomain(email);
                if (promotionProgram != null)
                {
                    //var moiProgram = promotionService.GetPromotionProgramByKey("MOI");
                    var enrollmentResponse = promotionService.EnrollUSerToPromotionProgram(CurrentUserID, 1, email);
                    if (enrollmentResponse.UserEndrollerd)
                    {
                        SendJoinProgramConfirmationEmail(email, promotionProgram.Id, CurrentUserID);
                    }
                }
                return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult MoiJoin()
        {
            PromotionProgramModel model = null;
            var promotionMoiProgram = promotionService.GetPromotionProgramByKey("MOI");
            if (promotionMoiProgram != null)
            {
                model = new PromotionProgramModel();
                model.Id = promotionMoiProgram.Id;
            }

            return View(model);
        }


        private bool IsUserJoinedToMOIProgram()
        {
            //var progUser = repository.Table.FirstOrDefault(x => x.UserId == CurrentUserID && !x.Approved.Value);
            //return progUser != null;
            var progUser = promotionService.GetPromotionProgramUser(CurrentUserID);
            return (progUser != null && progUser.EmailVerified && progUser.PromotionProgram.Key.ToLower() == "MOI");
        }


        private bool SendJoinProgramConfirmationEmail(string userEmail, int programId, string currentUserId)
        {
            JoinProgramEmailConfirmModel model = new JoinProgramEmailConfirmModel()
            {
                UserId = User.Identity.GetUserId<string>(),
                JoinRequestedDate = DateTime.Now,
                PromotionProgramId = programId,
                UserEmail = userEmail
            };
            var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), JOIN_PROMOTION_PROGRAM_SHARED_KEY);

            var emailSubject = LangText.JoinProgramConfirmationSubject;
            string url = Url.Action("ConfirmJoinProgram", "Promotion", new { token }, protocol: Request.Url.Scheme);
            string emailBody = string.Format(LangText.JoinProgramConfirmationBody, url);
            //return MailUtilities.SendMailOfPromotions(emailBody, emailSubject, tameenkConfig.SMTP.SenderEmailAddress, userEmail);
            MessageBodyModel messageBodyModel = new MessageBodyModel();
            messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PromoActivation.png";
            messageBodyModel.Language = CultureInfo.CurrentCulture.Name;
            messageBodyModel.MessageBody = emailBody;
            return MailUtilities.SendMailOfPromotions(messageBodyModel, emailSubject, "promotions@bcare.com.sa", userEmail);
        }
    }
}
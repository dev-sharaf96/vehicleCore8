using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Exceptions;
using Tameenk.Models;
using Tameenk.Models.Promotion;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.Promotions;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;

namespace Tameenk.Controllers
{
    //[Authorize]
   
    public class PromotionController : Controller
    {
        #region Fields
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly INotificationService _notificationService;
        private readonly IPromotionService _promotionService;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IQuotationService _quotationService;

        #endregion

        #region Ctor

        public PromotionController(IHttpClient httpClient, TameenkConfig tameenkConfig, INotificationService notificationService,
            IPromotionService promotionService, ILogger logger, IAuthorizationService authorizationService
            , IQuotationService quotationService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(IHttpClient));
            _config = tameenkConfig ?? throw new ArgumentNullException(nameof(TameenkConfig));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(INotificationService));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(IPromotionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            _authorizationService = authorizationService;
            _quotationService = quotationService;
        }

        #endregion

        // GET: Promotion
        [TameenkAuthorizeAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [TameenkAuthorizeAttribute]
        public async Task<ActionResult> Programs()
        {
            _logger.Log($"Promotion -> Programs <<start>>");
            var promotionPrograms = _promotionService.GetPromotionProgramsNoTracking();

            if (promotionPrograms != null)
            {
                _logger.Log($"Promotion -> Programs returned");

                List<PromotionProgramModel> model = new List<PromotionProgramModel>();

                foreach (var promotionProgram in promotionPrograms)
                {
                    PromotionProgramModel promotionProgramModel = new PromotionProgramModel();
                    promotionProgramModel.Id = promotionProgram.Id;
                    promotionProgramModel.Name = promotionProgram.Name;
                    promotionProgramModel.Description = promotionProgram.Description;
                    promotionProgramModel.IsPromoByEmail = promotionProgram.IsPromoByEmail;

                    model.Add(promotionProgramModel);
                }
                return View(model);
            }
            return View();
        }
        [TameenkAuthorizeAttribute]
        public async Task<ActionResult> JoinProgram(int promotionProgramId, string userInput)
        {
            string currentUserId = User.Identity.GetUserId<string>();
            string currentUserEmail = User.Identity.GetUserName();
            try
            {
                if (promotionProgramId < 1)
                    throw new TameenkArgumentException("Promotion program id can't be less than 1.", nameof(promotionProgramId));

                var program = _promotionService.GetPromotionProgramNoTracking(promotionProgramId);

                if (program == null)
                    throw new TameenkArgumentException($"There is no Promotion program for this id : {promotionProgramId}", nameof(promotionProgramId));

                if (program.IsPromoByEmail && !IsValidEmail(userInput))
                    throw new TameenkArgumentException("Email is't in a valid format.", nameof(userInput));

                var validationError = _promotionService.ValidateBeforeJoinProgram(userInput, promotionProgramId, program.IsPromoByEmail);
                if (string.IsNullOrWhiteSpace(validationError))
                {
                    string email = program.IsPromoByEmail ? userInput : currentUserEmail;
                    var enrollmentResponse = _promotionService.EnrollUSerToPromotionProgram(currentUserId, promotionProgramId, email);
                    //if user joined the program then send confirmation email 
                    if (enrollmentResponse.UserEndrollerd)
                    {
                        if (program.IsPromoByEmail) //Promo By Email
                        {
                            await SendJoinProgramConfirmationEmail(userInput, promotionProgramId).ConfigureAwait(false);
                            var result = new
                            {
                                Success = true,
                                Message = LangText.JoinProgramMailSent
                            };
                            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                        }
                        else //Promo By Code
                        {
                            _promotionService.ConfirmUserJoinProgram(currentUserId, promotionProgramId, currentUserEmail);
                            var result = new
                            {
                                Success = true,
                                Message = LangText.ProgramJoinedSuccessfully
                            };
                            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //show error that userd couldn't be added to this program.
                        var enrollErrorMessagesAsString = string.Join(Environment.NewLine, enrollmentResponse.Errors.Select(e => e.Description));

                        var errorMsg = LangText.ErrorWhileJoiningPromotionProgram + Environment.NewLine + enrollErrorMessagesAsString;
                        var result = new
                        {
                            Success = false,
                            Message = errorMsg
                        };
                        return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                    }
                }
                //user domain doesnt exist in program's domains
                else
                {
                    var result = new
                    {
                        Success = false,
                        Message = validationError
                    };
                    return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                _logger.Log($"PromotionController -> JoinProgram - (Promotion program id: {promotionProgramId}, User Input: {userInput})", ex, LogLevel.Error);

                var result = new
                {
                    Success = false,
                    ex.Message
                };
                return Json(result, "application/json", JsonRequestBehavior.AllowGet);
            }

        }

        [AllowAnonymous]
        public ActionResult ConfirmJoinProgram(string token, string hashed)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new TameenkArgumentNullException("token", "Token can't be null.");
            }

            if (string.IsNullOrWhiteSpace(hashed))
            {
                throw new TameenkArgumentNullException("hashed", "Hashed can't be null.");
            }
            var viewModel = new ConfirmJoinProgramViewModel();
            try
            {
                var decryptedToken = AESEncryption.DecryptString(Utilities.GetDecodeUrl(token), JOIN_PROMOTION_PROGRAM_SHARED_KEY);
                if (!SecurityUtilities.VerifyHashedData(hashed, token))
                {
                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorHashing });
                }
                var model = JsonConvert.DeserializeObject<JoinProgramEmailConfirmModel>(decryptedToken);
                //if (model.UserId != User.Identity.GetUserId<string>())
                //{
                //    _logger.Log("Promotion-> ConfirmJoinProgram user id not matched", LogLevel.Info);
                //    return RedirectToAction("Index", "Error");
                //}

                if (DateTime.Now.Subtract(model.JoinRequestedDate).TotalMinutes < 15)
                {
                    _promotionService.ConfirmUserJoinProgram(model.UserId, model.PromotionProgramId, model.UserEmail);
                    //when user join new program, invalidate old responses to get new responses with the new program code
                    _quotationService.InvalidateUserQuotationResponses(model.UserId);
                    viewModel.Message = LangText.ConfirmJoinProgramSuccess;
                    return View(viewModel);
                }
                else
                {
                    //join promotion program request is expired.
                    viewModel.Message = LangText.ConfirmJoinProgramRequestExpired;
                    return View(viewModel);
                }

            }
            catch (DecryptionFaildException ex)
            {
                //show error page that decription faild
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\Tameenk_Website\log\log1.txt","token is "+token+" =====>"+ ex.ToString());
                _logger.Log($"PromotionController -> ConfirmJoinProgram -> error in confirm joining user while decrypting the token.", ex, LogLevel.Error);
                viewModel.Message = LangText.ConfirmJoinProgramTokenInvalid;
                return View(viewModel);
            }

            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\Tameenk_Website\log\log11.txt", ex.ToString());
                _logger.Log($"PromotionController -> ConfirmJoinProgram -> error in confirm joining user", ex, LogLevel.Error);
                return RedirectToAction("Index", "Error");
            }
        }
        [TameenkAuthorizeAttribute]
        public ActionResult DisenrollUserFromPromotionProgram()
        {
            try
            {
                _promotionService.DisenrollUserFromPromotionProgram(User.Identity.GetUserId<string>());
                return Json(new { Success = true }, "application/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _logger.Log($"PromotionController->DisenrollUserFromPromotionProgram userId: {User.Identity.GetUserId<string>()}", ex);
                return Json(new { Success = false, Error = PromotionProgramResource.DisenrollUserErrorHappen }, "application/json", JsonRequestBehavior.AllowGet);
            }
        }
        [TameenkAuthorizeAttribute]
        private async Task SendJoinProgramConfirmationEmail(string userEmail, int programId)
        {
            JoinProgramEmailConfirmModel model = new JoinProgramEmailConfirmModel()
            {
                UserId = User.Identity.GetUserId<string>(),
                JoinRequestedDate = DateTime.Now,
                PromotionProgramId = programId,
                UserEmail = userEmail
            };
            var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), JOIN_PROMOTION_PROGRAM_SHARED_KEY);
            string hashed = SecurityUtilities.HashData(token, null);
            var emailSubject = LangText.JoinProgramConfirmationSubject;
            string url = Url.Action("ConfirmJoinProgram", "Promotion", new { token, hashed }, protocol: Request.Url.Scheme);
            string emailBody = string.Format(LangText.JoinProgramConfirmationBody, url);
            //MailUtilities.SendMailOfPromotions(emailBody, emailSubject, "promotions@bcare.com.sa", userEmail);
            MessageBodyModel messageBodyModel = new MessageBodyModel();
            messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PromoActivation.png";
            messageBodyModel.Language = CultureInfo.CurrentCulture.Name;
            messageBodyModel.MessageBody = emailBody;
            MailUtilities.SendMailOfPromotions(messageBodyModel, emailSubject, "promotions@bcare.com.sa", userEmail);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
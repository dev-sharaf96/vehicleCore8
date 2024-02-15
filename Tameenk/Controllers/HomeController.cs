using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tameenk;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;

using Tameenk.Models;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tamkeen.bll.UnitOfWork;

namespace Tameenak.Controllers
{
    public class HomeController : Controller
    {
        // Init Lookup db
        ILookupUOW lookupUOW;
        #region Declarations

        private readonly INotificationService _notificationService;
        private readonly WebContext _webContext;
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        #endregion

        public HomeController(ILogger logger, IHttpClient httpClient, INotificationService notificationService, WebContext webContext, TameenkConfig tameenkConfig, IAuthorizationService authorizationService)
        {
            _notificationService = notificationService;
            _webContext = webContext;
            _httpClient = httpClient;
            _config = tameenkConfig;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// init index view with data and draw Form ... 
        /// <returns></returns>
        [HandleError]
        public async Task<ActionResult> Index(string eid, int r = 0, string re = "",int c=0)
        {
            LanguageTwoLetterIsoCode lang = LanguageTwoLetterIsoCode.Ar;
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                lang = LanguageTwoLetterIsoCode.En;
            }
            if (TempData.ContainsKey("Checkout_HomePageErrors"))
            {
                var errors = TempData["Checkout_HomePageErrors"] as List<string>;
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError($"CheckoutError", error);
                    }
                }
            }
            var model = new HomeModel();
            if (!string.IsNullOrEmpty(eid))
            {
                model.IsEditRequest = true;
                model.QuotationRequestExternalId = eid;
                model.IsRenual = (r == 1) ? true : false;
                model.IsCustomCard = (c == 1) ? true : false;
                model.ReferenceId = re;
            }
            return View(model);
        }

        public ActionResult EditRequestInformation(string eid, int r = 0, string re = "", int c = 0)
        {
            LanguageTwoLetterIsoCode lang = LanguageTwoLetterIsoCode.Ar;
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                lang = LanguageTwoLetterIsoCode.En;
            }
            return RedirectToAction("Index", new { eid, r, re,c });
        }

        public ActionResult TermsCondition()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {

                return View("TermsConditionEn");
            }
            return View();
        }

        public ActionResult AboutUs()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {

                return View("AboutUsEn");
            }
            return View();
        }
        public ActionResult Contact()
        {
            ContactUsViewModel model = new ContactUsViewModel();
            model.IdentityUrl = _config.Identity.Url;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Contact(ContactUsViewModel model)
        {
            var sendEmailToUserTask = SendEmailToUserAfterContactUs(model);
            var sendUserMessageToSupportTask = SendUserMessageToSupport(model);
            await Task.WhenAll(sendEmailToUserTask, sendUserMessageToSupportTask);
            return Json(new { Success = true });
        }

        public ActionResult PrivacyPolicy()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {

                return View("privacyPolicyEn");
            }
            return View();

        }


        #region Private Methods

        /// <summary>
        /// Send email to user to tell him that we are processing his request
        /// </summary>
        /// <param name="model"></param>
        private async Task SendEmailToUserAfterContactUs(ContactUsViewModel model)
        {
            string emailSubject = LangText.ContactUsEmailSubject;
            string emailBody = LangText.ContactUsEmailBody;

            await _notificationService.SendEmailAsync(model.Email, emailSubject, emailBody);

        }


        /// <summary>
        /// Send Email to Info@bcare.sa with the user message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task SendUserMessageToSupport(ContactUsViewModel model)
        {
            string emailSubject = LangText.ContactUsSupportEmailSubject;
            string emailBody = $"Customer {model.Name} sent this message:{Environment.NewLine}{model.Message}{Environment.NewLine}";
            emailBody += model.Email == null ? "" : $"Customer Email: {model.Email}{Environment.NewLine}";
            emailBody += model.Mobile == null ? "" : $"Customer Mobile: {model.Mobile}";
            emailBody += model.Address == null ? "" : $"Customer Address: {model.Address}";

            await _notificationService.SendEmailAsync(ConfigurationManager.AppSettings["InfoEmailAddress"], emailSubject, emailBody);
        }


        #endregion

        #region AjaxOnly Methods

        [AjaxOnly]
        public ActionResult GetAccessToken()
        {
            //IdentityRequestLog identityRequestLog = new IdentityRequestLog();
            //identityRequestLog.Method = "GetAccessToken";
            try
            {
                var formParamters = new Dictionary<string, string>();
                formParamters.Add("grant_type", "client_credentials");
                formParamters.Add("client_Id", _config.Identity.ClientId);
                formParamters.Add("client_secret", _config.Identity.ClientSecret);
                formParamters.Add("curent_user_id", User.Identity.GetUserId<string>());
                var content = new FormUrlEncodedContent(formParamters);
                var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                postTask.ConfigureAwait(false);
                postTask.Wait();
                var response = postTask.Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {

                    //identityRequestLog.Response = jsonString;
                    var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                    if (User != null && User.Identity != null && !string.IsNullOrEmpty(User.Identity.GetUserId()))
                    {
                        var user = _authorizationService.GetUserDBByID(User.Identity.GetUserId());
                        result.isCorporateUser = (user.IsCorporateUser) ? true : false;
                    }

                    //identityRequestLog.ErrorCode = 1;
                    //identityRequestLog.ErrorDescription = "Success";
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);

                    return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //identityRequestLog.ErrorCode = 2;
                    //identityRequestLog.ErrorDescription = "Error";
                    //identityRequestLog.Response = response.ToString() + " response.IsSuccessStatusCode = " + response.IsSuccessStatusCode + " jsonString = " + jsonString;
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);

                    return Json("", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                //identityRequestLog.ErrorCode = 2;
                //identityRequestLog.ErrorDescription = "Error";
                //identityRequestLog.Response = ex.ToString();
                //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);

                var logId = DateTime.Now.GetTimestamp();
                _logger.Log($"Tameenk Home controller -> GetAccessToken [key={logId}]", ex);
                return Json(new
                {
                    errorId = logId,
                    errorMessage = ex.ToString(),
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> VerifyGoogleCaptcha(GoogleCaptchaReuqest theResponse)
        {
            try
            {
                var formParamters = new Dictionary<string, string>();
                formParamters.Add("secret", _config.GoogleCaptchaConfig.Secret);
                formParamters.Add("response", theResponse.Response);
                var content = new FormUrlEncodedContent(formParamters);

                var googleResponse = await _httpClient.PostAsync(_config.GoogleCaptchaConfig.Url, content).ConfigureAwait(false);

                var jsonString = await googleResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (googleResponse.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(jsonString);
                    return Json(result, "application/json", JsonRequestBehavior.AllowGet);
                }
                return Json(jsonString, "application/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var logId = DateTime.Now.GetTimestamp();
                _logger.Log($"VerifyGoogleCaptcha [key={logId}]", ex);
                return Json(new
                {
                    errorId = logId,
                    errorMessage = ex.GetBaseException().Message
                });
            }
        }
        #endregion
    }


    public class AccessTokenResult
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
        public bool isCorporateUser { get; set; }

    }

    public class GoogleCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
        [JsonProperty("challenge_ts")]
        public DateTime Challenge_ts { get; set; }

        // the hostname of the site where the reCAPTCHA was solved
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("error-codes")]
        public List<string> Error_codes { get; set; }

    }
    public class GoogleCaptchaReuqest
    {
        [JsonProperty("response")]
        public string Response { get; set; }
    }
}

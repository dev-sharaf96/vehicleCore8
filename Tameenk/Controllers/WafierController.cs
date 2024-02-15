using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Models;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Models.Promotion;
using Tameenk.Security.Encryption;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tamkeen.bll.Model;

namespace Tameenk.Controllers
{
    [Authorize]
    public class WafierController : Controller
    {
        #region Fields


        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IQuotationService _quotationService;
        private readonly IOrderService _orderService;
        private readonly IDriverService _driverService;
        private readonly Random _rnd;
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly ILogger _logger;
        private readonly IPromotionService _promotionService;
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor


        public WafierController(IPayfortPaymentService payfortPaymentService,
            IShoppingCartService shoppingCartService,
            IQuotationService quotationService,
            IOrderService orderService,
            IDriverService driverService,
            IHttpClient httpClient,
            IInsuranceCompanyService insuranceCompanyService,
            ISadadPaymentService sadadPaymentService,
            TameenkConfig tameenkConfig,
            ILogger logger,
            IPromotionService promotionService,
            INotificationService notificationService)
        {
            _payfortPaymentService = payfortPaymentService;
            _shoppingCartService = shoppingCartService;
            _quotationService = quotationService;
            _orderService = orderService;
            _driverService = driverService;
            _rnd = new Random(System.Environment.TickCount);
            _httpClient = httpClient;
            _config = tameenkConfig;
            _insuranceCompanyService = insuranceCompanyService;
            _sadadPaymentService = sadadPaymentService;
            _logger = logger;
            _promotionService = promotionService;
            _notificationService = notificationService;
        }

        #endregion

        #region Properties

        private string CurrentUserID
        {
            get
            {
                return User.Identity.GetUserId<string>();
            }
        }

        #endregion

        #region Actions

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_accessToken))
                    {
                        var formParamters = new Dictionary<string, string>();
                        formParamters.Add("grant_type", "client_credentials");
                        formParamters.Add("client_Id", _config.Identity.ClientId);
                        formParamters.Add("client_secret", _config.Identity.ClientSecret);

                        var content = new FormUrlEncodedContent(formParamters);
                        var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                        postTask.ConfigureAwait(false);
                        postTask.Wait();
                        var response = postTask.Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = response.Content.ReadAsStringAsync().Result;
                            var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                            _accessToken = result.access_token;
                        }
                    }
                    return _accessToken;

                }
                catch (Exception ex)
                {
                    var logId = DateTime.Now.GetTimestamp();
                    _logger.Log($"CheckoutController -> GetAccessToken [key={logId}]", ex);
                    return _accessToken;
                }
            }
        }
        public class AccessTokenResult
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }
            [JsonProperty("expires_in")]
            public int expires_in { get; set; }


        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (!string.IsNullOrWhiteSpace(CurrentUserID))
            {
                if (!IsUserJoinedToWafierProgram())
                {
                    return RedirectToAction("WafierJoin", "Wafier");
                }
            }
           
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> WafierJoin()
        {

            LanguageTwoLetterIsoCode lang = LanguageTwoLetterIsoCode.Ar;
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                lang = LanguageTwoLetterIsoCode.En;
            }
            if (IsUserJoinedToWafierProgram())
            {
                return RedirectToAction("Index", "Wafier");
            }

            PromotionProgramModel model = null;
            var promotionWafierProgram = _promotionService.GetPromotionProgramByKey("wafier");
            if (promotionWafierProgram != null)
            {
                model = new PromotionProgramModel();
                model.Id = 66;
            }
            return View(model);
        }

        #endregion

        private bool IsUserJoinedToWafierProgram()
        {
            var progUser = _promotionService.GetPromotionProgramUser(CurrentUserID);
            return (progUser != null && progUser.EmailVerified && progUser.PromotionProgram.Key.ToLower() == "wafier");
        }

    }
}
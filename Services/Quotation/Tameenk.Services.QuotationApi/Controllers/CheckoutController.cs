using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class CheckoutController : BaseApiController
    {
        #region Fields
        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IQuotationService _quotationService;
        private readonly IOrderService _orderService;
        private readonly IDriverService _driverService;
        private readonly IAuthorizationService _authorizationService;
        private readonly Random _rnd;
        private IWebApiContext _webApiContext;
        //private readonly IRepository<AspNetUser> _Userrepository;
        private readonly IHttpClient _httpClient;
        //private readonly IMapper _mapper;
        private readonly TameenkConfig _config;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly ILogger _logger;
        private readonly IRiyadBankMigsPaymentService _riyadBankMigsPaymentService;
        private readonly IPaymentMethodService _paymentMethodService;

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
        #endregion

        #region Ctor
        public CheckoutController(IPayfortPaymentService payfortPaymentService,
            IShoppingCartService shoppingCartService,
            IQuotationService quotationService,
            IOrderService orderService,
            IDriverService driverService,
            IHttpClient httpClient,
            IPaymentMethodService paymentMethodService,
            IWebApiContext webApiContext,
            //IMapper mapper,
            IAuthorizationService authorizationService,
            IInsuranceCompanyService insuranceCompanyService,
            ISadadPaymentService sadadPaymentService,
            IRiyadBankMigsPaymentService riyadBankMigsPaymentService,
            TameenkConfig tameenkConfig,
            ILogger logger)
        {
            _payfortPaymentService = payfortPaymentService;
            _shoppingCartService = shoppingCartService;
            _quotationService = quotationService;
            _orderService = orderService;
            _driverService = driverService;
            _rnd = new Random(System.Environment.TickCount);
            _httpClient = httpClient;
            _webApiContext = webApiContext;
            //_mapper = mapper;
            _paymentMethodService = paymentMethodService ?? throw new TameenkArgumentNullException(nameof(IPaymentMethodService));
            _authorizationService = authorizationService;
            _config = tameenkConfig;
            _riyadBankMigsPaymentService = riyadBankMigsPaymentService;
            _insuranceCompanyService = insuranceCompanyService;
            _sadadPaymentService = sadadPaymentService;
            _logger = logger;
            _payfortPaymentService = payfortPaymentService;
        }
        #endregion

        #region Actions

        [HttpPost]
        [Route("api/Checkout/AddItemToCart")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddItemToCartResponse>))]
        public IHttpActionResult AddItemToCart(AddItemToCartModel model)
        {
          
            return Ok();
        }

        [HttpGet]
        [Route("api/Checkout/CheckoutDetails")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CheckoutModel>))]
        public IHttpActionResult CheckoutDetails(string ReferenceId, string QtRqstExtrnlId)
        {
            return Ok();
        }



        [HttpGet]
        [Route("api/Checkout/GetPaymentMethods")]
        public IHttpActionResult GetPaymentMethods()
        {
            var paymentMethodModel = new List<PaymentMethodCode>();
            var paymentMethods = _paymentMethodService.GetPaymentMethods();
            paymentMethodModel = paymentMethods.Select(pm => pm.PaymentMethodCode).ToList();
            return Ok(paymentMethodModel);
        }

        [HttpGet]
        [Route("api/Checkout/PaymentRiyadBankResponse")]
        [AllowAnonymous]
        public IHttpActionResult PaymentRiyadBankResponse()
        {
            return Redirect($"{_config.ClientUrl.Url}Error");
        }


        [HttpPost]
        [Route("Checkout/PayfortResult")]
        [AllowAnonymous]
        public IHttpActionResult PayfortResult(FormDataCollection form)
        {
          return Redirect($"{_config.ClientUrl.Url}Error");
        }

        [HttpGet]
        [Route("api/Checkout/Purchased")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PurchasedProductModel>))]
        public IHttpActionResult Purchased(string referenceId)
        {
           return Ok();
        }
        #endregion
    }

    public class AccessTokenResult
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
    }
}
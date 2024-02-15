using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tameenk.App_Start;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Models;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.Quotations;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tameenk.Services.Quotation.Components;
using System.Web;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Checkout.Components.Output;

namespace Tameenk.Controllers
{
    public class ShoppingCartController : Controller
    {
        #region Fields

        // private ClientSignInManager _signInManager;
        private Tameenk.Services.Profile.Component.Membership.ClientSignInManager _signInManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly IQuotationService _quotationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDriverService _driverService;
        private readonly ICheckoutsService _checkoutsService;

        HashSet<string> companyKeysDependOnAddressCheck = new HashSet<string>
{
"Salama",
"Wala",
"ArabianShield",
"Tawuniya",
"MedGulf",
"Solidarity",
"AlRajhi",
"SAICO",
"TUIC",
"ACIG"
};
        private readonly IAddressService _addressService;
        private readonly ICheckoutContext _checkoutContext;

        #endregion

        #region Ctor

        public ShoppingCartController(IShoppingCartService shoppingCartService, IInsuranceCompanyService insuranceCompanyService
        , ILogger logger,
        IQuotationService quotationService,
        IHttpClient httpClient,
        TameenkConfig tameenkConfig,
        IAuthorizationService authorizationService
        , IDriverService driverService
        , IAddressService addressService, ICheckoutContext checkoutContext,
        ICheckoutsService checkoutsService)
        {
            _shoppingCartService = shoppingCartService;
            _insuranceCompanyService = insuranceCompanyService;
            _logger = logger;
            _httpClient = httpClient;
            _config = tameenkConfig;
            _quotationService = quotationService;
            _authorizationService = authorizationService;
            _driverService = driverService;

            _addressService = addressService;
            _checkoutContext = checkoutContext;
            _checkoutsService = checkoutsService;
        }
        #endregion

        //public ClientSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ClientSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}
        public Tameenk.Services.Profile.Component.Membership.ClientSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<Tameenk.Services.Profile.Component.Membership.ClientSignInManager>();

            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// Add product item to cart.
        /// </summary>
        /// <param name="model">The add item to cart model</param>
        /// <returns>Redirect to checkout</returns>
        //[HttpPost]
        //public ActionResult AddItemToCart(AddItemToCartModel model)
        //{
        //    var userId = User.Identity.GetUserId<string>() ?? Request.AnonymousID;
        //    try
        //    {
        //        _shoppingCartService.EmptyShoppingCart(userId, model.ReferenceId);
        //        _shoppingCartService.AddItemToCart(userId, model.ReferenceId, Guid.Parse(model.ProductId), model.SelectedProductBenfitId?.Select(b => new Product_Benefit
        //        {
        //            Id = b,
        //            IsSelected = true
        //        }).ToList());
        //        List<string> errors = new List<string>();
        //        var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(userId, model.ReferenceId);
        //        if (shoppingCartItem == null || DateTime.Now.AddHours(-16) > shoppingCartItem.QuotationResponseCreateDateTime)
        //        {
        //            var request = _quotationService.GetQuotationRequestDrivers(model.QuotaionRequestExternalId);
        //            errors.Add(CheckoutResources.RefreshQuotation);
        //            if (!TempData.ContainsKey("Checkout_HomePageErrors"))
        //                TempData.Add("Checkout_HomePageErrors", errors);
        //            return RedirectToAction("SearchResult", "Quotation", new
        //            {
        //                qtRqstExtrnlId = model.QuotaionRequestExternalId,
        //                TypeOfInsurance = request.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).InsuranceTypeCode.GetValueOrDefault(1),
        //                VehicleAgencyRepair = request.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).VehicleAgencyRepair,
        //                DeductibleValue = request.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).DeductibleValue
        //            });
        //        }
        //        var quotationRequest = _quotationService.GetQuotationRequestDrivers(model.QuotaionRequestExternalId);
        //        string quotaionRequestExternalId = quotationRequest?.ExternalId;
        //        short typeOfInsurance = shoppingCartItem.InsuranceTypeCode.GetValueOrDefault(1);
        //        bool? vehicleAgencyRepair = shoppingCartItem.VehicleAgencyRepair;
        //        int? deductibleValue = shoppingCartItem.DeductibleValue;

        //        if (shoppingCartItem.InsuranceCompanyId == 12) //Tawuniya
        //        {
        //            var selectedBenefits = shoppingCartItem.ShoppingCartItemBenefits?.Select(a => a.BenefitExternalId).ToList();
        //            var quotationOutput = _checkoutContext.ValidateTawuniyaQuotation(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
        //            shoppingCartItem.InsuranceCompanyId, Channel.Portal.ToString(), new Guid(userId), User.Identity.Name, selectedBenefits);
        //            if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
        //            {

        //                errors.Add(CheckoutResources.RefreshQuotation);
        //                if (!TempData.ContainsKey("Checkout_HomePageErrors"))
        //                    TempData.Add("Checkout_HomePageErrors", errors);
        //                return RedirectToAction("SearchResult", "Quotation", new
        //                {
        //                    qtRqstExtrnlId = model.QuotaionRequestExternalId,
        //                    TypeOfInsurance = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).InsuranceTypeCode.GetValueOrDefault(1),
        //                    VehicleAgencyRepair = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).VehicleAgencyRepair,
        //                    DeductibleValue = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).DeductibleValue
        //                });
        //            }
        //        }
        //        if (shoppingCartItem.InsuranceCompanyId == 14 && typeOfInsurance == 2) //Wataniya
        //        {
        //            var draftpolicyOutput = _checkoutContext.SendWataniyaDraftpolicy(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
        //            shoppingCartItem.InsuranceCompanyId, Channel.Portal.ToString(), new Guid(userId), User.Identity.Name);
        //            if (draftpolicyOutput.ErrorCode != PolicyOutput.ErrorCodes.Success)
        //            {
        //                errors.Add(CheckoutResources.WataniyaDraftPolicyError);
        //                if (!TempData.ContainsKey("Checkout_HomePageErrors"))
        //                    TempData.Add("Checkout_HomePageErrors", errors);
        //                return RedirectToAction("SearchResult", "Quotation", new
        //                {
        //                    qtRqstExtrnlId = model.QuotaionRequestExternalId,
        //                    TypeOfInsurance = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).InsuranceTypeCode.GetValueOrDefault(1),
        //                    VehicleAgencyRepair = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).VehicleAgencyRepair,
        //                    DeductibleValue = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).DeductibleValue
        //                });
        //            }
        //        }

        //        string referenceId = shoppingCartItem.ReferenceId;
        //        string productId = model.ProductId;
        //        if (shoppingCartItem.InsuranceCompanyId == 18 || shoppingCartItem.InsuranceCompanyId == 7
        //        || shoppingCartItem.InsuranceCompanyId == 5) //Alalamiya Or Walaa
        //        {
        //            NumberOfAccidentOutput numberOfAccidentOutput = _checkoutContext.ValidateNumberOfAccident(userId, User.Identity.Name, shoppingCartItem, quotationRequest, Channel.Portal.ToString(), model.SelectedProductBenfitId);
        //            if (numberOfAccidentOutput.ErrorCode == NumberOfAccidentOutput.ErrorCodes.HaveAccidents)
        //            {
        //                errors.Add(InsuranceProvidersResource.UpdatePriceDueToAccident);
        //                TempData.Add("Checkout_HomePageErrors", errors);
        //                return RedirectToAction("SearchResult", "Quotation", new
        //                {
        //                    qtRqstExtrnlId = quotaionRequestExternalId,
        //                    TypeOfInsurance = typeOfInsurance,
        //                    VehicleAgencyRepair = vehicleAgencyRepair,
        //                    DeductibleValue = deductibleValue
        //                });
        //            }
        //            if (numberOfAccidentOutput.ErrorCode != NumberOfAccidentOutput.ErrorCodes.Success)
        //            {
        //                _shoppingCartService.EmptyShoppingCart(userId, model.ReferenceId);
        //                return RedirectToAction("Index", "Error", new { message = CheckoutResources.NumberOfAccidentVlidationError });
        //            }
        //            referenceId = numberOfAccidentOutput.NewReferenceId;
        //            productId = numberOfAccidentOutput.newProductId;
        //        }
        //        string clearText = referenceId + "_" + model.QuotaionRequestExternalId + "_" + productId;
        //        string selectedProductBenfitId = string.Empty;
        //        if (model.SelectedProductBenfitId == null || model.SelectedProductBenfitId.Count() == 0)
        //            clearText += SecurityUtilities.HashKey;
        //        else
        //        {
        //            selectedProductBenfitId = string.Join(",", model.SelectedProductBenfitId);
        //            clearText += selectedProductBenfitId + SecurityUtilities.HashKey;
        //        }
        //        string hashed = SecurityUtilities.HashData(clearText, null);

        //        if (quotationRequest.IsRenewal.HasValue && quotationRequest.IsRenewal.Value && !string.IsNullOrEmpty(quotationRequest.PreviousReferenceId))
        //        {
        //            string currentUserId = User.Identity.GetUserId();
        //            if (string.IsNullOrEmpty(currentUserId))
        //            {
        //                var checkoutDetails = _checkoutsService.GetFromCheckoutDeatilsbyReferenceId(quotationRequest.PreviousReferenceId);
        //                if (checkoutDetails != null)
        //                {
        //                    var user = _authorizationService.GetUserDBByID(checkoutDetails.UserId);
        //                    if (user != null)
        //                    {
        //                        SignInManager.SignIn(user, true, true);
        //                        MigrateUser(user.Id);
        //                        return RedirectToAction("ClearBrowserLocalStorage", "Account", new { returnUrl = "/Checkout/CheckoutDetails?QtRqstExtrnlId=" + model.QuotaionRequestExternalId + "&ReferenceId=" + referenceId + "&ProductId=" + productId + "&selectedProductBenfitId=" + selectedProductBenfitId + "&hashed=" + hashed });
        //                    }
        //                }
        //            }
        //        }

        //        return RedirectToAction("CheckoutDetails", "Checkout", new { QtRqstExtrnlId = model.QuotaionRequestExternalId, referenceId, productId, selectedProductBenfitId, hashed });
        //    }
        //    catch (Exception exp)
        //    {
        //        _logger.Log(exp.ToString(), LogLevel.Error);
        //        _shoppingCartService.EmptyShoppingCart(userId, model.ReferenceId);
        //        return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });
        //    }
        //}


        private void MigrateUser(string userId)
        {
            var anonymousId = Request.AnonymousID;
            //var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
            if (string.IsNullOrWhiteSpace(anonymousId)) return;

            if (string.IsNullOrWhiteSpace(userId)) return;

            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
            //AnonymousIdentificationModule.ClearAnonymousIdentifier();

        }
        [NonAction]
        private void GetSaudiPostAddress(Driver driver, string userId)
        {
            if (driver == null)
                return;
            var addressToUpdate = _addressService.GetAddressesForDriver(driver.DriverId);
            if (addressToUpdate != null)
                return;

            _logger.Log($"CheckoutController -> GetSaudiPostAddress Calling saudi post with nin: {driver.NIN}");
            var accessToken = _authorizationService.GetAccessToken(userId).access_token;

            var responseJson = _httpClient.GetStringAsync($"{_config.Inquiry.Url}saudi-post/address?iqamaId={driver.NIN}", authorizationToken: accessToken).Result;
            _logger.Log($"CheckoutController -> GetSaudiPostAddress saudi post result: {responseJson}");
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                var resposne = JsonConvert.DeserializeObject<SaudiPostResponseModel>(responseJson);
                if (resposne != null)
                {
                    if (resposne.Data != null && resposne.Data.Addresses != null && resposne.Data.Addresses.Any())
                    {
                        var saudiPostAddress = resposne.Data.Addresses.FirstOrDefault();
                        var address = new Address
                        {
                            Address1 = saudiPostAddress.Address1,
                            Address2 = saudiPostAddress.Address2,
                            AdditionalNumber = saudiPostAddress.AdditionalNumber,
                            BuildingNumber = saudiPostAddress.BuildingNumber,
                            CityId = saudiPostAddress.CityId,
                            City = saudiPostAddress.City,
                            District = saudiPostAddress.District,
                            DriverId = driver.DriverId,
                            IsPrimaryAddress = saudiPostAddress.IsPrimaryAddress,
                            Latitude = saudiPostAddress.Latitude,
                            Longitude = saudiPostAddress.Longitude,
                            ObjLatLng = saudiPostAddress.ObjLatLng,
                            PKAddressID = saudiPostAddress.PKAddressID,
                            PolygonString = saudiPostAddress.PolygonString?.ToString(),
                            PostCode = saudiPostAddress.PostCode,
                            RegionId = saudiPostAddress.RegionId,
                            RegionName = saudiPostAddress.RegionName,
                            Restriction = saudiPostAddress.Restriction,
                            Street = saudiPostAddress.Street,
                            Title = saudiPostAddress.Title?.ToString(),
                            UnitNumber = saudiPostAddress.UnitNumber
                        };
                        _driverService.UpdateDriverAddress(driver, address);
                    }
                }
            }
        }

    }
}
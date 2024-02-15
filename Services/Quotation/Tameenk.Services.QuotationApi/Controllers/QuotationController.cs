using DocumentFormat.OpenXml.EMMA;
using MoreLinq;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Utilities;
using Tameenk.Data;
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.WebResources;
using Tameenk.Security.CustomAttributes;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core.Files;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Quotation.Components;

namespace Tameenk.Services.QuotationApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class QuotationController : BaseApiController
    {

        #region Fields
        //   private readonly IQuotationApiService _quotationApiService;
        private readonly IFileService _FileService;
        private readonly IVehicleService _vehicleService;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IQuotationService _quotationService;
        private readonly ILogger _logger;
        private readonly IQuotationContext _quotationContext;
        private readonly IPolicyService _policyService;
        private readonly HashSet<int> CompaniesIdForStaticDeductible = new HashSet<int> { 2, 8, 11, 20, 24 };
        private readonly HashSet<string> allowedLanguage = new HashSet<string>() { "ar", "en" };
        #endregion

        #region Ctor
        /// <summary>
        /// the Constructor
        /// </summary>
        /// <param name="quotationApiService"></param>
        /// <param name="quotationService"></param>
        /// <param name="FileService"></param>
        /// <param name="vehicleService">Vehicle Service</param>
        /// <param name="insuranceCompanyService">Insurance Company service</param>
        /// <param name="authorizationService"></param>
        /// <param name="logger"></param>
        public QuotationController(IQuotationService quotationService
             , IFileService FileService
             , IVehicleService vehicleService
             , IInsuranceCompanyService insuranceCompanyService
             , IAuthorizationService authorizationService, IQuotationContext quotationContext
            , ILogger logger, IPolicyService policyService)
        {
            _FileService = FileService ?? throw new ArgumentNullException(nameof(FileService));
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _insuranceCompanyService = insuranceCompanyService ?? throw new ArgumentNullException(nameof(insuranceCompanyService));
            _quotationService = quotationService ?? throw new ArgumentNullException(nameof(quotationService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            this._quotationContext = quotationContext;
            this._policyService = policyService;
        }
        #endregion

        #region method

        /// <summary>
        /// promotion from TPL to Comperhensive 
        /// </summary>
        /// <param name="qtRqstExtrnlId"></param>
        /// <param name="vehicleAgencyRepair"></param>

        /// <param name="deductibleValue"></param>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<ProductModel>))]
        [Route("api/quotation/lowest-product")]
        [HttpGet]
        public IHttpActionResult GetLowestProductByPrice(string qtRqstExtrnlId, bool vehicleAgencyRepair = false, short? deductibleValue = 2000, int productTypeId = 2)
        {
            try
            {
                Product product = _quotationService.GetLowestProductByPrice(qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, productTypeId);
                if (product == null)
                    //return Error("Not found product", HttpStatusCode.NotFound);
                    return Ok(product);

                var productModel = product.ToModel();
                if (productModel != null && product.InsuranceTypeCode == 2 && DateTime.Now.Date >= new DateTime(2022, 12, 21))  // BCare discount 5% As per Mubarak 18-12-2022
                {
                    var discount = (product.ProductPrice >= 1350 && product.ProductPrice <= 3999) ? 200 : Math.Round((product.ProductPrice * 5) / 100, 2);
                    productModel.ProductPrice -= discount;
                }

                return Ok(productModel);
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }
        }

        /// <summary>
        /// Get the quotation per insurance providers
        /// </summary>
        /// <param name="insuranceCompanyId">The company id.</param>
        /// <param name="qtRqstExtrnlId">The qutation request external id</param>
        /// <param name="insuranceTypeCode">Insurance type code(1 => TPL, 2 => Comperhensive)</param>
        /// <param name="vehicleAgencyRepair"></param>
        /// <param name="deductibleValue"></param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<QuotationResponseModel>))]
        //    [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<IEnumerable<QuotationOutput>>))]
        //  [Route("api/quote/get")]
        public IHttpActionResult GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel="Portal",bool OdQuotation = false)
        {
            DateTime excutionStartDate = DateTime.Now;
            if (channel.ToLower() == "android".ToLower() && insuranceTypeCode == 2 && insuranceCompanyId == 6)
            {
                return Error("No Product to show");
            }
            QuotationRequestLog log = new QuotationRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ExtrnlId = qtRqstExtrnlId;
            log.InsuranceTypeCode = insuranceTypeCode;
            log.CompanyId = insuranceCompanyId;
            log.Channel = channel;
            string currentUserId = _authorizationService.GetUserId(User);
            string currentUserName = User.Identity.IsAuthenticated ? User.Identity.Name : "";
            Guid selectedUserId = Guid.Empty;
            Guid.TryParse(currentUserId, out selectedUserId);
            if (!string.IsNullOrEmpty(currentUserName))
            {
                log.UserId = selectedUserId.ToString();
                log.UserName = currentUserName;
            }
         
            if (insuranceTypeCode == 1 && insuranceCompanyId != 12)// as per Fayssal 
            {
                vehicleAgencyRepair = false;
            }
            log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, insuranceTypeCode: {insuranceTypeCode}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";
            try
            {
                QuotationResponseModel responseModel = null;
                var quotationResponseCache = _quotationService.GetFromQuotationResponseCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId);
                bool cacheExist = false;
                if (quotationResponseCache != null)
                {
                    responseModel = JsonConvert.DeserializeObject<QuotationResponseModel>(quotationResponseCache.QuotationResponse);
                    if(responseModel!=null&&responseModel.Products.Any())
                    {
                        cacheExist = true;
                    }
                }
                if(!cacheExist)
                {
                    var quotationOutput = _quotationContext.GetQuote(insuranceCompanyId, qtRqstExtrnlId, log.Channel,
                        selectedUserId, currentUserName,log, excutionStartDate, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed,OdQuotation);
                    if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
                    {
                        return Error(quotationOutput.ErrorDescription);
                    }
                    if (quotationOutput.QuotationResponse.Products == null|| quotationOutput.QuotationResponse.Products.Count()==0)
                    {
                        return Error("No Product to show");
                    }
                    //if (insuranceTypeCode == 2 && insuranceCompanyId == 27)
                    //{
                    //    return Error("No Product to show");
                    //}
                    responseModel = quotationOutput.QuotationResponse.ToModel();
                    //if (insuranceTypeCode == 1 || insuranceTypeCode==7 || insuranceTypeCode==8 || insuranceCompanyId==12 || insuranceCompanyId==20)
                    //    responseModel.ShowTabby = false;
                    //else
                    //    responseModel.ShowTabby = true;
                    responseModel.ShowTabby = quotationOutput.ShowTabby;
                    foreach (var product in responseModel.Products)
                    {
                        product.IsRenewal = quotationOutput.IsRenewal;
                        if (product.InsuranceTypeCode == 1)
                            product.ShowTabby = quotationOutput.ActiveTabbyTPL;
                        else if (product.InsuranceTypeCode == 2)
                            product.ShowTabby = quotationOutput.ActiveTabbyComp;
                        else if (product.InsuranceTypeCode == 7)
                            product.ShowTabby = quotationOutput.ActiveTabbySanadPlus;
                        else if (product.InsuranceTypeCode == 8)
                            product.ShowTabby = quotationOutput.ActiveTabbyWafiSmart;
                        else if (product.InsuranceTypeCode == 13)
                            product.ShowTabby = quotationOutput.ActiveTabbyMotorPlus;
                        else
                            product.ShowTabby = false;
                        if (insuranceCompanyId == 8 && insuranceTypeCode == 2)
                        {
                            if (product.DeductableValue == 0)
                                product.DeductableValue = 2000;
                        }
                        if (product.InsuranceTypeCode == 1&&!string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePath))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_ar","_en");
                        }
                        else if(product.InsuranceTypeCode == 2 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_ar", "_en");
                        }
                        else if (product.InsuranceTypeCode == 8 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_Wafi").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_Wafi").Replace("_ar", "_en");
                        }
                        else if (product.InsuranceTypeCode == 9 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_OD").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_OD").Replace("_ar", "_en");
                        }
                        else if(product.InsuranceTypeCode == 7 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathSanadPlus))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_ar", "_en");
                        }
                        else if (product.InsuranceTypeCode == 13 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").Replace("_ar", "_en");
                        }
                        if (product.PriceDetails != null)
                        {
                            List<PriceDetailModel> priceDetails = new List<PriceDetailModel>();
                            var prices = product.PriceDetails.OrderBy(a => a.PriceType.Order).ToList();
                            foreach (var price in prices)
                            {
                                if (price.PriceValue > 0)
                                {
                                    if (price.PriceTypeCode == 12)
                                    {
                                        if (insuranceCompanyId == 22)
                                        {
                                            price.PriceType.EnglishDescription = "COVID-19 Vaccine campaign";
                                            price.PriceType.ArabicDescription = "خصم مبادرة اللقاح كرونا";
                                        }
                                        else if (insuranceCompanyId == 25)
                                        {
                                            price.PriceType.EnglishDescription = "Voluntary Excess Discount";
                                            price.PriceType.ArabicDescription = "خصم مبلغ التحمل الإضافي";
                                        }
                                    }

                                    if (price.PriceTypeCode == 1 && insuranceCompanyId == 20 && DateTime.Now.Date >= new DateTime(2022, 09, 20) && DateTime.Now.Date <= new DateTime(2022, 12, 21))
                                    {
                                        price.PriceType.ArabicDescription = "خصم الخريف";
                                        price.PriceType.EnglishDescription = "Autumn discount";
                                    }
                                   else if (price.PriceTypeCode == 1 && DateTime.Now.Date <= new DateTime(2022, 09, 30))
                                    {
                                        price.PriceType.EnglishDescription = "National Day Discount";
                                        price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                    }

                                    if (price.PriceTypeCode == 1)
                                    {
                                        //if (insuranceCompanyId == 5 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 22))
                                        //{
                                        //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        //}
                                        //else if (insuranceCompanyId == 14 && DateTime.Now.Date >= new DateTime(2022, 02, 20) && DateTime.Now.Date <= new DateTime(2022, 02, 27))
                                        //{
                                        //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        //}
                                        if (insuranceCompanyId == 20) // Rajhi
                                        {
                                            //if (DateTime.Now.Date >= new DateTime(2022, 02, 18) && DateTime.Now.Date <= new DateTime(2022, 02, 26))
                                            //{
                                            //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                            //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                            //}
                                            if (DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 30))
                                            {
                                                price.PriceType.EnglishDescription = "National Day discount";
                                                price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                            }
                                            else if (DateTime.Now.Date >= new DateTime(2023, 01, 5).Date)
                                            {
                                                price.PriceType.EnglishDescription = "Special Discount";
                                                price.PriceType.ArabicDescription = "خصم حصري";
                                            }
                                            //else if (DateTime.Now.Date >= new DateTime(2022, 03, 21) && DateTime.Now.Date <= new DateTime(2022, 06, 21)) // End Date --> To be appointed letter 
                                            //{
                                            //    price.PriceType.EnglishDescription = "Spring Discount";
                                            //    price.PriceType.ArabicDescription = "خصم الربيع";
                                            //}
                                            //else if (DateTime.Now.Date >= new DateTime(2022, 06, 21)) // End Date --> To be appointed letter 
                                            //{
                                            //    price.PriceType.ArabicDescription = "خصم الصيف";
                                            //    price.PriceType.EnglishDescription = "Summer Discount";
                                            //}
                                        }
                                        else if (insuranceCompanyId == 2 && DateTime.Now.Date >= new DateTime(2023, 09, 15) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // ACIG
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        else if (insuranceCompanyId == 4 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 27)) // AICC
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        else if (insuranceCompanyId == 5 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 24)) // TUIC
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        else if (insuranceCompanyId == 7 && DateTime.Now.Date >= new DateTime(2023, 09, 22) && DateTime.Now.Date <= new DateTime(2023, 09, 24)) // Wala
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        else if (insuranceCompanyId == 9 && DateTime.Now.Date >= new DateTime(2023, 02, 22) && DateTime.Now.Date <= new DateTime(2023, 02, 23)) // ArabianShield
                                        {
                                            price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                            price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        }
                                        else if (insuranceCompanyId == 11 && DateTime.Now.Date >= new DateTime(2023, 09, 17) && DateTime.Now.Date <= new DateTime(2023, 09, 27)) // GGI
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        else if (insuranceCompanyId == 12 && DateTime.Now.Date >= new DateTime(2023, 09, 11) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // Tawuniya
                                        {
                                            price.PriceType.EnglishDescription = "National Day - Insure & Safe Discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطني - أمّن تسلم";
                                        }
                                        else if (insuranceCompanyId == 13 && DateTime.Now.Date >= new DateTime(2023, 02, 21) && DateTime.Now.Date <= new DateTime(2023, 02, 25)) // Salama
                                        {
                                            price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                            price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        }
                                        else if ((insuranceCompanyId == 14 && insuranceTypeCode == 2) && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // Wataniya
                                        {
                                            price.PriceType.EnglishDescription = "Saudi National Day Discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                        }
                                        else if (insuranceCompanyId == 17 && DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // UCA
                                        {
                                            price.PriceType.EnglishDescription = "Saudi National Day Discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                        }
                                        else if ((insuranceCompanyId == 24 && insuranceTypeCode != 2) && DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 28)) // Allianz
                                        {
                                            price.PriceType.EnglishDescription = "Saudi National Day Discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                        }
                                        else if (insuranceCompanyId == 25 && DateTime.Now.Date >= new DateTime(2023, 02, 22) && DateTime.Now.Date <= new DateTime(2023, 02, 23)) // (AXA / GIG)
                                        {
                                            price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                            price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        }
                                        //else if (insuranceCompanyId == 26 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 24))
                                        //{
                                        //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        //}
                                        else if (insuranceCompanyId == 27 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 25)) // Buruj
                                        {
                                            price.PriceType.EnglishDescription = "National Day discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
                                        }
                                        //else if (insuranceCompanyId == 22 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
                                        //{
                                        //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        //}
                                        //else if (insuranceCompanyId == 9 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
                                        //{
                                        //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                        //}
                                    }

                                    if (insuranceCompanyId == 14 && price.PriceTypeCode == 3 && DateTime.Now.Date <= new DateTime(2022, 09, 30))
                                    {
                                        price.PriceType.EnglishDescription = "National Day Discount";
                                        price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                    }
                                    priceDetails.Add(price);
                                }
                            }

                            ////
                            /// 1- BCare discount 5% As per Mubarak 18-12-2022
                            /// 2- https://bcare.atlassian.net/browse/VW-859
                            if (product.InsuranceTypeCode == 2 && DateTime.Now.Date >= new DateTime(2022, 12, 21))
                            {
                                var bcareDiscountPrice = new PriceDetailModel();
                                bcareDiscountPrice.PriceType = new PriceTypeModel();
                                bcareDiscountPrice.PriceType.ArabicDescription = "خصم بي كير";
                                bcareDiscountPrice.PriceType.EnglishDescription = "BCare discount";
                                bcareDiscountPrice.PriceValue = (product.ProductPrice >= 1350 && product.ProductPrice <= 3999) ? 200 : Math.Round((product.ProductPrice * 5) / 100, 2);

                                product.ProductPrice -= bcareDiscountPrice.PriceValue;
                                priceDetails.Add(bcareDiscountPrice);
                            }

                            if (priceDetails.Count() > 0)
                                product.PriceDetails = priceDetails;
                        }
                        if (product.Product_Benefits != null)
                        {
                            foreach (var benfit in product.Product_Benefits)
                            {
                                if (benfit.BenefitId != 0)
                                    continue;
                                var serviceProduct = quotationOutput.Products.Where(a => a.ProductId == product.ExternalProductId).FirstOrDefault();
                                if (serviceProduct == null)
                                    continue;
                                // get specific benfit from the selected product above
                                var serviceBenfitInfo = serviceProduct.Benefits.Where(a => a.BenefitId == benfit.BenefitExternalId).FirstOrDefault();
                                if (serviceBenfitInfo == null)
                                    continue;
                                benfit.Benefit.ArabicDescription = serviceBenfitInfo.BenefitNameAr;
                                benfit.Benefit.EnglishDescription = serviceBenfitInfo.BenefitNameEn;
                            }
                            product.Product_Benefits = product.Product_Benefits.OrderByDescending(a => a.IsReadOnly).ToList();
                            int indexOfBenefit14= product.Product_Benefits.ToList().FindIndex(a => a.BenefitId == 14);
                            if (indexOfBenefit14 > 0)
                            {
                                product.Product_Benefits = product.Product_Benefits.Move(indexOfBenefit14, 1, 0).ToList();
                            }

                        }
                    }
                    if (insuranceTypeCode == 2|| insuranceTypeCode == 9 || (insuranceTypeCode == 1 && insuranceCompanyId == 12))
                    {
                        if (!CompaniesIdForStaticDeductible.Contains(insuranceCompanyId))
                            responseModel.Products = responseModel.Products.OrderByDescending(x => x.DeductableValue).ToList();

                        #region Rearrange deductible // As per Fayssal & Mubarak  @22-12-2022 to return list arranged Descending

                        //var hasDeductable10000 = false;
                        //var deductable10000Index = 0;
                        //var hasDeductable4000 = false;
                        //var deductable4000Index = 0;
                        //for (int i = 0; i < responseModel.Products.Count; i++)
                        //{
                        //    if (responseModel.Products.ElementAt(i).DeductableValue == 10000 && insuranceCompanyId != 5)
                        //    {
                        //        hasDeductable10000 = true;
                        //        deductable10000Index = i;
                        //        break;
                        //    }
                        //    if (responseModel.Products.ElementAt(i).DeductableValue == 4000&& insuranceCompanyId==5)
                        //    {
                        //        hasDeductable4000 = true;
                        //        deductable4000Index = i;
                        //        break;
                        //    }
                        //}
                        //if (hasDeductable10000)
                        //{
                        //    responseModel.Products = responseModel.Products.Move(deductable10000Index, 1, 0).ToList();
                        //}
                        //if (hasDeductable4000&& insuranceCompanyId == 5)
                        //{
                        //    responseModel.Products = responseModel.Products.Move(deductable4000Index, 1, 0).ToList();
                        //}

                        #endregion

                        if (vehicleAgencyRepair && insuranceCompanyId == 22)
                        {
                            var defaultProduct = responseModel.Products.FirstOrDefault();
                            var agencyRepairBenefit = defaultProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
                            if (agencyRepairBenefit != null && agencyRepairBenefit.BenefitPrice.HasValue && agencyRepairBenefit.BenefitPrice > 0)
                            {
                                responseModel.Products.Remove(defaultProduct);
                                var benefitVat = (agencyRepairBenefit.BenefitPrice.Value * 15) / 100;
                                defaultProduct.ProductPrice = defaultProduct.ProductPrice + agencyRepairBenefit.BenefitPrice.Value + benefitVat;
                                responseModel.Products.Add(defaultProduct);
                                int listCount = responseModel.Products.Count();
                                responseModel.Products = responseModel.Products.Move(listCount-1, 1, 0).ToList() ;
                                if (defaultProduct.DeductableValue != deductibleValue)
                                {
                                    var deductableProduct = responseModel.Products.Where(a => a.DeductableValue == deductibleValue).FirstOrDefault();
                                    if (deductableProduct != null)
                                    {
                                        var agencyBenefit = deductableProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
                                        if (agencyBenefit != null && agencyBenefit.BenefitPrice.HasValue && agencyBenefit.BenefitPrice > 0)
                                        {
                                            int productIndex = responseModel.Products.ToList().IndexOf(deductableProduct);
                                            responseModel.Products.Remove(deductableProduct);
                                            var vat = (agencyBenefit.BenefitPrice.Value * 15) / 100;
                                            deductableProduct.ProductPrice = deductableProduct.ProductPrice + agencyBenefit.BenefitPrice.Value + vat;
                                            responseModel.Products.Add(deductableProduct);
                                            responseModel.Products = responseModel.Products.Move(listCount - 1, 1, productIndex).ToList();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    QuotationResponseCache cache = new QuotationResponseCache();
                    cache.InsuranceCompanyId = insuranceCompanyId;
                    cache.ExternalId = qtRqstExtrnlId;
                    cache.InsuranceTypeCode = insuranceTypeCode;
                    cache.VehicleAgencyRepair = vehicleAgencyRepair;
                    cache.DeductibleValue = deductibleValue;
                    cache.UserId = selectedUserId;
                    string jsonResponse = JsonConvert.SerializeObject(responseModel);
                    cache.QuotationResponse = jsonResponse;
                    string exception = string.Empty;
                    _quotationService.InsertIntoQuotationResponseCache(cache, out exception);
                }
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }
        }
        

        /// <summary>
        /// this will be helpful in case of integration testing phase
        /// to test the enitre quotation requests added into the db against a certian insurance company
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <param name="insuranceTypeCode"></param>
        /// <param name="vehicleAgencyRepair"></param>
        /// <param name="deductibleValue"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("api/quotation/InsuranceCompanyIntegrationTestGetQuote")]
        //public IHttpActionResult InsuranceCompanyIntegrationTestGetQuote(int insuranceCompanyId, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null)
        //{
        //    QuotationRequestLog log = new QuotationRequestLog();
        //    DateTime utcTestStart = DateTime.Now;
        //    var quotationRepo = EngineContext.Current.Resolve<IRepository<QuotationRequest>>();

        //    var dbContext = EngineContext.Current.Resolve<IDbContext>();
        //    dbContext.ExecuteSqlCommand("UPDATE QuotationRequest SET RequestPolicyEffectiveDate = DATEADD(DAY,5,GETDATE())");
        //    dbContext.ExecuteSqlCommand("UPDATE QuotationResponse SET CreateDateTime = DATEADD(DAY,-15,GETDATE())");

        //    var enitreQuotationReqeuests = quotationRepo.Table
        //        .DistinctBy(x => new { x.InsuredId, x.VehicleId })
        //        .OrderByDescending(x => x.ID)
        //        .Skip(20)
        //        .Take(100)
        //        .Select(x => x.ExternalId).ToList();

        //    var predefinedLogInfo = new ServiceRequestLog();

        //    foreach (var quotId in enitreQuotationReqeuests)
        //    {
        //        try
        //        {
        //            // _quotationContext.GetQuotation(insuranceCompanyId, quotId, predefinedLogInfo, log, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, automatedTest: true);
        //        }
        //        catch (Exception)
        //        { }
        //    }


        //    return Ok();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("api/quotation/DownloadQuotationAutomatedTestExcelSheet")]
        //public HttpResponseMessage DownloadQuotationAutomatedTestExcelSheet()
        //{

        //    var fileName = _quotationContext.ExportAutomatedTestResultToExcel(true);
        //    var fileBytes = File.ReadAllBytes(fileName);

        //    var result = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new ByteArrayContent(fileBytes)
        //    };
        //    result.Content.Headers.ContentDisposition =
        //        new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = fileName.Split('\\').Last()
        //        };
        //    result.Content.Headers.ContentType =
        //        new MediaTypeHeaderValue("application/octet-stream");

        //    return result;
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("api/quotation/DownloadPolicyAutomatedTestExcelSheet")]
        //public HttpResponseMessage DownloadPolicyAutomatedTestExcelSheet()
        //{

        //    var fileName = _quotationContext.ExportAutomatedTestResultToExcel(false);
        //    var fileBytes = File.ReadAllBytes(fileName);

        //    var result = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new ByteArrayContent(fileBytes)
        //    };
        //    result.Content.Headers.ContentDisposition =
        //        new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = fileName.Split('\\').Last()
        //        };
        //    result.Content.Headers.ContentType =
        //        new MediaTypeHeaderValue("application/octet-stream");

        //    return result;
        //}

        #region DLL File 

        /// <summary>
        /// save dll file of company
        /// </summary>
        /// <param name="nameOfFile">name space of company</param>
        /// <param name="file">binary data to dll file</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK)]
        [Route("api/quotation/dll-file")]
        [HttpPost]
        public IHttpActionResult SaveDLLCompanyFile(string nameOfFile, [FromBody] byte[] file)
        {
            try
            {
                _FileService.SaveFileInBin(nameOfFile, file);
                return Ok();
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);

            }


        }
        /// <summary>
        /// check if dll file of company exist in bin folder or not
        /// </summary>
        /// <param name="nameOfFile">name space of company</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [Route("api/quotation/dll-exist")]
        [HttpGet]
        public IHttpActionResult CheckDllFileExist(string nameOfFile)
        {
            try
            {
                return Ok(_FileService.FileExist(nameOfFile));
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }


        }

        /// <summary>
        /// check if dll file valid or not
        /// </summary>
        /// <param name="nameSpace">Name of nameSpace</param>
        /// <param name="nameofClass">Name of Class</param> 
        /// <param name="file">dll file in binary data</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [Route("api/quotation/valid-dll")]
        [HttpPost]
        public IHttpActionResult ValidateDllFile(string nameSpace, string nameofClass, [FromBody] byte[] file)
        {
            try
            {
                return Ok(_FileService.ValidateDllFile(nameSpace, nameofClass, file));
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }

        }

        /// <summary>
        /// delete file of company
        /// </summary>
        /// <param name="nameOfFile">name space of company</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK)]
        [Route("api/quotation/delete-dll")]
        [HttpGet]
        public IHttpActionResult DeleteDLLCompanyFile(string nameOfFile)
        {
            try
            {
                _FileService.DeleteFile(nameOfFile);
                return Ok();
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }


        }

        #endregion

        #region website profile API's

        /// <summary>
        /// Get Quoation Request by user id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<QuotationRequestModel>>))]
        [Route("api/quotation/user-quotation-request")]
        public IHttpActionResult GetQuotationRequestsByUserId(string userId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new TameenkArgumentNullException("Id");
                var result = _quotationContext.GetQuotationRequestsByUserId(userId, pageIndex, pageSize);
                //then convert to model
                IEnumerable<QuotationRequestModel> dataModel = result.Select(e => e.ToModel());
                dataModel = dataModel.ToList();
                foreach (QuotationRequestModel item in dataModel)
                {
                    item.RemainingTimeToExpireInSeconds = item.CreatedDateTime.AddHours(16).Subtract(DateTime.Now).TotalSeconds;
                }
                return Ok(dataModel, dataModel.Count());
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }
        }

        /// <summary>
        /// get number of offers for specific user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<int>))]
        [Route("api/quotation/user-offers")]
        public IHttpActionResult GetUserOffersCount(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new TameenkArgumentNullException("Id");


                return Ok(_quotationContext.GetUserOffersCount(id));

            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }
        }




        [HttpGet]
        [Route("api/quotation/getQuotation")]
        public IHttpActionResult GetQuotation(string qtRqstExtrnlId, int TypeOfInsurance = 1, bool VehicleAgencyRepair = false, short DeductibleValue = 0, string lang = "ar", string channel = "Web")
        {
            Output<QuotationResponseModel> output = new Output<QuotationResponseModel>();
            try
            {
               
                if (!ValidateSearchResult(qtRqstExtrnlId, TypeOfInsurance, VehicleAgencyRepair, DeductibleValue))
                {
                    //return RedirectToAction("Index", "Home");

                    output.ErrorCode = Output<QuotationResponseModel>.ErrorCodes.NotSuccess;
                    output.ErrorDescription = "";
                    return Single(output);
                }
                var quotationRequest = _quotationService.GetQuotationRequest(qtRqstExtrnlId);
                if (quotationRequest != null && quotationRequest.CreatedDateTime < DateTime.Now.AddHours(-16))
                {
                    // return RedirectToAction("Index", "Error", new { key = "QuotationExpired" });
                    output.ErrorCode = Output<QuotationResponseModel>.ErrorCodes.NotSuccess;
                    output.ErrorDescription = "";
                    return Single(output);
                }
                //Need to be generic
                //when typeOfInsurance  =  2 then check if there a request to companyies with this search criteria is requested and generatead in the db or not
                //if generated then will return the data from db
                //else then request the companies and save the result into db
                if (TypeOfInsurance == 2 && DeductibleValue == 2000)
                {
                    // _quotationRequestServices.WaitUntilComprehensiveQuotsGenerated(qtRqstExtrnlId);
                }

                //if any company return errors we save dumy product into the db and set request time before 16hr
                bool saveResponseToDbWithOldDate = false;
                // bool.TryParse(ConfigurationManager.AppSettings["ShowInsuranceCompanyErrors"], out saveResponseToDbWithOldDate);
                var result = BuildQuotationResponseModel(_quotationService.GetQuotationRequestDrivers(qtRqstExtrnlId), TypeOfInsurance, VehicleAgencyRepair, DeductibleValue == 0 ? null : (short?)DeductibleValue, saveResponseToDbWithOldDate, lang);

                output.Result.DeductibleValuesList = GetDeductibleValuesList();
                //if insurance company is 1 set the deductible value = 1500 so that make it the default selected in the drpdown of deductibleValues
                if (TypeOfInsurance == 1)
                {
                    result.DeductibleValue = 2000;// (short?)Config.DefaultDeductibleValue;
                }
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<QuotationResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Single(output);
            }
        }


        [HttpGet]
        [Route("api/quotation/getBenefits")]
        public IHttpActionResult GetBenefits()
        {
            var benefits = new List<KeyValuePair<int, string>>();
             benefits.Add(new KeyValuePair<int, string>(0, "pin"));
             benefits.Add(new KeyValuePair<int, string>(0, "pin"));
             benefits.Add(new KeyValuePair<int, string>(1, "svg-driver"));
             benefits.Add(new KeyValuePair<int, string>(2, "driver-passenger"));
             benefits.Add(new KeyValuePair<int, string>(3, "geographic-coverage"));
             benefits.Add(new KeyValuePair<int, string>(4, "theft-fire-frontglass"));
             benefits.Add(new KeyValuePair<int, string>(5, "roadside-assistance"));
             benefits.Add(new KeyValuePair<int, string>(6, "car-replacement"));
             benefits.Add(new KeyValuePair<int, string>(7, "AgencyRepairs"));
             benefits.Add(new KeyValuePair<int, string>(8, "svg-noclaim"));
             benefits.Add(new KeyValuePair<int, string>(9, "geographicbahrain-coverage"));
             benefits.Add(new KeyValuePair<int, string>(10, "geographicbahraingcc-coverage"));
             benefits.Add(new KeyValuePair<int, string>(11, "geographicbahrainnorth-coverage"));
             benefits.Add(new KeyValuePair<int, string>(12, "waiver"));
             benefits.Add(new KeyValuePair<int, string>(13, "theft-fire-frontglass"));
             benefits.Add(new KeyValuePair<int, string>(14, "14"));
             benefits.Add(new KeyValuePair<int, string>(15, "15"));
             benefits.Add(new KeyValuePair<int, string>(16, "16"));
             benefits.Add(new KeyValuePair<int, string>(17, "17"));
             benefits.Add(new KeyValuePair<int, string>(18, "18"));
             benefits.Add(new KeyValuePair<int, string>(19, "19"));
            return Single(benefits);
        }

        
        [HttpGet]
        [Route("api/quotation/getCompaniesGrades")]
        public IHttpActionResult GetCompaniesGrades()
        {
            try
            {
                return Ok(_policyService.GetComapaniesGrade());
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);

            }
        }
        public QuotationResponseModelForProfile BuildQuotationResponseModel(QuotationRequest quotationRequest, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool saveQuotResponseWithOldDate = false, string lang = "ar")
        {
            var result = new QuotationResponseModelForProfile();

            result.QtRqstExtrnlId = quotationRequest.ExternalId;
            result.IsRenewal = (quotationRequest.IsRenewal.HasValue && quotationRequest.IsRenewal.Value) ? true : false;
            result.RenewalReferenceId = (!string.IsNullOrEmpty(quotationRequest.PreviousReferenceId)) ? quotationRequest.PreviousReferenceId : null;

            result.Vehicle = new VehicleModelForProfile();
            result.Vehicle.Id = quotationRequest.Vehicle.ID;
            result.Vehicle.Maker = quotationRequest.Vehicle.VehicleMaker;
            result.Vehicle.MakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value : default(short);
            result.Vehicle.FormatedMakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value.ToString("0000") : default(string);
            result.Vehicle.Model = quotationRequest.Vehicle.VehicleModel;
            result.Vehicle.ModelYear = quotationRequest.Vehicle.ModelYear;
            result.Vehicle.PlateTypeCode = quotationRequest.Vehicle.PlateTypeCode;
            if(quotationRequest.Vehicle.PlateTypeCode.HasValue)
                result.Vehicle.PlateColor = CarPlateUtils.GetCarPlateColorByCode((int)quotationRequest.Vehicle.PlateTypeCode);
            if (quotationRequest.Vehicle.VehicleIdTypeId == 2 && !string.IsNullOrEmpty(quotationRequest.Vehicle.CustomCardNumber))
                result.Vehicle.CustomCardNumber = quotationRequest.Vehicle.CustomCardNumber;
            result.Vehicle.CarPlate = new CarPlateInfo(quotationRequest.Vehicle.CarPlateText1,
            quotationRequest.Vehicle.CarPlateText2, quotationRequest.Vehicle.CarPlateText3,
            quotationRequest.Vehicle.CarPlateNumber.HasValue ? quotationRequest.Vehicle.CarPlateNumber.Value : 0);

            var nCDFreeYearsInfo= _quotationService.GetNCDFreeYearsInfo(quotationRequest.NajmNcdFreeYears.Value);
            if(nCDFreeYearsInfo!=null)
            {
                result.NCDFreeYearsEn = nCDFreeYearsInfo.EnglishDescription;
                result.NCDFreeYearsAr = nCDFreeYearsInfo.ArabicDescription;
                result.NCDFreeYears = lang.ToUpper().StartsWith("EN") ? nCDFreeYearsInfo.EnglishDescription : nCDFreeYearsInfo.ArabicDescription;
            }
            result.TypeOfInsurance = insuranceTypeCode;

            result.TypeOfInsuranceText = insuranceTypeCode == 1 ? GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo(lang)) : GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo(lang));
            if (insuranceTypeCode == 1)
            {
                result.TypeOfInsuranceTextAr = GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo("Ar"));
                result.TypeOfInsuranceTextEn =GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo("En"));
            }
            if (insuranceTypeCode == 2)
            {
                result.TypeOfInsuranceTextAr = GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo("Ar"));
                result.TypeOfInsuranceTextEn = GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo("En"));
            }
            result.DeductibleValue = deductibleValue;
            result.VehicleAgencyRepair = vehicleAgencyRepair;

            return result;
        }
        public List<int> GetDeductibleValuesList()
        {
            List<int> list = new List<int>()
            {
                250,
                500,
                750,
                1000,
                1500,
                2000,
                2500,
                3000,
                4000,
                5000,
                6000,
                7500,
                10000,
                20000
            };

            return list;
        }
        private bool ValidateSearchResult(string qtRqstExtrnlId, int TypeOfInsurance, bool VehicleAgencyRepair, short DeductibleValue)
        {
            if (string.IsNullOrEmpty(qtRqstExtrnlId))
            {
                return false;
            }
            if (TypeOfInsurance < 0 || TypeOfInsurance > 2)
                return false;
            if (DeductibleValue < 0)
                return false;
            return true;
        }

        [HttpGet]
        [Route("api/quotation/getVehicleInfo")]
        public IHttpActionResult GetVehicleInfo(string qtRqstExtrnlId, int typeOfInsurance = 1, string lang = "ar")
        {
            try
            {
                if(string.IsNullOrEmpty(qtRqstExtrnlId))
                    return Single("qtRqstExtrnlId is null");
                var result = _quotationContext.GetVehicleInfo(qtRqstExtrnlId, typeOfInsurance, false, null, false, lang);
                if (result != null)
                {
                    return Single(result);
                }
                //var vehicleInfo = _quotationService.GetQuotationRequestAndVehicleInfo(qtRqstExtrnlId);
                //if (vehicleInfo != null)
                //{
                //    var result = BuildQuotationResponseModel(vehicleInfo, typeOfInsurance, false, null, false, lang);
                //    return Single(result);
                //}
                return Single("vehicleInfo is null");
            }
            catch (Exception ex)
            {
                QuotationOutput output = new QuotationOutput();
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Single(output);
            }

        }
        [HttpPost]
        [Route("api/quotation/ShareQuotation")]
        public IHttpActionResult ShareQuotation(string phone, string email, string externalId, int ShareType,string url, string channel,string lang="ar")
        {
            string userId = _authorizationService.GetUserId(User);
           var output= _quotationContext.ShareQuotation(phone, email, externalId, userId,(QuotationShareTypes)ShareType, url, channel,lang);
            if(output.ErrorCode==ShareQuotationOutput.ErrorCodes.Success)
            {
                return Ok(output.ErrorDescription);
            }
            else
            {
                QuotationRequestLog log = new QuotationRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "An error occurred";
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

                return Ok(output);
            }
        }

        [HttpGet]
        [Route("api/quotation/getAllCompanies")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<InsuranceCompanyModel>>))]
        public IHttpActionResult GetAllInsuranceCompanies()
        {
            QuotationRequestLog log = new QuotationRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.NIN = "GetAllInsuranceCompanies";

            try
            {
                var result = _insuranceCompanyService.GetAllInsuranceCompanies(0, int.MaxValue, "InsuranceCompanyID", true, false);
                if (result == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "_insuranceCompanyService.GetAllInsuranceCompanies return null";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return Error("Ann error occured");
                }

                IEnumerable<InsuranceCompanyModel> dataModel = null;
                dataModel = result.OrderBy(a => a.Order).Select(e => e.ToModel());
                if (dataModel == null)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "dataModel is null";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return Error("Ann error occured");
                }

                return Ok(dataModel, result.TotalCount);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return Error("Ann error occured");
            }
        }

        #endregion


        private LanguageTwoLetterIsoCode GetCurrentLanguage(string lang)
        {
            return lang.ToLower() == "en" ? LanguageTwoLetterIsoCode.En : LanguageTwoLetterIsoCode.Ar;
        }


        #endregion
    }
}

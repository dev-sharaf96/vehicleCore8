using DocumentFormat.OpenXml.EMMA;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
// using System.Web.Http; //By Niaz Upgrade-Assistant todo
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.QuotationNew.Components;
using System.Threading;
using System.Threading.Tasks;
// using PrometheusLib.Classes; //By Niaz Upgrade-Assistant todo
using Tameenk.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.QuotationNew.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class QuotationController : BaseApiController
    {

        #region Fields

        private readonly IQuotationAuthorizationService _authorizationService;
        private readonly IQuotationNewContext _quotationContext;
        private readonly IAsyncQuotationContext _asyncQuotationContext;
        private readonly IAsyncCompanyQuotationContext _asyncCompanyQuotationContext;

        private readonly HashSet<int> CompaniesIdForStaticDeductible = new HashSet<int> { 2, 8, 11, 20, 24 };

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
        public QuotationController(IQuotationAuthorizationService authorizationService, IQuotationNewContext quotationContext
                        , IAsyncQuotationContext asyncQuotationContext, IAsyncCompanyQuotationContext asyncCompanyQuotationContext)
        {
            _authorizationService = authorizationService;
            _quotationContext = quotationContext;
            _asyncQuotationContext = asyncQuotationContext;
            _asyncCompanyQuotationContext = asyncCompanyQuotationContext;
        }
        #endregion

        #region method

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
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<QuotationResponseModel>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<IEnumerable<QuotationOutput>>))]
        //public IHttpActionResult GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false)
        //{
        //    //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //    //cancellationTokenSource.CancelAfter(5000);

        //    DateTime excutionStartDate = DateTime.Now;
        //    if (channel.ToLower() == "android".ToLower() && insuranceTypeCode == 2 && insuranceCompanyId == 6)
        //        return Error("No Product to show");

        //    QuotationRequestLog log = new QuotationRequestLog();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.ExtrnlId = qtRqstExtrnlId;
        //    log.InsuranceTypeCode = insuranceTypeCode;
        //    log.CompanyId = insuranceCompanyId;
        //    log.Channel = channel;
        //    log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, insuranceTypeCode: {insuranceTypeCode}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";

        //    string currentUserId = _authorizationService.GetUserId(User);
        //    string currentUserName = User.Identity.IsAuthenticated ? User.Identity.Name : "";
        //    Guid selectedUserId = Guid.Empty;
        //    Guid.TryParse(currentUserId, out selectedUserId);
        //    if (!string.IsNullOrEmpty(currentUserName))
        //    {
        //        log.UserId = selectedUserId.ToString();
        //        log.UserName = currentUserName;
        //    }

        //    if (insuranceTypeCode == 1 && insuranceCompanyId != 12)// as per Fayssal
        //        vehicleAgencyRepair = false;

        //    try
        //    {
        //        QuotationResponseModel responseModel = null;
        //        var quotationResponseCache = _quotationContext.GetFromQuotationResponseCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId);
        //        bool cacheExist = false;
        //        if (quotationResponseCache != null)
        //        {
        //            responseModel = JsonConvert.DeserializeObject<QuotationResponseModel>(quotationResponseCache.QuotationResponse);
        //            if (responseModel != null && responseModel.Products.Any())
        //            {
        //                cacheExist = true;
        //            }
        //        }
        //        if (!cacheExist)
        //        {
        //            var quotationOutput = _quotationContext.GetQuote(insuranceCompanyId, qtRqstExtrnlId, log.Channel,
        //                selectedUserId, currentUserName, log, excutionStartDate, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
        //            if (quotationOutput.ErrorCode != QuotationNewOutput.ErrorCodes.Success)
        //            {
        //                return Error(quotationOutput.ErrorDescription);
        //            }
        //            if (quotationOutput.QuotationResponse.Products == null || quotationOutput.QuotationResponse.Products.Count() == 0)
        //            {
        //                return Error("No Product to show");
        //            }
        //            //if (insuranceTypeCode == 2 && insuranceCompanyId == 27)
        //            //{
        //            //    return Error("No Product to show");
        //            //}
        //            responseModel = quotationOutput.QuotationResponse.ToModel();
        //            //if (insuranceTypeCode == 1 || insuranceTypeCode==7 || insuranceTypeCode==8 || insuranceCompanyId==12 || insuranceCompanyId==20)
        //            //    responseModel.ShowTabby = false;
        //            //else
        //            //    responseModel.ShowTabby = true;
        //            responseModel.ShowTabby = quotationOutput.ShowTabby;
        //            foreach (var product in responseModel.Products)
        //            {
        //                product.IsRenewal = quotationOutput.IsRenewal;
        //                if (product.InsuranceTypeCode == 1)
        //                    product.ShowTabby = quotationOutput.ActiveTabbyTPL;
        //                else if (product.InsuranceTypeCode == 2)
        //                    product.ShowTabby = quotationOutput.ActiveTabbyComp;
        //                else if (product.InsuranceTypeCode == 7)
        //                    product.ShowTabby = quotationOutput.ActiveTabbySanadPlus;
        //                else if (product.InsuranceTypeCode == 8)
        //                    product.ShowTabby = quotationOutput.ActiveTabbyWafiSmart;
        //                else if (product.InsuranceTypeCode == 13)
        //                    product.ShowTabby = quotationOutput.ActiveTabbyMotorPlus;
        //                else
        //                    product.ShowTabby = false;
        //                if (insuranceCompanyId == 8 && insuranceTypeCode == 2)
        //                {
        //                    if (product.DeductableValue == 0)
        //                        product.DeductableValue = 2000;
        //                }
        //                if (product.InsuranceTypeCode == 1 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePath))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_ar", "_en");
        //                }
        //                else if (product.InsuranceTypeCode == 2 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_ar", "_en");
        //                }
        //                else if (product.InsuranceTypeCode == 8 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_Wafi").Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_Wafi").Replace("_ar", "_en");
        //                }
        //                else if (product.InsuranceTypeCode == 9 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_OD").Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_Comp", "_OD").Replace("_ar", "_en");
        //                }
        //                else if (product.InsuranceTypeCode == 7 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathSanadPlus))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_ar", "_en");
        //                }
        //                else if (product.InsuranceTypeCode == 13 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
        //                {
        //                    product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").Replace("_en", "_ar");
        //                    product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").Replace("_ar", "_en");
        //                }
        //                if (product.PriceDetails != null)
        //                {
        //                    List<PriceDetailModel> priceDetails = new List<PriceDetailModel>();
        //                    var prices = product.PriceDetails.OrderBy(a => a.PriceType.Order).ToList();
        //                    foreach (var price in prices)
        //                    {
        //                        if (price.PriceValue > 0)
        //                        {
        //                            if (price.PriceTypeCode == 12)
        //                            {
        //                                if (insuranceCompanyId == 22)
        //                                {
        //                                    price.PriceType.EnglishDescription = "COVID-19 Vaccine campaign";
        //                                    price.PriceType.ArabicDescription = "خصم مبادرة اللقاح كرونا";
        //                                }
        //                                else if (insuranceCompanyId == 25)
        //                                {
        //                                    price.PriceType.EnglishDescription = "Voluntary Excess Discount";
        //                                    price.PriceType.ArabicDescription = "خصم مبلغ التحمل الإضافي";
        //                                }
        //                            }

        //                            if (price.PriceTypeCode == 1 && insuranceCompanyId == 20 && DateTime.Now.Date >= new DateTime(2022, 09, 20) && DateTime.Now.Date <= new DateTime(2022, 12, 21))
        //                            {
        //                                price.PriceType.ArabicDescription = "خصم الخريف";
        //                                price.PriceType.EnglishDescription = "Autumn discount";
        //                            }
        //                            else if (price.PriceTypeCode == 1 && DateTime.Now.Date <= new DateTime(2022, 09, 30))
        //                            {
        //                                price.PriceType.EnglishDescription = "National Day Discount";
        //                                price.PriceType.ArabicDescription = "خصم اليوم الوطني";
        //                            }

        //                            if (price.PriceTypeCode == 1)
        //                            {
        //                                //if (insuranceCompanyId == 5 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 22))
        //                                //{
        //                                //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                //}
        //                                //else if (insuranceCompanyId == 14 && DateTime.Now.Date >= new DateTime(2022, 02, 20) && DateTime.Now.Date <= new DateTime(2022, 02, 27))
        //                                //{
        //                                //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                //}
        //                                if (insuranceCompanyId == 20) // Rajhi
        //                                {
        //                                    //if (DateTime.Now.Date >= new DateTime(2022, 02, 18) && DateTime.Now.Date <= new DateTime(2022, 02, 26))
        //                                    //{
        //                                    //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                    //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                    //}
        //                                    if (DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 30))
        //                                    {
        //                                        price.PriceType.EnglishDescription = "National Day discount";
        //                                        price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                    }
        //                                    else if (DateTime.Now.Date >= new DateTime(2023, 01, 5).Date)
        //                                    {
        //                                        price.PriceType.EnglishDescription = "Special Discount";
        //                                        price.PriceType.ArabicDescription = "خصم حصري";
        //                                    }
        //                                    //else if (DateTime.Now.Date >= new DateTime(2022, 03, 21) && DateTime.Now.Date <= new DateTime(2022, 06, 21)) // End Date --> To be appointed letter 
        //                                    //{
        //                                    //    price.PriceType.EnglishDescription = "Spring Discount";
        //                                    //    price.PriceType.ArabicDescription = "خصم الربيع";
        //                                    //}
        //                                    //else if (DateTime.Now.Date >= new DateTime(2022, 06, 21)) // End Date --> To be appointed letter 
        //                                    //{
        //                                    //    price.PriceType.ArabicDescription = "خصم الصيف";
        //                                    //    price.PriceType.EnglishDescription = "Summer Discount";
        //                                    //}
        //                                }
        //                                else if (insuranceCompanyId == 2 && DateTime.Now.Date >= new DateTime(2023, 09, 15) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // ACIG
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                else if (insuranceCompanyId == 4 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 27)) // AICC
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                else if (insuranceCompanyId == 5 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 24)) // TUIC
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                else if (insuranceCompanyId == 7 && DateTime.Now.Date >= new DateTime(2023, 09, 22) && DateTime.Now.Date <= new DateTime(2023, 09, 24)) // Wala
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                else if (insuranceCompanyId == 9 && DateTime.Now.Date >= new DateTime(2023, 02, 22) && DateTime.Now.Date <= new DateTime(2023, 02, 23)) // ArabianShield
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                }
        //                                else if (insuranceCompanyId == 11 && DateTime.Now.Date >= new DateTime(2023, 09, 17) && DateTime.Now.Date <= new DateTime(2023, 09, 27)) // GGI
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                else if (insuranceCompanyId == 12 && DateTime.Now.Date >= new DateTime(2023, 09, 11) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // Tawuniya
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day - Insure & Safe Discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطني - أمّن تسلم";
        //                                }
        //                                else if (insuranceCompanyId == 13 && DateTime.Now.Date >= new DateTime(2023, 02, 21) && DateTime.Now.Date <= new DateTime(2023, 02, 25)) // Salama
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                }
        //                                else if ((insuranceCompanyId == 14 && insuranceTypeCode == 2) && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // Wataniya
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi National Day Discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطني";
        //                                }
        //                                else if (insuranceCompanyId == 17 && DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 30)) // UCA
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi National Day Discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطني";
        //                                }
        //                                else if ((insuranceCompanyId == 24 && insuranceTypeCode != 2) && DateTime.Now.Date >= new DateTime(2023, 09, 21) && DateTime.Now.Date <= new DateTime(2023, 09, 28)) // Allianz
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi National Day Discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطني";
        //                                }
        //                                else if (insuranceCompanyId == 25 && DateTime.Now.Date >= new DateTime(2023, 02, 22) && DateTime.Now.Date <= new DateTime(2023, 02, 23)) // (AXA / GIG)
        //                                {
        //                                    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                }
        //                                //else if (insuranceCompanyId == 26 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 24))
        //                                //{
        //                                //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                //}
        //                                else if (insuranceCompanyId == 27 && DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 25)) // Buruj
        //                                {
        //                                    price.PriceType.EnglishDescription = "National Day discount";
        //                                    price.PriceType.ArabicDescription = "خصم اليوم الوطنى";
        //                                }
        //                                //else if (insuranceCompanyId == 22 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
        //                                //{
        //                                //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                //}
        //                                //else if (insuranceCompanyId == 9 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
        //                                //{
        //                                //    price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
        //                                //    price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
        //                                //}
        //                            }

        //                            if (insuranceCompanyId == 14 && price.PriceTypeCode == 3 && DateTime.Now.Date <= new DateTime(2022, 09, 30))
        //                            {
        //                                price.PriceType.EnglishDescription = "National Day Discount";
        //                                price.PriceType.ArabicDescription = "خصم اليوم الوطني";
        //                            }
        //                            priceDetails.Add(price);
        //                        }
        //                    }

        //                    ////
        //                    /// 1- BCare discount 5% As per Mubarak 18-12-2022
        //                    /// 2- https://bcare.atlassian.net/browse/VW-859
        //                    if (product.InsuranceTypeCode == 2 && DateTime.Now.Date >= new DateTime(2022, 12, 21))
        //                    {
        //                        var bcareDiscountPrice = new PriceDetailModel();
        //                        bcareDiscountPrice.PriceType = new PriceTypeModel();
        //                        bcareDiscountPrice.PriceType.ArabicDescription = "خصم بي كير";
        //                        bcareDiscountPrice.PriceType.EnglishDescription = "BCare discount";
        //                        bcareDiscountPrice.PriceValue = (product.ProductPrice >= 1350 && product.ProductPrice <= 3999) ? 200 : Math.Round((product.ProductPrice * 5) / 100, 2);

        //                        product.ProductPrice -= bcareDiscountPrice.PriceValue;
        //                        priceDetails.Add(bcareDiscountPrice);
        //                    }

        //                    if (priceDetails.Count() > 0)
        //                        product.PriceDetails = priceDetails;
        //                }
        //                if (product.Product_Benefits != null)
        //                {
        //                    foreach (var benfit in product.Product_Benefits)
        //                    {
        //                        if (benfit.BenefitId != 0)
        //                            continue;
        //                        var serviceProduct = quotationOutput.Products.Where(a => a.ProductId == product.ExternalProductId).FirstOrDefault();
        //                        if (serviceProduct == null)
        //                            continue;
        //                        // get specific benfit from the selected product above
        //                        var serviceBenfitInfo = serviceProduct.Benefits.Where(a => a.BenefitId == benfit.BenefitExternalId).FirstOrDefault();
        //                        if (serviceBenfitInfo == null)
        //                            continue;
        //                        benfit.Benefit.ArabicDescription = serviceBenfitInfo.BenefitNameAr;
        //                        benfit.Benefit.EnglishDescription = serviceBenfitInfo.BenefitNameEn;
        //                    }
        //                    product.Product_Benefits = product.Product_Benefits.OrderByDescending(a => a.IsReadOnly).ToList();
        //                    int indexOfBenefit14 = product.Product_Benefits.ToList().FindIndex(a => a.BenefitId == 14);
        //                    if (indexOfBenefit14 > 0)
        //                    {
        //                        product.Product_Benefits = product.Product_Benefits.Move(indexOfBenefit14, 1, 0).ToList();
        //                    }

        //                }
        //            }
        //            if (insuranceTypeCode == 2 || insuranceTypeCode == 9 || (insuranceTypeCode == 1 && insuranceCompanyId == 12))
        //            {
        //                if (!CompaniesIdForStaticDeductible.Contains(insuranceCompanyId))
        //                    responseModel.Products = responseModel.Products.OrderByDescending(x => x.DeductableValue).ToList();

        //                #region Rearrange deductible // As per Fayssal & Mubarak  @22-12-2022 to return list arranged Descending

        //                //var hasDeductable10000 = false;
        //                //var deductable10000Index = 0;
        //                //var hasDeductable4000 = false;
        //                //var deductable4000Index = 0;
        //                //for (int i = 0; i < responseModel.Products.Count; i++)
        //                //{
        //                //    if (responseModel.Products.ElementAt(i).DeductableValue == 10000 && insuranceCompanyId != 5)
        //                //    {
        //                //        hasDeductable10000 = true;
        //                //        deductable10000Index = i;
        //                //        break;
        //                //    }
        //                //    if (responseModel.Products.ElementAt(i).DeductableValue == 4000&& insuranceCompanyId==5)
        //                //    {
        //                //        hasDeductable4000 = true;
        //                //        deductable4000Index = i;
        //                //        break;
        //                //    }
        //                //}
        //                //if (hasDeductable10000)
        //                //{
        //                //    responseModel.Products = responseModel.Products.Move(deductable10000Index, 1, 0).ToList();
        //                //}
        //                //if (hasDeductable4000&& insuranceCompanyId == 5)
        //                //{
        //                //    responseModel.Products = responseModel.Products.Move(deductable4000Index, 1, 0).ToList();
        //                //}

        //                #endregion

        //                if (vehicleAgencyRepair && insuranceCompanyId == 22)
        //                {
        //                    var defaultProduct = responseModel.Products.FirstOrDefault();
        //                    var agencyRepairBenefit = defaultProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
        //                    if (agencyRepairBenefit != null && agencyRepairBenefit.BenefitPrice.HasValue && agencyRepairBenefit.BenefitPrice > 0)
        //                    {
        //                        responseModel.Products.Remove(defaultProduct);
        //                        var benefitVat = (agencyRepairBenefit.BenefitPrice.Value * 15) / 100;
        //                        defaultProduct.ProductPrice = defaultProduct.ProductPrice + agencyRepairBenefit.BenefitPrice.Value + benefitVat;
        //                        responseModel.Products.Add(defaultProduct);
        //                        int listCount = responseModel.Products.Count();
        //                        responseModel.Products = responseModel.Products.Move(listCount - 1, 1, 0).ToList();
        //                        if (defaultProduct.DeductableValue != deductibleValue)
        //                        {
        //                            var deductableProduct = responseModel.Products.Where(a => a.DeductableValue == deductibleValue).FirstOrDefault();
        //                            if (deductableProduct != null)
        //                            {
        //                                var agencyBenefit = deductableProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
        //                                if (agencyBenefit != null && agencyBenefit.BenefitPrice.HasValue && agencyBenefit.BenefitPrice > 0)
        //                                {
        //                                    int productIndex = responseModel.Products.ToList().IndexOf(deductableProduct);
        //                                    responseModel.Products.Remove(deductableProduct);
        //                                    var vat = (agencyBenefit.BenefitPrice.Value * 15) / 100;
        //                                    deductableProduct.ProductPrice = deductableProduct.ProductPrice + agencyBenefit.BenefitPrice.Value + vat;
        //                                    responseModel.Products.Add(deductableProduct);
        //                                    responseModel.Products = responseModel.Products.Move(listCount - 1, 1, productIndex).ToList();
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            QuotationResponseCache cache = new QuotationResponseCache();
        //            cache.InsuranceCompanyId = insuranceCompanyId;
        //            cache.ExternalId = qtRqstExtrnlId;
        //            cache.InsuranceTypeCode = insuranceTypeCode;
        //            cache.VehicleAgencyRepair = vehicleAgencyRepair;
        //            cache.DeductibleValue = deductibleValue;
        //            cache.UserId = selectedUserId;
        //            string jsonResponse = JsonConvert.SerializeObject(responseModel);
        //            cache.QuotationResponse = jsonResponse;
        //            string exception = string.Empty;
        //            _quotationContext.InsertIntoQuotationResponseCache(cache, out exception);
        //        }
        //        return Ok(responseModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        QuotationNewOutput output = new QuotationNewOutput();
        //        output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

        //        log.ServerIP = Utilities.GetInternalServerIP();
        //        log.UserAgent = Utilities.GetUserAgent();
        //        log.UserIP = Utilities.GetUserIPAddress();
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
        //        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);

        //        return Ok(output);
        //    }
        //}

        #endregion


        #region New Methods Async

        public async Task<IActionResult> GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false)
        {
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //cancellationTokenSource.CancelAfter(5000);
            //By Niaz Upgrade-Assistant todo
            // PrometheusHttpRequestModule.IncrementQuotationCounter();

            //if (!EnableQuotationApiFeatureFlag.EnableQuotationApi)
            //    return BadRequest();

            DateTime excutionStartDate = DateTime.Now;
            if (channel.ToLower() == "android".ToLower() && insuranceTypeCode == 2 && insuranceCompanyId == 6)
                return Error("No Product to show");

            QuotationRequestLog log = new QuotationRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ExtrnlId = qtRqstExtrnlId;
            log.InsuranceTypeCode = insuranceTypeCode;
            log.CompanyId = insuranceCompanyId;
            log.Channel = channel;
            log.Referer = Utilities.GetFullUrlReferrer();
            log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, insuranceTypeCode: {insuranceTypeCode}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";

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
                vehicleAgencyRepair = false;

            try
            {
                QuotationResponseModel responseModel = null;
                bool cacheExist = false;
                var quotationResponseCache = await _asyncQuotationContext.GetFromQuotationResponseCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId);
                if (quotationResponseCache != null && quotationResponseCache.Products.Any())
                {
                    responseModel = quotationResponseCache;
                    cacheExist = true;
                }

                if (cacheExist)
                    return Ok(responseModel);

                var quotationOutput = await _asyncQuotationContext.HandleGetQuote(insuranceCompanyId, qtRqstExtrnlId, log.Channel,
                        selectedUserId, currentUserName, log, excutionStartDate, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
                if (quotationOutput == null)
                    return Error("No Product to show");
                if (quotationOutput.ErrorCode != QuotationNewOutput.ErrorCodes.Success)
                    return Error(quotationOutput.ErrorDescription);
                if (quotationOutput.QuotationResponse.Products == null || quotationOutput.QuotationResponse.Products.Count() == 0)
                    return Error("No Product to show");

                responseModel = quotationOutput.QuotationResponse.ToModel();
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
                    if (product.InsuranceTypeCode == 1 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePath))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePath.ToLower().Replace("_ar", "_en");
                    }
                    else if (product.InsuranceTypeCode == 2 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.ToLower().Replace("_ar", "_en");
                    }
                    else if (product.InsuranceTypeCode == 8 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").ToLower().Replace("_ar", "_en");
                    }
                    else if (product.InsuranceTypeCode == 9 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").ToLower().Replace("_ar", "_en");
                    }
                    else if (product.InsuranceTypeCode == 7 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathSanadPlus))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_ar", "_en");
                    }
                    else if (product.InsuranceTypeCode == 13 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").ToLower().Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").ToLower().Replace("_ar", "_en");
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
                                    else if (insuranceCompanyId == 24)
                                    {
                                        price.PriceType.EnglishDescription = "Promotion";
                                        price.PriceType.ArabicDescription = "عرض ترويجي";
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
                                    else if (insuranceCompanyId == 14) // Wataniya
                                    {
                                        if (insuranceTypeCode == 2 && (DateTime.Now.Date >= new DateTime(2023, 09, 23) && DateTime.Now.Date <= new DateTime(2023, 09, 30))) // Wataniya
                                        {
                                            price.PriceType.EnglishDescription = "Saudi National Day Discount";
                                            price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                        }
                                    }
                                    else if (insuranceCompanyId == 17) // UCA
                                    {
                                        //price.PriceType.EnglishDescription = "Hosting 2034 world cup discount";
                                        //price.PriceType.ArabicDescription = "خصم إستضافة كأس العالم 2034";
                                        price.PriceType.EnglishDescription = "Saudi Hosting EXPO 2030 discount";
                                        price.PriceType.ArabicDescription = "خصم استضافة السعودية لإكسبو 2030";
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
                        if ((product.InsuranceTypeCode == 2 || product.InsuranceTypeCode == 13) && DateTime.Now.Date >= new DateTime(2022, 12, 21))
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
                        int indexOfBenefit14 = product.Product_Benefits.ToList().FindIndex(a => a.BenefitId == 14);
                        if (indexOfBenefit14 > 0)
                        {
                            product.Product_Benefits = product.Product_Benefits.Move(indexOfBenefit14, 1, 0).ToList();
                        }

                    }
                }
                if (insuranceTypeCode == 2 || insuranceTypeCode == 9 || insuranceTypeCode == 13 || (insuranceTypeCode == 1 && insuranceCompanyId == 12))
                {
                    if (responseModel.Products.Count() >= 1 && IsAllProductsWithSamePrice(responseModel.Products.ToList()))
                        responseModel.Products = responseModel.Products.OrderBy(x => x.DeductableValue).ToList();

                    else if (!CompaniesIdForStaticDeductible.Contains(insuranceCompanyId))
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
                            responseModel.Products = responseModel.Products.Move(listCount - 1, 1, 0).ToList();
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

                //_asyncQuotationContext.InsertQuotationResponseIntoInmemoryCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId, JsonConvert.SerializeObject(responseModel));
                _asyncQuotationContext.InsertQuotationResponseIntoInmemoryCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId, responseModel);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                QuotationNewOutput output = new QuotationNewOutput();
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
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

        #endregion


        #region Async method & 1 request per each company

        //public async Task<IHttpActionResult> GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, bool vehicleAgencyRepair, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false)
        //{
        //    DateTime excutionStartDate = DateTime.Now;
        //    string currentUserId = _authorizationService.GetUserId(User);
        //    string currentUserName = User.Identity.IsAuthenticated ? User.Identity.Name : "";
        //    Guid selectedUserId = Guid.Empty;
        //    Guid.TryParse(currentUserId, out selectedUserId);

        //    try
        //    {
        //        var quotationOutput = await _asyncCompanyQuotationContext.HandleGetQuote(insuranceCompanyId, qtRqstExtrnlId, channel.ToLower(),
        //                selectedUserId, currentUserName, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
        //        if (quotationOutput == null)
        //            return Error("No Product to show");

        //        return Ok(quotationOutput);
        //    }
        //    catch (Exception ex)
        //    {
        //        QuotationNewOutput output = new QuotationNewOutput();
        //        output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));

        //        QuotationRequestLog log = new QuotationRequestLog();
        //        log.UserIP = Utilities.GetUserIPAddress();
        //        log.ServerIP = Utilities.GetInternalServerIP();
        //        log.UserAgent = Utilities.GetUserAgent();
        //        log.ExtrnlId = qtRqstExtrnlId;
        //        //log.InsuranceTypeCode = insuranceTypeCode;
        //        log.CompanyId = insuranceCompanyId;
        //        log.Channel = channel;
        //        log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";
        //        log.ServerIP = Utilities.GetInternalServerIP();
        //        log.UserAgent = Utilities.GetUserAgent();
        //        log.UserIP = Utilities.GetUserIPAddress();
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
        //        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
        //        return Ok(output);
        //    }
        //}

        #endregion


        #region Private Methods

        private bool IsAllProductsWithSamePrice(List<ProductModel> products)
        {
            var firstPrice = products[0].ProductPrice;
            return products.All(item => item.ProductPrice == firstPrice);
        }

        #endregion


    }
}

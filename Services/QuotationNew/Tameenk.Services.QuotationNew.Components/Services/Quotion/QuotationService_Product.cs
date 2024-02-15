using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net.Http;
using System.Text;
using Tameenk.Core.Domain.Entities;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Common.Utilities;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Core.Data;
using Tameenk.Resources.WebResources;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Core.Caching;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core;
using VehicleInsurance = Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Extensions;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Configuration;
using System.Threading.Tasks;
using QuotationIntegrationDTO = Tameenk.Integration.Dto.Quotation;
using Tameenk.Redis;
using Tameenk.Core.Domain;
using NLog;
using MoreLinq;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService
    {
        private QuotationServiceResponse RequestQuotationProducts(QuotationServiceRequest requestMessage, QuotationResponse quotationResponse, InsuranceCompany insuranceCompany, ServiceRequestLog predefinedLogInfo, bool automatedTest, out string errors)
        {
            errors = string.Empty;
            try
            {
                requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

                QuotationServiceResponse results = null;
                IInsuranceProvider provider = null;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + quotationResponse.InsuranceTypeCode);
                if (instance != null && insuranceCompany.Key != "Tawuniya")
                {
                    provider = instance as IInsuranceProvider;
                }
                if (instance == null)
                {
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);

                    if (providerType != null)
                    {
                        if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                        {
                            //not resolved
                            instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                        }
                        provider = instance as IInsuranceProvider;
                    }
                    if (provider == null)
                    {
                        throw new Exception("Unable to find provider.");
                    }
                    if (insuranceCompany.Key != "Tawuniya")
                        Utilities.AddValueToCache("instance_" + providerFullTypeName + quotationResponse.InsuranceTypeCode, instance, 1440);

                    if (provider != null)
                    {
                      
                            results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                    }
                    scope.Dispose();
                }
                else
                {
                    if (provider != null)
                    {
                     
                            results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                        
                    }
                }
                // Remove products if price is zero
                if (results != null && results.Products != null)
                {

                    results.Products = results.Products.Where(e => e.ProductPrice > 0).ToList();

                    var showZeroPremium = _tameenkConfig.Quotatoin.showZeroPremium;

                    if (showZeroPremium)
                    {
                        // Remove products if basic perineum equal zero
                        results.Products = results.Products.Where(e => e.PriceDetails.Any(p => p.PriceTypeCode == 7 && p.PriceValue > 0)).ToList();

                    }
                    // Remove benefits if price is zero
                    foreach (var prod in results.Products)
                    {
                        if (prod.Benefits != null && prod.Benefits.Count() > 0)
                        {
                            prod.Benefits = prod.Benefits.Where(e => e.BenefitPrice > 0 || (e.IsReadOnly && e.IsSelected == true)).ToList();
                        }
                    }
                }

                return results;
            }
            catch (Exception exp)
            {
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(predefinedLogInfo);
                errors = exp.ToString();
                return null;
            }

        }

        private IEnumerable<Product> ExcludeProductOrBenefitWithZeroPrice(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                var productBenefits = new List<Product_Benefit>();
                productBenefits.AddRange(product.Product_Benefits.Where(x => x.BenefitPrice > 0 || (x.IsReadOnly && x.IsSelected.HasValue && x.IsSelected == true)));
                product.Product_Benefits = productBenefits;
            }

            return products.Where(x => x.ProductPrice > 0);
        }

        private QuotationOutPut HandleProuduct_Terms_Price_payment(QuotationNewOutput quotationResponse, QuotationOutPut quotationOutPut,int insuranceCompanyId,int insuranceTypeCode)
        {
            quotationOutPut.QuotationResponseModel = quotationResponse.QuotationResponse.ToModel();
            quotationOutPut.QuotationResponseModel.ShowTabby = quotationResponse.ShowTabby;
            foreach (var product in quotationOutPut.QuotationResponseModel.Products)
            {
                product.IsRenewal = quotationResponse.IsRenewal;
                if (product.InsuranceTypeCode == 1)
                    product.ShowTabby = quotationResponse.ActiveTabbyTPL;
                else if (product.InsuranceTypeCode == 2)
                    product.ShowTabby = quotationResponse.ActiveTabbyComp;
                else if (product.InsuranceTypeCode == 7)
                    product.ShowTabby = quotationResponse.ActiveTabbySanadPlus;
                else if (product.InsuranceTypeCode == 8)
                    product.ShowTabby = quotationResponse.ActiveTabbyWafiSmart;
                else if (product.InsuranceTypeCode == 13)
                    product.ShowTabby = quotationResponse.ActiveTabbyMotorPlus;
                else
                    product.ShowTabby = false;
                if (insuranceCompanyId == 8 && insuranceTypeCode == 2)
                {
                    if (product.DeductableValue == 0)
                        product.DeductableValue = 2000;
                }
                if (product.InsuranceTypeCode == 1 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePath))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePath.ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePath.ToLower().Replace("_ar", "_en");
                }
                else if (product.InsuranceTypeCode == 2 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePathComp))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePathComp.ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePathComp.ToLower().Replace("_ar", "_en");
                }
                else if (product.InsuranceTypeCode == 8 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePathComp))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").ToLower().Replace("_ar", "_en");
                }
                else if (product.InsuranceTypeCode == 9 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePathComp))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").ToLower().Replace("_ar", "_en");
                }
                else if (product.InsuranceTypeCode == 7 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePathSanadPlus))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePathSanadPlus.ToLower().Replace("_ar", "_en");
                }
                else if (product.InsuranceTypeCode == 13 && !string.IsNullOrEmpty(quotationResponse.TermsAndConditionsFilePathComp))
                {
                    product.TermsFilePathAr = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").ToLower().Replace("_en", "_ar");
                    product.TermsFilePathEn = quotationResponse.TermsAndConditionsFilePathComp.Replace("_Comp", "_MotorPlus").ToLower().Replace("_ar", "_en");
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
                        var serviceProduct = quotationResponse.Products.Where(a => a.ProductId == product.ExternalProductId).FirstOrDefault();
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
            return quotationOutPut;
        }
    }
}

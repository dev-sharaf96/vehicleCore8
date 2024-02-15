using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;
using Tameenk.Resources.WebResources;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService
    {
        private QuotationNewOutput GetQuotationResponseDetails(QuotationNewRequestDetails quoteRequest, InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false, string policyNo = null, string policyExpiryDate = null, bool OdQuotation = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();

            QuotationNewOutput output = new QuotationNewOutput();
            DateTime startDateTime = DateTime.Now;
            string referenceId = string.Empty;
            referenceId = getNewReferenceId();
            log.RefrenceId = referenceId;
            DateTime beforeCallingDB = DateTime.Now;
            string exception = string.Empty;
            if (quoteRequest == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest is null";
                return output;
            }

            ////
            /// validate for quotation expiration as per Khaled@27-11-2023 2:30 PM
            if (DateTime.Now.AddHours(-16) > quoteRequest.QuotationCreatedDate)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.QuotationExpired;
                output.ErrorDescription = WebResources.quotations_is_expired;
                output.LogDescription = "quoteRequest is expired";
                return output;
            }

            if (string.IsNullOrEmpty(quoteRequest.NationalId))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Insured is null or empty";
                return output;
            }
            log.NIN = quoteRequest.NationalId;
            if (string.IsNullOrEmpty(quoteRequest.MainDriverNin))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Driver is null";
                return output;
            }
            predefinedLogInfo.DriverNin = quoteRequest.MainDriverNin;

            if (string.IsNullOrEmpty(quoteRequest.VehicleId.ToString()))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Vehicle is null ";
                return output;
            }
            if (insuranceCompany.InsuranceCompanyID == 8 && quoteRequest.VehicleIdType == VehicleIdType.CustomCard
                && (quoteRequest.VehicleBodyCode == 1 || quoteRequest.VehicleBodyCode == 2 || quoteRequest.VehicleBodyCode == 3 || quoteRequest.VehicleBodyCode == 19 || quoteRequest.VehicleBodyCode == 20))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = "No supported product with medgulf with such information";
                output.LogDescription = "MedGulf Invalid Body Type with Custom Card body type is " + quoteRequest.VehicleBodyCode;
                return output;
            }

            if (quoteRequest.Cylinders >= 0 && quoteRequest.Cylinders <= 4)
                quoteRequest.EngineSizeId = 1;
            else if (quoteRequest.Cylinders >= 5 && quoteRequest.Cylinders <= 7)
                quoteRequest.EngineSizeId = 2;
            else
                quoteRequest.EngineSizeId = 3;

            if (quoteRequest.VehicleIdType == VehicleIdType.CustomCard)
                predefinedLogInfo.VehicleId = quoteRequest.CustomCardNumber;
            else
                predefinedLogInfo.VehicleId = quoteRequest.SequenceNumber;
            log.VehicleId = predefinedLogInfo.VehicleId;
            if (quoteRequest.NationalId.StartsWith("7") && !quoteRequest.OwnerTransfer && (insuranceCompany.InsuranceCompanyID == 12 || insuranceCompany.InsuranceCompanyID == 14))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for Tawuniya and Wataniya";
                return output;
            }
            if (quoteRequest.NationalId.StartsWith("7") && insuranceCompany.InsuranceCompanyID == 25) //AXA
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for AXA";
                return output;
            }
            if (quoteRequest.RequestPolicyEffectiveDate.HasValue && quoteRequest.RequestPolicyEffectiveDate.Value <= DateTime.Now.Date)
            {
                DateTime effectiveDate = DateTime.Now.AddDays(1);
                quoteRequest.RequestPolicyEffectiveDate = new DateTime(effectiveDate.Year, effectiveDate.Month, effectiveDate.Day, effectiveDate.Hour, effectiveDate.Minute, effectiveDate.Second);
                var quoteRequestInfo = _quotationRequestRepository.Table.Where(a => a.ExternalId == qtRqstExtrnlId).FirstOrDefault();
                if (quoteRequestInfo != null)
                {
                    quoteRequestInfo.RequestPolicyEffectiveDate = quoteRequest.RequestPolicyEffectiveDate;
                    _quotationRequestRepository.Update(quoteRequestInfo);
                }
            }

            output.QuotationResponse = new QuotationResponse()
            {
                ReferenceId = referenceId,
                RequestId = quoteRequest.ID,
                InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                VehicleAgencyRepair = vehicleAgencyRepair,
                DeductibleValue = deductibleValue,
                CreateDateTime = startDateTime,
                InsuranceCompanyId = insuranceCompany.InsuranceCompanyID
            };
            string promotionProgramCode = string.Empty;
            int promotionProgramId = 0;
            DateTime beforeGettingRequestMessage = DateTime.Now;
            var requestMessage = GetQuotationRequestMessage(quoteRequest, output.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue, out promotionProgramCode, out promotionProgramId);
            log.RequestMessageResponseTimeInSeconds = DateTime.Now.Subtract(beforeGettingRequestMessage).TotalSeconds;
            //if (insuranceCompany.InsuranceCompanyID == 21 && string.IsNullOrEmpty(requestMessage.PromoCode))
            //{
            //    output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
            //    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
            //    output.LogDescription = "PromoCode is null for Saico ";
            //    return output;
            //}
            if (insuranceCompany.InsuranceCompanyID == 6 && requestMessage.VehicleUseCode == 2)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.CommercialProductNotSupported;
                output.ErrorDescription = "Commercial product is not supported";
                output.LogDescription = "Commercial product is not supported";
                return output;
            }
            if (insuranceCompany.Key.ToLower() == "malath")
            {
                if (insuranceTypeCode == 2)
                    requestMessage.DeductibleValue = null;
                else if (insuranceTypeCode == 9)
                {
                    if (OdQuotation)
                    {
                        requestMessage.PolicyNo = "new";
                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(DateTime.UtcNow.AddYears(1).ToString());
                    }
                    else
                    {
                        requestMessage.PolicyNo = policyNo;
                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                    }
                }
            }

            string errors = string.Empty;
            DateTime beforeCallingQuoteService = DateTime.Now;
            predefinedLogInfo.VehicleAgencyRepair = requestMessage.VehicleAgencyRepair;
            predefinedLogInfo.City = requestMessage.InsuredCity;
            predefinedLogInfo.ChassisNumber = requestMessage.VehicleChassisNumber;
            var response = RequestQuotationProducts(requestMessage, output.QuotationResponse, insuranceCompany, predefinedLogInfo, automatedTest, out errors);
            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCallingQuoteService).TotalSeconds;
            if (response == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response is null due to errors, " + errors;
                return output;
            }
            if (response.Products == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products is null due to errors, " + errors;
                return output;
            }
            if (response.Products.Count() == 0)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products.Count() is null due to errors, " + errors;
                return output;
            }
            output.Products = response.Products;
            var products = new List<Product>();
            DateTime beforeHandlingProducts = DateTime.Now;
            var allBenefitst = _benefitRepository.Table.ToList();
            var allPriceTypes = _priceTypeRepository.Table.ToList();
            foreach (var p in response.Products)
            {
                var product = p.ToEntity();
                if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode)
                    && (insuranceCompaniesExcluedFromSchemesQuotations != null && !insuranceCompaniesExcluedFromSchemesQuotations.Contains(insuranceCompany.InsuranceCompanyID)))
                    product.IsPromoted = true;
                product.ProviderId = insuranceCompany.InsuranceCompanyID;
                if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                    product.InsuranceTypeCode = insuranceTypeCode;

                if (product.Product_Benefits != null)
                {
                    foreach (var pb in product.Product_Benefits)
                    {
                        pb.Benefit = allBenefitst.FirstOrDefault(bf => pb.BenefitId.HasValue && bf.Code == pb.BenefitId.Value);
                        if (pb.BenefitId == 0)
                        {
                            var serviceBenfitInfo = p.Benefits.Where(a => a.BenefitId == pb.BenefitExternalId).FirstOrDefault();
                            if (serviceBenfitInfo != null)
                            {
                                pb.BenefitNameAr = serviceBenfitInfo.BenefitNameAr;
                                pb.BenefitNameEn = serviceBenfitInfo.BenefitNameEn;
                            }
                        }
                        else
                        {
                            pb.BenefitNameAr = pb.Benefit.ArabicDescription;
                            pb.BenefitNameEn = pb.Benefit.EnglishDescription;
                        }
                        if (pb.BenefitId == 7 && vehicleAgencyRepair == true && insuranceTypeCode != 9)
                        {
                            pb.IsSelected = true;
                        }
                    }
                }
                product.CreateDateTime = DateTime.Now;
                product.ReferenceId = output.QuotationResponse.ReferenceId;

                // Load price details from database.
                foreach (var pd in product.PriceDetails)
                {
                    pd.IsCheckedOut = false;
                    pd.CreateDateTime = DateTime.Now;
                    pd.PriceType = allPriceTypes.FirstOrDefault(pt => pt.Code == pd.PriceTypeCode);
                }
                product.QuotaionNo = response.QuotationNo;
                products.Add(product);
            }
            output.QuotationResponse.Products = products;
            if (!string.IsNullOrEmpty(promotionProgramCode) && promotionProgramId != 0)
            {
                output.QuotationResponse.PromotionProgramCode = promotionProgramCode;
                output.QuotationResponse.PromotionProgramId = promotionProgramId;
            }
            if (!string.IsNullOrEmpty(quoteRequest.InsuredCityYakeenCode.ToString()))
                output.QuotationResponse.CityId = quoteRequest.InsuredCityYakeenCode;
            output.QuotationResponse.ICQuoteReferenceNo = response.QuotationNo;
            _quotationResponseRepository.Insert(output.QuotationResponse);
            log.ProductResponseTimeInSeconds = DateTime.Now.Subtract(beforeHandlingProducts).TotalSeconds;
            output.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(output.QuotationResponse.Products).ToList();
            if (insuranceTypeCode == 1 && insuranceCompany.InsuranceCompanyID != 14 && insuranceCompany.InsuranceCompanyID != 17 && insuranceCompany.InsuranceCompanyID != 9)
            {
                var tplbenefit = allBenefitst.Where(a => a.Code == 14).FirstOrDefault();
                if (tplbenefit != null)
                {
                    Product_Benefit prodBenefit = new Product_Benefit();
                    prodBenefit.Benefit = tplbenefit;
                    prodBenefit.BenefitNameAr = tplbenefit.ArabicDescription;
                    prodBenefit.BenefitNameEn = tplbenefit.EnglishDescription;
                    prodBenefit.BenefitId = tplbenefit.Code;
                    prodBenefit.BenefitExternalId = tplbenefit.Code.ToString();
                    prodBenefit.IsSelected = true;
                    prodBenefit.IsReadOnly = true;
                    output.QuotationResponse.Products.FirstOrDefault()?.Product_Benefits?.Add(prodBenefit);
                }

            }

            if (quoteRequest.IsRenewal.HasValue && quoteRequest.IsRenewal.Value && insuranceCompany.InsuranceCompanyID != 13)
            {
                output.ActiveTabbyComp = true;
                output.ActiveTabbyTPL = true;
                output.ActiveTabbySanadPlus = true;
                output.ActiveTabbyWafiSmart = true;
                output.ActiveTabbyMotorPlus = true;
                output.IsRenewal = true;
            }
            else
            {
                output.ActiveTabbyComp = insuranceCompany.ActiveTabbyComp;
                output.ActiveTabbyTPL = insuranceCompany.ActiveTabbyTPL;
                output.ActiveTabbySanadPlus = insuranceCompany.ActiveTabbySanadPlus;
                output.ActiveTabbyWafiSmart = insuranceCompany.ActiveTabbyWafiSmart;
                output.ActiveTabbyMotorPlus = insuranceCompany.ActiveTabbyMotorPlus;
            }

            output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.LogDescription = "Success";
            return output;
        }


        private string getNewReferenceId()
        {
            string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (_quotationResponseRepository.TableNoTracking.Any(a => a.ReferenceId == referenceId))
                return getNewReferenceId();
            return referenceId;
        }
    }
}

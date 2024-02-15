using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Core.Domain.Entities;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Common.Utilities;
using Tameenk.Resources.WebResources;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using System.Threading.Tasks;
using Tameenk.Redis;
using Tameenk.Core.Domain;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService 
    {

        public async Task<QuotationNewOutput> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            var task = Task.Run(() => 
            {
                return GetQuote(insuranceCompanyId, qtRqstExtrnlId, channel, userId, userName, log, excutionStartDate, parentRequestId
                            , insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
            });

            return task.Result;
        }

        private async Task<QuotationNewOutput> GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            QuotationNewOutput output = new QuotationNewOutput();
            output.QuotationResponse = new QuotationResponse();
            try
            {
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceCompanyId == 0)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 9 && !string.IsNullOrEmpty(policyExpiryDate))
                {
                    var oldPolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                    if (oldPolicyExpiryDate.HasValue && oldPolicyExpiryDate.Value.CompareTo(DateTime.Now.AddDays(90)) <= 0)
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.InvalidODPolicyExpiryDate;
                        output.ErrorDescription = SubmitInquiryResource.InvalidODPolicyExpiryDate;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"Can't proceed OD request , as TPL policy expiry date is {oldPolicyExpiryDate.Value} and it's less than 90 days";
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }
                if (insuranceCompanyId == 22 && insuranceTypeCode == 9 && !OdQuotation)
                {
                    if (string.IsNullOrEmpty(hashed))
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = SubmitInquiryResource.ErrorHashing;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Hashed value is empty";
                        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }

                    string clearText = string.Format("{0}_{1}_{2}_{3}", true, policyNo, Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate), SecurityUtilities.HashKey);
                    if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.HashedNotMatched;
                        output.ErrorDescription = SubmitInquiryResource.ErrorHashing;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }

                var insuranceCompany = GetById(insuranceCompanyId);
                log.CompanyName = insuranceCompany.Key;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 2 && !insuranceCompany.IsActiveComprehensive)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "Comprehensive products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Comprehensive products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 1 && !insuranceCompany.IsActiveTPL)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "TPL products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TPL products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 13 && !insuranceCompany.IsActiveMotorPlus)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "Motor Plus products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Motor Plus products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }

                if (insuranceCompany.Key.ToLower() == "tawuniya" || insuranceTypeCode == 1)
                    deductibleValue = null;
                else if (!deductibleValue.HasValue)
                    deductibleValue = (int?)2000;

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompanyId;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;
                predefinedLogInfo.Channel = channel.ToString();
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;
                predefinedLogInfo.Method = "Quotation";

                var quotationNewRequestDetails = await RedisCacheManager.Instance.GetAsync<QuotationRequestDetailsCachingModel>($"{quotationRequestCach_Base_KEY}_{qtRqstExtrnlId}");
                QuotationNewRequestDetails requestDetails = null;
                if (quotationNewRequestDetails != null && quotationNewRequestDetails.QuotationDetails != null)
                {
                    requestDetails = quotationNewRequestDetails.QuotationDetails.ToModel();
                }
                else
                {
                    requestDetails = await GetQuotationRequestDetailsByExternalId(qtRqstExtrnlId);
                    if (requestDetails == null)
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                        output.LogDescription = "failed to get Quotation Request from DB, please check the log file";
                        return output;
                    }
                }

                output = GetQuotationResponseDetails(requestDetails, insuranceCompany, qtRqstExtrnlId, predefinedLogInfo, log, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo: policyNo, policyExpiryDate: policyExpiryDate, OdQuotation: OdQuotation);
                if (output == null)
                    return null;
                if (output.ErrorCode != QuotationNewOutput.ErrorCodes.Success)
                {
                    //output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
                    //output.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.LogDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return (output.ErrorCode == QuotationNewOutput.ErrorCodes.QuotationExpired) ? output : null;
                }
                if (output.QuotationResponse == null || output.QuotationResponse.Products == null || output.QuotationResponse.Products.Count == 0)
                    return null;

                if (OdQuotation)
                    output.QuotationResponse.InsuranceCompany.ShowQuotationToUser = false;
                if (insuranceCompany.AllowAnonymousRequest.HasValue && insuranceCompany.AllowAnonymousRequest.Value)
                    output.QuotationResponse.CompanyAllowAnonymous = true;
                if (userId != Guid.Empty)
                    output.QuotationResponse.AnonymousRequest = false;
                if (insuranceCompany.ShowQuotationToUser.HasValue && !insuranceCompany.ShowQuotationToUser.Value)
                    output.QuotationResponse.Products = null;
                if (insuranceCompany.HasDiscount.HasValue && output.QuotationResponse.Products != null)
                {
                    if (insuranceCompany.HasDiscount.Value && DateTime.Now < insuranceCompany.DiscountEndDate)
                    {
                        insuranceCompany.HasDiscount = true;
                        output.QuotationResponse.HasDiscount = true;
                        output.QuotationResponse.DiscountText = insuranceCompany.DiscountText;
                    }
                    else
                    {
                        insuranceCompany.HasDiscount = false;
                    }
                }
                else
                {
                    insuranceCompany.HasDiscount = false;
                }
                if (insuranceTypeCode == 1)
                    output.ShowTabby = insuranceCompany.ActiveTabbyTPL;
                else if (insuranceTypeCode == 2)
                    output.ShowTabby = insuranceCompany.ActiveTabbyComp;
                else if (insuranceTypeCode == 7)
                    output.ShowTabby = insuranceCompany.ActiveTabbySanadPlus;
                else if (insuranceTypeCode == 8)
                    output.ShowTabby = insuranceCompany.ActiveTabbyWafiSmart;
                else if (insuranceTypeCode == 13)
                    output.ShowTabby = insuranceCompany.ActiveTabbyMotorPlus;
                else
                    output.ShowTabby = false;
          
                output.TermsAndConditionsFilePath = insuranceCompany.TermsAndConditionsFilePath;
                output.TermsAndConditionsFilePathComp = insuranceCompany.TermsAndConditionsFilePathComp;
                output.TermsAndConditionsFilePathSanadPlus = insuranceCompany.TermsAndConditionsFilePathSanadPlus;
       
                if (insuranceCompanyId == 21 && log.UserId != "1b4cdb65-9804-4ab4-86a8-af62bf7812d7" && log.UserId != "ebf4df2c-c9bb-4d7d-91fe-4b9208c1631a") // As per Mubarak @ 18-01-2024 12:23 PM
                    output.QuotationResponse.Products = null;

                if (insuranceTypeCode == 1 && output.QuotationResponse.Products != null && output.QuotationResponse.Products.Any(a => a.ProductPrice >= 5000))
                {
                    var productsLessThan5000 = output.QuotationResponse.Products.Where(a => a.ProductPrice <= 4999).ToList();
                    output.QuotationResponse.Products = productsLessThan5000;
                }


                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }
        }
        private async Task<QuotationNewRequestDetails> GetQuotationRequestDetailsByExternalId(string externalId)
        {
            //QuotationNewRequestDetails requests = await _redisCacheManager.GetAsync<QuotationNewRequestDetails>($"{quotationRequestDetailsCach_Base_KEY}_{externalId}");
            //if (requests != null)
            //    return requests;

            //var scope = EngineContext.Current.ContainerManager.Scope();
            //var providerType = Type.GetType("TameenkObjectContext, Tameenk.Data");
            //IDbContext _dbContext = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope) as IDbContext;
            //var scheduleTaskService = EngineContext.Current.ContainerManager.Resolve<IDbContext>("", scope);

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestDetailsByExternalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(nationalIDParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                QuotationNewRequestDetails requests = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationNewRequestDetails>(reader).FirstOrDefault();
                if (requests != null)
                {
                    requests.AdditionalDrivers = new List<Driver>();
                    requests.MainDriverViolation = new List<DriverViolation>();
                    requests.MainDriverLicenses = new List<DriverLicense>();
                    reader.NextResult();
                    requests.AdditionalDrivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Driver>(reader).ToList();
                    reader.NextResult();
                    requests.MainDriverViolation = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                    reader.NextResult();
                    requests.MainDriverLicenses = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                }

                //RedisCacheManager redisCacheManager = RedisCacheManager.Instance;
                await RedisCacheManager.Instance.SetAsync($"{quotationRequestDetailsCach_Base_KEY}_{externalId}", requests, quotationResponseCach_TiMe);
                return requests;
            }
            catch (Exception exp)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\GetQuotationRequestDetailsByExternalId_exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private InsuranceCompany GetById(int insuranceCompanyId)
        {
            return GetAllinsuranceCompany().FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
        }
    }
}

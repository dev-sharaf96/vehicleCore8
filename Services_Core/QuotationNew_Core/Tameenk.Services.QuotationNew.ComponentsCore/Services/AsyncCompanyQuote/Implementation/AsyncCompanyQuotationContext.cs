using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Caching;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Linq;
using System.Data.Entity;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Resources.WebResources;
using Newtonsoft.Json;
using Tameenk.Integration.Dto.Quotation;


namespace Tameenk.Services.QuotationNew.Components
{
    public class AsyncCompanyQuotationContext : IAsyncCompanyQuotationContext
    {

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        //private readonly ICompanyQuotationService _companyQuotationService;
        private readonly ICompanyQuotationService _companyQuotationService;

        #endregion

        public AsyncCompanyQuotationContext(ICacheManager cacheManager, IRepository<InsuranceCompany> insuranceCompanyRepository, ICompanyQuotationService companyQuotationService)
        {
            _cacheManager = cacheManager;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _companyQuotationService = companyQuotationService;
        }


        #region Public Methods

        public async Task<List<QuotationResponseModel>> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, DateTime excutionStartDate, Guid? parentRequestId = null, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            QuotationRequestLog log = new QuotationRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ExtrnlId = qtRqstExtrnlId;
            log.CompanyId = insuranceCompanyId;
            log.Channel = channel;
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";

            if (channel.ToLower() == "android".ToLower() && insuranceCompanyId == 6)
                return null;

            if (string.IsNullOrEmpty(qtRqstExtrnlId))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.EmptyInputParamter, "qtRqstExtrnlId is null", "qtRqstExtrnlId is null", excutionStartDate);
                return null;
            }
            if (insuranceCompanyId == 0)
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.EmptyInputParamter, SubmitInquiryResource.insuranceCompanyId, "insurance Company Id is required", excutionStartDate);
                return null;
            }

            var insuranceCompany = await GetById(insuranceCompanyId);
            log.CompanyName = insuranceCompany.Key;
            if (insuranceCompany == null)
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.EmptyInputParamter, SubmitInquiryResource.insuranceCompanyId, "insurance Company is null", excutionStartDate);
                return null;
            }
            if (!insuranceCompany.IsActive)
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.CompanyIsNotActive, SubmitInquiryResource.insuranceCompanyId, "insurance Company is not active", excutionStartDate);
                return null;
            }
            if (!insuranceCompany.IsActiveTPL && !insuranceCompany.IsActiveComprehensive)
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.CompanyIsNotActive, SubmitInquiryResource.insuranceCompanyId, "insurance Company is not active tpl or comp", excutionStartDate);
                return null;
            }

            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
            predefinedLogInfo.UserID = userId;
            predefinedLogInfo.UserName = userName;
            predefinedLogInfo.RequestId = parentRequestId;
            predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
            predefinedLogInfo.Channel = channel.ToString();
            predefinedLogInfo.ExternalId = qtRqstExtrnlId;

            //string exception = string.Empty;
            DateTime beforeCallingDB = DateTime.Now;
            var quoteRequest = await GetQuotationRequestDetailsByExternalId(qtRqstExtrnlId);
            log.DabaseResponseTimeInSeconds = DateTime.Now.Subtract(beforeCallingDB).TotalSeconds;

            if (quoteRequest == null)
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.ServiceDown, WebResources.SerivceIsCurrentlyDown, $"quoteRequest is null", excutionStartDate);
                return null;
            }
            if (string.IsNullOrEmpty(quoteRequest.VehicleId.ToString()))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.ServiceDown, WebResources.SerivceIsCurrentlyDown, $"quoteRequest.Vehicle is null", excutionStartDate);
                return null;
            }
            if (string.IsNullOrEmpty(quoteRequest.NationalId))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.ServiceDown, WebResources.SerivceIsCurrentlyDown, $"quoteRequest.Insured is null or empty", excutionStartDate);
                return null;
            }
            log.NIN = quoteRequest.NationalId;

            if (string.IsNullOrEmpty(quoteRequest.MainDriverNin))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.ServiceDown, WebResources.SerivceIsCurrentlyDown, $"quoteRequest.Driver is null", excutionStartDate);
                return null;
            }
            predefinedLogInfo.DriverNin = quoteRequest.MainDriverNin;

            if (insuranceCompany.Key.ToLower() == "tawuniya")
                deductibleValue = null;
            else if (!deductibleValue.HasValue)
                deductibleValue = (int?)2000;

            QuotationResponseModel[] tasksResult = await GetQuote(insuranceCompany, quoteRequest, predefinedLogInfo, channel, userId, userName, log, excutionStartDate
                                , parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);

            if (tasksResult == null)
                return null;

            return await HandleGetQuoteResponse(tasksResult);
        }

        #endregion

        #region Private Methods

        private void HandleGetQuoteLog(QuotationRequestLog log, QuotationNewOutput.ErrorCodes errorCode, string outputMessage, string logMessage, DateTime excutionStartDate)
        {
            //output.ErrorCode = errorCode;
            //output.ErrorDescription = outputMessage;
            log.ErrorCode = (int)errorCode;
            log.ErrorDescription = logMessage;
            log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
            QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
        }

        private bool ValidateODRequestData(QuotationRequestLog log, string policyNo, string policyExpiryDate, string hashed, bool OdQuotation, DateTime excutionStartDate)
        {
            if (!string.IsNullOrEmpty(policyExpiryDate))
            {
                var oldPolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                if (oldPolicyExpiryDate.HasValue && oldPolicyExpiryDate.Value.CompareTo(DateTime.Now.AddDays(90)) <= 0)
                {
                    HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.InvalidODPolicyExpiryDate, SubmitInquiryResource.InvalidODPolicyExpiryDate, $"Can't proceed OD request , as TPL policy expiry date is {oldPolicyExpiryDate.Value} and it's less than 90 days", excutionStartDate);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(hashed))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.EmptyInputParamter, SubmitInquiryResource.ErrorHashing, "Hashed value is empty", excutionStartDate);
                return false;
            }

            string clearText = string.Format("{0}_{1}_{2}_{3}", true, policyNo, Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate), SecurityUtilities.HashKey);
            if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
            {
                HandleGetQuoteLog(log, QuotationNewOutput.ErrorCodes.HashedNotMatched, SubmitInquiryResource.ErrorHashing, $"Hashed Not Matched as clear text is: {clearText} and hashed is: {hashed}", excutionStartDate);
                return false;
            }

            return true;
        }

        private async Task<QuotationNewRequestDetails> GetQuotationRequestDetailsByExternalId(string externalId)
        {
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
                return requests;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\3-GetQuotationRequestDetailsByExternalId_" + externalId + "_Exception.txt", ex.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private async Task<QuotationResponseModel[]> GetQuote(InsuranceCompany insuranceCompany, QuotationNewRequestDetails quoteRequest, ServiceRequestLog predefinedLogInfo, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId, bool vehicleAgencyRepair, int? deductibleValue, string policyNo, string policyExpiryDate, string hashed, bool odQuotation)
        {
            List<Task<QuotationResponseModel>> tasks = new List<Task<QuotationResponseModel>>();

            if (insuranceCompany.IsActiveTPL)
            {
                if (insuranceCompany.InsuranceCompanyID != 12)// as per Fayssal
                    vehicleAgencyRepair = false;

                tasks.Add(_companyQuotationService.HandleGetQuote(insuranceCompany, quoteRequest, 1, predefinedLogInfo, channel, userId, userName, log, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, odQuotation));
            }
            if (insuranceCompany.IsActiveComprehensive && insuranceCompany.InsuranceCompanyID != 12)
            {
                tasks.Add(_companyQuotationService.HandleGetQuote(insuranceCompany, quoteRequest, 2, predefinedLogInfo, channel, userId, userName, log, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, odQuotation));
            }
            if (insuranceCompany.InsuranceCompanyID == 22 && odQuotation)
            {
                var checkOdRequest = ValidateODRequestData(log, policyNo, policyExpiryDate, hashed, odQuotation, excutionStartDate);
                if (!checkOdRequest)
                    return null;

                tasks.Add(_companyQuotationService.HandleGetQuote(insuranceCompany, quoteRequest, 9, predefinedLogInfo, channel, userId, userName, log, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, odQuotation));
            }
            if (insuranceCompany.InsuranceCompanyID == 22)
            {
                tasks.Add(_companyQuotationService.HandleGetQuote(insuranceCompany, quoteRequest, 13, predefinedLogInfo, channel, userId, userName, log, excutionStartDate, parentRequestId, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, odQuotation));
            }

            return await Task.WhenAll(tasks);
        }

        private async Task<List<QuotationResponseModel>> HandleGetQuoteResponse(QuotationResponseModel[] tasksResult)
        {
            List<QuotationResponseModel> quotationResponse = new List<QuotationResponseModel>();
            foreach (var result in tasksResult)
            {
                if (result == null || result.Products == null || result.Products.Count <= 0)
                    continue;

                quotationResponse.Add(result);
            }

            if (quotationResponse == null || quotationResponse.Count == 0)
                return null;

            return quotationResponse;
        }

        #endregion


        #region  Get From Caching

        private async Task<InsuranceCompany> GetById(int insuranceCompanyId)
        {
            return GetAllinsuranceCompany().FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
        }

        private IEnumerable<InsuranceCompany> GetAllinsuranceCompany()
        {
            return _cacheManager.Get("tameenk.insurance.companies.all", 20, () =>
            {
                return _insuranceCompanyRepository.TableNoTracking.Include(c => c.Contact).ToList();
            });
        }

        #endregion
    }
}

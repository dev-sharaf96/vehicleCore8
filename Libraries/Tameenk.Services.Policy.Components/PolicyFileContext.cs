using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.Vehicles;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Core.Domain.Enums.Quotations;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyFileContext: IPolicyFileContext
    {
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IAddressService _addressService;
        private readonly IRepository<BankNins> _bankNinRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<AutoleasingDepreciationSettingHistory> _autoleasingDepreciationSettingHistoryRepository;
        private readonly IVehicleService _vehicleService;
        public PolicyFileContext(IInsuranceCompanyService insuranceCompanyService
            , IRepository<QuotationResponse> quotationResponseRepository,IAddressService addressService,
            IRepository<BankNins> bankNinRepository, IRepository<Bank> bankRepository,
            IRepository<AutoleasingDepreciationSettingHistory> autoleasingDepreciationSettingHistoryRepository,
            IVehicleService vehicleService
            )
        {
            _insuranceCompanyService = insuranceCompanyService;
            _quotationResponseRepository = quotationResponseRepository;
            _addressService = addressService;
            _bankNinRepository = bankNinRepository;
            _bankRepository = bankRepository;
            _autoleasingDepreciationSettingHistoryRepository = autoleasingDepreciationSettingHistoryRepository;
            _vehicleService = vehicleService;
        }

        public PolicyGenerationOutput GeneratePolicyPdfFile(PolicyResponse policy, int companyId,string channel, LanguageTwoLetterIsoCode selectedLanguage, bool generateTemplateFromOurSide)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            PdfGenerationLog log = new PdfGenerationLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            PolicyGenerationOutput output = new PolicyGenerationOutput();
            try
            {
                if (policy == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Policy is sent to method null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.ReferenceId = policy?.ReferenceId;
                log.PolicyNo = policy?.PolicyNo;
                log.ServiceRequest = JsonConvert.SerializeObject(policy);
                if (companyId == 0)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Company is is sent to method 0";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.CompanyID = companyId;
                var insuranceCompany = _insuranceCompanyService.GetById(companyId);

                if (insuranceCompany == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    output.ErrorDescription = "Insurance Company Is Null with id "+companyId;
                  
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;

                if ((!string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) && generateTemplateFromOurSide)
                || (!string.IsNullOrEmpty(insuranceCompany.AutoleaseReportTemplateName) && log.Channel.ToLower() == Channel.autoleasing.ToString().ToLower()))
                {
                    output.IsPdfGeneratedByBcare = true;
                    var policyFile = GeneratePolicyFileFromPolicyDetails(policy.ReferenceId, insuranceCompany, selectedLanguage, log);
                    if (policyFile.ErrorCode != PolicyGenerationOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = policyFile.ErrorCode;
                        output.ErrorDescription = policyFile.ErrorDescription;
                        output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                        return output;
                    }
                    policy.PolicyFile = policyFile.File;
                    output.File = policyFile.File;
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.EPolicyStatus = EPolicyStatus.Available;
                    return output;
                }
                output.IsPdfGeneratedByBcare = false;
                byte[] policyFileByteArray = null;
                if (!string.IsNullOrEmpty(policy.PolicyFileUrl))
                {
                    log.RetrievingMethod = "FileUrl";
                    string fileURL = policy.PolicyFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    log.ServiceURL = fileURL;
                    if (insuranceCompany.Key == "GGI")
                    {
                        log.RetrievingMethod = "FileUrlWebRequest";
                        string responseData = string.Empty;
                        var request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fileURL);
                        request.Timeout = 30000;
                        var response = (System.Net.HttpWebResponse)request.GetResponse();
                        Stream s = response.GetResponseStream();
                        using (var streamReader = new MemoryStream())
                        {
                            s.CopyTo(streamReader);
                            policyFileByteArray = streamReader.ToArray();
                        }
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        if (policyFileByteArray == null)
                        {
                            output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = "policy FileByte Array is returned null";
                            output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }

                        output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.EPolicyStatus = EPolicyStatus.Available;
                        output.File = policyFileByteArray;

                        policy.PolicyFile = policyFileByteArray;
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                    else
                    {
                        if (insuranceCompany.Key == "Malath" && fileURL.ToLower().StartsWith("http://"))
                        {
                            fileURL = fileURL.Replace("http://", "https://");
                        }
                        //using (DownloadFileExtendedWebClient client = new DownloadFileExtendedWebClient(1800000))
                        //{
                        //    policyFileByteArray = client.DownloadData(fileURL);
                        //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        //    if (policyFileByteArray == null)
                        //    {
                        //        output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                        //        output.ErrorDescription = "policy FileByte Array is returned null";
                        //        output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                        //        log.ErrorCode = (int)output.ErrorCode;
                        //        log.ErrorDescription = output.ErrorDescription;
                        //        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        //        return output;
                        //    }

                        //    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                        //    output.ErrorDescription = "Success";
                        //    output.EPolicyStatus = EPolicyStatus.Available;
                        //    output.File = policyFileByteArray;

                        //    policy.PolicyFile = policyFileByteArray;
                        //    log.ServiceResponse = "Success";
                        //    log.ErrorCode = (int)output.ErrorCode;
                        //    log.ErrorDescription = output.ErrorDescription;
                        //    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        //    return output;
                        //}

                        dtBeforeCalling = DateTime.Now;
                        var request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fileURL);
                        request.Timeout = 120000;
                        var response = (System.Net.HttpWebResponse)request.GetResponse();
                        Stream s = response.GetResponseStream();
                        using (var streamReader = new MemoryStream())
                        {
                            s.CopyTo(streamReader);
                            policyFileByteArray = streamReader.ToArray();
                        }
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        if (policyFileByteArray == null)
                        {
                            output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = "policy FileByte Array is returned null";
                            output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }

                        if (insuranceCompany.Key == "ACIG")
                        {
                            string exception = string.Empty;
                            bool validPdf = Utilities.IsValidPdf(policyFileByteArray, out exception);
                            if (!validPdf || !string.IsNullOrEmpty(exception))
                            {
                                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                                output.ErrorDescription = "policy FileByte Array is not valid pdf";
                                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : output.ErrorDescription;
                                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                                return output;
                            }
                        }

                        output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.EPolicyStatus = EPolicyStatus.Available;
                        output.File = policyFileByteArray;

                        policy.PolicyFile = policyFileByteArray;
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                }
                if (policy.PolicyFile != null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.File = policy.PolicyFile;
                    output.EPolicyStatus = EPolicyStatus.Available;

                    log.RetrievingMethod = "Bytes";
                    log.ServiceResponse = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                IInsuranceProvider provider = null;
                var providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
                if (instance != null)
                {
                    provider = instance as IInsuranceProvider;
                }
                if (instance == null)
                {
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);
                    if (providerType == null)
                    {
                        output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ProviderTypeIsNull;
                        output.ErrorDescription = "providerType Is Null";
                        output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                        log.ErrorDescription = output.ErrorDescription;
                        log.ErrorCode = (int)output.ErrorCode;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                    Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);
                }
                if (provider == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ProviderIsNull;
                    output.ErrorDescription = "provider Is Null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var policyScheduleOutput = provider.PolicySchedule(policy.PolicyNo, policy.ReferenceId);
                if (policyScheduleOutput.ErrorCode != FileServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.PolicyScheduleError;
                    output.ErrorDescription = policyScheduleOutput.ServiceResponse;
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.RetrievingMethod = "PolicySchedule";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = policyScheduleOutput.ErrorDescription;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (policyScheduleOutput.IsSchedule)
                {
                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.ServiceResponseTimeInSeconds = policyScheduleOutput.ServiceResponseTimeInSeconds;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;

                    if (!string.IsNullOrEmpty(policyScheduleOutput.PolicyFileUrl))
                    {
                        policy.PolicyFileUrl = policyScheduleOutput.PolicyFileUrl;

                        log.RetrievingMethod = "PolicyScheduleFileUrl";
                        string fileURL = policy.PolicyFileUrl;
                        fileURL = fileURL.Replace(@"\\", @"//");
                        fileURL = fileURL.Replace(@"\", @"/");
                        log.ServiceURL = fileURL;
                        using (System.Net.WebClient client = new System.Net.WebClient())
                        {

                            policyFileByteArray = client.DownloadData(fileURL);
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            if (policyFileByteArray == null)
                            {
                                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                                output.ErrorDescription = "policy FileByte Array is returned null";
                                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = output.ErrorDescription;
                                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                                return output;
                            }

                            output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.EPolicyStatus = EPolicyStatus.Available;
                            output.File = policyFileByteArray;

                            policy.PolicyFile = policyFileByteArray;
                            log.ServiceResponse = "Success";
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }
                    }
                    if (policyScheduleOutput.PolicyFile != null)
                    {
                        output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.File = policyScheduleOutput.PolicyFile;
                        output.EPolicyStatus = EPolicyStatus.Available;

                        policyFileByteArray = policyScheduleOutput.PolicyFile;
                        policy.PolicyFile = policyScheduleOutput.PolicyFile;
                        log.RetrievingMethod = "PolicyScheduleBytes";
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.PolicyScheduleError;
                    output.ErrorDescription = policyScheduleOutput.ErrorDescription;
                    output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                    log.RetrievingMethod = "PolicySchedule";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                output.ErrorDescription = "No File Url or file data in company response";
                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                output.IsPdfGeneratedByBcare = true;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
        }
        public PolicyGenerationOutput GeneratePolicyFileFromPolicyDetails(string referenceId, InsuranceCompany insuranceCompany, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log)
        {
            PolicyGenerationOutput output = new PolicyGenerationOutput();
            try
            {
                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;

                var modelOutput = GetPolicyTemplateGenerationModel(referenceId);
                if (modelOutput.ErrorCode != PolicyGenerationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = modelOutput.ErrorCode;
                    output.ErrorDescription = modelOutput.ErrorDescription;

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                var policyTemplateModel = modelOutput.PolicyModel;
                string policyDetailsJsonString = JsonConvert.SerializeObject(policyTemplateModel);
                log.ServiceRequest = policyDetailsJsonString;
                log.CompanyName = insuranceCompany?.Key;
                if (string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) && string.IsNullOrEmpty(insuranceCompany.AutoleaseReportTemplateName))
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    output.ErrorDescription = "report template name is null";

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                ReportGenerationModel reportGenerationModel = new ReportGenerationModel
                {
                    ReportType = insuranceCompany?.ReportTemplateName,
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                if (log.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                {
                    reportGenerationModel.ReportType = insuranceCompany?.AutoleaseReportTemplateName;
                }
                reportGenerationModel.ReportType = reportGenerationModel.ReportType.Replace("#ProductType", policyTemplateModel.ProductType);
                reportGenerationModel.ReportType = reportGenerationModel.ReportType.Replace("#RoadSideAssistant", policyTemplateModel.HasRoadsideAssistanceBenefit ? "WithRoadsideAssistance" : "WithoutRoadsideAssistance");
                
                if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                    reportGenerationModel.ReportType += "_En";
                else
                    reportGenerationModel.ReportType += "_Ar";

                if (!string.IsNullOrEmpty(policyTemplateModel.ProductDescription) && !string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) && insuranceCompany.Key == "AlRajhi")
                {
                    reportGenerationModel.ReportType = policyTemplateModel.ProductDescription;
                }
                if (!string.IsNullOrEmpty(policyTemplateModel.ProductDescription) && !string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) &&
                    (policyTemplateModel.ProductDescription.ToLower() == "Wafi Comprehensive".ToLower() || policyTemplateModel.ProductDescription.ToLower() == "Wafi Smart".ToLower() || policyTemplateModel.ProductDescription.ToLower() == "Wafi Basic".ToLower()) && insuranceCompany.Key == "AlRajhi")
                {
                    if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        reportGenerationModel.ReportType += "_En";
                    else
                        reportGenerationModel.ReportType += "_Ar";
                }
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(6);
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();


                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);

                log.ServiceResponse = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
        }
        public PolicyFileInfo GetPolicyFileInfo(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            PolicyFileInfo policyFileInfo = new PolicyFileInfo();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyPdfFileInfo";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                policyFileInfo.QuotationRequestInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationRequestInfo>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.CheckoutDetailsInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetailsInfo>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Invoice = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Tameenk.Core.Domain.Entities.Policy>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItem = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemInfo>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.PriceDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList();
                reader.NextResult();

                policyFileInfo.OrderItemBenefit = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefitInfo>(reader).ToList();
                reader.NextResult();

                policyFileInfo.MainDriverLicenses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.AdditionalDrivers = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AdditionalDriverInfo>(reader).ToList();
                reader.NextResult();

                policyFileInfo.AdditionalDriversLicense = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.MakerModelDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<MakerModelDetails>(reader).FirstOrDefault();

                return policyFileInfo;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        public PolicyGenerationOutput GetPolicyTemplateGenerationModel(string referenceId)
        {
            var output = new PolicyGenerationOutput();
            PolicyFileInfo policyFileInfo = null;
            try
            {
                string exception = string.Empty;
                policyFileInfo = GetPolicyFileInfo(referenceId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.QuotationResponseIsNull;
                    output.ErrorDescription = "there is an error while getting policy info " + exception;
                    return output;
                }
                if (policyFileInfo == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.QuotationResponseIsNull;
                    output.ErrorDescription = "policyFileInfo is null";
                    return output;
                }
                if (policyFileInfo.QuotationRequestInfo == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.QuotationResponseIsNull;
                    output.ErrorDescription = "policyFileInfo.QuotationRequestInfo is null";
                    return output;
                }
                if (policyFileInfo.CheckoutDetailsInfo == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.CheckoutDetailsIsNull;
                    output.ErrorDescription = "policyFileInfo.CheckoutDetailsInfo is null";
                    return output;
                }
                var selectedLanguage = policyFileInfo.CheckoutDetailsInfo.SelectedLanguage;
                var stringLang = Enum.GetName(typeof(LanguageTwoLetterIsoCode), selectedLanguage).ToLower();
                if (policyFileInfo.OrderItem == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.OrderItemIsNull;
                    output.ErrorDescription = "policyFileInfo.OrderItem is null";
                    return output;
                }
                if (policyFileInfo.Invoice == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = "policyFileInfo.Invoice is null";
                    return output;
                }
                if (policyFileInfo.Policy == null)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.PolicyIsNull;
                    output.ErrorDescription = "policyFileInfo.Policy is null";
                    return output;
                }
                if (policyFileInfo.PriceDetail == null || policyFileInfo.PriceDetail.Count() == 0)
                {
                    output.ErrorCode = PolicyGenerationOutput.ErrorCodes.PriceDetailIsNull;
                    output.ErrorDescription = "policyFileInfo.PriceDetail is null";
                    return output;
                }
                PolicyResponse policy = new PolicyResponse();
                policy.PolicyEffectiveDate = policyFileInfo.Policy.PolicyEffectiveDate;
                policy.PolicyIssuanceDate = policyFileInfo.Policy.PolicyIssueDate;
                policy.PolicyExpiryDate = policyFileInfo.Policy.PolicyExpiryDate;
                policy.PolicyNo = policyFileInfo.Policy.PolicyNo;
                policy.ReferenceId = referenceId;

                Address mainDriverAddress = null;
                if (policyFileInfo.QuotationRequestInfo.AddressId.HasValue)
                {
                    mainDriverAddress = _addressService.GetAddressDetailsNoTracking(policyFileInfo.QuotationRequestInfo.AddressId.Value);
                }
                if (mainDriverAddress == null)
                {
                    mainDriverAddress = _addressService.GetAddressesByNin(policyFileInfo.QuotationRequestInfo.NationalId);
                }

                List<AdditionalDriverInfo> additionalDrivers = new List<AdditionalDriverInfo>();
                additionalDrivers = policyFileInfo.AdditionalDrivers;
                AdditionalDriverInfo secondDriver = null;
                AdditionalDriverInfo thirdDriver = null;
                AdditionalDriverInfo fourthDriver = null;
                AdditionalDriverInfo fifthDriver = null;

                if (additionalDrivers != null && additionalDrivers.Count > 0)
                    secondDriver = additionalDrivers[0];
                if (additionalDrivers != null && additionalDrivers.Count > 1)
                    thirdDriver = additionalDrivers[1];
                if (additionalDrivers != null && additionalDrivers.Count > 2)
                    fourthDriver = additionalDrivers[2];
                if (additionalDrivers != null && additionalDrivers.Count > 3)
                    fifthDriver = additionalDrivers[3];


                PolicyTemplateGenerationModel policyTemplateModel = new PolicyTemplateGenerationModel();
                policyTemplateModel.PolicyAdditionalCoversAr = new List<string>();
                policyTemplateModel.PolicyAdditionalCoversEn = new List<string>();
                policyTemplateModel.PolicyCoverAgeLimitAr = new List<string>();
                policyTemplateModel.PolicyCoverAgeLimitEn = new List<string>();

                policyTemplateModel.PolicyNo = policy.PolicyNo;
                policyTemplateModel.ReferenceNo = policy.ReferenceId;
                policyTemplateModel.BenfitPrice = "0.00";


                policyTemplateModel.InsuredAge = policyFileInfo.QuotationRequestInfo.InsuredBirthDate.GetUserAge().ToString();
                policyTemplateModel.InsuredBuildingNo = mainDriverAddress?.BuildingNumber;

                policyTemplateModel.ProductTypeAr = policyFileInfo.QuotationRequestInfo.ProductTypeArabicDescription;
                policyTemplateModel.ProductType = policyFileInfo.QuotationRequestInfo.ProductTypeEnglishDescription;

                policyTemplateModel.PrintingDate = DateTime.Now.ToString("dd/MM/yyyy",new CultureInfo("en-US"));
                policyTemplateModel.VehicleCapacity = policyFileInfo.QuotationRequestInfo.VehicleLoad.ToString();
                policyTemplateModel.PolicyIssueDate = policy.PolicyIssuanceDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                if (policy.PolicyEffectiveDate.HasValue)
                {
                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                    {
                        policyTemplateModel.InsuranceStartDay = policy.PolicyEffectiveDate?.ToString("dddd", new CultureInfo("ar-SA"));
                        policyTemplateModel.InsuranceEndDay = policy.PolicyExpiryDate?.ToString("dddd", new CultureInfo("ar-SA"));
                    }
                    else
                    {
                        policyTemplateModel.InsuranceStartDay = policy.PolicyEffectiveDate?.ToString("dddd", new CultureInfo("en-US"));
                        policyTemplateModel.InsuranceEndDay = policy.PolicyExpiryDate?.ToString("dddd", new CultureInfo("en-US"));
                    }
                }

                if (policy.PolicyIssuanceDate.HasValue)
                {
                    policyTemplateModel.PolicyIssueTime = policy.PolicyIssuanceDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                else
                {
                    policyTemplateModel.PolicyIssueTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                if (policy.PolicyExpiryDate.HasValue)
                {
                    policyTemplateModel.PolicyExpireTime = policy.PolicyExpiryDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                else
                {
                    policyTemplateModel.PolicyExpireTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                if (policy.PolicyEffectiveDate.HasValue)
                {
                    policyTemplateModel.PolicyEffectiveTime = policy.PolicyEffectiveDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                else
                {
                    policyTemplateModel.PolicyEffectiveTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                }
                policyTemplateModel.InsuranceStartDate = policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")); ;
                policyTemplateModel.InsuranceStartDateDay = policy.PolicyEffectiveDate?.ToString("dddd", new CultureInfo("en-US"));
                policyTemplateModel.InsuranceEndDate = policy.PolicyExpiryDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                policyTemplateModel.InsuranceEndDateDay = policy.PolicyExpiryDate?.ToString("dddd", new CultureInfo("en-US"));

                System.Globalization.DateTimeFormatInfo HijriDTFI;
                HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                HijriDTFI.Calendar = new System.Globalization.UmAlQuraCalendar();
                HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

                if (policy.PolicyEffectiveDate.HasValue)
                {
                    policyTemplateModel.InsuranceStartDateH = policy.PolicyEffectiveDate.Value.ToString("dd-MM-yyyy", HijriDTFI);
                }
                if (policy.PolicyExpiryDate.HasValue)
                {
                    policyTemplateModel.InsuranceEndDateH = policy.PolicyExpiryDate.Value.ToString("dd-MM-yyyy", HijriDTFI);
                }
                if (policy.PolicyIssuanceDate.HasValue)
                {
                    policyTemplateModel.PolicyIssueDateH = policy.PolicyIssuanceDate.Value.ToString("dd-MM-yyyy", HijriDTFI);
                }
                decimal totalBenfitPrice = 0;
                decimal _pACoverForDriverOnly = 0;
                decimal _passengerIncluded = 0;

                policyTemplateModel.PACoverForDriverAndPassenger = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.PACoverForDriverAndPassengerEn = "Not Included";
                policyTemplateModel.PassengerIncluded = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.CoverforDriverBelow21Years = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.GeographicalExtensionBahrain = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Covered" : "غير مشمولة";
                policyTemplateModel.GeographicalCoverageAr = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Covered" : "غير مشمولة";
                policyTemplateModel.PersonalAccidentBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.NaturalHazardsBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.WindscreenFireTheftBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.HandbagCoverReimbursementBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.ChildSeatCoverReimbursementBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.HireLimit2000 = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.HireLimit3000 = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                policyTemplateModel.TotalYearlyDepText = "يتم خصم 20% بحد أعلى ويتم التعويض بالقيمة السوقية أو القيمة التأمينية أيهما أقل";
                policyTemplateModel.DriverRelatedToInsured_Above21Ar = "غير مشمولة";
                policyTemplateModel.DriverRelatedToInsured_Above21En = "Not Included";
                policyTemplateModel.HireCar_1500_Max15DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_1500_Max15DaysEn = "Not Included";
                policyTemplateModel.DriverRelatedToInsured_FamilyCoverAr = "غير مشمولة";
                policyTemplateModel.DriverRelatedToInsured_FamilyCoverEn = "Not Included";
                policyTemplateModel.HireCar_120_Max15DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_120_Max15DaysEn = "Not Included";
                policyTemplateModel.HireCar_150_Max15DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_150_Max15DaysEn = "Not Included";
                policyTemplateModel.HireCar_180_Max15DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_180_Max15DaysEn = "Not Included";
                policyTemplateModel.HireCar_2000_Max20DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_2000_Max20DaysEn = "Not Included";
                policyTemplateModel.HireCar_4000_Max20DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_4000_Max20DaysEn = "Not Included";
                policyTemplateModel.HireCar_5000_Max20DaysAr = "غير مشمولة";
                policyTemplateModel.HireCar_5000_Max20DaysEn = "Not Included";
                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "غير مشمولة";
                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Not Included";
                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersAr = "غير مشمولة";
                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersEn = "Not Included";
                policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAAr = "غير مشمولة";
                policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAEn = "Not Included";
                policyTemplateModel.CarReplacement_150 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                policyTemplateModel.CarReplacement_200 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                policyTemplateModel.CarReplacement_75 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 8) //MedGUlf
                {
                    policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverOnlyEn = "Not Included";
                    policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                    policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Included";
                    policyTemplateModel.HireCarBenefit = "غير مشمولة";
                    policyTemplateModel.HireCarBenefitEn = "Not Included";
                    policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                    policyTemplateModel.GCCCoverBenefitEn = "Not Included";
                    policyTemplateModel.UnNamedDriver = "غير مشمولة";
                    policyTemplateModel.UnNamedDriverEn = "Not Included";
                    policyTemplateModel.DeathInjuryMedic = "غير مشمولة";
                    policyTemplateModel.DeathInjuryMedicEn = "Not Covered";
                }
                else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11) //GGI
                {
                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = "غير مشمولة";
                    policyTemplateModel.HireCarBenefit = "غير مشمولة";
                    policyTemplateModel.UnNamedDriver = "غير مشمولة";
                    policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                    policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                    policyTemplateModel.PersonalAccidentBenefit = "غير مشمولة";
                    policyTemplateModel.HireCarBenefitEn = "Not Covered";
                    policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Not Covered";
                    policyTemplateModel.UnNamedDriverEn = "Not Covered";
                    policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Covered";
                    policyTemplateModel.GCCCoverBenefitEn = "Not Covered";
                    policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                    policyTemplateModel.PersonalAccidentBenefitEn = "Not Covered";
                }
                else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 13) //Salama
                {
                    policyTemplateModel.PersonalAccidentForDriverPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.PassengersPersonalAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.TowingWithoutAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.TowingWithAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.AuthorizedRepairLimitPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.PersonalBelongingsPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.ReplacementOfKeysPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.NonApplicationOfDepreciationInCaseOfTotalLossPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.NaturalHazardsBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.TheftFirePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.LiabilitiesToThirdPartiesPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.RoadsideAssistanceBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.DeathInjuryMedic = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.GCCCoverBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.UnNamedDriver = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                }
                else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 24) //Allianz
                {
                    policyTemplateModel.GCC_12Month = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.GE_Jordan_Lebanon_12months = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.GE_Egypt_Sudan_Turky_12months = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.GCC_Jordan_Lebanon_Egypt_Sudan_12months = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.MaxLimit = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.OwnDamage = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.NaturalDisasters = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.TheftFire = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.Towing = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.EmergencyMedical = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                    policyTemplateModel.PersonalAccidentForDriverPrice = "0.00";
                    policyTemplateModel.PassengersPersonalAccidentPrice = "0.00";
                    policyTemplateModel.HireCarBenefit = "0.00";
                    policyTemplateModel.GeoGraphicalBenefits = "0.00";
                    policyTemplateModel.DeathInjuryMedicPrice = "0.00";
                    policyTemplateModel.DeathInjuryMedicEn = "Not Included";
                    policyTemplateModel.DeathInjuryMedic = "غير مشمولة";
                    policyTemplateModel.DeathInjuryMedicPrice = "0.00";
                }
                else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 26) // Amana
                {
                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = "غير مشمولة";
                    policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Not Covered";
                    policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                    policyTemplateModel.PACoverForDriverAndPassenger = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverAndPassengerEn = "Not Covered";
                    policyTemplateModel.HireCarBenefit = "غير مشمولة";
                    policyTemplateModel.HireCarBenefitEn = "Not Covered";
                    policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                    policyTemplateModel.GCCCoverBenefitEn = "Not Covered";
                    policyTemplateModel.GeographicalCoverageAr = "غير مشمولة";
                    policyTemplateModel.GeographicalCoverageEn = "Not Covered";
                    policyTemplateModel.UnNamedDriver = "غير مشمولة";
                    policyTemplateModel.UnNamedDriverEn = "Not Covered";
                    policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "غير مشمولة";
                    policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Not Covered";
                    policyTemplateModel.PassengerIncluded = "غير مشمولة";
                    policyTemplateModel.PassengerIncludedEn = "Not Covered";
                    policyTemplateModel.DriverRelatedToInsured_Above21Ar = "غير مشمولة";
                    policyTemplateModel.DriverRelatedToInsured_Above21En = "Not Covered";
                    policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                    policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Covered";
                    policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                    policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                    policyTemplateModel.HireLimit2000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                    policyTemplateModel.HireLimit3000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                    policyTemplateModel.CarReplacement_100_10DaysAr = "غير مشمولة";
                    policyTemplateModel.CarReplacement_100_10DaysEn = "Not Covered";
                    policyTemplateModel.CarReplacement_200_10DaysAr = "غير مشمولة";
                    policyTemplateModel.CarReplacement_200_10DaysEn = "Not Covered";
                    policyTemplateModel.CarReplacement_300_10DaysAr = "غير مشمولة";
                    policyTemplateModel.CarReplacement_300_10DaysEn = "Not Covered";
                    policyTemplateModel.Benefit_57_Ar = "غير مشمولة";
                    policyTemplateModel.Benefit_57_En = "Not Covered";
                    policyTemplateModel.Benefit_58_Ar = "غير مشمولة";
                    policyTemplateModel.Benefit_58_En = "Not Covered";
                }
                else
                {
                    policyTemplateModel.PACoverForDriverOnly = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.HireCarBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.GCCCoverBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                    policyTemplateModel.UnNamedDriver = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Not Included" : "غير مشمولة";
                }

                if (policyFileInfo.Invoice.TotalPrice.HasValue && policyFileInfo.Invoice.Vat.HasValue)
                {
                    policyTemplateModel.VatableAmount = Math.Round(policyFileInfo.Invoice.TotalPrice.Value - policyFileInfo.Invoice.Vat.Value, 2).ToString();
                }
                //handel Agency Repair
                if (policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.HasValue && policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.Value == true)
                {
                    policyTemplateModel.VehicleAgencyRepairEn = "Agency Repair";

                    policyTemplateModel.VehicleAgencyRepairBenfitEn = "Included";
                    policyTemplateModel.VehicleAgencyRepairValue = "1500 SR";
                    policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                    policyTemplateModel.VehicleAgencyRepairAr = "لدى الوكالة";
                    policyTemplateModel.VehicleAgencyRepairValue = selectedLanguage == LanguageTwoLetterIsoCode.En ? "1500 SR" : "1500 ريال";
                }
                else
                {
                    policyTemplateModel.VehicleAgencyRepairBenfitEn = "Not Included";
                    policyTemplateModel.VehicleAgencyRepairEn = "Workshop repair approved by the Company";
                    policyTemplateModel.VehicleAgencyRepairValue = "750 SR";
                    policyTemplateModel.VehicleAgencyRepairBenfit = "غير مشمولة";
                    policyTemplateModel.VehicleAgencyRepairAr = "لدى الورش المعتمدة من قبل الشركة";
                    policyTemplateModel.VehicleAgencyRepairValue = selectedLanguage == LanguageTwoLetterIsoCode.En ? "750 SR" : "750 ريال";

                }
                List<string> _selectedBenefits = new List<string>();
                List<string> _geoGraphicalBenefits = new List<string>();
                var benefitList = new List<BenfitSummaryModel>();
                var PAacidantList = new List<BenfitSummaryModel>();
                var CarEndorsment120 = new List<BenfitSummaryModel>();
                var CarEndorsment150 = new List<BenfitSummaryModel>();
                var CarEndorsment180 = new List<BenfitSummaryModel>();
                var RoadAssistanttList = new List<BenfitSummaryModel>();
                var AccidentOutsideTerritory = new List<BenfitSummaryModel>();
                var DeathPhysicalInjuriesList = new List<BenfitSummaryModel>();
                var DeathPhysicalInjuriesListForPassengers = new List<BenfitSummaryModel>();
                var CarEndorsmentList = new List<BenfitSummaryModel>();

                decimal totalBenefitPrice = 0;
                if (policyFileInfo.OrderItemBenefit != null && policyFileInfo.OrderItemBenefit.Count() > 0)
                {
                    totalBenfitPrice = policyFileInfo.OrderItemBenefit.Select(x => x.Price).Sum();
                    policyTemplateModel.BenfitPrice = Math.Round(totalBenfitPrice, 2).ToString();
                    policyTemplateModel.PolicyAdditionalCoversAr = new List<string>();
                    policyTemplateModel.PolicyAdditionalCoversEn = new List<string>();
                    foreach (var benefit in policyFileInfo.OrderItemBenefit)
                    {
                        BenfitSummaryModel benefitObject = new BenfitSummaryModel();

                        policyTemplateModel.SelectedBenfits += benefit.BenefitExternalId + ",";
                        policyTemplateModel.PolicyAdditionalCoversAr.Add(benefit.ArabicDescription);
                        policyTemplateModel.PolicyAdditionalCoversEn.Add(benefit.EnglishDescription);

                        if (benefit.BenefitExternalId =="7" || benefit.BenefitId ==7 || benefit.Code==7 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId != 8)
                        {
                                policyTemplateModel.VehicleAgencyRepairEn = "Agency Repair";
                                policyTemplateModel.VehicleAgencyRepairBenfitEn = "Included";
                                policyTemplateModel.VehicleAgencyRepairValue = benefit.Price.ToString();
                                policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                                policyTemplateModel.VehicleAgencyRepairAr = "لدى الوكالة";
                        }
                        else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 6) // Saqr
                        {
                            if (benefit.Code == 5)
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "المساعدة على الطريق" : "Roadside Assistance";
                            }
                            else
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = string.Empty;
                            }
                        }
                        else if (benefit.Code == 1 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 10)
                        {
                            policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                            policyTemplateModel.PACoverForDriverOnlyEn = "Included";
                            continue;
                        }
                        else if (benefit.Code == 2 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 10)
                        {
                            policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                            policyTemplateModel.PACoverForDriverAndPassengerEn = "Included";
                        }
                        else if (benefit.Code == 2 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11)
                        {
                            policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                            policyTemplateModel.PACoverForDriverAndPassengerEn = "Included";
                        }
                        else if (policyFileInfo.CheckoutDetailsInfo.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                        {
                            if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 12)
                            {
                                if (benefit.BenefitExternalId == "2")
                                    _selectedBenefits.Add("1");
                                else if (benefit.BenefitExternalId == "6")
                                    _selectedBenefits.Add("3");
                                else if (benefit.BenefitExternalId == "10")
                                    _selectedBenefits.Add("2");
                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join(",", _selectedBenefits);
                            }
                            else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 14)
                            {
                                if (benefit.BenefitExternalId == "MNHS")
                                    _selectedBenefits.Add("Natural Hazards");
                                else if (benefit.BenefitExternalId == "MME")
                                    _selectedBenefits.Add("Medical Expenses");
                                else if (benefit.BenefitExternalId == "MTB")
                                    _selectedBenefits.Add("Towing Benefit");

                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join("\n", _selectedBenefits);
                            }
                            else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 23) //Tokio
                            {
                                _selectedBenefits.Add(benefit.ArabicDescription);

                                if (benefit.BenefitId == 9 || benefit.BenefitId == 10 || benefit.BenefitId == 11) // Geo-graphical benefits
                                {
                                    _geoGraphicalBenefits.Add(benefit.ArabicDescription);
                                }

                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join(", ", _selectedBenefits);
                                policyTemplateModel.GeoGraphicalBenefits = string.Join(", ", _geoGraphicalBenefits);
                            }
                        }
                        else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 18) // Alamia
                        {
                            if (benefit.Code == 5)
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "نعم" : "Yes";
                            }
                            else
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "لا" : "No";
                            }
                        }
                        else if (benefit.BenefitExternalId == "4" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        if (benefit.BenefitExternalId == "8" && policyFileInfo.OrderItem.ProductNameEn == "Wafi Smart" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.GCCCoverBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "9" && policyFileInfo.OrderItem.ProductNameEn == "Wafi Comprehensive" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.GCCCoverBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "3" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.PersonalAccidentBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "6" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.NaturalHazardsBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "5" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.WindscreenFireTheftBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "10" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.HandbagCoverReimbursementBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "11" && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.ChildSeatCoverReimbursementBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitId == 1 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)//saico
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "الحوادث الشخصية للسائق";
                            else
                                policyTemplateModel.BenefitsList += " - " + "Personal accidents of the driver";
                        }
                        else if (benefit.BenefitId == 3 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "الاخطار الطبيعية";
                            else
                                policyTemplateModel.BenefitsList += " - " + "NATURAL PERIL";
                        }
                        else if (benefit.BenefitId == 4 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "سرقة وحرائق";
                            else
                                policyTemplateModel.BenefitsList += " - " + "THIFT AND FIRE";
                        }
                        else if (benefit.BenefitId == 7 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "اصلاح وكالة";
                            else
                                policyTemplateModel.BenefitsList += " - " + "AGENCY REPAIR";
                        }
                        else if (benefit.BenefitId == 8 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "الحوادث الشخصية للركاب";
                            else
                                policyTemplateModel.BenefitsList += " - " + "Personal accidents of passengers";
                        }
                        else if (benefit.BenefitId == 14 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "تغطية ضد الغير";
                            else
                                policyTemplateModel.BenefitsList += " - " + "Third Party coverage";
                        }
                        else if (benefit.BenefitId == 16 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "تغطية شامل";
                            else
                                policyTemplateModel.BenefitsList += " - " + "Comprehensive Coverage";
                        }
                        else if (benefit.BenefitId == 17 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "ضرر كامل";
                            else
                                policyTemplateModel.VehicleAgencyRepairBenfitEn += " - " + "TOTAL LOSS";
                        }
                        else if (benefit.BenefitId == 18 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "ضرر جزئي";
                            else
                                policyTemplateModel.BenefitsList += " - " + "PARTIAL LOSS";
                        }
                        else if (benefit.BenefitId == 19 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 21)
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.BenefitsList += " - " + "مصاريف طبية";
                            else
                                policyTemplateModel.BenefitsList += " - " + "MEDICAL EXPINCES";
                        }
                        else if (benefit.Code == 6 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 23)// Tokio Marine
                        {
                            policyTemplateModel.AlternativeCarAr = "سيارة بديلة";
                            policyTemplateModel.AlternativeCarEn = "Hire Car";
                        }
                        else if (benefit.Code == 0 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 23)
                        {
                            if (benefit.BenefitExternalId == "2101")
                            {
                                policyTemplateModel.GCC_1MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(1 شهر)";
                                policyTemplateModel.GCC_1MonthEn = "Geographical Extension-GCC (1Month)";
                            }
                            else if (benefit.BenefitExternalId == "2102")
                            {
                                policyTemplateModel.GCC_3MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(3 أشهر)";
                                policyTemplateModel.GCC_3MonthEn = "Geographical Extension-GCC (3Month)";
                            }
                            else if (benefit.BenefitExternalId == "2103")
                            {
                                policyTemplateModel.GCC_6MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(6 أشهر)";
                                policyTemplateModel.GCC_6MonthEn = "Geographical Extension-GCC (6Month)";
                            }
                            else if (benefit.BenefitExternalId == "2104")
                            {
                                policyTemplateModel.NONGCC_1MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (1 شهر)";
                                policyTemplateModel.NONGCC_1MonthEn = "Geographical Extension-Non GCC (1Month)";
                            }
                            else if (benefit.BenefitExternalId == "2105")
                            {
                                policyTemplateModel.NONGCC_3MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (3 أشهر)";
                                policyTemplateModel.NONGCC_3MonthEn = "Geographical Extension-Non GCC (3Month)";
                            }
                            else if (benefit.BenefitExternalId == "2106")
                            {
                                policyTemplateModel.NONGCC_6MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (6 أشهر)";
                                policyTemplateModel.NONGCC_6MonthEn = "Geographical Extension-Non GCC (6Month)";
                            }
                            else if (benefit.BenefitExternalId == "2096")
                            {
                                policyTemplateModel.NONDEPRECIATIONCOVERAr = "عدم تطبيق شرط نسبة الاستهلاك في الخسارة الكلية";
                                policyTemplateModel.NONDEPRECIATIONCOVEREn = "NON DEPRECIATION COVER";
                            }
                        }
                        else if (benefit.BenefitId == 2 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11)
                        {
                            policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                            policyTemplateModel.PACoverForDriverAndPassengerEn = "Covered";
                        }
                        else if ((benefit.Code == 1 || benefit.Code == 2))
                        {
                            policyTemplateModel.DriverCovered = true;
                            _pACoverForDriverOnly = benefit.Price;
                            policyTemplateModel.PACoverForDriverOnly = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.Code == 2)
                        {
                            policyTemplateModel.PassengerCovered = true;
                            policyTemplateModel.PACoverForDriverAndPassenger = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.Code == 8)
                        {
                            _passengerIncluded = benefit.Price;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                policyTemplateModel.PassengerIncluded = (_passengerIncluded > 0) ? _passengerIncluded.ToString() : "مشمولة";
                            else
                                policyTemplateModel.PassengerIncluded = (_passengerIncluded > 0) ? _passengerIncluded.ToString() : "Included";
                        }
                        else if (benefit.Code == 0 && benefit.BenefitExternalId == "22")
                        {
                            policyTemplateModel.CoverforDriverBelow21Years = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.BenefitExternalId == "6")
                        {
                            policyTemplateModel.HireCarBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.Code == 0 && benefit.BenefitExternalId == "19")
                        {
                            policyTemplateModel.GCCCoverBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        else if (benefit.Code == 5)
                        {
                            policyTemplateModel.HasRoadsideAssistanceBenefit = true;
                            policyTemplateModel.RoadsideAssistanceBenefit = selectedLanguage == LanguageTwoLetterIsoCode.En ? "Included" : "مشمولة";
                        }
                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 4 && (benefit.Code == 8 || benefit.BenefitId == 8 || benefit.BenefitExternalId == "8"))                        {                            policyTemplateModel.PACoverForDriverAndPassenger = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";                        }                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 25 && (benefit.Code == 8 || benefit.BenefitId == 8 || benefit.BenefitExternalId == "8"))                        {                            policyTemplateModel.PACoverForDriverOnly = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";                        }
                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 13)
                        {
                            if (benefit.BenefitExternalId == "6" || benefit.BenefitId == 6 || benefit.Code == 6)
                            {
                                policyTemplateModel.AuthorizedRepairLimitPrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "7" || benefit.BenefitId == 7 || benefit.Code == 7)
                            {
                                policyTemplateModel.PersonalBelongingsPrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "4" || benefit.BenefitId == 4 || benefit.Code == 4)
                            {
                                policyTemplateModel.TowingWithoutAccidentPrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "8" || benefit.BenefitId == 8 || benefit.Code == 8)
                            {
                                policyTemplateModel.ReplacementOfKeysPrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "23" || benefit.BenefitId == 23 || benefit.Code == 23)
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "1" || benefit.BenefitId == 1 || benefit.Code == 1)
                            {
                                policyTemplateModel.PACoverForDriverOnly = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "2" || benefit.BenefitId == 2 || benefit.Code == 2)
                            {
                                policyTemplateModel.PassengerIncluded = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "108" || benefit.BenefitId == 108 || benefit.Code == 108)
                            {
                                policyTemplateModel.GeographicalCoverageAr = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "9" || benefit.BenefitId == 9 || benefit.Code == 9)
                            {
                                policyTemplateModel.NonApplicationOfDepreciationInCaseOfTotalLossPrice = benefit.Price.ToString("#.##");
                                policyTemplateModel.TotalYearlyDepText = "";
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "107" || benefit.BenefitId == 107 || benefit.Code == 107)
                            {
                                policyTemplateModel.NaturalHazardsBenefit = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "110" || benefit.BenefitId == 110 || benefit.Code == 110)
                            {
                                policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "5" || benefit.BenefitId == 5 || benefit.Code == 5)
                            {
                                policyTemplateModel.TowingWithAccidentPrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "109" || benefit.BenefitId == 109 || benefit.Code == 109)
                            {
                                policyTemplateModel.TheftFirePrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "135" || benefit.BenefitId == 6 || benefit.Code == 6)
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "137" || benefit.BenefitId == 37 || benefit.Code == 37)
                            {
                                policyTemplateModel.DeathInjuryMedic = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "138" || benefit.BenefitId == 39 || benefit.Code == 39)
                            {
                                policyTemplateModel.GCCCoverBenefit = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "139" || benefit.BenefitId == 30 || benefit.Code == 30)
                            {
                                policyTemplateModel.UnNamedDriver = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                            else if (benefit.BenefitExternalId == "136" || benefit.BenefitId == 5 || benefit.Code == 5)
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = benefit.Price.ToString("#.##");
                                totalBenefitPrice += benefit.Price;
                            }
                        }
                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 8)
                        {
                            if (benefit.BenefitId == 1 || benefit.BenefitExternalId == "2")
                            {
                                policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                policyTemplateModel.PACoverForDriverOnlyEn = "Included";
                                continue;
                            }
                            if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "6")
                            {
                                policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                policyTemplateModel.GCCCoverBenefitEn = "Included";
                                continue;
                            }
                            if (benefit.BenefitId == 5 || benefit.BenefitExternalId == "12")
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = " مشمولة";
                                policyTemplateModel.RoadsideAssistanceBenefitEn = "Included";
                                continue;
                            }
                            if (benefit.BenefitId == 6 || benefit.BenefitExternalId == "1")
                            {
                                policyTemplateModel.HireCarBenefit = " مشمولة";
                                policyTemplateModel.HireCarBenefitEn = "Included";
                                continue;
                            }
                            if (benefit.BenefitId == 30)
                            {
                                policyTemplateModel.UnNamedDriver = "مشمولة";
                                policyTemplateModel.UnNamedDriverEn = "Included";
                                continue;
                            }
                            if (benefit.BenefitId == 37)
                            {
                                policyTemplateModel.DeathInjuryMedic = "مشمولة";
                                policyTemplateModel.DeathInjuryMedicEn = "Included";
                                continue;
                            }
                        }
                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 5)
                        {
                            policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                            policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                            policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Included";
                            policyTemplateModel.CarReplacement_150 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                            policyTemplateModel.CarReplacement_200 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                            policyTemplateModel.CarReplacement_75 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";

                            if (benefit.BenefitId == 1)
                            {
                                //policyTemplateModel.PersonalAccidentBenefit = "مشمولة";
                                //policyTemplateModel.PersonalAccidentBenefitEn = "Covered";
                                policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                            }
                            else if (benefit.BenefitId == 9)
                            {
                                policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                            }
                            else if (benefit.BenefitId == 10)
                            {
                                policyTemplateModel.GCCCoverBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                            }

                            if (benefit.BenefitExternalId == "CARREP-150")
                            {
                                policyTemplateModel.CarReplacement_150 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                            }
                            else if (benefit.BenefitExternalId == "CARREP-200")
                            {
                                policyTemplateModel.CarReplacement_200 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                            }
                            else if (benefit.BenefitExternalId == "CARREP-75")
                            {
                                policyTemplateModel.CarReplacement_75 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                            }
                        }

                        if (benefit.BenefitId == 28)
                        {
                            policyTemplateModel.DriverRelatedToInsured_Above21Ar = "مشمولة";
                            policyTemplateModel.DriverRelatedToInsured_Above21En = "Included";
                        }
                        if (benefit.BenefitId == 29)
                        {
                            policyTemplateModel.HireCar_1500_Max15DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_1500_Max15DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 30)
                        {
                            policyTemplateModel.DriverRelatedToInsured_FamilyCoverAr = "مشمولة";
                            policyTemplateModel.DriverRelatedToInsured_FamilyCoverEn = "Included";
                        }
                        if (benefit.BenefitId == 31)
                        {
                            policyTemplateModel.HireCar_120_Max15DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_120_Max15DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 32)
                        {
                            policyTemplateModel.HireCar_150_Max15DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_150_Max15DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 33)
                        {
                            policyTemplateModel.HireCar_180_Max15DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_180_Max15DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 34)
                        {
                            policyTemplateModel.HireCar_2000_Max20DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_2000_Max20DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 35)
                        {
                            policyTemplateModel.HireCar_4000_Max20DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_4000_Max20DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 36)
                        {
                            policyTemplateModel.HireCar_5000_Max20DaysAr = "مشمولة";
                            policyTemplateModel.HireCar_5000_Max20DaysEn = "Included";
                        }
                        if (benefit.BenefitId == 37)
                        {
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "مشمولة";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Included";
                        }
                        if (benefit.BenefitId == 38)
                        {
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersAr = "مشمولة";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersEn = "Included";
                        }
                        if (benefit.BenefitId == 39)
                        {
                            policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAAr = "مشمولة";
                            policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAEn = "Included";
                        }

                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 27)
                        {
                            if (benefit.BenefitId == 24001)
                            {
                                policyTemplateModel.GE_Jordan_Lebanon_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.BenefitId == 24002)
                            {
                                policyTemplateModel.GCC_12Month = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.BenefitId == 24003)
                            {
                                policyTemplateModel.GE_Egypt_Sudan_Turky_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.BenefitId == 24004)
                            {
                                policyTemplateModel.GCC_Jordan_Lebanon_Egypt_Sudan_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }

                            if (benefit.BenefitId == 34 || benefit.BenefitExternalId == "34")
                            {
                                benefitObject.BenefitDescriptionAr = "استئجار سيارة بحد أقصى 2,000 ريال سعودي لكل مركبة، بحد اقصى 100 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يوم";
                                benefitObject.BenefitDescriptionEn = "Hire car facility Up to Maximum limit of SR.2,000/- per vehicle at the rate of SR.100 per day, Maximum 20 days";
                            }
                            else if (benefit.BenefitId == 35 || benefit.BenefitExternalId == "35")
                            {
                                benefitObject.BenefitDescriptionAr = " استئجار سيارة بحد أقصى 4,000 ريال سعودي لكل مركبة، بحد اقصى 200 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يو";
                                benefitObject.BenefitDescriptionEn = " Hire car facility Up to Maximum limit of SR.4,000 / -per vehicle at the rate of SR.200 per day, Maximum 20 days";
                            }
                            else if (benefit.BenefitId == 36 || benefit.BenefitExternalId == "36")
                            {
                                benefitObject.BenefitDescriptionAr = "استئجار سيارة بحد أقصى 5,000 ريال سعودي لكل مركبة، بحد اقصى 250 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يوم";
                                benefitObject.BenefitDescriptionEn = "Hire car facility Up to Maximum limit of SR.5,000/- per vehicle at the rate of SR.250 per day, Maximum 20 days";
                            }

                            if (benefitObject != null && (!string.IsNullOrEmpty(benefitObject.BenefitDescriptionAr) || !string.IsNullOrEmpty(benefitObject.BenefitDescriptionEn)))
                                CarEndorsmentList.Add(benefitObject);

                            if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "10")
                                AccidentOutsideTerritory.Add(new BenfitSummaryModel());

                            if (benefit.BenefitId == 37 || benefit.BenefitExternalId == "37")
                                DeathPhysicalInjuriesList.Add(new BenfitSummaryModel());

                            if (benefit.BenefitId == 38 || benefit.BenefitExternalId == "38")
                                DeathPhysicalInjuriesListForPassengers.Add(new BenfitSummaryModel());

                            if (RoadAssistanttList.Count < 1 && (benefit.BenefitId == 34 || benefit.BenefitId == 35 || benefit.BenefitId == 36))
                                RoadAssistanttList.Add(new BenfitSummaryModel());
                        }

                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 14)
                        {
                            policyTemplateModel.SubTotal = HandlePolicySubTotal(policyFileInfo.PriceDetail, policyTemplateModel, totalBenfitPrice);

                            if (benefit.BenefitId == 117 || benefit.BenefitExternalId == "117")
                            {
                                policyTemplateModel.PassengerIncluded = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 118 || benefit.BenefitExternalId == "118")
                            {
                                policyTemplateModel.PACoverForDriverOnly = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 119 || benefit.BenefitExternalId == "119")
                            {
                                policyTemplateModel.GCCCoverBenefit = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 120 || benefit.BenefitExternalId == "120")
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 121 || benefit.BenefitExternalId == "121")
                            {
                                policyTemplateModel.Towing = Math.Round(benefit.Price, 2).ToString();
                            }
                            //else if (benefit.BenefitId == 122 || benefit.BenefitExternalId == "122")
                            //{
                            //    policyTemplateModel.EmergencyMedical = benefit.Price.ToString();
                            //}
                            else if (benefit.BenefitId == 259 || benefit.BenefitExternalId == "259")
                            {
                                policyTemplateModel.LiabilitiesToThirdPartiesPrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 260 || benefit.BenefitExternalId == "260")
                            {
                                policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 261 || benefit.BenefitExternalId == "261")
                            {
                                policyTemplateModel.NaturalDisasters = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 262 || benefit.BenefitExternalId == "262")
                            {
                                policyTemplateModel.TheftFirePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 265 || benefit.BenefitExternalId == "265")
                            {
                                policyTemplateModel.TowingWithAccidentPrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 266 || benefit.BenefitExternalId == "266")
                            {
                                policyTemplateModel.EmergencyMedical = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 489 || benefit.BenefitExternalId == "489")
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 490 || benefit.BenefitExternalId == "490")
                            {
                                policyTemplateModel.UnNamedDriver = Math.Round(benefit.Price, 2).ToString();
                            }
                        }

                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                        {
                            if (benefit.BenefitExternalId == "1")
                            {
                                benefitObject.BenefitDescriptionAr = "المسؤولية المدنية تجاه الغير بحد أقصى 10,000,000 ريال في كل حالة";
                                benefitObject.BenefitDescriptionEn = "Third Party Liability Up to SR 10 min per one occurrence";
                            }
                            else if (benefit.BenefitExternalId == "2")
                            {
                                benefitObject.BenefitDescriptionAr = "إعفاء من نسبة استهلاك قطع الغيار";
                                benefitObject.BenefitDescriptionEn = "Zero depriciation spare parts";
                            }
                            else if (benefit.BenefitExternalId == "3")
                            {
                                benefitObject.BenefitDescriptionAr = "تكاليف حالات الطوارئ الطبية";
                                benefitObject.BenefitDescriptionEn = "Emergency Medical expenses";
                            }
                            if (benefit.BenefitExternalId == "4" && (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.HasValue && policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.Value == 2))
                            {
                                benefitObject.BenefitDescriptionAr = "التعویض بحسب قیمة المركبة السوقیة على أن لا تتعدى القیمة التامینة";
                                benefitObject.BenefitDescriptionEn = "Market value not exceeding declared Sum Insured";
                            }
                            else if (benefit.BenefitExternalId == "5" && (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.HasValue && policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.Value == 2))
                            {
                                benefitObject.BenefitDescriptionAr = "تغطية الخسارة الكلية أو الجزئية للمركبة";
                                benefitObject.BenefitDescriptionEn = "Coverage for total or partial loss of Vehicle";
                            }
                            else if (benefit.BenefitExternalId == "6")
                            {
                                benefitObject.BenefitDescriptionAr = "الكوارث الطبيعية, الأمطار والبرد , والعواصف";
                                benefitObject.BenefitDescriptionEn = "Natural Hazards rain, hail & flood";
                            }
                            else if (benefit.BenefitExternalId == "7")
                            {
                                benefitObject.BenefitDescriptionAr = "تغطية الحوادث الشخصية للسائق و الركاب";
                                benefitObject.BenefitDescriptionEn = "Personal Accident (Driver and Passengers)";
                            }
                            else if (benefit.BenefitExternalId == "8" && (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.HasValue && policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.Value == 2))
                            {
                                benefitObject.BenefitDescriptionAr = "المساعدة على الطريق";
                                benefitObject.BenefitDescriptionEn = "Road Side Assistance";
                            }
                            else if (benefit.BenefitExternalId == "9")
                            {
                                benefitObject.BenefitDescriptionAr = "التغطية سارية في دول الخليج";
                                benefitObject.BenefitDescriptionEn = "Valid in GCC Countries";
                            }
                            else if (benefit.BenefitExternalId == "10")
                            {
                                benefitObject.BenefitDescriptionAr = "استأجر سيارة";
                                benefitObject.BenefitDescriptionEn = "Hire car";
                            }
                            else if (benefit.BenefitExternalId == "11")
                            {
                                benefitObject.BenefitDescriptionAr = "تغطية السائق ذو صلة قرابة بالمؤمن له ( الأب, الأم, الزوج, الزوجة, الأبن, الابنة, الأخ, الأخت) أو السائق الذي يكون تحت كفالة المؤمن له, أو يعمل لدى المؤمن له بموجب عقد عمل";
                                benefitObject.BenefitDescriptionEn = "Family cover - the driver related to the insured, such as parents, spouse, sons, daughters, brother and sister or the insured's domestic worker or someone who work for the insured based on a labor law";
                            }

                            if (!string.IsNullOrEmpty(benefitObject.BenefitDescriptionAr) || !string.IsNullOrEmpty(benefitObject.BenefitDescriptionEn))
                            {
                                benefitList.Add(benefitObject);
                            }
                        }

                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 26)
                        {
                            if (benefit.BenefitId == 1)
                            {
                                policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                            }
                            else if (benefit.BenefitId == 2 || benefit.BenefitExternalId == "2")
                            {
                                policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                policyTemplateModel.PACoverForDriverAndPassengerEn = "Covered";
                            }
                            else if (benefit.BenefitId == 5 || benefit.BenefitExternalId == "5")
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                policyTemplateModel.RoadsideAssistanceBenefitEn = "Covered";

                            }
                            else if (benefit.BenefitId == 6 || benefit.BenefitExternalId == "6")
                            {
                                policyTemplateModel.HireCarBenefit = "مشمولة";
                                policyTemplateModel.HireCarBenefitEn = "Covered";
                            }
                            else if (benefit.BenefitId == 8 || benefit.BenefitExternalId == "8")
                            {
                                policyTemplateModel.PassengerIncluded = "مشمولة";
                                policyTemplateModel.PassengerIncludedEn = "Covered";
                            }
                            else if (benefit.BenefitId == 9 || benefit.BenefitExternalId == "9")
                            {
                                policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                            }
                            else if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "10")
                            {
                                policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                policyTemplateModel.GCCCoverBenefitEn = "Covered";
                            }
                            else if (benefit.BenefitId == 11 || benefit.BenefitExternalId == "11")
                            {
                                policyTemplateModel.GeographicalCoverageAr = "مشمولة";
                                policyTemplateModel.GeographicalCoverageEn = "Covered";
                            }
                            else if (benefit.BenefitId == 28 || benefit.BenefitExternalId == "28")
                            {
                                policyTemplateModel.DriverRelatedToInsured_Above21Ar = "مشمولة";
                                policyTemplateModel.DriverRelatedToInsured_Above21En = "Covered";

                            }
                            else if (benefit.BenefitId == 30 || benefit.BenefitExternalId == "30")
                            {
                                policyTemplateModel.UnNamedDriver = "مشمولة";
                                policyTemplateModel.UnNamedDriverEn = "Covered";
                            }
                            else if (benefit.BenefitId == 37 || benefit.BenefitExternalId == "37")
                            {
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "مشمولة";
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Covered";
                            }
                            else if (benefit.BenefitId == 45 || benefit.BenefitExternalId == "45")
                            {
                                policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                            }
                            else if (benefit.BenefitId == 54 || benefit.BenefitExternalId == "54")
                            {
                                policyTemplateModel.CarReplacement_100_10DaysAr = "مشمولة";
                                policyTemplateModel.CarReplacement_100_10DaysEn = "Covered";

                            }
                            else if (benefit.BenefitId == 55 || benefit.BenefitExternalId == "55")
                            {
                                policyTemplateModel.CarReplacement_200_10DaysAr = "مشمولة";
                                policyTemplateModel.CarReplacement_200_10DaysEn = "Covered";
                            }
                            else if (benefit.BenefitId == 56 || benefit.BenefitExternalId == "56")
                            {
                                policyTemplateModel.CarReplacement_300_10DaysAr = "مشمولة";
                                policyTemplateModel.CarReplacement_300_10DaysEn = "Covered";
                            }
                            else if (benefit.BenefitId == 57 || benefit.BenefitExternalId == "57")
                            {
                                policyTemplateModel.Benefit_57_Ar = "مشمولة";
                                policyTemplateModel.Benefit_57_En = "Covered";
                            }
                            else if (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58")
                            {
                                policyTemplateModel.Benefit_58_Ar = "مشمولة";
                                policyTemplateModel.Benefit_58_En = "Covered";
                            }
                            else if (benefit.BenefitId == 6200)
                            {
                                policyTemplateModel.HireLimit2000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                            }
                            else if (benefit.BenefitId == 6300)
                            {
                                policyTemplateModel.HireLimit3000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                            }

                            if ((benefit.BenefitId == 54 || benefit.BenefitExternalId == "54") || (benefit.BenefitId == 55 || benefit.BenefitExternalId == "55") || (benefit.BenefitId == 56 || benefit.BenefitExternalId == "56"))
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = "مشمولة";
                                policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Covered";
                            }
                            if ((benefit.BenefitId == 57 || benefit.BenefitExternalId == "57") || (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58"))
                            {
                                policyTemplateModel.GeographicalCoverageAr = "مشمولة";
                                policyTemplateModel.GeographicalCoverageEn = "Covered";
                            }
                        }

                        if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11 && policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.Value == 2)
                        {
                            if (benefit.BenefitId == 5)
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                policyTemplateModel.RoadsideAssistanceBenefitEn = "Covered";
                            }
                            else if (benefit.BenefitId == 23)
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = "مشمولة";
                                policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Covered";
                            }
                            else if (benefit.BenefitId == 6)
                            {
                                policyTemplateModel.HireCarBenefit = "مشمولة";
                                policyTemplateModel.HireCarBenefitEn = "Covered";
                            }
                            else if (benefit.BenefitId == 1)
                            {
                                policyTemplateModel.PersonalAccidentBenefit = "مشمولة";
                                policyTemplateModel.PersonalAccidentBenefitEn = "Covered";
                            }
                            else if (benefit.BenefitId == 30)
                            {
                                policyTemplateModel.UnNamedDriver = "مشمولة";
                                policyTemplateModel.UnNamedDriverEn = "Covered";
                            }
                            else if (benefit.BenefitId == 8)
                            {
                                policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                            }
                            else if (benefit.BenefitId == 10)
                            {
                                policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                policyTemplateModel.GCCCoverBenefitEn = "Covered";
                            }
                        }
                    }

                    if (benefitList != null && benefitList.Count >= 1)
                        policyTemplateModel.BenefitSummary = benefitList;

                    policyTemplateModel.CarEndorsmentList = CarEndorsmentList;
                    policyTemplateModel.AccidentOutsideTerritory = AccidentOutsideTerritory;
                    policyTemplateModel.DeathPhysicalInjuriesList = DeathPhysicalInjuriesList;
                    policyTemplateModel.DeathPhysicalInjuriesListForPassengers = DeathPhysicalInjuriesListForPassengers;
                    policyTemplateModel.RoadAssistanttList = RoadAssistanttList;
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 8|| policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 14)
                {
                    if (secondDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[0]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[0]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.AddtionalDriverOneOccupation = secondDriver?.AdditionalDriverOccupationName;
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        if (!string.IsNullOrEmpty(secondDriver.AdditionalDriverSocialStatusName))
                            policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver?.AdditionalDriverSocialStatusName;
                    }
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 13)
                {
                    policyTemplateModel.SumAdditionalBenefitPremiumtPrice = totalBenefitPrice.ToString("#.##");
                    policyTemplateModel.VehicleRegistrationType = policyFileInfo.QuotationRequestInfo.VehiclePlateTypeArabicDescription;
                    policyTemplateModel.VehicleRegistrationTypeEn = policyFileInfo.QuotationRequestInfo.VehiclePlateTypeEnglishDescription;

                    var socialStatusCode = policyTemplateModel.SocialStatusCode;
                    var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                    policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo("ar-SA"));
                    policyTemplateModel.SocialStatusEn = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo("en-US"));

                    //policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                    policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                    policyTemplateModel.MainDriverGenderEn = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo("en-US"));

                    if (secondDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[0]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[0]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                        policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver?.AdditionalDriverSocialStatusName; ; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                        policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.AdditionalDriverFullNameEn : secondDriver.AdditionalDriverFullNameAr;
                        policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.AdditionalDriverOccupationName;
                        if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        {
                            if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                            else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            else
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                        }
                    }
                    if (thirdDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[1]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[1]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverTwoLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                        policyTemplateModel.ThirdDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? thirdDriver.AdditionalDriverFullNameEn : thirdDriver.AdditionalDriverFullNameAr;
                        policyTemplateModel.AddtionalDriverTwoSocialStatus = thirdDriver?.AdditionalDriverSocialStatusName; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                        policyTemplateModel.AddtionalDriverOneOccupation = thirdDriver.AdditionalDriverOccupationName;
                        if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        {
                            if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                            else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            else
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                        }
                    }
                }

                decimal? totalPrice = policyFileInfo.Invoice.TotalPrice;
                decimal? vat = policyFileInfo.Invoice.Vat;

                decimal vatPrice = 0;
                decimal fees = 0;
                decimal extraPremiumPrice = 0;
                decimal discount1 = 0;
                decimal discount = 0;
                decimal bcareCommission = 0;
                decimal clalmLoadingAmount = 0;
                decimal additionAgeContribution = 0;
                decimal loyalDiscount = 0;
                decimal specialDiscount2 = 0;
                decimal vatPecentage = 0;
                decimal schemesDiscount = 0;
                decimal schemesDiscountPercentage = 0;

                foreach (var price in policyFileInfo.PriceDetail)
                {
                    switch (price.PriceTypeCode)
                    {
                        case 1: discount1 += price.PriceValue; break;
                        case 2: discount += price.PriceValue; break;
                        case 3: loyalDiscount += price.PriceValue; break;
                        case 4: clalmLoadingAmount += price.PriceValue; break;
                        case 5: additionAgeContribution += price.PriceValue; break;
                        case 6: fees += price.PriceValue; break;
                        case 7: extraPremiumPrice += price.PriceValue; break;
                        case 8: vatPrice += price.PriceValue; vatPecentage += price.PercentageValue.HasValue ? price.PercentageValue.Value : 0; break;
                        case 9: bcareCommission += price.PriceValue; break;
                        case 11: schemesDiscount += price.PriceValue; schemesDiscountPercentage += price.PercentageValue.HasValue ? price.PercentageValue.Value : 0; break;
                        case 12: specialDiscount2 += price.PriceValue; break;
                    }
                }
                var _benfitCode2 = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 2).FirstOrDefault();
                var _benfitCode3 = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 3).FirstOrDefault();
                policyTemplateModel.ClalmLoadingAmount = clalmLoadingAmount.ToString();
                policyTemplateModel.TotalPremium = totalPrice.HasValue ? totalPrice.Value.ToString("#.##") : "0.00";
                policyTemplateModel.VATAmount = vat.HasValue ? vat.Value.ToString("#.##") : "0.00";
                policyTemplateModel.VATPercentage = vatPecentage.ToString();
                policyTemplateModel.SpecialDiscount = discount1.ToString("#.##");
                policyTemplateModel.SpecialDiscountPercentage = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PercentageValue?.ToString();
                policyTemplateModel.SpecialDiscount2 = (specialDiscount2 > 0) ? specialDiscount2.ToString("#.##") : "0.00";
                policyTemplateModel.SpecialDiscount2Percentage = (!string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2Percentage)) ? policyTemplateModel.SpecialDiscount2Percentage : "0.00" ;
                   
                policyTemplateModel.NoClaimDiscount = (_benfitCode2 != null && _benfitCode2.PercentageValue > 0) ? _benfitCode2.PercentageValue.Value.ToString("#.##") : "0.00";
                policyTemplateModel.NCDAmount = discount > 0 ? discount.ToString("#.##") : "0.00";

                if (string.IsNullOrEmpty(policyTemplateModel.SchemesDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SchemesDiscount.Trim()))//Voucher Discount 
                {
                    policyTemplateModel.SchemesDiscount = schemesDiscount.ToString("#.##");
                    policyTemplateModel.SchemesDiscountPercentage = schemesDiscountPercentage.ToString();
                }

                policyTemplateModel.NCDPercentage = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString().Replace(".00", string.Empty).Trim();
                if (string.IsNullOrEmpty(policyTemplateModel.NCDPercentage))
                {
                    policyTemplateModel.NCDPercentage = "0";
                }
                policyTemplateModel.LoyaltyDiscount = loyalDiscount > 0 ? loyalDiscount.ToString("#.##") : "0.00";
                policyTemplateModel.LoyaltyDiscountPercentage = (_benfitCode3 != null && _benfitCode3.PercentageValue > 0) ? _benfitCode3.PercentageValue.ToString() : "0.00";

                policyTemplateModel.OfficePremium = extraPremiumPrice.ToString("#.##");
                if (!string.IsNullOrEmpty(policyTemplateModel.OfficePremium))
                {
                    var _vat = (vat.HasValue) ? vat.Value : 0;
                    decimal officePremium = 0;
                    if (decimal.TryParse(policyTemplateModel.OfficePremium, out officePremium))
                    {
                        decimal officePremiumWithoutVat = officePremium - _vat;
                        policyTemplateModel.OfficePremiumWithoutVAT = officePremiumWithoutVat.ToString("#.##");
                    }
                    else
                    {
                        policyTemplateModel.OfficePremiumWithoutVAT = "0.00";

                    }
                    if (!string.IsNullOrEmpty(policyTemplateModel.TotalPremium) && totalPrice.HasValue)
                    {
                        decimal totalPremiumWithoutVAT = totalPrice.Value - _vat;
                        policyTemplateModel.TotalPremiumWithoutVAT = totalPremiumWithoutVAT.ToString("#.##");
                    }
                    else
                    {
                        policyTemplateModel.TotalPremiumWithoutVAT = "0.00";

                    }
                }
                var specialDiscount = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue;
                var noClaimDiscount = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue;
                var loyaltyDiscount = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue;
                var additionaLoading = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 4).FirstOrDefault()?.PriceValue;
                var additionalAgeContribution = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 5).FirstOrDefault()?.PriceValue;
                var adminFees = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 6).FirstOrDefault()?.PriceValue;
                var basicPremium = policyFileInfo.PriceDetail?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue;

                var productPriceWithouVaT = basicPremium != null && basicPremium.HasValue ? basicPremium.Value : 0
                    + adminFees != null && adminFees.HasValue ? adminFees.Value : 0
                    + additionalAgeContribution != null && additionalAgeContribution.HasValue ? additionalAgeContribution.Value : 0
                    + additionaLoading != null && additionaLoading.HasValue ? additionaLoading.Value : 0
                    - loyaltyDiscount != null && loyaltyDiscount.HasValue ? loyaltyDiscount.Value : 0
                    - noClaimDiscount != null && noClaimDiscount.HasValue ? noClaimDiscount.Value : 0
                    - specialDiscount != null && specialDiscount.HasValue ? specialDiscount.Value : 0;

                decimal totalPremium = 0;
                decimal.TryParse(policyTemplateModel.TotalPremium, out totalPremium);

                policyTemplateModel.TPLCommissionAmount = (totalPremium * 2 / 100).ToString("#.##");
                policyTemplateModel.ComprehesiveCommissionAmount = (totalPremium * 15 / 100).ToString("#.##");
                if (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.HasValue)
                {
                    if (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode.Value == 1)
                        policyTemplateModel.CommissionPaid = policyTemplateModel.TPLCommissionAmount;
                    else
                        policyTemplateModel.CommissionPaid = policyTemplateModel.ComprehesiveCommissionAmount;

                }
                policyTemplateModel.TPLTotalSubscriptionPremium = ((totalPremium * 2 / 100) + totalPremium).ToString();
                policyTemplateModel.ComprehensiveTotalSubscriptionPremium = ((totalPremium * 15 / 100) + totalPremium).ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredCity) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredCity.Trim()))
                    policyTemplateModel.InsuredCity = mainDriverAddress?.City;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredDisctrict) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredDisctrict.Trim()))
                    policyTemplateModel.InsuredDisctrict = mainDriverAddress?.District;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredID) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredID.Trim()))
                    policyTemplateModel.InsuredID = policyFileInfo.QuotationRequestInfo.NationalId;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredMobile) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredMobile.Trim()))
                    policyTemplateModel.InsuredMobile = policyFileInfo.CheckoutDetailsInfo.Phone;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredIbanNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredIbanNumber.Trim()))
                    policyTemplateModel.InsuredIbanNumber = policyFileInfo.CheckoutDetailsInfo.IBAN;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredBankName) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredBankName.Trim()))
                    policyTemplateModel.InsuredBankName = policyFileInfo.CheckoutDetailsInfo.BankArabicDescription;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameAr) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameAr.Trim()))
                    policyTemplateModel.InsuredNameAr = policyFileInfo.QuotationRequestInfo.InsuredNameAr;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameEn) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameEn.Trim()))
                    policyTemplateModel.InsuredNameEn = policyFileInfo.QuotationRequestInfo.InsuredNameEn;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredStreet) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredStreet.Trim()))
                    policyTemplateModel.InsuredStreet = mainDriverAddress?.Street;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredZipCode) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredZipCode.Trim()))
                    policyTemplateModel.InsuredZipCode = mainDriverAddress?.PostCode;

                if (string.IsNullOrEmpty(policyTemplateModel.InsuredNationalAddress) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNationalAddress.Trim()))
                {
                    policyTemplateModel.InsuredNationalAddress = string.Empty;
                    policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredBuildingNo) ? policyTemplateModel.InsuredBuildingNo + " " : string.Empty;
                    policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredStreet) ? policyTemplateModel.InsuredStreet + " " : string.Empty;
                    policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredDisctrict) ? policyTemplateModel.InsuredDisctrict + " " : string.Empty;
                    policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredCity) ? policyTemplateModel.InsuredCity : string.Empty;
                    policyTemplateModel.InsuredNationalAddress.TrimEnd();
                }

                policyTemplateModel.VehicleBodyAr = policyFileInfo.QuotationRequestInfo.VehicleBodyTypeArabicDescription;
                policyTemplateModel.VehicleBodyEn = policyFileInfo.QuotationRequestInfo.VehicleBodyTypeEnglishDescription;
                policyTemplateModel.VehicleChassis = policyFileInfo.QuotationRequestInfo.ChassisNumber;
                policyTemplateModel.VehicleColorAr = policyFileInfo.QuotationRequestInfo.MajorColor;
                policyTemplateModel.VehicleColorEn = policyFileInfo.QuotationRequestInfo.MajorColor;
                policyTemplateModel.VehicleCustomNo = policyFileInfo.QuotationRequestInfo.CustomCardNumber;
                policyTemplateModel.VehicleMakeAr = policyFileInfo.QuotationRequestInfo.VehicleMaker;
                policyTemplateModel.VehicleMakeEn = policyFileInfo.QuotationRequestInfo.VehicleMaker;
                policyTemplateModel.VehicleModelAr = policyFileInfo.QuotationRequestInfo.VehicleModel;
                policyTemplateModel.VehicleModelEn = policyFileInfo.QuotationRequestInfo.VehicleModel;

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 4 && policyFileInfo.MakerModelDetails != null)
                {
                    policyTemplateModel.VehicleModelAr = policyFileInfo.MakerModelDetails.ModelAr;
                    policyTemplateModel.VehicleModelEn = policyFileInfo.MakerModelDetails.ModelEn;
                    policyTemplateModel.VehicleMakeAr = policyFileInfo.MakerModelDetails.MakerAr;
                    policyTemplateModel.VehicleMakeEn = policyFileInfo.MakerModelDetails.MakerEn;
                }


                policyTemplateModel.VehicleWeight = policyFileInfo.QuotationRequestInfo.VehicleWeight.ToString();
                policyTemplateModel.VehicleLoad = policyFileInfo.QuotationRequestInfo.VehicleLoad.ToString();
                //policyTemplateModel.VehicleUsingPurposesAr = policyFileInfo.QuotationRequestInfo.VehicleUse.ToString();
                //policyTemplateModel.VehicleUsingPurposesEn = policyFileInfo.QuotationRequestInfo.VehicleUse.ToString();
                var vehicleUseId = policyFileInfo.QuotationRequestInfo.VehicleUseId;
                var vehicleUseEnumKey = (vehicleUseId > 0) ? Enum.GetName(typeof(VehicleUse), vehicleUseId) : String.Empty;
                policyTemplateModel.VehicleUsingPurposesAr = (!string.IsNullOrEmpty(vehicleUseEnumKey)) ? VehicleUseResource.ResourceManager.GetString(vehicleUseEnumKey, CultureInfo.GetCultureInfo("ar-SA")) : "";
                policyTemplateModel.VehicleUsingPurposesEn = (!string.IsNullOrEmpty(vehicleUseEnumKey)) ? VehicleUseResource.ResourceManager.GetString(vehicleUseEnumKey, CultureInfo.GetCultureInfo("en-SA")) : "";

                policyTemplateModel.VehicleEngineSize = policyFileInfo.QuotationRequestInfo.EngineSize?.ToString();
                policyTemplateModel.VehicleOdometerReading = policyFileInfo.QuotationRequestInfo.CurrentMileageKM?.ToString();
                policyTemplateModel.VehicleModificationDetails = policyFileInfo.QuotationRequestInfo.ModificationDetails;
                policyTemplateModel.VehicleOwnerID = policyFileInfo.QuotationRequestInfo.CarOwnerNIN;
                policyTemplateModel.VehicleOwnerName = policyFileInfo.QuotationRequestInfo.CarOwnerName;
                policyTemplateModel.VehiclePlateNoAr = policyFileInfo.QuotationRequestInfo.CarPlateNumber?.ToString();
                policyTemplateModel.VehiclePlateNoEn = policyFileInfo.QuotationRequestInfo.CarPlateNumber?.ToString();
                if (!string.IsNullOrEmpty(policyFileInfo.QuotationRequestInfo.CarPlateText3))
                {
                    policyTemplateModel.VehiclePlateText = policyFileInfo.QuotationRequestInfo.CarPlateText3 + " " + policyFileInfo.QuotationRequestInfo.CarPlateText2 + " " + policyFileInfo.QuotationRequestInfo.CarPlateText1;
                }
                else
                {
                    policyTemplateModel.VehiclePlateText = policyFileInfo.QuotationRequestInfo.CarPlateText2 + " " + policyFileInfo.QuotationRequestInfo.CarPlateText1;
                }
                policyTemplateModel.VehiclePlateNoEn = policyFileInfo.QuotationRequestInfo.CarPlateNumber?.ToString();
                policyTemplateModel.VehiclePlateTypeAr = policyFileInfo.QuotationRequestInfo.VehiclePlateTypeArabicDescription;
                policyTemplateModel.VehiclePlateTypeEn = policyFileInfo.QuotationRequestInfo.VehiclePlateTypeEnglishDescription;
                policyTemplateModel.VehicleRegistrationExpiryDate = policyFileInfo.QuotationRequestInfo.LicenseExpiryDate;
                policyTemplateModel.VehicleSequenceNo = policyFileInfo.QuotationRequestInfo.SequenceNumber;
                policyTemplateModel.VehicleRegistrationType = policyFileInfo.QuotationRequestInfo.VehiclePlateTypeArabicDescription;
                policyTemplateModel.VehicleYear = policyFileInfo.QuotationRequestInfo.ModelYear?.ToString();

                ////
                /// as per Rawan https://bcare.atlassian.net/browse/VW-853
                policyTemplateModel.VehicleValue = policyFileInfo.QuotationRequestInfo.VehicleValue?.ToString();
                policyTemplateModel.VehicleLimitValue = (policyFileInfo.OrderItem.VehicleLimitValue.HasValue == true && policyFileInfo.OrderItem.VehicleLimitValue.Value > 0)
                                                            ? policyFileInfo.OrderItem.VehicleLimitValue.Value.ToString()
                                                            : policyFileInfo.QuotationRequestInfo.VehicleValue?.ToString();

                policyTemplateModel.HasTrailerAr = "لا";
                policyTemplateModel.HasTrailerEn = "No";
                if (policyFileInfo.QuotationRequestInfo.HasTrailer)
                {
                    policyTemplateModel.HasTrailerAr = "نعم";
                    policyTemplateModel.HasTrailerEn = "Yes";
                }

                policyTemplateModel.OtherUsesAr = "لا";
                policyTemplateModel.OtherUsesEn = "No";
                if (policyFileInfo.QuotationRequestInfo.OtherUses)
                {
                    policyTemplateModel.OtherUsesAr = "نعم";
                    policyTemplateModel.OtherUsesEn = "Yes";
                }

                policyTemplateModel.VehicleModificationAr = "لا";
                policyTemplateModel.VehicleModificationEn = "No";
                if (policyFileInfo.QuotationRequestInfo.HasModifications)
                {
                    policyTemplateModel.VehicleModificationAr = "نعم";
                    policyTemplateModel.VehicleModificationEn = "Yes";
                }

                if (policyFileInfo.OrderItem.DeductableValue.HasValue && policyFileInfo.OrderItem.DeductableValue.Value == 0 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 13)
                {
                    policyTemplateModel.DeductibleValue = "0"; 
                }
               else if (policyFileInfo.OrderItem.DeductableValue.HasValue && policyFileInfo.OrderItem.DeductableValue.Value > 0)
                {
                    policyTemplateModel.DeductibleValue = policyFileInfo.OrderItem.DeductableValue.Value.ToString();
                }
                else
                {
                    policyTemplateModel.DeductibleValue = policyFileInfo.QuotationRequestInfo.DeductibleValue?.ToString();
                }
                //if (policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.HasValue && policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.Value == true)
                //{
                //    policyTemplateModel.VehicleAgencyRepairEn = "Agency Repair";

                //    policyTemplateModel.VehicleAgencyRepairBenfitEn = "Included";
                //    policyTemplateModel.VehicleAgencyRepairValue = "1500 SR";
                //    policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                //    policyTemplateModel.VehicleAgencyRepairAr = "لدى الوكالة";
                //    policyTemplateModel.VehicleAgencyRepairValue = selectedLanguage == LanguageTwoLetterIsoCode.En ? "1500 SR" : "1500 ريال";
                //}
                //else
                //{
                //    policyTemplateModel.VehicleAgencyRepairBenfitEn = "Not Included";
                //    policyTemplateModel.VehicleAgencyRepairEn = "Workshop repair approved by the Company";
                //    policyTemplateModel.VehicleAgencyRepairValue = "750 SR";

                //    policyTemplateModel.VehicleAgencyRepairBenfit = "غير مشمولة";
                //    policyTemplateModel.VehicleAgencyRepairAr = "لدى الورش المعتمدة من قبل الشركة";
                //    policyTemplateModel.VehicleAgencyRepairValue = selectedLanguage == LanguageTwoLetterIsoCode.En ? "750 SR" : "750 ريال";

                //}
                if (policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.HasValue)
                {
                    policyTemplateModel.VehicleRepairMethod = policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.Value ? "Agency" : "Workshop";
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 8)
                {
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                    {
                        policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                        policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                    }

                    policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                    if (secondDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[0]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[0]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.AddtionalDriverOneOccupation = secondDriver?.AdditionalDriverOccupationName;
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        if (!string.IsNullOrEmpty(secondDriver.AdditionalDriverSocialStatusName))
                            policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver?.AdditionalDriverSocialStatusName;
                    }
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                {
                    if (policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.HasValue && policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.Value == true)
                        policyTemplateModel.VehicleAgencyRepairAr = "تغطية شامل اصلاح داخل و خارج الوكالة";
                    else
                        policyTemplateModel.VehicleAgencyRepairAr = "تغطية شامل اصلاح خارج الوكالة";

                    if (policyFileInfo.QuotationRequestInfo.NationalId.StartsWith("7"))
                        policyTemplateModel.OwnerAsDriver = policyFileInfo.QuotationRequestInfo.NationalId;
                    else if (policyFileInfo.QuotationRequestInfo.NationalId == policyFileInfo.QuotationRequestInfo.MainDriverNin)
                        policyTemplateModel.OwnerAsDriver = "YES";
                    else
                        policyTemplateModel.OwnerAsDriver = "NO";
                }

                if (string.IsNullOrEmpty(policyTemplateModel.NCDFreeYears) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDFreeYears.Trim()))
                    policyTemplateModel.NCDFreeYears = policyFileInfo.QuotationRequestInfo.NajmNcdFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverName.Trim()))
                    policyTemplateModel.MainDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? policyFileInfo.QuotationRequestInfo.MainDriverFullNameAr.Replace(" - ", " ") : policyFileInfo.QuotationRequestInfo.MainDriverFullNameEn.Replace(" - ", " ");

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverIDNumber.Trim()))
                    policyTemplateModel.MainDriverIDNumber = policyFileInfo.QuotationRequestInfo.MainDriverNin;

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverGender.Trim()))
                    policyTemplateModel.MainDriverGender = policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverDateofBirth.Trim()))
                {
                    policyTemplateModel.MainDriverDateofBirth = policyFileInfo.QuotationRequestInfo.MainDriverDateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    policyTemplateModel.MainDriverDateofBirthH = policyFileInfo.QuotationRequestInfo.MainDriverDateOfBirthG.ToString("dd-MM-yyyy", HijriDTFI);
                }

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNumberofyearseligiblefor.Trim()))
                    policyTemplateModel.MainDriverNumberofyearseligiblefor = policyFileInfo.QuotationRequestInfo.MainDriverNCDFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNoClaimsDiscount.Trim()))
                    policyTemplateModel.MainDriverNoClaimsDiscount = policyFileInfo.QuotationRequestInfo.MainDriverNCDReference;

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverResidentialAddressCity.Trim()))
                    policyTemplateModel.MainDriverResidentialAddressCity = mainDriverAddress?.City;
                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverResidentialAddressCity))
                {
                    policyTemplateModel.MainDriverResidentialAddressCity = mainDriverAddress?.City;
                }

                if (string.IsNullOrEmpty(policyTemplateModel.MainDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverFrequencyofDrivingVehicle.Trim()))
                    policyTemplateModel.MainDriverFrequencyofDrivingVehicle = policyFileInfo.QuotationRequestInfo.MainDriverDrivingPercentage?.ToString();

                if (!policyTemplateModel.EducationCode.HasValue)
                    policyTemplateModel.EducationCode = policyFileInfo.QuotationRequestInfo.MainDriverEducationId;

                if (string.IsNullOrEmpty(policyTemplateModel.Education) || string.IsNullOrWhiteSpace(policyTemplateModel.Education.Trim()))
                    policyTemplateModel.Education = policyFileInfo.QuotationRequestInfo.MainDriverEducationName.ToString();

                if (!policyTemplateModel.OccupationCode.HasValue)
                    policyTemplateModel.OccupationCode = policyFileInfo.QuotationRequestInfo.MainDriverOccupationId;

                if (string.IsNullOrEmpty(policyTemplateModel.Occupation) || string.IsNullOrWhiteSpace(policyTemplateModel.Occupation.Trim()))
                {
                    //policyTemplateModel.Occupation = policyFileInfo.QuotationRequestInfo.MainDriverOccupationAr;
                    policyTemplateModel.Occupation = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? policyFileInfo.QuotationRequestInfo.MainDriverOccupationAr : policyFileInfo.QuotationRequestInfo.MainDriverOccupationEn;
                }

                if (string.IsNullOrEmpty(policyTemplateModel.SocialStatus) || string.IsNullOrWhiteSpace(policyTemplateModel.SocialStatus.Trim()))
                {
                    //policyTemplateModel.SocialStatus = policyFileInfo.QuotationRequestInfo?.MainDriverSocialStatusName?.ToString();
                    var socialStatusCode = policyFileInfo.QuotationRequestInfo.MainDriverSocialStatusId;
                    var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                    policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo(stringLang));
                }

                if (!policyTemplateModel.SocialStatusCode.HasValue)
                    policyTemplateModel.SocialStatusCode = policyFileInfo.QuotationRequestInfo.MainDriverSocialStatusId;

                if (string.IsNullOrEmpty(policyTemplateModel.AdditionalNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.AdditionalNumber.Trim()))
                    policyTemplateModel.AdditionalNumber = mainDriverAddress?.AdditionalNumber;
                if (string.IsNullOrEmpty(policyTemplateModel.AdditionalNumber))
                {
                    policyTemplateModel.AdditionalNumber = mainDriverAddress?.AdditionalNumber;
                }
                if (string.IsNullOrEmpty(policyTemplateModel.UnitNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.UnitNumber.Trim()))
                    policyTemplateModel.UnitNumber = mainDriverAddress?.UnitNumber;
                if (string.IsNullOrEmpty(policyTemplateModel.UnitNumber))
                {
                    policyTemplateModel.UnitNumber = mainDriverAddress?.UnitNumber;
                }

                if (string.IsNullOrEmpty(policyTemplateModel.District) || string.IsNullOrWhiteSpace(policyTemplateModel.District.Trim()))
                    policyTemplateModel.District = mainDriverAddress?.District;
                if (string.IsNullOrEmpty(policyTemplateModel.District))
                {
                    policyTemplateModel.District = mainDriverAddress?.District;
                }
                if (string.IsNullOrEmpty(policyTemplateModel.RegionName) || string.IsNullOrWhiteSpace(policyTemplateModel.RegionName.Trim()))
                    policyTemplateModel.RegionName = mainDriverAddress?.RegionName;
                if (string.IsNullOrEmpty(policyTemplateModel.RegionName))
                {
                    policyTemplateModel.RegionName = mainDriverAddress?.RegionName;
                }
                if (string.IsNullOrEmpty(policyTemplateModel.Email) || string.IsNullOrWhiteSpace(policyTemplateModel.Email.Trim()))
                    policyTemplateModel.Email = policyFileInfo.CheckoutDetailsInfo.Email;
                if (!policyTemplateModel.NationalityId.HasValue)
                    policyTemplateModel.NationalityId = policyFileInfo.QuotationRequestInfo.MainDriverNationalityCode;
                if (string.IsNullOrEmpty(policyTemplateModel.Nationality) || string.IsNullOrWhiteSpace(policyTemplateModel.Nationality.Trim()))
                {
                    if (policyFileInfo.QuotationRequestInfo.MainDriverNationalityCode.HasValue)
                    {
                        var nationality = _addressService.GetNationality(policyFileInfo.QuotationRequestInfo.MainDriverNationalityCode.Value);
                        if (nationality != null)
                        {
                            policyTemplateModel.Nationality = policyFileInfo.CheckoutDetailsInfo.SelectedLanguage == LanguageTwoLetterIsoCode.En ? nationality.EnglishDescription : nationality.ArabicDescription;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(policyFileInfo.QuotationRequestInfo.MainDriverMobileNumber) && (string.IsNullOrEmpty(policyTemplateModel.LesseeMobileNo) || string.IsNullOrWhiteSpace(policyTemplateModel.LesseeMobileNo.Trim())))
                    policyTemplateModel.LesseeMobileNo = Utilities.ValidatePhoneNumber(policyFileInfo.QuotationRequestInfo.MainDriverMobileNumber);

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverName.Trim()))
                    policyTemplateModel.SecondDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? secondDriver?.AdditionalDriverFullNameAr : secondDriver?.AdditionalDriverFullNameEn;

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverIDNumber.Trim()))
                    policyTemplateModel.SecondDriverIDNumber = secondDriver?.AdditionalDriverNin;

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverGender.Trim()))
                    policyTemplateModel.SecondDriverGender = secondDriver?.AdditionalDriverGender.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverDateofBirth.Trim()))
                {
                    policyTemplateModel.SecondDriverDateofBirth = secondDriver?.AdditionalDriverDateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    policyTemplateModel.SecondDriverAge = secondDriver?.AdditionalDriverDateOfBirthG.GetUserAge().ToString();
                }
                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNumberofyearseligiblefor.Trim()))
                    policyTemplateModel.SecondDriverNumberofyearseligiblefor = secondDriver?.AdditionalDriverNCDFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNoClaimsDiscount.Trim()))
                    policyTemplateModel.SecondDriverNoClaimsDiscount = secondDriver?.AdditionalDriverNCDReference;

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverResidentialAddressCity.Trim()))
                    policyTemplateModel.SecondDriverResidentialAddressCity = secondDriver?.AdditionalDriverCityName;

                if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle.Trim()))
                    policyTemplateModel.SecondDriverFrequencyofDrivingVehicle = secondDriver?.AdditionalDriverDrivingPercentage?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverName.Trim()))
                    policyTemplateModel.ThirdDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? thirdDriver?.AdditionalDriverFullNameAr : thirdDriver?.AdditionalDriverFullNameEn;

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverIDNumber.Trim()))
                    policyTemplateModel.ThirdDriverIDNumber = thirdDriver?.AdditionalDriverNin;

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverGender.Trim()))
                    policyTemplateModel.ThirdDriverGender = thirdDriver?.AdditionalDriverGender.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverDateofBirth.Trim()))
                {
                    policyTemplateModel.ThirdDriverDateofBirth = thirdDriver?.AdditionalDriverDateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    policyTemplateModel.ThirdDriverAge = thirdDriver?.AdditionalDriverDateOfBirthG.GetUserAge().ToString();
                }
                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNumberofyearseligiblefor.Trim()))
                    policyTemplateModel.ThirdDriverNumberofyearseligiblefor = thirdDriver?.AdditionalDriverNCDFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNoClaimsDiscount.Trim()))
                    policyTemplateModel.ThirdDriverNoClaimsDiscount = thirdDriver?.AdditionalDriverNCDReference;

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverResidentialAddressCity.Trim()))
                    policyTemplateModel.ThirdDriverResidentialAddressCity = thirdDriver?.AdditionalDriverCityName;

                if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle.Trim()))
                    policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle = thirdDriver?.AdditionalDriverDrivingPercentage?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverName.Trim()))
                    policyTemplateModel.FourthDriverName = fourthDriver?.AdditionalDriverFullNameAr;

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverIDNumber.Trim()))
                    policyTemplateModel.FourthDriverIDNumber = fourthDriver?.AdditionalDriverNin;

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverGender.Trim()))
                    policyTemplateModel.FourthDriverGender = fourthDriver?.AdditionalDriverGender.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverDateofBirth.Trim()))
                {
                    policyTemplateModel.FourthDriverDateofBirth = fourthDriver?.AdditionalDriverDateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    policyTemplateModel.FourthDriverAge = fourthDriver?.AdditionalDriverDateOfBirthG.GetUserAge().ToString();
                }
                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNumberofyearseligiblefor.Trim()))
                    policyTemplateModel.FourthDriverNumberofyearseligiblefor = fourthDriver?.AdditionalDriverNCDFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNoClaimsDiscount.Trim()))
                    policyTemplateModel.FourthDriverNoClaimsDiscount = fourthDriver?.AdditionalDriverNCDReference;

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverResidentialAddressCity.Trim()))
                    policyTemplateModel.FourthDriverResidentialAddressCity = fourthDriver?.AdditionalDriverCityName;

                if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle.Trim()))
                    policyTemplateModel.FourthDriverFrequencyofDrivingVehicle = fourthDriver?.AdditionalDriverDrivingPercentage?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverName.Trim()))
                    policyTemplateModel.FifthDriverName = fifthDriver?.AdditionalDriverFullNameAr;

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverIDNumber.Trim()))
                    policyTemplateModel.FifthDriverIDNumber = fifthDriver?.AdditionalDriverNin;

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverGender.Trim()))
                    policyTemplateModel.FifthDriverGender = fifthDriver?.AdditionalDriverGender.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverDateofBirth.Trim()))
                {
                    policyTemplateModel.FifthDriverDateofBirth = fifthDriver?.AdditionalDriverDateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    policyTemplateModel.FifthDriverAge = fifthDriver?.AdditionalDriverDateOfBirthG.GetUserAge().ToString();
                }
                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverNumberofyearseligiblefor.Trim()))
                    policyTemplateModel.FifthDriverNumberofyearseligiblefor = fifthDriver?.AdditionalDriverNCDFreeYears?.ToString();

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverNoClaimsDiscount.Trim()))
                    policyTemplateModel.FifthDriverNoClaimsDiscount = fifthDriver?.AdditionalDriverNCDReference;

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverResidentialAddressCity.Trim()))
                    policyTemplateModel.FifthDriverResidentialAddressCity = fifthDriver?.AdditionalDriverCityName;

                if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverFrequencyofDrivingVehicle.Trim()))
                    policyTemplateModel.FifthDriverFrequencyofDrivingVehicle = fifthDriver?.AdditionalDriverDrivingPercentage?.ToString();

                if (!policyTemplateModel.ChildrenBelow16Years.HasValue)
                    policyTemplateModel.ChildrenBelow16Years = policyFileInfo.QuotationRequestInfo.MainDriverChildrenBelow16Years.HasValue ? policyFileInfo.QuotationRequestInfo.MainDriverChildrenBelow16Years.Value : 0;

                policyTemplateModel.BasicPremium = extraPremiumPrice.ToString();

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 4) // AICC
                {
                    policyTemplateModel.PACoverForDriverOnly = "0.00";
                    policyTemplateModel.PACoverForDriverOnlyEn = "0.00";
                    policyTemplateModel.PACoverForDriverAndPassenger = "0.00";
                    policyTemplateModel.PACoverForDriverAndPassengerEn = "0.00";
                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = "0.00";
                    policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "0.00";
                    policyTemplateModel.RoadsideAssistanceBenefit = "0.00";
                    policyTemplateModel.RoadsideAssistanceBenefitEn = "0.00";
                    policyTemplateModel.GCCCoverBenefit = "0.00";
                    policyTemplateModel.GCCCoverBenefitEn = "0.00";
                    policyTemplateModel.DeathInjuryMedic = "0.00";
                    policyTemplateModel.DeathInjuryMedicEn = "0.00";
                    policyTemplateModel.UnNamedDriver = "0.00";
                    policyTemplateModel.UnNamedDriverEn = "0.00";
                    policyTemplateModel.GeographicalCoverageAr = "0.00";
                    policyTemplateModel.GeographicalCoverageEn = "0.00";
                    //policyTemplateModel.SpecialDiscount = discount1.ToString("#.##");
                    policyTemplateModel.SpecialDiscount = (discount1 > 0) ? discount1.ToString() : "0.00";
                    policyTemplateModel.Discount = (discount > 0) ? discount.ToString("#.##") : "0.00";
                    policyTemplateModel.SubTotal = HandlePolicySubTotal(policyFileInfo.PriceDetail, policyTemplateModel, totalBenfitPrice);
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                    {
                        policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                        policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                    }

                    if (policyFileInfo.OrderItemBenefit != null)
                    {
                        foreach (var benefit in policyFileInfo.OrderItemBenefit)
                        {
                            if (benefit == null)
                                continue;

                            if (benefit.BenefitExternalId == "4")
                                policyTemplateModel.HireCarBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";

                            if (benefit.BenefitId == 1)
                            {
                                policyTemplateModel.PACoverForDriverOnly = benefit.Price.ToString("F");
                                policyTemplateModel.PACoverForDriverOnlyEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 2)
                            {
                                policyTemplateModel.PACoverForDriverAndPassenger = benefit.Price.ToString("F");
                                policyTemplateModel.PACoverForDriverAndPassengerEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 5)
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = benefit.Price.ToString("F");
                                policyTemplateModel.RoadsideAssistanceBenefitEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 10)
                            {
                                policyTemplateModel.GCCCoverBenefit = benefit.Price.ToString("F");
                                policyTemplateModel.GCCCoverBenefitEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 6)
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = benefit.Price.ToString("F");
                                policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 30)
                            {
                                policyTemplateModel.UnNamedDriver = benefit.Price.ToString("F");
                                policyTemplateModel.UnNamedDriverEn = benefit.Price.ToString("F");
                            }
                            else if (benefit.BenefitId == 37)
                            {
                                policyTemplateModel.DeathInjuryMedic = benefit.Price.ToString("F");
                                policyTemplateModel.DeathInjuryMedicEn = benefit.Price.ToString("F");
                            }
                            else if ((benefit.BenefitId == 57 || benefit.BenefitExternalId == "57") || (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58")) // assgin one of them on GeographicalCoverageAr
                            {
                                policyTemplateModel.GeographicalCoverageAr = benefit.Price.ToString("F");
                                policyTemplateModel.GeographicalCoverageEn = benefit.Price.ToString("F");
                            }
                        }
                    }

                    var bankNin = _bankNinRepository.TableNoTracking.Where(a => a.NIN == policyFileInfo.QuotationRequestInfo.NationalId).FirstOrDefault();
                    if (bankNin != null)
                    {
                        var bankData = _bankRepository.TableNoTracking.Where(a => a.Id == bankNin.BankId).FirstOrDefault();
                        if (bankData != null)
                            policyTemplateModel.BankNationalAddress = bankData.NationalAddress;
                    }

                    if (secondDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[0]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[0]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                        policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.AdditionalDriverFullNameEn : secondDriver.AdditionalDriverFullNameAr;
                        policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.AdditionalDriverOccupationName;
                        if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        {
                            if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                            else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            else
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                        }
                    }
                    else
                        policyTemplateModel.SecondDriverRelationship = string.Empty;


                    if (thirdDriver != null)
                    {
                        policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));

                    }
                }
                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 27) //Buruj
                {
                    policyTemplateModel.VehicleUsingPurposesAr = VehicleUseResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.VehicleUse.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                    policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                    if (secondDriver != null)
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                    if (thirdDriver != null)
                        policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));

                    policyTemplateModel.SubTotal = HandlePolicySubTotal(policyFileInfo.PriceDetail, policyTemplateModel, totalBenfitPrice);
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11) // GGI
                {
                    policyTemplateModel.SpecialDiscount = (discount1 > 0) ? discount1.ToString() : "0";
                    policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                    policyTemplateModel.TransmissionType = (policyFileInfo.QuotationRequestInfo.TransmissionType.HasValue) ? Enum.GetName(typeof(TransmissionType), policyFileInfo.QuotationRequestInfo.TransmissionTypeId.Value) : "";

                    var OfficePremium = extraPremiumPrice + totalBenfitPrice;
                    policyTemplateModel.OfficePremium = OfficePremium.ToString();
                    policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";
                    policyTemplateModel.LoyaltyDiscountPercentage = (_benfitCode3 != null && _benfitCode3.PercentageValue > 0) ? _benfitCode3.PercentageValue.ToString() : "0.00";
                    policyTemplateModel.NCDAmount = discount > 0 ? string.Format("{0:N}", discount) : "0.00";
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20) // Alrajhi
                {
                    if (policyFileInfo.OrderItem.ProductNameEn == "Wafi Smart")
                    {
                        var totalPremiumValue = double.Parse(policyTemplateModel.TotalPremium) * 0.04;
                        policyTemplateModel.CommissionAmmount = totalPremiumValue.ToString("#.##");
                    }

                    if (policyFileInfo.OrderItem.ProductNameEn == "Wafi Basic")
                    {
                        var totalPremiumValue = double.Parse(policyTemplateModel.TotalPremium) * 0.02;
                        policyTemplateModel.CommissionAmmount = totalPremiumValue.ToString("#.##");
                    }
                    if (policyFileInfo.OrderItem.ProductNameEn == "Wafi Comprehensive")
                    {
                        var totalPremiumValue = double.Parse(policyTemplateModel.TotalPremium) * 0.1;
                        policyTemplateModel.CommissionAmmount = totalPremiumValue.ToString("#.##");
                    }

                    ////
                    /// delete any percentage multiplication as company return the coorect percentage  @12-4-2023 as per Rawan
                    var ncdPercentage = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue;// * 100;
                    policyTemplateModel.NCDPercentage = ncdPercentage.HasValue ? ncdPercentage.ToString() : "0";
                    var specialDiscountPercentage = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PercentageValue;// * 100;
                    policyTemplateModel.SpecialDiscountPercentage = specialDiscountPercentage.HasValue ? specialDiscountPercentage.ToString() : "0";

                    var OfficePremium = extraPremiumPrice + totalBenfitPrice;
                    policyTemplateModel.OfficePremium = OfficePremium.ToString();
                    policyTemplateModel.ActualAmount = (OfficePremium + vat).ToString();
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 18) // Alamia (Live)
                {
                    policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                    {
                        policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                        policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                    }

                    if (secondDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[0]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[0]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                        policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver?.AdditionalDriverSocialStatusName; ; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                        policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.AdditionalDriverFullNameEn : secondDriver.AdditionalDriverFullNameAr;
                        policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.AdditionalDriverOccupationName;
                        if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        {
                            if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                            else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            else
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                        }
                    }
                    if (thirdDriver != null)
                    {
                        var AddtionalOneLicenseTypeEnumKey = (policyFileInfo.AdditionalDriversLicense[1]?.LicenseId > 0) ? Enum.GetName(typeof(LicenseTypeEnum), policyFileInfo.AdditionalDriversLicense[1]?.LicenseId) : String.Empty;
                        policyTemplateModel.AddtionalDriverTwoLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                        policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.AdditionalDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                        policyTemplateModel.ThirdDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? thirdDriver.AdditionalDriverFullNameEn : thirdDriver.AdditionalDriverFullNameAr;
                        policyTemplateModel.AddtionalDriverTwoSocialStatus = thirdDriver?.AdditionalDriverSocialStatusName; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                        policyTemplateModel.AddtionalDriverOneOccupation = thirdDriver.AdditionalDriverOccupationName;
                        if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        {
                            if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                            else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            else
                                policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                        }
                    }
                }

                var _policyFees = 0;

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 4 || policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 24)
                {
                    string vColor = string.Empty;
                    GetVehicleColor(out vColor, policyFileInfo.QuotationRequestInfo.MajorColor, 24);
                    policyTemplateModel.VehicleColorEn = vColor;
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 24)//Alianz 
                {
                    var _annualPremiumIncludingLoadingsCharges = extraPremiumPrice + additionAgeContribution + fees;
                    policyTemplateModel.AnnualPremiumIncludingLoadingsCharges = _annualPremiumIncludingLoadingsCharges.ToString();

                    policyTemplateModel.Charges = fees.ToString();
                    var _benfitCode4 = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 4).FirstOrDefault();
                    policyTemplateModel.RiskPremiumTrafficViolationLoading = (_benfitCode4 != null && _benfitCode4.PriceValue > 0) ? _benfitCode4.PriceValue.ToString("#.##") : "0.00";

                    var _annualPremiumbeforeNoClaimsDiscount = _annualPremiumIncludingLoadingsCharges + _passengerIncluded + _pACoverForDriverOnly + _benfitCode4?.PriceValue;
                    policyTemplateModel.AnnualPremiumbeforeNoClaimsDiscount = _annualPremiumbeforeNoClaimsDiscount.ToString();

                    policyTemplateModel.TotalAnnualPremiumAfterNoClaimsDiscount = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue).ToString();

                    var _loyaltyDiscountValue = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue;
                    policyTemplateModel.AnnualPremiumAfterLoyaltyDiscount = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue).ToString();

                    var _bcareCommission = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue) * 2 / 100;
                    if(_bcareCommission.HasValue&& _bcareCommission>0)
                         policyTemplateModel.BcareCommission = Math.Round((double)_bcareCommission, 2).ToString();
                    policyTemplateModel.PolicyFees = _policyFees.ToString();
                    policyTemplateModel.TotalAnnualPremiumWithoutVAT = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue + _policyFees).ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                        policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                    
                    policyTemplateModel.SubTotal = HandlePolicySubTotal(policyFileInfo.PriceDetail, policyTemplateModel, totalBenfitPrice);

                    foreach (var item in policyFileInfo.OrderItemBenefit)
                    {
                        if (item.BenefitExternalId == "PADRV")
                        {
                            policyTemplateModel.PersonalAccidentBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.PersonalAccidentForDriverPrice = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "PAPSG")
                        {
                            policyTemplateModel.PassengerIncluded = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";

                            policyTemplateModel.PassengersPersonalAccidentPrice = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "CARREP-150")
                        {
                            policyTemplateModel.CarReplacement_150 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.HireCarBenefit = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "CARREP-200")
                        {
                            policyTemplateModel.CarReplacement_200 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.HireCarBenefit = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "CARREP-75")
                        {
                            policyTemplateModel.CarReplacement_75 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.HireCarBenefit = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "GEXT-2")
                        {
                            policyTemplateModel.GCC_12Month = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "GEXT-3")
                        {
                            policyTemplateModel.GeographicalCoverageAr = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "GEXT-4")
                        {
                            policyTemplateModel.GE_Jordan_Lebanon_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "GEXT-5")
                        {
                            policyTemplateModel.GE_Egypt_Sudan_Turky_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "GEXT-6")
                        {
                            policyTemplateModel.GCC_Jordan_Lebanon_Egypt_Sudan_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                        }
                        else if (item.BenefitExternalId == "24008")
                        {
                            policyTemplateModel.MaxLimit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                        }
                        else if (item.BenefitExternalId == "24009")
                        {
                            policyTemplateModel.OwnDamage = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                        }
                        else if (item.BenefitExternalId == "3")
                        {
                            policyTemplateModel.NaturalDisasters = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                        }
                        else if (item.BenefitExternalId == "4")
                        {
                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                            {
                                policyTemplateModel.TheftFire = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                        }
                        else if (item.BenefitExternalId == "24")
                        {
                            policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                        }
                        else if (item.BenefitExternalId == "19")
                        {
                            policyTemplateModel.EmergencyMedical = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                        }
                        else if (item.BenefitId == 37 || item.BenefitExternalId == "37")
                        {
                            policyTemplateModel.DeathInjuryMedicPrice = (item.Price > 0) ? Math.Round(item.Price, 2).ToString() : "0.00";
                            policyTemplateModel.DeathInjuryMedic = "مشمولة";
                            policyTemplateModel.DeathInjuryMedicEn = "Included";
                            DeathPhysicalInjuriesList.Add(new BenfitSummaryModel());
                        }
                    }                }
                else if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 10)//Ahlia
                {
                    policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0";
                    policyTemplateModel.ClalmLoadingAmount = (clalmLoadingAmount > 0) ? clalmLoadingAmount.ToString() : "0";
                    policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                    policyTemplateModel.AccidentLoadingAmount = "0";

                    policyTemplateModel.GeographicalCoverageAr = "غير مشمولة";
                    policyTemplateModel.GeographicalCoverageEn = "Not Included";

                    policyTemplateModel.DriverAgeCoverageBlew21Ar = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageBlew21En = "Not Included";
                    policyTemplateModel.DriverAgeCoverageFrom18To20Ar = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageFrom18To20En = "Not Included";
                    policyTemplateModel.DriverAgeCoverageFrom21To24Ar = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageFrom21To24En = "Not Included";
                    policyTemplateModel.DriverAgeCoverageAbove24Ar = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageAbove24En = "Not Included";
                    policyTemplateModel.DriverAgeCoverageFrom17To21Ar_Tpl = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageFrom17To21En_Tpl = "Not Included";
                    policyTemplateModel.DriverAgeCoverageFrom21To24Ar_Tpl = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageFrom21To24En_Tpl = "Not Included";
                    policyTemplateModel.DriverAgeCoverageFrom24To29Ar_Tpl = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageFrom24To29En_Tpl = "Not Included";
                    policyTemplateModel.DriverAgeCoverageAbove29Ar_Tpl = "غير مشمولة";
                    policyTemplateModel.DriverAgeCoverageAbove29En_Tpl = "Not Included";
                    policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverOnlyEn = "Not Included";
                    policyTemplateModel.PACoverForDriverAndPassenger = "غير مشمولة";
                    policyTemplateModel.PACoverForDriverAndPassengerEn = "Not Included";

                    var insuredAge = policyFileInfo.QuotationRequestInfo.InsuredBirthDate.GetUserAge();
                    // comp prop
                    if (insuredAge < 21)
                    {
                        policyTemplateModel.DriverAgeCoverageBlew21Ar = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageBlew21En = "Included";

                        if (insuredAge <= 20 && insuredAge >= 18)
                        {
                            policyTemplateModel.DriverAgeCoverageFrom18To20Ar = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageFrom18To20En = "Included";
                        }
                    }
                    else if (insuredAge <= 24 && insuredAge >= 21)
                    {
                        policyTemplateModel.DriverAgeCoverageFrom21To24Ar = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom21To24En = "Included";
                    }
                    else
                    {
                        policyTemplateModel.DriverAgeCoverageAbove24Ar = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageAbove24En = "Included";
                    }

                    // tpl prop
                    if (insuredAge <= 21 && insuredAge >= 17)
                    {
                        policyTemplateModel.DriverAgeCoverageFrom17To21Ar_Tpl = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom17To21En_Tpl = "Included";
                    }
                    else if (insuredAge <= 24 && insuredAge > 21)
                    {
                        policyTemplateModel.DriverAgeCoverageFrom21To24Ar_Tpl = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom21To24En_Tpl = "Included";
                    }
                    else if (insuredAge <= 29 && insuredAge > 24)
                    {
                        policyTemplateModel.DriverAgeCoverageFrom24To29Ar_Tpl = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom24To29En_Tpl = "Included";
                    }
                    else
                    {
                        policyTemplateModel.DriverAgeCoverageAbove29Ar_Tpl = "مشمولة";
                        policyTemplateModel.DriverAgeCoverageAbove29En_Tpl = "Included";
                    }
                }
                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 26)
                {
                    policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo(stringLang));
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                    {
                        policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                        policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                    }
                    if (policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.HasValue && policyFileInfo.QuotationRequestInfo.VehicleAgencyRepair.Value == true)
                    {
                        policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                        policyTemplateModel.VehicleAgencyRepairBenfitEn = "Covered";
                    }

                    var socialStatusCode = policyTemplateModel.SocialStatusCode;
                    var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                    policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo(stringLang));

                    if (policyFileInfo.QuotationRequestInfo.TransmissionTypeId.HasValue && policyFileInfo.QuotationRequestInfo.TransmissionTypeId.Value > 0)
                    {
                        var TransmissionTypeId = policyFileInfo.QuotationRequestInfo.TransmissionTypeId.Value;
                        var TransmissionTypeEnumKey = (TransmissionTypeId > 0) ? Enum.GetName(typeof(TransmissionType), TransmissionTypeId) : String.Empty;
                        policyTemplateModel.TransmissionType = (!string.IsNullOrEmpty(TransmissionTypeEnumKey)) ? TransmissionTypeResource.ResourceManager.GetString(TransmissionTypeEnumKey, CultureInfo.GetCultureInfo(selectedLanguage.ToString())) : "";
                    }
                    if (policyFileInfo.QuotationRequestInfo.ParkingLocationId.HasValue || policyFileInfo.QuotationRequestInfo.ParkingLocationId.Value > 0)
                    {
                        var parkingLocationId = policyFileInfo.QuotationRequestInfo.ParkingLocationId.Value;
                        var parkingLocationEnumKey = (parkingLocationId > 0) ? Enum.GetName(typeof(ParkingLocation), parkingLocationId) : String.Empty;
                        policyTemplateModel.VehicleOvernightParkingLocationCode = (!string.IsNullOrEmpty(parkingLocationEnumKey)) ? ParkingLocationResource.ResourceManager.GetString(parkingLocationEnumKey, CultureInfo.GetCultureInfo(selectedLanguage.ToString())) : "";
                    }
                    if (!string.IsNullOrEmpty(policyTemplateModel.Occupation) || !string.IsNullOrWhiteSpace(policyTemplateModel.Occupation.Trim()))
                    {
                        policyTemplateModel.Occupation = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? policyFileInfo.QuotationRequestInfo.MainDriverOccupationAr : policyFileInfo.QuotationRequestInfo.MainDriverOccupationEn;
                    }
                }
                else
                {
                    policyTemplateModel.AnnualPremiumIncludingLoadingsCharges = "";
                    policyTemplateModel.BcareCommission = bcareCommission.ToString();
                    policyTemplateModel.Charges = "";
                    policyTemplateModel.RiskPremiumTrafficViolationLoading = "N/A";
                    policyTemplateModel.TotalAnnualPremiumAfterNoClaimsDiscount = (extraPremiumPrice - discount).ToString();
                    policyTemplateModel.AnnualPremiumAfterLoyaltyDiscount = (extraPremiumPrice - loyalDiscount).ToString();
                    policyTemplateModel.PolicyFees = fees.ToString();
                    policyTemplateModel.TotalAnnualPremiumWithoutVAT = (extraPremiumPrice - vatPrice).ToString();
                    policyTemplateModel.AnnualPremiumbeforeNoClaimsDiscount = "";
                }

                //policyTemplateModel.VehicleOvernightParkingLocationCode = (policyFileInfo.QuotationRequestInfo.ParkingLocationId.HasValue) ? Enum.GetName(typeof(ParkingLocation), policyFileInfo.QuotationRequestInfo.ParkingLocationId.Value) : "";
                if (policyFileInfo.QuotationRequestInfo.ParkingLocationId.HasValue)
                {
                    var parkingLocationId = policyFileInfo.QuotationRequestInfo.ParkingLocationId.Value;
                    var parkingLocationEnumKey = (parkingLocationId > 0) ? Enum.GetName(typeof(ParkingLocation), parkingLocationId) : String.Empty;
                    policyTemplateModel.VehicleOvernightParkingLocationCode = (!string.IsNullOrEmpty(parkingLocationEnumKey)) ? ParkingLocationResource.ResourceManager.GetString(parkingLocationEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                }

                //policyTemplateModel.TransmissionType = (policyFileInfo.QuotationRequestInfo.TransmissionType.HasValue) ? Enum.GetName(typeof(TransmissionType), policyFileInfo.QuotationRequestInfo?.TransmissionTypeId.Value) : "";
                if (policyFileInfo.QuotationRequestInfo.TransmissionTypeId.HasValue)
                {
                    var transmissionTypeId = policyFileInfo.QuotationRequestInfo.TransmissionTypeId.Value;
                    var transmissionTypeEnumKey = (transmissionTypeId > 0) ? Enum.GetName(typeof(TransmissionType), transmissionTypeId) : String.Empty;
                    policyTemplateModel.TransmissionType = (!string.IsNullOrEmpty(transmissionTypeEnumKey)) ? TransmissionTypeResource.ResourceManager.GetString(transmissionTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                policyTemplateModel.AdditionalAgeContribution = additionAgeContribution.ToString();
                policyTemplateModel.AdditionalPremium = (clalmLoadingAmount + additionAgeContribution) > 0 ? (clalmLoadingAmount + additionAgeContribution).ToString("#.##") : "0.00";

                policyTemplateModel.MainDriverLicenseExpiryDate = policyFileInfo.MainDriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;

                if(secondDriver!=null)
                    policyTemplateModel.SecondDriverLicenseExpiryDate = policyFileInfo.AdditionalDriversLicense.Where(a=>a.DriverId== secondDriver.DriverId).OrderByDescending(a => a.LicenseId).FirstOrDefault()?.ExpiryDateH;
                if (thirdDriver != null)
                    policyTemplateModel.ThirdDriverLicenseExpiryDate = policyFileInfo.AdditionalDriversLicense.Where(a=>a.DriverId== thirdDriver.DriverId).OrderByDescending(a => a.LicenseId).FirstOrDefault()?.ExpiryDateH;
                if (fourthDriver != null)
                    policyTemplateModel.FourthDriverLicenseExpiryDate = policyFileInfo.AdditionalDriversLicense.Where(a=>a.DriverId== fourthDriver.DriverId).OrderByDescending(a => a.LicenseId).FirstOrDefault()?.ExpiryDateH;
                if (fifthDriver != null)
                    policyTemplateModel.FifthDriverLicenseExpiryDate = policyFileInfo.AdditionalDriversLicense.Where(a=>a.DriverId== fifthDriver.DriverId).OrderByDescending(a=> a.LicenseId).FirstOrDefault()?.ExpiryDateH;

                //policyTemplateModel.DriverLicenseTypeCode = policyFileInfo.MainDriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc.ToString();
                var licenseTypeCode = policyFileInfo.MainDriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                var LicenseTypeEnumKey = (licenseTypeCode.HasValue && licenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                policyTemplateModel.DriverLicenseTypeCode = (!string.IsNullOrEmpty(LicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(LicenseTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";

                if (policy.PolicyEffectiveDate.HasValue && policyFileInfo.QuotationRequestInfo.ModelYear.HasValue && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId != 13)
                {
                    int yearsDifference = policy.PolicyEffectiveDate.Value.Year - policyFileInfo.QuotationRequestInfo.ModelYear.Value;
                    if (yearsDifference > 5)
                    {
                        policyTemplateModel.PartialLossClaimsEn = "- In case of Partial Loss claims: deduct 15% from the cost of new parts.";
                        policyTemplateModel.PartialLossClaimsAr = "في حالة الخسارة الجزئية: يخصم 15 % من قطع الغيار المستبدلة .";

                        policyTemplateModel.TotalLossClaimsEn = "- In case of Total Loss claims: deduct 20% from the insured value or market value whichever is less.";
                        policyTemplateModel.TotalLossClaimsAr = "في حالة الخسارة الكلية: يخصم 20 % من القيمة المؤمنة او القيمة السوقية أيهما أقل.";
                    }
                    else
                    {
                        policyTemplateModel.PartialLossClaimsEn = "- In case of Partial Loss claims: Nil.";
                        policyTemplateModel.PartialLossClaimsAr = "في حالة الخسارة الجزئية : لا يوجد .";

                        policyTemplateModel.TotalLossClaimsEn = "- In case of Total Loss claims: deduct 20% from the insured value or market value whichever is less.";
                        policyTemplateModel.TotalLossClaimsAr = "في حالة الخسارة الكلية: يخصم 20 % من القيمة المؤمنة او القيمة السوقية أيهما أقل.";
                    }
                }

                   if (policyFileInfo.QuotationRequestInfo.ModelYear.HasValue && policyFileInfo.QuotationRequestInfo.ModelYear.Value > 0 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 13)
                   {
                    int partialLoss = DateTime.Now.Year - policyFileInfo.QuotationRequestInfo.ModelYear.Value;
                    int totlaLoss = 0;
                    if (partialLoss <= -2)
                    {
                        totlaLoss = 0;
                    }
                    else if (partialLoss >= -1 && partialLoss <= 0)
                    {
                        totlaLoss = 5;
                    }
                    else if (partialLoss >= 1 && partialLoss <= 3)
                    {
                        totlaLoss = 10;
                    }
                    else if (partialLoss >= 4 && partialLoss <= 7)
                    {
                        totlaLoss = 15;
                    }
                    else if (partialLoss >= 8 && partialLoss <= 9)
                    {
                        totlaLoss = 20;
                    }
                    else
                    {
                        totlaLoss = 25;
                    }
                    policyTemplateModel.PartialLossClaimsEn = totlaLoss.ToString() + "%";
                }

                policyTemplateModel.Drivers = new List<DriverPloicyGenerationDto>();
                foreach (var additionalDriver in additionalDrivers)
                {
                    var driverDto = new DriverPloicyGenerationDto();
                    driverDto.DriverTypeCode = 2;
                    long driverId = 0;
                    long.TryParse(additionalDriver?.AdditionalDriverNin, out driverId);
                    driverDto.DriverId = driverId;
                    driverDto.DriverIdTypeCode = additionalDriver.IsCitizen ? 1 : 2;
                    driverDto.DriverBirthDate = additionalDriver.AdditionalDriverDateOfBirthH;
                    driverDto.DriverBirthDateG = additionalDriver.AdditionalDriverDateOfBirthG;
                    driverDto.DriverFirstNameAr = additionalDriver.AdditionalDriverFirstNameAr;
                    driverDto.DriverFirstNameEn = additionalDriver.AdditionalDriverFirstNameEn;
                    driverDto.DriverMiddleNameAr = additionalDriver.AdditionalDriverSecondNameAr;
                    driverDto.DriverMiddleNameEn = additionalDriver.AdditionalDriverSecondNameEn;
                    driverDto.DriverLastNameAr = additionalDriver.AdditionalDriverLastNameAr;
                    driverDto.DriverLastNameEn = additionalDriver.AdditionalDriverLastNameEn;
                    driverDto.DriverFullNameAr = $"{additionalDriver.AdditionalDriverFullNameAr}";
                    driverDto.DriverFullNameEn = $"{additionalDriver.AdditionalDriverFullNameEn}";
                    driverDto.IsAgeLessThen21YearsAR = DateTime.Now.Year - additionalDriver.AdditionalDriverDateOfBirthG.Year < 21 ? "نعم" : "لا";
                    driverDto.IsAgeLessThen21YearsEN = DateTime.Now.Year - additionalDriver.AdditionalDriverDateOfBirthG.Year < 21 ? "Yes" : "No";
                    driverDto.DriverOccupation = additionalDriver.AdditionalDriverResidentOccupation;
                    driverDto.DriverNOALast5Years = additionalDriver.AdditionalDriverNOALast5Years;
                    driverDto.DriverNOCLast5Years = additionalDriver.AdditionalDriverNOCLast5Years;
                    driverDto.DriverNCDFreeYears = additionalDriver.AdditionalDriverNCDFreeYears;
                    driverDto.DriverNCDReference = additionalDriver.AdditionalDriverNCDReference;
                    driverDto.DriverGenderCode = additionalDriver.AdditionalDriverGender.GetCode();
                    driverDto.DriverSocialStatusCode = additionalDriver.AdditionalDriverSocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.AdditionalDriverNationalityCode.HasValue ?
                            additionalDriver.AdditionalDriverNationalityCode.Value.ToString() : "113";
                    driverDto.DriverOccupationCode = additionalDriver.AdditionalDriverOccupationCode;
                    driverDto.DriverDrivingPercentage = additionalDriver.AdditionalDriverDrivingPercentage;
                    driverDto.DriverEducationCode = additionalDriver.AdditionalDriverEducationId;
                    driverDto.DriverMedicalConditionCode = additionalDriver.MedicalConditionId;
                    driverDto.DriverChildrenBelow16Years = additionalDriver.AdditionalDriverChildrenBelow16Years;
                    driverDto.DriverHomeCityCode = additionalDriver.AdditionalDriverCityId?.ToString();
                    driverDto.DriverHomeCity = additionalDriver.AdditionalDriverCityNameAr;
                    driverDto.DriverWorkCityCode = additionalDriver.AdditionalDriverWorkCityId?.ToString();
                    driverDto.DriverWorkCity = additionalDriver.AdditionalDriverWorkCityNameAr;
                    driverDto.NCDAmount = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue.ToString();
                    driverDto.NCDPercentage = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString();

                    policyTemplateModel.Drivers.Add(driverDto);
                }

                if (policyFileInfo.QuotationRequestInfo.CardIdTypeId == 1 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId != 20)
                {
                    policyTemplateModel.InsuredIdTypeCode = policyFileInfo.QuotationRequestInfo.CardIdTypeId.ToString();
                    policyTemplateModel.InsuredIdTypeEnglishDescription = "Citizen";
                    policyTemplateModel.InsuredIdTypeArabicDescription = "مواطن";
                }
                if (policyFileInfo.QuotationRequestInfo.CardIdTypeId == 2 && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId != 20)
                {
                    policyTemplateModel.InsuredIdTypeCode = policyFileInfo.QuotationRequestInfo.CardIdTypeId.ToString();
                    policyTemplateModel.InsuredIdTypeEnglishDescription = "Resident";
                    policyTemplateModel.InsuredIdTypeArabicDescription = "مقيم";
                }
                if ((policyFileInfo.QuotationRequestInfo.CardIdTypeId == 1 || policyFileInfo.QuotationRequestInfo.CardIdTypeId == 2) && policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)//AlRajhi
                {
                    policyTemplateModel.InsuredIdTypeCode = policyFileInfo.QuotationRequestInfo.CardIdTypeId.ToString();
                    policyTemplateModel.InsuredIdTypeEnglishDescription = "Individual";
                    policyTemplateModel.InsuredIdTypeArabicDescription = "أفراد";
                }
                if (policyFileInfo.QuotationRequestInfo.CardIdTypeId == 1 && policyFileInfo.QuotationRequestInfo.NationalId.StartsWith("7"))
                {
                    policyTemplateModel.InsuredIdTypeCode = policyFileInfo.QuotationRequestInfo.CardIdTypeId.ToString();
                    policyTemplateModel.InsuredIdTypeEnglishDescription = "Companies";
                    policyTemplateModel.InsuredIdTypeArabicDescription = "شركات";
                }

                if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11 || policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20|| policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 23|| policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 25)
                {
                    //if (policyFileInfo.QuotationRequestInfo.InsuranceTypeCode == 1) // tpl --> vahicle value from vehicle it self (VehicleValue)
                    //{
                    //    policyTemplateModel.VehicleValue = policyFileInfo.QuotationRequestInfo.VehicleValue?.ToString();
                    //    policyTemplateModel.VehicleLimitValue = policyFileInfo.QuotationRequestInfo.VehicleValue?.ToString();
                    //}
                    //else // comp --> if product returned with (VehicleLimitValue) take this value else take it from vehicle it self (VehicleValue)
                    //{
                    //    policyTemplateModel.VehicleValue = (policyFileInfo.OrderItem.VehicleLimitValue.HasValue == true && policyFileInfo.OrderItem.VehicleLimitValue.Value > 0)
                    //                                        ? policyFileInfo.OrderItem.VehicleLimitValue.Value.ToString()
                    //                                        : policyFileInfo.QuotationRequestInfo.VehicleValue?.ToString();
                    //    policyTemplateModel.VehicleLimitValue = policyTemplateModel.VehicleValue;
                    //}
                    if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 20)
                    {
                        policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";

                        var _benfitCode9 = policyFileInfo.PriceDetail.Where(x => x.PriceTypeCode == 9).FirstOrDefault();
                        policyTemplateModel.BcareCommission = (_benfitCode9 != null) ? _benfitCode9?.PercentageValue.ToString() : "0";
                        policyTemplateModel.BcareCommissionValue = (_benfitCode9 != null) ? _benfitCode9.PriceValue.ToString() : "0.00";
                    }

                    if (policyFileInfo.QuotationRequestInfo.InsuranceCompanyId == 11) // GGI
                    {
                        policyTemplateModel.SubTotal = HandlePolicySubTotal(policyFileInfo.PriceDetail, policyTemplateModel, totalBenfitPrice);
                        policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                        policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";
                        policyTemplateModel.SpecialDiscount = discount1 > 0 ? discount1.ToString() : "0";
                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(policyFileInfo.QuotationRequestInfo.MainDriverGender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));

                        //policyTemplateModel.VehicleOvernightParkingLocationCode = (vehicle.ParkingLocationId.HasValue) ?
                        //ParkingLocationResource.ResourceManager.GetString(vehicle.ParkingLocationId.Value.ToString(), CultureInfo.GetCultureInfo(stringLang)) : "";

                        //policyTemplateModel.TransmissionType = (vehicle.TransmissionType.HasValue) ?
                        //TransmissionTypeResource.ResourceManager.GetString(vehicle.TransmissionType.HasValue.ToString(), CultureInfo.GetCultureInfo(stringLang)) : "";
                    }
                }

                policyTemplateModel.ProductDescription = policyFileInfo.OrderItem.ProductNameEn;

                if (policyFileInfo.CheckoutDetailsInfo.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                {
                    var bankNin = _bankNinRepository.TableNoTracking.Where(a => a.NIN == policyFileInfo.QuotationRequestInfo.NationalId).FirstOrDefault();
                    if (bankNin != null)
                    {

                        var bankData = _bankRepository.TableNoTracking.Where(a => a.Id == bankNin.BankId).FirstOrDefault();
                        if (bankData != null)
                        {
                            policyTemplateModel.BankNationalAddress = bankData.NationalAddress;
                            policyTemplateModel.LessorMobileNo = Utilities.ValidatePhoneNumber(bankData.PhoneNumber);
                        }
                    }

                    #region Deprecation Data
                    var depreciationSetting = _autoleasingDepreciationSettingHistoryRepository.TableNoTracking.FirstOrDefault(d => d.ExternalId == policyFileInfo.QuotationRequestInfo.ExternalId);
                    if (depreciationSetting != null)
                    {
                        List<string> annualPercentages = new List<string>();
                        if (!depreciationSetting.FirstYear.Equals(null))
                        {
                            annualPercentages.Add(depreciationSetting.FirstYear.ToString().Replace(".00", "") + "%");
                        }
                        if (!depreciationSetting.SecondYear.Equals(null))
                        {
                            annualPercentages.Add(depreciationSetting.SecondYear.ToString().Replace(".00", "") + "%");
                        }
                        if (!depreciationSetting.ThirdYear.Equals(null))
                        {
                            annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Replace(".00", "") + "%");
                        }
                        if (!depreciationSetting.FourthYear.Equals(null))
                        {
                            annualPercentages.Add(depreciationSetting.FourthYear.ToString().Replace(".00", "") + "%");
                        }
                        if (!depreciationSetting.FifthYear.Equals(null))
                        {
                            annualPercentages.Add(depreciationSetting.FifthYear.ToString().Replace(".00", "") + "%");
                        }

                        policyTemplateModel.AnnualDeprecationPercentages = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Replace(".00", "") + "%";

                        var vehicleValue = policyFileInfo.QuotationRequestInfo.VehicleValue;
                        Decimal? DepreciationValue1 = 0;
                        Decimal? DepreciationValue2 = 0;
                        Decimal? DepreciationValue3 = 0;
                        Decimal? DepreciationValue4 = 0;
                        Decimal? DepreciationValue5 = 0;
                        List<Decimal?> depreciationValues = new List<Decimal?>();
                        var currentVehicleValue = vehicleValue;

                        if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                        {
                            if (depreciationSetting.IsDynamic)
                            {
                                DepreciationValue1 = vehicleValue;

                                if (depreciationSetting.SecondYear != 0)
                                    DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                                if (depreciationSetting.ThirdYear != 0)
                                    DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                                if (depreciationSetting.FourthYear != 0)
                                    DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                                if (depreciationSetting.FifthYear != 0)
                                    DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                            }
                            else
                            {
                                DepreciationValue1 = currentVehicleValue;
                                DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                                DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                                DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                                DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                            }
                        }
                        else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                        {
                            if (depreciationSetting.IsDynamic)
                            {
                                DepreciationValue1 = vehicleValue;

                                if (depreciationSetting.SecondYear != 0)
                                    DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                                if (depreciationSetting.ThirdYear != 0)
                                    DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                                if (depreciationSetting.FourthYear != 0)
                                    DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                                if (depreciationSetting.FifthYear != 0)
                                    DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                            }
                            else
                            {
                                DepreciationValue1 = vehicleValue;
                                DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                                DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                                DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                                DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                            }
                        }

                        policyTemplateModel.FirstYearDepreciation = Math.Round(DepreciationValue1.Value, 2).ToString().Replace(".00", "");
                        policyTemplateModel.SecondYearDepreciation = Math.Round(DepreciationValue2.Value, 2).ToString().Replace(".00", "");
                        policyTemplateModel.ThirdYearDepreciation = Math.Round(DepreciationValue3.Value, 2).ToString().Replace(".00", "");
                        policyTemplateModel.FourthYearDepreciation = Math.Round(DepreciationValue4.Value, 2).ToString().Replace(".00", "");
                        policyTemplateModel.FifthYearDepreciation = Math.Round(DepreciationValue5.Value, 2).ToString().Replace(".00", "");
                    }

                    #endregion
                }

                output.PolicyModel = policyTemplateModel;
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PolicyGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        private void GetVehicleColor(out string vehicleColorEn, string vehicleMajorColor, int companyId)
        {
            vehicleColorEn = vehicleMajorColor; //default value
            var secondMajorCollor = string.Empty;
            if (vehicleMajorColor[0] == 'ا')
                secondMajorCollor = 'أ' + vehicleMajorColor.Substring(1);
            else if (vehicleMajorColor[0] == 'أ')
                secondMajorCollor = 'ا' + vehicleMajorColor.Substring(1);
            else
                secondMajorCollor = vehicleMajorColor;



            var vehiclesColors = _vehicleService.GetVehicleColors();
            var vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor || color.ArabicDescription == secondMajorCollor);
            if (vColor == null)
            {
                if (vehicleMajorColor.Contains(' '))
                {
                    vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor.Split(' ')[0] || color.ArabicDescription == secondMajorCollor.Split(' ')[0]);
                    if (vColor != null)
                    {
                        vehicleColorEn = vColor.EnglishDescription;
                    }
                }
            }
            else
            {
                vehicleColorEn = vColor.EnglishDescription;
            }
        }

        private string HandlePolicySubTotal(ICollection<PriceDetail> priceDetails, PolicyTemplateGenerationModel policyTemplateModel, decimal benefitsPrice)
        {
            decimal subTotal = 0;
            try
            {
                decimal _officePremium = 0;
                decimal _loading = 0;
                decimal _NCDAmount = 0;
                decimal _LoyaltyDiscount = 0;
                decimal _SchemesDiscount = 0;
                decimal _SpecialDiscount2 = 0;

                //if (!string.IsNullOrEmpty(policyTemplateModel.OfficePremium))
                //    decimal.TryParse(policyTemplateModel.OfficePremium, out _officePremium);
                //if (!string.IsNullOrEmpty(policyTemplateModel.ClalmLoadingAmount))
                //    decimal.TryParse(policyTemplateModel.ClalmLoadingAmount, out _loading);
                //if (!string.IsNullOrEmpty(policyTemplateModel.NCDAmount))
                //    decimal.TryParse(policyTemplateModel.NCDAmount, out _NCDAmount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscount))
                //    decimal.TryParse(policyTemplateModel.LoyaltyDiscount, out _LoyaltyDiscount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.SchemesDiscount))
                //    decimal.TryParse(policyTemplateModel.SchemesDiscount, out _SchemesDiscount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2))
                //    decimal.TryParse(policyTemplateModel.SpecialDiscount2, out _SpecialDiscount2);

                //subTotal = (_officePremium + _loading) - (_NCDAmount + _LoyaltyDiscount + _SchemesDiscount + _SpecialDiscount2);

                foreach (var price in priceDetails)
                {
                    switch (price.PriceTypeCode)
                    {
                        case 2: _NCDAmount += price.PriceValue; break;
                        case 3: _LoyaltyDiscount += price.PriceValue; break;
                        case 4: _loading += price.PriceValue; break;
                        case 7: _officePremium += price.PriceValue; break;
                        case 11: _SchemesDiscount += price.PriceValue; break;
                        case 12: _SpecialDiscount2 += price.PriceValue; break;
                    }
                }

                subTotal = (_officePremium + _loading + benefitsPrice) - (_NCDAmount + _LoyaltyDiscount + _SchemesDiscount + _SpecialDiscount2);
            }
            catch (Exception ex)
            {
                subTotal = 0;
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandlePolicySubTotal" + policyTemplateModel.ReferenceNo + ".txt", ex.ToString());
            }

            return subTotal.ToString();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Extensions;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.PolicyApi.Extensions;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Services
{
    public class PolicyRequestService : IPolicyRequestService
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<PolicyFile> _policyFileRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueue;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IPolicyEmailService _policyEmailService;
        private readonly ILogger _logger;

        public PolicyRequestService(IRepository<Policy> policyRepository,
            IInvoiceService invoiceService, IPolicyEmailService policyEmailService,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<PolicyFile> policyFileRepository,
            IRepository<PolicyProcessingQueue> policyProcessingQueue,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            ILogger logger)
        {
            _invoiceService = invoiceService;
            _policyRepository = policyRepository;
            _policyEmailService = policyEmailService;
            _checkoutDetailRepository = checkoutDetailRepository;
            _orderItemRepository = orderItemRepository;
            _policyFileRepository = policyFileRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _invoiceRepository = invoiceRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _logger = logger;
            _policyProcessingQueue = policyProcessingQueue;
        }

        /// <summary>
        /// Send Policy File to Client
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <param name="policyResponse">policy Response</param>
        /// <param name="email">Eamil</param>
        /// <param name="userLanguage">user Language</param>
        /// <returns></returns>
        public async Task<bool> SendPolicyFileToClient(string referenceId, PolicyResponse policyResponse, string email, LanguageTwoLetterIsoCode userLanguage)
        {

            SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
            {
                PolicyResponseMessage = policyResponse,
                ReceiverEmailAddress = email,
                ReferenceId = referenceId,
                UserLanguage = userLanguage,
                PolicyFileByteArray = policyResponse.PolicyFile,
                InvoiceFileByteArray = null,
                IsPolicyGenerated = true,
                IsShowErrors = false
            };

            _logger.Log($"PolicyRequestService -> SendPolicyFileToClient >>> Before <<< SendPolicyViaMail (reference id : {referenceId}, user language : {userLanguage}, show errors : {false})");
            await _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto);
            _logger.Log($"PolicyRequestService -> SendPolicyFileToClient >>> After <<< SendPolicyViaMail (reference id : {referenceId}, user language : {userLanguage}, show errors : {false})");
            return true;
        }

        public async Task<PolicyOutput> SubmitPolicyAsync(string referenceId, LanguageTwoLetterIsoCode userLanguage, ServiceRequestLog predefinedLogInfo, bool showErrors = false)
        {
            PolicyOutput output = new PolicyOutput();
            try
            {
                var checkoutDetails = _checkoutDetailRepository.Table.Include(c => c.Driver)
                            .Include(c => c.Driver.Addresses)
                            .Include(c => c.OrderItems.Select(oi => oi.Product.PriceDetails))
                            .Include(c => c.OrderItems.Select(oi => oi.OrderItemBenefits.Select(oib => oib.Benefit)))
                            .Include(c => c.BankCode)
                            .Include(c => c.Vehicle)
                            .FirstOrDefault(c => c.ReferenceId == referenceId
                                                && c.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                                                && c.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                                                && c.PolicyStatusId != (int)EPolicyStatus.Available);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = "Checkout Details Is Null";
                    return output;
                }
                Guid selectedUserId = Guid.Empty;
                Guid.TryParse(checkoutDetails.UserId, out selectedUserId);
                predefinedLogInfo.UserID = selectedUserId;
                if (checkoutDetails.Driver != null)
                    predefinedLogInfo.DriverNin = checkoutDetails.Driver.NIN;
                if (checkoutDetails.Vehicle != null)
                {
                    if (checkoutDetails.Vehicle.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.CustomCardNumber;
                    else
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.SequenceNumber;
                }

                var quoteResponse = _quotationResponseRepository.Table
                   .Include(e => e.QuotationRequest)
                   .Include(e => e.QuotationRequest.Vehicle)
                   .Include(e => e.QuotationRequest.Driver)
                   .Include(e => e.InsuranceCompany)
                   .FirstOrDefault(q => q.ReferenceId == referenceId);

                if (quoteResponse == null)
                {
                    output.ErrorCode = 3; ;
                    output.ErrorDescription = "Quotation Response Is Null";
                    return output;
                }
                var companyId = quoteResponse.InsuranceCompanyId;
                var insuranceCompany = quoteResponse.InsuranceCompany;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = 4;
                    output.ErrorDescription = "CompanyId  Is Null";
                    return output;
                }
                if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentSuccess && checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Pending)
                {
                    output.ErrorCode = 5;
                    output.ErrorDescription = "Invalid Policy Status id as id is " + checkoutDetails.PolicyStatusId;
                    return output;
                }
                var invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                if (invoice == null)
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "Invoice Is Null";
                    return output;
                }
                var orderItem = checkoutDetails.OrderItems.FirstOrDefault();
                if (orderItem == null)
                {
                    output.ErrorCode = 7;
                    output.ErrorDescription = "Order Item Is Null";
                    return output;
                }
                var selectedBenefits = orderItem.OrderItemBenefits.Select(b =>
                        new BenefitRequest()
                        {
                            BenefitId = b.BenefitExternalId,
                        }).ToList();

                var policyRequestMessage = new PolicyRequest()
                {
                    ReferenceId = referenceId,
                    QuotationNo = orderItem.Product.QuotaionNo,
                    ProductId = orderItem.Product.ExternalProductId,
                    Benefits = selectedBenefits,
                    InsuredId = long.Parse(checkoutDetails.Driver.NIN),
                    InsuredMobileNumber = Utilities.Remove966FromMobileNumber(checkoutDetails.Phone),
                    InsuredEmail = checkoutDetails.Email,
                    InsuredBuildingNo = int.Parse(checkoutDetails.Driver.Addresses.First().BuildingNumber),
                    InsuredZipCode = int.Parse(checkoutDetails.Driver.Addresses.First().PostCode),
                    InsuredAdditionalNumber = int.Parse(checkoutDetails.Driver.Addresses.First().AdditionalNumber),
                    InsuredUnitNo = int.Parse(checkoutDetails.Driver.Addresses.First().UnitNumber),
                    InsuredCity = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().City) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().City,
                    InsuredDistrict = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().District) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().District,
                    InsuredStreet = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().Street) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().Street,
                    PaymentMethodCode = checkoutDetails.PaymentMethodId.GetValueOrDefault(),
                    PaymentAmount = invoice.TotalPrice.Value,
                    PaymentBillNumber = invoice.InvoiceNo.ToString(),
                    InsuredBankCode = checkoutDetails.BankCode.Code,
                    PaymentUsername = "UNKNOWN",
                    InsuredIBAN = checkoutDetails.IBAN,
                    PolicyEffectiveDate = quoteResponse?.QuotationRequest?.RequestPolicyEffectiveDate,
                    QuotationRequestId = quoteResponse.QuotationRequest.ID,
                    ProductInternalId = orderItem.Product.Id

                };
                PolicyResponse policyResponseMessage = null;
                predefinedLogInfo.CompanyID = companyId;
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    output.ErrorCode = 8;
                    output.ErrorDescription = "provider Type is Null";
                    return output;
                }
                IInsuranceProvider provider = null;
                object instance;
                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                if (provider == null)
                {
                    output.ErrorCode = 9;
                    output.ErrorDescription = "provider is Null";
                    return output;
                }

                if (string.IsNullOrEmpty(policyRequestMessage.InsuredStreet) || string.IsNullOrWhiteSpace(policyRequestMessage.InsuredStreet.Trim()) || policyRequestMessage.InsuredStreet == "0")
                {
                    policyRequestMessage.InsuredStreet = "غير معروف";//WebResources.Unknown;
                }
                policyResponseMessage = provider.GetPolicy(policyRequestMessage, predefinedLogInfo);

                if (policyResponseMessage == null || (policyResponseMessage.Errors != null && policyResponseMessage.Errors.Count() > 0) || policyResponseMessage.StatusCode == 2)
                {
                    UpdatePolicyStatus(checkoutDetails, EPolicyStatus.Pending);
                    SendPolicyViaMailDto sendPolicy = new SendPolicyViaMailDto()
                    {
                        PolicyResponseMessage = policyResponseMessage,
                        ReceiverEmailAddress = checkoutDetails.Email,
                        ReferenceId = referenceId,
                        UserLanguage = userLanguage,
                        PolicyFileByteArray = null,
                        InvoiceFileByteArray = null,
                        IsPolicyGenerated = false,
                        IsShowErrors = showErrors
                    };
                    var TrialNumbersOfGeneratingPlicy = _policyProcessingQueue.TableNoTracking
                        .Where(x => x.ReferenceId == referenceId)
                        .Select(x => x.ProcessingTries)
                        .FirstOrDefault();
                    if (TrialNumbersOfGeneratingPlicy == 0)
                    {
                        await _policyEmailService.SendPolicyViaMail(sendPolicy);
                    }
                    //StringBuilder servcieErrors = new StringBuilder();
                    //StringBuilder servcieErrorsCodes = new StringBuilder();

                    //foreach (var error in policyResponseMessage?.Errors)
                    //{
                    //    servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                    //    servcieErrorsCodes.AppendLine(error.Code);
                    //}
                    output.ErrorCode = 10;
                   // output.ErrorDescription = "policyResponseMessage is Null or contains errors " + servcieErrors;
                    return output;
                }
                policyResponseMessage.ReferenceId = referenceId;
                byte[] invoiceFilePdf = null;
                Invoice invoiceInfo = await _invoiceService.GenerateAndSaveInvoicePdf(policyRequestMessage, policyResponseMessage);
                invoiceFilePdf = invoiceInfo == null ? null : (invoiceInfo.InvoiceFile == null ? null : invoiceInfo.InvoiceFile.InvoiceData);
                var policyId = SavePolicy(policyResponseMessage, companyId);
                if (!policyId.HasValue)
                {
                    output.ErrorCode = 11;
                    output.ErrorDescription = "Failed To Save Policy to database or may policy already exist";
                    return output;
                }
                invoice.PolicyId = policyId;
                _invoiceRepository.Update(invoice);
                PdfGenerationLog log = new PdfGenerationLog();
                log.Channel = "Portal";

                //GetPolicyFile(userLanguage, checkoutDetails, companyId, policyResponseMessage, log);
                var policyStatus = HandleInsurancePolicyFileResult(policyResponseMessage, companyId, userLanguage, log);
                checkoutDetails.PolicyStatusId = (int)policyStatus;
                checkoutDetails.ModifiedDate = DateTime.Now;
                _checkoutDetailRepository.Update(checkoutDetails);
                if (policyStatus != EPolicyStatus.Available)
                {
                    SendPolicyViaMailDto mailDto = new SendPolicyViaMailDto()
                    {
                        PolicyResponseMessage = policyResponseMessage,
                        ReceiverEmailAddress = checkoutDetails.Email,
                        ReferenceId = referenceId,
                        UserLanguage = userLanguage,
                        PolicyFileByteArray = policyResponseMessage.PolicyFile,
                        InvoiceFileByteArray = invoiceFilePdf,
                        IsPolicyGenerated = false,
                        IsShowErrors = showErrors
                    };
                    await _policyEmailService.SendPolicyViaMail(mailDto);
                    output.ErrorCode = 13;
                    output.ErrorDescription = "Failed to generate or download the pdf file";
                    return output;
                }
                SavePolicyFile(policyResponseMessage);
                SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                {
                    PolicyResponseMessage = policyResponseMessage,
                    ReceiverEmailAddress = checkoutDetails.Email,
                    ReferenceId = referenceId,
                    UserLanguage = userLanguage,
                    PolicyFileByteArray = policyResponseMessage.PolicyFile,
                    InvoiceFileByteArray = invoiceFilePdf,
                    IsPolicyGenerated = true,
                    IsShowErrors = showErrors
                };
                await _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto);
                output.ErrorCode = 1;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = 12;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public async Task<bool> GeneratePolicyAsync(string referenceId, LanguageTwoLetterIsoCode userLanguage, ServiceRequestLog predefinedLogInfo, bool showErrors = false)
        {
            //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync >>> Start <<< GeneratePolicyAsync (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})");
            var checkoutDetails = _checkoutDetailRepository.Table.Include(c => c.Driver)
                        .Include(c => c.Driver.Addresses)
                        .Include(c => c.OrderItems.Select(oi => oi.Product.PriceDetails))
                        .Include(c => c.OrderItems.Select(oi => oi.OrderItemBenefits.Select(oib => oib.Benefit)))
                        .Include(c => c.CheckoutAdditionalDrivers)
                        .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver))
                        .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver).Select(d => d.DriverLicenses))
                        .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver).Select(d => d.Addresses))
                        .Include(c => c.BankCode)
                        .Include(c => c.Vehicle)
                        .Include(c => c.AdditionalInfo)
                        .FirstOrDefault(c => c.ReferenceId == referenceId);

            if (checkoutDetails != null)
            {
                Guid selectedUserId = Guid.Empty;
                Guid.TryParse(checkoutDetails.UserId, out selectedUserId);
                predefinedLogInfo.UserID = selectedUserId;
                //predefinedLogInfo.UserName = currentUserName;
                if (checkoutDetails.Driver != null)
                    predefinedLogInfo.DriverNin = checkoutDetails.Driver.NIN;
                if (checkoutDetails.Vehicle != null)
                {
                    if (checkoutDetails.Vehicle.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.CustomCardNumber;
                    else
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.SequenceNumber;

                }
            }
            var quotationResponse = _quotationResponseRepository.Table.FirstOrDefault(e => e.ReferenceId == referenceId);

            //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync generating policy request message");
            PolicyRequest policyRequestMessage = GeneratePolicyRequestMessage(checkoutDetails, referenceId);
            if (policyRequestMessage != null)
            {
                PolicyResponse policyResponseMessage = null;
                try
                {
                    //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync generating policy from insurance company id:{quotationResponse.InsuranceCompanyId}");
                    predefinedLogInfo.CompanyID = quotationResponse.InsuranceCompanyId;
                    policyResponseMessage = GetPolicy(policyRequestMessage, quotationResponse.InsuranceCompanyId, predefinedLogInfo);
                    if (policyResponseMessage != null)
                        policyResponseMessage.ReferenceId = referenceId;
                }
                catch (Exception ex)
                {
                    _logger.Log($"PolicyRequestService -> GeneratePolicyAsync get policy (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})", ex);
                }


                if (policyResponseMessage == null ||
                    (policyResponseMessage.Errors != null && policyResponseMessage.Errors.Count() > 0) ||
                    policyResponseMessage.StatusCode == 2)
                {
                    //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync policyResponseMessage: {JsonConvert.SerializeObject(policyResponseMessage)}");
                    UpdatePolicyStatus(checkoutDetails, EPolicyStatus.Pending);
                    //log the response errors if it has errors
                    LogPolicyResponseErrorsIfExist(policyResponseMessage);
                    SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                    {
                        PolicyResponseMessage = policyResponseMessage,
                        ReceiverEmailAddress = checkoutDetails.Email,
                        // ReceiverEmailAddress = "safaahusin@yahoo.com",
                        ReferenceId = referenceId,
                        UserLanguage = userLanguage,
                        PolicyFileByteArray = null,
                        InvoiceFileByteArray = null,
                        IsPolicyGenerated = false,
                        IsShowErrors = showErrors
                    };

                    // Ali Shawky
                    var TrialNumbersOfGeneratingPlicy = _policyProcessingQueue.TableNoTracking
                        .Where(x => x.ReferenceId == referenceId)
                        .Select(x => x.ProcessingTries)
                        .FirstOrDefault();
                    if (TrialNumbersOfGeneratingPlicy == 0)
                    {
                        await _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto);
                    }
                    return false;
                }

                Invoice invoice = null;
                byte[] invoiceFilePdf = null;
                try
                {
                    invoice = await _invoiceService.GenerateAndSaveInvoicePdf(policyRequestMessage, policyResponseMessage);
                    invoiceFilePdf = invoice == null ? null
                            : (invoice.InvoiceFile == null ? null : invoice.InvoiceFile.InvoiceData);
                }
                catch (Exception ex)
                {
                    _logger.Log($"PolicyRequestService -> GeneratePolicyAsync generate & save invoice pdf (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})", ex);
                }


                try
                {

                    var policyId = SavePolicy(policyResponseMessage, quotationResponse.InsuranceCompanyId);
                    if (policyId.HasValue)
                    {
                        //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync policy generated with id:{policyId}");
                        invoice.PolicyId = policyId;
                        _invoiceRepository.Update(invoice);
                    }
                    PdfGenerationLog log = new PdfGenerationLog();
                    log.Channel = "Portal";

                    GetPolicyFile(userLanguage, checkoutDetails, quotationResponse.InsuranceCompanyId, policyResponseMessage, log);
                }
                catch (Exception ex)
                {
                    _logger.Log($"PolicyRequestService -> GeneratePolicyAsync -> Save policy (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})", ex);
                }
                finally
                {
                    SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                    {
                        PolicyResponseMessage = policyResponseMessage,
                        ReceiverEmailAddress = checkoutDetails.Email,
                        ReferenceId = referenceId,
                        UserLanguage = userLanguage,
                        PolicyFileByteArray = policyResponseMessage.PolicyFile,
                        InvoiceFileByteArray = invoiceFilePdf,
                        IsPolicyGenerated = true,
                        IsShowErrors = showErrors
                    };
                    //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync >>> Before <<< SendPolicyViaMail (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})");
                    await _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto);
                    //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync >>> After <<< SendPolicyViaMail (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})");
                }
                //_logger.Log($"PolicyRequestService -> GeneratePolicyAsync >>> End <<< GeneratePolicyAsync return true (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})");
                return true;
            }
            _logger.Log($"PolicyRequestService -> GeneratePolicyAsync >>> End <<< GeneratePolicyAsync return false (reference id : {referenceId}, user language : {userLanguage}, show errors : {showErrors})");
            return false;
        }



        private void GetPolicyFile(LanguageTwoLetterIsoCode userLanguage, CheckoutDetail checkoutDetails, int insuranceCompanyId, PolicyResponse policyResponseMessage, PdfGenerationLog log)
        {
            var policyStatus = HandleInsurancePolicyFileResult(policyResponseMessage, insuranceCompanyId, userLanguage, log);
            UpdatePolicyStatus(checkoutDetails, policyStatus);

            SavePolicyFile(policyResponseMessage);
        }

        public void GetFailedPolicyFile(string referenceId, string channel = "RetrialMechanism")
        {

            PdfGenerationLog log = new PdfGenerationLog();
            log.Channel = channel;
            log.ReferenceId = referenceId;
            log.ServerIP = Utilities.GetInternalServerIP();
            DateTime dtBefore = DateTime.Now;
            try
            {
                var policyRequestLog = ServiceRequestLogDataAccess.GetPolicyByRefernceId(referenceId);
                if (policyRequestLog == null)
                {
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "policyRequestLog is null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return;
                }
                log.ServiceRequest = policyRequestLog.ServiceRequest;
                log.CompanyName = policyRequestLog.CompanyName;
                log.CompanyID = policyRequestLog.CompanyID;
                log.DriverNin = policyRequestLog.DriverNin;
                log.VehicleId = policyRequestLog.VehicleId;

                PolicyResponse policyResponseMessage = null;
                if (policyRequestLog.CompanyName.ToLower() == "MedGulf".ToLower())
                {
                    var responseValue = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(policyRequestLog.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        Errors = responseValue.Errors,
                        PolicyFileUrl = responseValue.PolicyFileUrl,
                        PolicyNo = responseValue.PolicyNo,
                        ReferenceId = responseValue.ReferenceId,
                        StatusCode = responseValue.StatusCode,
                        PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyEffectiveDate),
                        PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyExpiryDate),
                        PolicyIssuanceDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyIssuanceDate)
                    };
                }
                else
                {
                    policyResponseMessage = JsonConvert.DeserializeObject<PolicyResponse>(policyRequestLog.ServiceResponse);
                }
                var checkoutDetails = _checkoutDetailRepository.Table.Include(c => c.Driver)
                          .Include(c => c.Driver.Addresses)
                          .Include(c => c.OrderItems.Select(oi => oi.Product.PriceDetails))
                          .Include(c => c.OrderItems.Select(oi => oi.OrderItemBenefits.Select(oib => oib.Benefit)))
                          .Include(c => c.CheckoutAdditionalDrivers)
                          .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver))
                          .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver).Select(d => d.DriverLicenses))
                          .Include(c => c.CheckoutAdditionalDrivers.Select(d => d.Driver).Select(d => d.Addresses))
                          .Include(c => c.BankCode)
                          .Include(c => c.Vehicle)
                          .Include(c => c.AdditionalInfo)
                          .FirstOrDefault(c => c.ReferenceId == referenceId);
                if (checkoutDetails == null)
                {
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "checkoutDetails is null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return;
                }
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                //GetPolicyFile(lang, checkoutDetails, policyRequestLog.CompanyID.Value, policyResponseMessage, log);
                var policyStatus = HandleInsurancePolicyFileResult(policyResponseMessage, policyRequestLog.CompanyID.Value, lang, log);
                if (policyStatus == EPolicyStatus.Available)
                {
                    checkoutDetails.PolicyStatusId = (int)policyStatus;
                    _checkoutDetailRepository.Update(checkoutDetails);
                    SavePolicyFile(policyResponseMessage);
                    if (channel == "Dashboard")
                    {
                        var policyProcessingQueue = _policyProcessingQueue.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                        if (policyProcessingQueue != null)
                        {
                            policyProcessingQueue.ProcessedOn = DateTime.Now;
                            policyProcessingQueue.ProcessedOn = DateTime.Now;
                            DateTime dtAfter = DateTime.Now;
                            policyProcessingQueue.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                            _policyProcessingQueue.Update(policyProcessingQueue);
                        }
                    }

                    SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                    {
                        PolicyResponseMessage = policyResponseMessage,
                        ReceiverEmailAddress = checkoutDetails.Email,
                        ReferenceId = policyResponseMessage.ReferenceId,
                        UserLanguage = lang,
                        PolicyFileByteArray = policyResponseMessage.PolicyFile,
                        IsPolicyGenerated = true,
                        IsShowErrors = false
                    };
                    _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto).Wait();
                }
            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = exp.GetBaseException().ToString();
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
            }
        }


        private EPolicyStatus HandleInsurancePolicyFileResult(PolicyResponse policy, int iCompanyId, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log)
        {
            log.ServerIP = ServicesUtilities.GetServerIP();
            try
            {
                if (policy == null)
                {
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Policy is sent to method null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }
                log.ReferenceId = policy?.ReferenceId;
                log.PolicyNo = policy?.PolicyNo;
                log.ServiceRequest = JsonConvert.SerializeObject(policy);

                if (iCompanyId == 0)
                {
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Company is is sent to method 0";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }
                log.CompanyID = iCompanyId;
                var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(
                    i => i.InsuranceCompanyID == iCompanyId);

                if (insuranceCompany == null)
                {
                    log.ErrorDescription = "Insurance Company Is Null";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }

                log.CompanyName = insuranceCompany?.NameEN;

                var providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                IInsuranceProvider provider = null;
                if (providerType == null)
                {

                    log.ErrorDescription = "providerType Is Null";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ProviderTypeIsNull;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }

                object instance;
                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;

                if (provider == null)
                {
                    log.ErrorDescription = "provider Is Null";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ProviderIsNull;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }


                var policyScheduleOutput = provider.PolicySchedule(policy.PolicyNo, policy.ReferenceId);
                if (policyScheduleOutput.ErrorCode != FileServiceOutput.ErrorCodes.Success)
                {
                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.RetrievingMethod = "PolicySchedule";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                    log.ErrorDescription = policyScheduleOutput.ErrorDescription;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                }
                if (policyScheduleOutput.IsSchedule)
                {
                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.ServiceResponseTimeInSeconds = policyScheduleOutput.ServiceResponseTimeInSeconds;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    if (policyScheduleOutput.PolicyFile != null)
                    {
                        policy.PolicyFile = policyScheduleOutput.PolicyFile;
                    }
                    else if (!string.IsNullOrEmpty(policyScheduleOutput.PolicyFileUrl))
                    {
                        policy.PolicyFileUrl = policyScheduleOutput.PolicyFileUrl;
                    }
                    else
                    {
                        log.RetrievingMethod = "PolicySchedule";
                        log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                        log.ErrorDescription = policyScheduleOutput.ErrorDescription;
                        log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return EPolicyStatus.PolicyFileDownloadFailure;
                    }
                }

                byte[] policyFileByteArray = null;
                if (policy.PolicyFile != null)
                {

                    policyFileByteArray = policy.PolicyFile;
                    log.RetrievingMethod = !policyScheduleOutput.IsSchedule ? "Bytes" : "PolicySchedule";
                    log.ServiceResponse = "Success";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.Success;
                    log.ErrorDescription = "Success";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return EPolicyStatus.Available;
                }
                if (!string.IsNullOrEmpty(policy.PolicyFileUrl))
                {
                    log.RetrievingMethod = !policyScheduleOutput.IsSchedule ? "FileUrl" : "PolicySchedule";
                    string fileURL = policy.PolicyFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    log.ServiceURL = fileURL;
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        DateTime dtBeforeCalling = DateTime.Now;
                        policyFileByteArray = client.DownloadData(fileURL);
                        DateTime dtAfterCalling = DateTime.Now;
                        log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        if (policyFileByteArray == null)
                        {
                            log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                            log.ErrorDescription = "policy FileByte Array is returned null";
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return EPolicyStatus.PolicyFileDownloadFailure;
                        }
                        policy.PolicyFile = policyFileByteArray;
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.Success;
                        log.ErrorDescription = "Success";
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return EPolicyStatus.Available;
                    }
                }
                policyFileByteArray = GeneratePolicyFileFromPolicyDetails(policy, iCompanyId, selectedLanguage, log);
                policy.PolicyFile = policyFileByteArray;
                if (policyFileByteArray == null)
                    return EPolicyStatus.PolicyFileGeneraionFailure;
                else
                    return EPolicyStatus.Available;
            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = exp.GetBaseException().ToString();
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return EPolicyStatus.PolicyFileDownloadFailure;
            }
        }

        private byte[] GeneratePolicyFileFromPolicyDetails(PolicyResponse policy, int iCompanyId, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log)
        {
            try
            {
                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;
                log.ReferenceId = policy.ReferenceId;
                log.PolicyNo = policy.PolicyNo;
                log.ServerIP = ServicesUtilities.GetServerIP();
                log.CompanyID = iCompanyId;
                if (string.IsNullOrEmpty(log.Channel))
                    log.Channel = "Portal";
                log.ServiceRequest = JsonConvert.SerializeObject(policy);
                var modelOutput = GetPolicyTemplateGenerationModel(policy);
                if (modelOutput.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
                {
                    log.ErrorDescription = modelOutput.ErrorDescription;
                    log.ErrorCode = (int)modelOutput.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }

                var policyTemplateModel = modelOutput.Output;
                string policyDetailsJsonString = JsonConvert.SerializeObject(policyTemplateModel);
                log.ServiceRequest = policyDetailsJsonString;

                var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(
                    i => i.InsuranceCompanyID == iCompanyId);

                if (insuranceCompany == null)
                {
                    log.ErrorDescription = "Insurance Company Is Null";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }
                ReportGenerationModel reportGenerationModel = new ReportGenerationModel
                {
                    ReportType = insuranceCompany?.ReportTemplateName,
                    ReportDataAsJsonString = policyDetailsJsonString
                };

                log.CompanyName = insuranceCompany?.NameEN;

                reportGenerationModel.ReportType = reportGenerationModel.ReportType.Replace("#ProductType", policyTemplateModel.ProductType);
                if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                    reportGenerationModel.ReportType += "_En";
                else
                    reportGenerationModel.ReportType += "_Ar";
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Service return null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }
                if (response.Content == null)
                {
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Service response content return null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Service response content result return null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ServiceError;
                    log.ErrorDescription = "Service http status code is not ok it returned " + response;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return null;
                }
                log.ServiceResponse = "Success";
                log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
            }
            catch (Exception ex)
            {
                log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.GetBaseException().ToString();
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return null;
            }
        }


        /// <summary>
        /// convert date time from hijri to Georgion 
        /// and return it as string
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns>data time as string</returns>
        private string GetDateWithGeorgion(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                var convertDate = dateTime?.ToString("dd/MM/yyyy HH:mm:ss", cultureEnglish.DateTimeFormat);


                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);

                return convertDate;

            }
            return "";
        }

        private string GetShortDateWithGeorgion(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                var convertDate = dateTime?.ToString("dd/MM/yyyy", cultureEnglish.DateTimeFormat);


                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);

                return convertDate;

            }
            return "";
        }
        private PdfGenerationOutput GetPolicyTemplateGenerationModel(PolicyResponse policy)
        {
            var output = new PdfGenerationOutput();
            try
            {
                var quotationResponseQry = _quotationResponseRepository.Table
                    .Include(e => e.QuotationRequest.Driver.Addresses)
                    .Include(e => e.QuotationRequest.Vehicle.VehicleBodyType)
                    .Include(e => e.QuotationRequest.Vehicle.VehiclePlateType)
                    .Include(e => e.QuotationRequest.Insured)
                    .Include(e => e.QuotationRequest.Drivers.Select(d => d.Addresses))
                    .Include(e => e.ProductType)
                    .Where(e => e.ReferenceId == policy.ReferenceId);

                var quotationResponse = quotationResponseQry.FirstOrDefault();
                if (quotationResponse == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.QuotationResponseIsNull;
                    output.ErrorDescription = "Quotation Response object is null";
                    return output;
                }
                var checkoutDetails = _checkoutDetailRepository.Table.Where(e => e.ReferenceId == policy.ReferenceId).FirstOrDefault();
                if (checkoutDetails == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.CheckoutDetailsIsNull;
                    output.ErrorDescription = "checkout Details object is null";
                    return output;
                }
                var orderItem = _orderItemRepository.Table.Include(x => x.Product.PriceDetails).Where(e => e.CheckoutDetailReferenceId == policy.ReferenceId).FirstOrDefault();
                if (orderItem == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.OrderItemIsNull;
                    output.ErrorDescription = "order Item object is null";
                    return output;
                }
                PolicyTemplateGenerationModel policyTemplateModel = null;
                if (quotationResponse != null)
                {
                    if (policy.PolicyDetails == null)
                        policy.PolicyDetails = new PolicyDetails();
                    policyTemplateModel = policy.PolicyDetails.ToTemplateModel();

                    var insured = quotationResponse.QuotationRequest.Insured;
                    var mainDriver = quotationResponse.QuotationRequest.Driver;
                    var vehicle = quotationResponse.QuotationRequest.Vehicle;
                    var additionalDrivers = quotationResponse.QuotationRequest.Drivers.Where(x => x.NIN.ToUpper() != insured.NationalId.ToUpper()).ToList();
                    Driver secondDriver = null;
                    Driver thirdDriver = null;
                    Driver fourthDriver = null;

                    if (additionalDrivers != null && additionalDrivers.Count > 0)
                        secondDriver = additionalDrivers[0];
                    if (additionalDrivers != null && additionalDrivers.Count > 1)
                        thirdDriver = additionalDrivers[1];
                    if (additionalDrivers != null && additionalDrivers.Count > 2)
                        fourthDriver = additionalDrivers[2];

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredAge) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredAge.Trim()))
                        policyTemplateModel.InsuredAge = insured.BirthDate.GetUserAge().ToString();
                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredBuildingNo) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredBuildingNo.Trim()))
                        policyTemplateModel.InsuredBuildingNo = mainDriver.Addresses.FirstOrDefault()?.BuildingNumber;

                    policyTemplateModel.ProductTypeAr = quotationResponse.ProductType?.ArabicDescription;
                    policyTemplateModel.ProductType = quotationResponse.ProductType?.EnglishDescription;
                    policyTemplateModel.PrintingDate = DateTime.Now.ToString("dd/MM/yyyy");


                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyNo) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyNo.Trim()))
                        policyTemplateModel.PolicyNo = policy.PolicyNo;

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueDate) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueDate.Trim()))
                        policyTemplateModel.PolicyIssueDate = policy.PolicyIssuanceDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueTime) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueTime.Trim()))
                    {
                        if (policy.PolicyIssuanceDate != null && policy.PolicyIssuanceDate.Value.Hour != 0 && policy.PolicyIssuanceDate.Value.Minute != 0)
                            policyTemplateModel.PolicyIssueTime = policy.PolicyIssuanceDate?.ToString("HH:mm", new CultureInfo("en-US"));
                        else
                            policyTemplateModel.PolicyIssueTime = DateTime.Now.ToString("HH:mm", new CultureInfo("en-US"));
                    }

                    //if (string.IsNullOrEmpty(policyTemplateModel.InsuranceStartDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceStartDateH.Trim()))
                    //    policyTemplateModel.InsuranceStartDateH = policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")); ;

                    //if (string.IsNullOrEmpty(policyTemplateModel.InsuranceEndDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceEndDateH.Trim()))
                    //    policyTemplateModel.InsuranceEndDateH = policy.PolicyEffectiveDate?.AddYears(1).AddDays(-1).ToString("dd-MM-yyyy", new CultureInfo("en-US"));


                    //if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueDateH.Trim()))
                    //    policyTemplateModel.PolicyIssueDateH = policy.PolicyIssuanceDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));


                    if (string.IsNullOrEmpty(policyTemplateModel.InsuranceStartDate) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceStartDate.Trim()))
                        policyTemplateModel.InsuranceStartDate = policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")); ;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuranceEndDate) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceEndDate.Trim()))
                        policyTemplateModel.InsuranceEndDate = policy.PolicyEffectiveDate?.AddYears(1).AddDays(-1).ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (policyTemplateModel.PolicyAdditionalCoversAr == null || policyTemplateModel.PolicyAdditionalCoversAr.Count < 1)
                    {
                        if (orderItem != null && orderItem.Product != null && orderItem.Product.Product_Benefits != null)
                        {
                            foreach (var benefit in orderItem.Product.Product_Benefits)
                            {
                                if (benefit != null && benefit.Benefit != null)
                                    policyTemplateModel.PolicyAdditionalCoversAr.Add(benefit.Benefit.ArabicDescription);
                            }
                        }
                    }

                    if (policyTemplateModel.PolicyAdditionalCoversEn == null || policyTemplateModel.PolicyAdditionalCoversEn.Count < 1)
                    {
                        if (orderItem != null && orderItem.Product != null && orderItem.Product.Product_Benefits != null)
                        {
                            foreach (var benefit in orderItem.Product.Product_Benefits)
                            {
                                if (benefit != null && benefit.Benefit != null)
                                    policyTemplateModel.PolicyAdditionalCoversEn.Add(benefit.Benefit.EnglishDescription);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.PACoverForDriverOnly) || string.IsNullOrWhiteSpace(policyTemplateModel.PACoverForDriverOnly.Trim()))
                    {
                        if (orderItem != null && orderItem.Product != null && orderItem.Product.Product_Benefits != null)
                        {
                            foreach (var benefit in orderItem.Product.Product_Benefits)
                            {
                                if (benefit != null && benefit.Benefit != null && (benefit.Benefit.Code == 1 || benefit.Benefit.Code == 2))
                                {
                                    policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    break;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.PACoverForDriverAndPassenger) || string.IsNullOrWhiteSpace(policyTemplateModel.PACoverForDriverAndPassenger.Trim()))
                    {
                        if (orderItem != null && orderItem.Product != null && orderItem.Product.Product_Benefits != null)
                        {
                            foreach (var benefit in orderItem.Product.Product_Benefits)
                            {
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 2)
                                {
                                    policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                    break;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.TotalPremium) || string.IsNullOrWhiteSpace(policyTemplateModel.TotalPremium.Trim()))
                        policyTemplateModel.TotalPremium = orderItem?.Price.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.OfficePremium) || string.IsNullOrWhiteSpace(policyTemplateModel.OfficePremium.Trim()))
                        policyTemplateModel.OfficePremium = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.VATAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.VATAmount.Trim()))
                        policyTemplateModel.VATAmount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 8).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.VATPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.VATPercentage.Trim()))
                        policyTemplateModel.VATPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 8).FirstOrDefault()?.PercentageValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SpecialDiscount.Trim()))
                        policyTemplateModel.SpecialDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.NoClaimDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.NoClaimDiscount.Trim()))
                        policyTemplateModel.NoClaimDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.NCDAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDAmount.Trim()))
                        policyTemplateModel.NCDAmount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.NCDPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDPercentage.Trim()))
                        policyTemplateModel.NCDPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.LoyaltyDiscount.Trim()))
                        policyTemplateModel.LoyaltyDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscountPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.LoyaltyDiscountPercentage.Trim()))
                        policyTemplateModel.LoyaltyDiscountPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PercentageValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.TPLCommissionAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.TPLCommissionAmount.Trim())
                        || string.IsNullOrEmpty(policyTemplateModel.ComprehesiveCommissionAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.ComprehesiveCommissionAmount.Trim()))
                    {
                        var specialDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue;
                        var noClaimDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue;
                        var loyaltyDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue;
                        var additionaLoading = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 4).FirstOrDefault()?.PriceValue;
                        var additionalAgeContribution = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 5).FirstOrDefault()?.PriceValue;
                        var adminFees = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 6).FirstOrDefault()?.PriceValue;
                        var basicPremium = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue;

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
                        if (orderItem?.Product?.QuotationResponse?.InsuranceTypeCode != null)
                        {
                            if (orderItem?.Product?.QuotationResponse?.InsuranceTypeCode.Value == 1)
                                policyTemplateModel.CommissionPaid = policyTemplateModel.TPLCommissionAmount;
                            else
                                policyTemplateModel.CommissionPaid = policyTemplateModel.ComprehesiveCommissionAmount;

                        }
                    }


                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredCity) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredCity.Trim()))
                        policyTemplateModel.InsuredCity = mainDriver.Addresses.First().City;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredDisctrict) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredDisctrict.Trim()))
                        policyTemplateModel.InsuredDisctrict = mainDriver.Addresses.First().District;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredID) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredID.Trim()))
                        policyTemplateModel.InsuredID = insured.NationalId;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredMobile) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredMobile.Trim()))
                        policyTemplateModel.InsuredMobile = checkoutDetails.Phone;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredIbanNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredIbanNumber.Trim()))
                        policyTemplateModel.InsuredIbanNumber = checkoutDetails.IBAN;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredBankName) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredBankName.Trim()))
                        policyTemplateModel.InsuredBankName = checkoutDetails.BankCode?.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameAr) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameAr.Trim()))
                        policyTemplateModel.InsuredNameAr = $"{insured.FirstNameAr} {insured.MiddleNameAr} {insured.LastNameAr}";

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameEn) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameEn.Trim()))
                        policyTemplateModel.InsuredNameEn = $"{insured.FirstNameEn} {insured.MiddleNameEn} {insured.LastNameEn}";

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredStreet) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredStreet.Trim()))
                        policyTemplateModel.InsuredStreet = mainDriver.Addresses.First().Street;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredZipCode) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredZipCode.Trim()))
                        policyTemplateModel.InsuredZipCode = mainDriver.Addresses.First().PostCode;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleBodyAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleBodyAr.Trim()))
                        policyTemplateModel.VehicleBodyAr = vehicle.VehicleBodyType.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleBodyEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleBodyEn.Trim()))
                        policyTemplateModel.VehicleBodyEn = vehicle.VehicleBodyType.EnglishDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleChassis) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleChassis.Trim()))
                        policyTemplateModel.VehicleChassis = vehicle.ChassisNumber;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleColorAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleColorAr.Trim()))
                        policyTemplateModel.VehicleColorAr = vehicle.MajorColor;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleColorEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleColorEn.Trim()))
                        policyTemplateModel.VehicleColorEn = vehicle.MajorColor;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleCustomNo) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleCustomNo.Trim()))
                        policyTemplateModel.VehicleCustomNo = vehicle.CustomCardNumber;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleMakeAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleMakeAr.Trim()))
                        policyTemplateModel.VehicleMakeAr = vehicle.VehicleMaker;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleMakeEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleMakeEn.Trim()))
                        policyTemplateModel.VehicleMakeEn = vehicle.VehicleMaker;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModelAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModelAr.Trim()))
                        policyTemplateModel.VehicleModelAr = vehicle.VehicleModel;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModelEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModelEn.Trim()))
                        policyTemplateModel.VehicleModelEn = vehicle.VehicleModel;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleWeight) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleWeight.Trim()))
                        policyTemplateModel.VehicleWeight = vehicle.VehicleWeight.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleLoad) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleLoad.Trim()))
                        policyTemplateModel.VehicleLoad = vehicle.VehicleLoad.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleUsingPurposesAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleUsingPurposesAr.Trim()))
                        policyTemplateModel.VehicleUsingPurposesAr = vehicle.VehicleUse.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleUsingPurposesEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleUsingPurposesEn.Trim()))
                        policyTemplateModel.VehicleUsingPurposesEn = vehicle.VehicleUse.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleEngineSize) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleEngineSize.Trim()))
                        policyTemplateModel.VehicleEngineSize = vehicle.EngineSize?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOdometerReading) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOdometerReading.Trim()))
                        policyTemplateModel.VehicleOdometerReading = vehicle.CurrentMileageKM?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModificationDetails.Trim()))
                        policyTemplateModel.VehicleModificationDetails = vehicle.ModificationDetails;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOwnerID) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOwnerID.Trim()))
                        policyTemplateModel.VehicleOwnerID = vehicle.CarOwnerNIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOwnerName) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOwnerName.Trim()))
                        policyTemplateModel.VehicleOwnerName = vehicle.CarOwnerName;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoAr.Trim()))
                        policyTemplateModel.VehiclePlateNoAr = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoEn.Trim()))
                        policyTemplateModel.VehiclePlateNoEn = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateText) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateText.Trim()))
                        policyTemplateModel.VehiclePlateText = vehicle.CarPlateText1 + " " + vehicle.CarPlateText2 + " " + vehicle.CarPlateText3;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoEn.Trim()))
                        policyTemplateModel.VehiclePlateNoEn = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateTypeAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateTypeAr.Trim()))
                        policyTemplateModel.VehiclePlateTypeAr = vehicle?.VehiclePlateType?.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateTypeEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateTypeEn.Trim()))
                        policyTemplateModel.VehiclePlateTypeEn = vehicle?.VehiclePlateType?.EnglishDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRegistrationExpiryDate) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRegistrationExpiryDate.Trim()))
                        policyTemplateModel.VehicleRegistrationExpiryDate = vehicle?.LicenseExpiryDate;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleSequenceNo) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleSequenceNo.Trim()))
                        policyTemplateModel.VehicleSequenceNo = vehicle?.SequenceNumber;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleValue) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleValue.Trim()))
                        policyTemplateModel.VehicleValue = vehicle.VehicleValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRegistrationType) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRegistrationType.Trim()))
                        policyTemplateModel.VehicleRegistrationType = vehicle?.VehiclePlateType?.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleYear) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleYear.Trim()))
                        policyTemplateModel.VehicleYear = vehicle?.ModelYear?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.DeductibleValue) || string.IsNullOrWhiteSpace(policyTemplateModel.DeductibleValue.Trim()))
                        policyTemplateModel.DeductibleValue = quotationResponse?.DeductibleValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleAgencyRepairEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleAgencyRepairEn.Trim()))
                        policyTemplateModel.VehicleAgencyRepairEn = quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true ? "Agency" : "Workshop";

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleAgencyRepairAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleAgencyRepairAr.Trim()))
                        policyTemplateModel.VehicleAgencyRepairAr = quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true ? "وكالة" : "ورشة";


                    if (string.IsNullOrEmpty(policyTemplateModel.NCDFreeYears) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDFreeYears.Trim()))
                        policyTemplateModel.NCDFreeYears = quotationResponse.QuotationRequest.NajmNcdFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverName.Trim()))
                        policyTemplateModel.MainDriverName = mainDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverIDNumber.Trim()))
                        policyTemplateModel.MainDriverIDNumber = mainDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverGender.Trim()))
                        policyTemplateModel.MainDriverGender = mainDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverDateofBirth.Trim()))
                        policyTemplateModel.MainDriverDateofBirth = mainDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.MainDriverNumberofyearseligiblefor = mainDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.MainDriverNoClaimsDiscount = mainDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.MainDriverResidentialAddressCity = mainDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.MainDriverFrequencyofDrivingVehicle = mainDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverName.Trim()))
                        policyTemplateModel.SecondDriverName = secondDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverIDNumber.Trim()))
                        policyTemplateModel.SecondDriverIDNumber = secondDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverGender.Trim()))
                        policyTemplateModel.SecondDriverGender = secondDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverDateofBirth.Trim()))
                        policyTemplateModel.SecondDriverDateofBirth = secondDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.SecondDriverNumberofyearseligiblefor = secondDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.SecondDriverNoClaimsDiscount = secondDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.SecondDriverResidentialAddressCity = secondDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.SecondDriverFrequencyofDrivingVehicle = secondDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverName.Trim()))
                        policyTemplateModel.ThirdDriverName = thirdDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverIDNumber.Trim()))
                        policyTemplateModel.ThirdDriverIDNumber = thirdDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverGender.Trim()))
                        policyTemplateModel.ThirdDriverGender = thirdDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverDateofBirth.Trim()))
                        policyTemplateModel.ThirdDriverDateofBirth = thirdDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.ThirdDriverNumberofyearseligiblefor = thirdDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.ThirdDriverNoClaimsDiscount = thirdDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.ThirdDriverResidentialAddressCity = thirdDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle = thirdDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverName.Trim()))
                        policyTemplateModel.FourthDriverName = fourthDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverIDNumber.Trim()))
                        policyTemplateModel.FourthDriverIDNumber = fourthDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverGender.Trim()))
                        policyTemplateModel.FourthDriverGender = fourthDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverDateofBirth.Trim()))
                        policyTemplateModel.FourthDriverDateofBirth = fourthDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.FourthDriverNumberofyearseligiblefor = fourthDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.FourthDriverNoClaimsDiscount = fourthDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.FourthDriverResidentialAddressCity = fourthDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.FourthDriverFrequencyofDrivingVehicle = fourthDriver?.DrivingPercentage?.ToString();


                    policyTemplateModel.Drivers = new List<DriverPloicyGenerationDto>();
                    foreach (var additionalDriver in additionalDrivers)
                    {
                        var driverDto = new DriverPloicyGenerationDto();
                        driverDto.DriverTypeCode = 2;
                        long driverId = 0;
                        long.TryParse(additionalDriver?.NIN, out driverId);
                        driverDto.DriverId = driverId;
                        driverDto.DriverIdTypeCode = additionalDriver.IsCitizen ? 1 : 2;
                        driverDto.DriverBirthDate = additionalDriver.DateOfBirthH;
                        driverDto.DriverBirthDateG = additionalDriver.DateOfBirthG;
                        driverDto.DriverFirstNameAr = additionalDriver.FirstName;
                        driverDto.DriverFirstNameEn = additionalDriver.EnglishFirstName;
                        driverDto.DriverMiddleNameAr = additionalDriver.SecondName;
                        driverDto.DriverMiddleNameEn = additionalDriver.EnglishSecondName;
                        driverDto.DriverLastNameAr = additionalDriver.LastName;
                        driverDto.DriverLastNameEn = additionalDriver.EnglishLastName;
                        driverDto.DriverFullNameAr = $"{additionalDriver.FullArabicName}";
                        driverDto.DriverFullNameEn = $"{additionalDriver.FullEnglishName}";
                        driverDto.IsAgeLessThen21YearsAR = DateTime.Now.Year - additionalDriver.DateOfBirthG.Year < 21 ? "نعم" : "لا";
                        driverDto.IsAgeLessThen21YearsEN = DateTime.Now.Year - additionalDriver.DateOfBirthG.Year < 21 ? "Yes" : "No";
                        driverDto.DriverOccupation = additionalDriver.ResidentOccupation;
                        driverDto.DriverNOALast5Years = additionalDriver.NOALast5Years;
                        driverDto.DriverNOCLast5Years = additionalDriver.NOCLast5Years;
                        driverDto.DriverNCDFreeYears = additionalDriver.NCDFreeYears;
                        driverDto.DriverNCDReference = additionalDriver.NCDReference;
                        driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                        driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                        driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                                additionalDriver.NationalityCode.Value.ToString() : "113";
                        driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                        driverDto.DriverDrivingPercentage = additionalDriver.DrivingPercentage;
                        driverDto.DriverEducationCode = additionalDriver.EducationId;
                        driverDto.DriverMedicalConditionCode = additionalDriver.MedicalConditionId;
                        driverDto.DriverChildrenBelow16Years = additionalDriver.ChildrenBelow16Years;
                        driverDto.DriverHomeCityCode = additionalDriver.City?.Code.ToString();
                        driverDto.DriverHomeCity = additionalDriver.City?.ArabicDescription;
                        driverDto.DriverWorkCityCode = additionalDriver.WorkCity?.Code.ToString();
                        driverDto.DriverWorkCity = additionalDriver.WorkCity?.ArabicDescription;
                        driverDto.NCDAmount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue.ToString();
                        driverDto.NCDPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString();

                        policyTemplateModel.Drivers.Add(driverDto);
                    }
                }
                output.Output = policyTemplateModel;
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().ToString();
                return output;
            }
        }

        private PolicyResponse GetPolicy(PolicyRequest policyRequestMessage, int companyId, ServiceRequestLog predefinedLogInfo)
        {
            //_logger.Log($"PolicyRequestService -> GetPolicy >>> Start get policy <<< (policy request message : {JsonConvert.SerializeObject(policyRequestMessage)}, company id : {companyId})");
            var providerFullTypeName = string.Empty;
            var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(
                i => i.InsuranceCompanyID == companyId);

            if (insuranceCompany != null)
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

            PolicyResponse results = null;

            var scope = EngineContext.Current.ContainerManager.Scope();
            var providerType = Type.GetType(providerFullTypeName);
            IInsuranceProvider provider = null;
            if (providerType != null)
            {
                object instance;
                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
            }
            if (provider == null)
            {
                _logger.Log($"PolicyRequestService -> GetPolicy >>> Unable to find insurance provider <<< (policy request message : {JsonConvert.SerializeObject(policyRequestMessage)}, company id : {companyId})");
                throw new Exception("Unable to find provider.");
            }
            if (provider != null)
            {
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredStreet) || string.IsNullOrWhiteSpace(policyRequestMessage.InsuredStreet.Trim()) || policyRequestMessage.InsuredStreet == "0")
                {
                    policyRequestMessage.InsuredStreet = "غير معروف";//WebResources.Unknown;
                }
                results = provider.GetPolicy(policyRequestMessage, predefinedLogInfo);
            }
            _logger.Log($"PolicyRequestService -> GetPolicy >>> End get policy <<< (policy request message : {JsonConvert.SerializeObject(policyRequestMessage)}, company id : {companyId}, result : {JsonConvert.SerializeObject(results)})");
            return results;

        }


        private PolicyRequest GeneratePolicyRequestMessage(CheckoutDetail checkoutDetails, string referenceId)
        {
            _logger.Log($"PolicyRequestService -> GeneratePolicyRequestMessage started, checkoutdetails.PolicyStatusId: {checkoutDetails.PolicyStatusId}");
            if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PaymentSuccess || checkoutDetails.PolicyStatusId == (int)EPolicyStatus.Pending)
            {
                var quoteResponse = _quotationResponseRepository.Table
                    .Include(e => e.QuotationRequest)
                    .Include(e => e.QuotationRequest.Vehicle)
                    .Include(e => e.QuotationRequest.Driver)
                    .FirstOrDefault(q => q.ReferenceId == referenceId);
                //get the recent invoice
                var invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                var orderItem = checkoutDetails.OrderItems.FirstOrDefault();

                // Commented By Ali
                //var selectedBenefitsData = orderItem.Product.Product_Benefits
                //        .Where(b => b.IsSelected.HasValue && b.IsSelected.Value);

                var selectedBenefits = orderItem.OrderItemBenefits.Select(b =>
                    new BenefitRequest()
                    {
                        BenefitId = b.BenefitExternalId,
                    }).ToList();

                var policyRequestMessagge = new PolicyRequest()
                {
                    ReferenceId = referenceId,
                    QuotationNo = orderItem.Product.QuotaionNo,
                    ProductId = orderItem.Product.ExternalProductId,
                    Benefits = selectedBenefits,
                    InsuredId = long.Parse(checkoutDetails.Driver.NIN),
                    InsuredMobileNumber = Utilities.Remove966FromMobileNumber(checkoutDetails.Phone),
                    InsuredEmail = checkoutDetails.Email,
                    InsuredBuildingNo = int.Parse(checkoutDetails.Driver.Addresses.First().BuildingNumber),
                    InsuredZipCode = int.Parse(checkoutDetails.Driver.Addresses.First().PostCode),
                    InsuredAdditionalNumber = int.Parse(checkoutDetails.Driver.Addresses.First().AdditionalNumber),
                    InsuredUnitNo = int.Parse(checkoutDetails.Driver.Addresses.First().UnitNumber),
                    InsuredCity = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().City) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().City,
                    InsuredDistrict = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().District) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().District,
                    InsuredStreet = string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.First().Street) ? "غير معروف"
                        : checkoutDetails.Driver.Addresses.First().Street,
                    PaymentMethodCode = checkoutDetails.PaymentMethodId.GetValueOrDefault(),
                    PaymentAmount = invoice.TotalPrice.Value,
                    PaymentBillNumber = invoice.InvoiceNo.ToString(),

                    InsuredBankCode = checkoutDetails.BankCode.Code,
                    PaymentUsername = "UNKNOWN",
                    InsuredIBAN = checkoutDetails.IBAN,
                    PolicyEffectiveDate = quoteResponse?.QuotationRequest?.RequestPolicyEffectiveDate,
                    QuotationRequestId = quoteResponse.QuotationRequest.ID,
                    ProductInternalId = orderItem.Product.Id


                };

                return policyRequestMessagge;
            }

            return null;
        }

        public PolicyOutput GeneratePolicyManually(PolicyData policyInfo)
        {
            PolicyOutput output = new PolicyOutput();
            DateTime dtBefore = DateTime.Now;
            try
            {
                if (policyInfo == null)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = "policyInfo is null";
                    return output;
                }
                if (string.IsNullOrEmpty(policyInfo.PolicyNo))
                {
                    output.ErrorCode = 3;
                    output.ErrorDescription = "PolicyNo is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(policyInfo.ReferenceId))
                {
                    output.ErrorCode = 4;
                    output.ErrorDescription = "ReferenceId is empty";
                    return output;
                }
                if (!policyInfo.PolicyIssuanceDate.HasValue)
                {
                    output.ErrorCode = 5;
                    output.ErrorDescription = "PolicyIssuanceDate is empty";
                    return output;
                }
                if (!policyInfo.PolicyEffectiveDate.HasValue)
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "PolicyEffectiveDate is empty";
                    return output;
                }
                if (!policyInfo.PolicyExpiryDate.HasValue)
                {
                    output.ErrorCode = 7;
                    output.ErrorDescription = "PolicyExpiryDate is empty";
                    return output;
                }
                if (policyInfo.PolicyFile == null)
                {
                    output.ErrorCode = 8;
                    output.ErrorDescription = "PolicyFile is empty";
                    return output;
                }
                var checkoutDetails = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == policyInfo.ReferenceId);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = 9;
                    output.ErrorDescription = "checkoutDetails is null";
                    return output;
                }
                if (checkoutDetails.PolicyStatusId == 4)
                {
                    output.ErrorCode = 10;
                    output.ErrorDescription = "Policy is already success";
                    return output;
                }
                PolicyResponse policy = new PolicyResponse();
                policy.ReferenceId = policyInfo.ReferenceId;
                policy.PolicyNo = policyInfo.PolicyNo;
                policy.PolicyEffectiveDate = policyInfo.PolicyEffectiveDate;
                policy.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                policy.PolicyFile = policyInfo.PolicyFile;
                policy.PolicyIssuanceDate = policyInfo.PolicyIssuanceDate;

                var policyId = SavePolicy(policy, policyInfo.CompanyID);
                if (!policyId.HasValue)
                {
                    output.ErrorCode = 11;
                    output.ErrorDescription = "failed to save policy as it may be exist";
                    return output;
                }
                var invoice = _invoiceRepository.Table.FirstOrDefault(c => c.ReferenceId == policy.ReferenceId);
                if (invoice != null)
                {
                    invoice.PolicyId = policyId;
                    _invoiceRepository.Update(invoice);
                }
                //QuotationResponse quotationResponse = new QuotationResponse();
                //quotationResponse.InsuranceCompany.InsuranceCompanyID =  policyInfo.CompanyID;
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                PdfGenerationLog log = new PdfGenerationLog();
                log.Channel = policyInfo.Channel;
                //var policyStatus = HandleInsurancePolicyFileResult(policy, policyInfo.CompanyID, lang, log);
                //if (policyStatus != EPolicyStatus.Available)
                //{
                //    output.ErrorCode = 1;
                //    output.ErrorDescription = "Failed to get pdf file";
                //    return output;
                //}
                SavePolicyFile(policy);
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Available;
                _checkoutDetailRepository.Update(checkoutDetails);
               
                var policyProcessingQueue = _policyProcessingQueue.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == policy.ReferenceId);
                if (policyProcessingQueue != null)
                {
                    policyProcessingQueue.ProcessedOn = DateTime.Now;
                    DateTime dtAfter = DateTime.Now;
                    policyProcessingQueue.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    _policyProcessingQueue.Update(policyProcessingQueue);
                }
                SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                {
                    PolicyResponseMessage = policy,
                    ReceiverEmailAddress = checkoutDetails.Email,
                    ReferenceId = policy.ReferenceId,
                    UserLanguage = lang,
                    PolicyFileByteArray = policy.PolicyFile,
                    IsPolicyGenerated = true,
                    IsShowErrors = false
                };
                _policyEmailService.SendPolicyViaMail(sendPolicyViaMailDto).Wait();
                output.ErrorCode = 1;
                output.ErrorDescription = "Success";
                return output;
            }

            catch (Exception exp)
            {
                output.ErrorCode = 12;
                output.ErrorDescription = exp.GetBaseException().ToString();
                return output;
            }
        }

        public int? SavePolicy(PolicyResponse policy, int? companyId)
        {
            int? result = null;
            var isExist = _policyRepository.Table.Any(a => a.CheckOutDetailsId == policy.ReferenceId);
            if (!isExist)
            {
                var policyData = new Policy
                {
                    CheckOutDetailsId = policy.ReferenceId,
                    PolicyEffectiveDate = policy.PolicyEffectiveDate,
                    PolicyExpiryDate = policy.PolicyExpiryDate,
                    PolicyIssueDate = policy.PolicyIssuanceDate,
                    PolicyNo = policy.PolicyNo,
                    StatusCode = Convert.ToByte(policy.StatusCode),
                    InsuranceCompanyID = companyId
                };

                var jsonSerializer = new JavaScriptSerializer();

                PolicyDetail policyDetail = null;
                if (policy.PolicyDetails != null)
                {
                    policyDetail = new PolicyDetail()
                    {
                        Id = policyData.Id,
                        Policy = policyData,
                        DocumentSerialNo = policy.PolicyDetails.DocumentSerialNo,
                        PolicyNo = policy.PolicyDetails.PolicyNo,
                        InsuranceStartDate = policy.PolicyDetails.InsuranceStartDate,
                        InsuranceEndDate = policy.PolicyDetails.InsuranceEndDate,
                        PolicyCoverTypeEn = policy.PolicyDetails.PolicyCoverTypeEn,
                        PolicyCoverTypeAr = policy.PolicyDetails.PolicyCoverTypeAr,
                        PolicyCoverAgeLimitEn = jsonSerializer.Serialize(policy.PolicyDetails.PolicyCoverAgeLimitEn),
                        PolicyCoverAgeLimitAr = jsonSerializer.Serialize(policy.PolicyDetails.PolicyCoverAgeLimitAr),
                        PolicyAdditionalCoversEn = jsonSerializer.Serialize(policy.PolicyDetails.PolicyAdditionalCoversEn),
                        PolicyAdditionalCoversAr = jsonSerializer.Serialize(policy.PolicyDetails.PolicyAdditionalCoversAr),
                        PolicyGeographicalAreaEn = policy.PolicyDetails.PolicyGeographicalAreaEn,
                        PolicyGeographicalAreaAr = policy.PolicyDetails.PolicyGeographicalAreaAr,
                        InsuredNameEn = policy.PolicyDetails.InsuredNameEn,
                        InsuredNameAr = policy.PolicyDetails.InsuredNameAr,
                        InsuredMobile = policy.PolicyDetails.InsuredMobile,
                        InsuredID = policy.PolicyDetails.InsuredID,
                        InsuredCity = policy.PolicyDetails.InsuredCity,
                        InsuredDisctrict = policy.PolicyDetails.InsuredDisctrict,
                        InsuredStreet = policy.PolicyDetails.InsuredStreet,
                        InsuredBuildingNo = policy.PolicyDetails.InsuredBuildingNo,
                        InsuredZipCode = policy.PolicyDetails.InsuredZipCode,
                        InsuredAdditionalNo = policy.PolicyDetails.InsuredAdditionalNo,
                        VehicleMakeEn = policy.PolicyDetails.VehicleMakeEn,
                        VehicleMakeAr = policy.PolicyDetails.VehicleMakeAr,
                        VehicleModelEn = policy.PolicyDetails.VehicleModelEn,
                        VehicleModelAr = policy.PolicyDetails.VehicleModelAr,
                        VehiclePlateTypeEn = policy.PolicyDetails.VehiclePlateTypeEn,
                        VehiclePlateTypeAr = policy.PolicyDetails.VehiclePlateTypeAr,
                        VehiclePlateNoEn = policy.PolicyDetails.VehiclePlateNoEn,
                        VehiclePlateNoAr = policy.PolicyDetails.VehiclePlateNoAr,
                        VehicleChassis = policy.PolicyDetails.VehicleChassis,
                        VehicleBodyEn = policy.PolicyDetails.VehicleBodyEn,
                        VehicleBodyAr = policy.PolicyDetails.VehicleBodyAr,
                        VehicleYear = policy.PolicyDetails.VehicleYear,
                        VehicleColorEn = policy.PolicyDetails.VehicleColorEn,
                        VehicleColorAr = policy.PolicyDetails.VehicleColorAr,
                        VehicleCapacity = policy.PolicyDetails.VehicleCapacity,
                        VehicleSequenceNo = policy.PolicyDetails.VehicleSequenceNo,
                        VehicleCustomNo = policy.PolicyDetails.VehicleCustomNo,
                        VehicleOwnerName = policy.PolicyDetails.VehicleOwnerName,
                        VehicleOwnerID = policy.PolicyDetails.VehicleOwnerID,
                        VehicleUsingPurposesEn = policy.PolicyDetails.VehicleUsingPurposesEn,
                        VehicleUsingPurposesAr = policy.PolicyDetails.VehicleUsingPurposesAr,
                        VehicleRegistrationExpiryDate = policy.PolicyDetails.VehicleRegistrationExpiryDate,
                        VehicleValue = policy.PolicyDetails.VehicleValue,
                        OfficePremium = policy.PolicyDetails.OfficePremium,
                        PACover = policy.PolicyDetails.PACover,
                        ValueExcess = policy.PolicyDetails.ValueExcess,
                        TotalPremium = policy.PolicyDetails.TotalPremium,
                        NCDPercentage = policy.PolicyDetails.NCDPercentage,
                        NCDAmount = policy.PolicyDetails.NCDAmount,
                        VATPercentage = policy.PolicyDetails.VATPercentage,
                        VATAmount = policy.PolicyDetails.VATAmount,
                        CommissionPaid = policy.PolicyDetails.CommissionPaid,
                        PolicyFees = policy.PolicyDetails.PolicyFees,
                        ClalmLoadingPercentage = policy.PolicyDetails.ClalmLoadingPercentage,
                        ClalmLoadingAmount = policy.PolicyDetails.ClalmLoadingAmount,
                        AgeLoadingAmount = policy.PolicyDetails.AgeLoadingAmount,
                        AgeLoadingPercentage = policy.PolicyDetails.AgeLoadingPercentage,
                        DeductibleValue = policy.PolicyDetails.DeductibleValue,
                        InsuranceEndDateH = policy.PolicyDetails.InsuranceEndDateH,
                        InsuranceStartDateH = policy.PolicyDetails.InsuranceStartDateH,
                        InsuredAge = policy.PolicyDetails.InsuredAge,
                        NCDFreeYears = policy.PolicyDetails.NCDFreeYears,
                        AccidentNo = policy.PolicyDetails.AccidentNo,
                        AccidentLoadingAmount = policy.PolicyDetails.AccidentLoadingAmount,
                        AccidentLoadingPercentage = policy.PolicyDetails.AccidentLoadingPercentage,
                        PolicyIssueDate = policy.PolicyDetails.PolicyIssueDate,
                        PolicyIssueTime = policy.PolicyDetails.PolicyIssueTime
                    };
                    policyData.PolicyDetail = policyDetail;
                }

                _policyRepository.Insert(policyData);
                result = policyData.Id;
            }


            return result;
        }

        private void UpdatePolicyStatus(CheckoutDetail checkoutDetail, EPolicyStatus status)
        {
            //update policy status 
            checkoutDetail.PolicyStatusId = (int)status;
            //save the updates into db
            _checkoutDetailRepository.Update(checkoutDetail);
        }

        private Guid SavePolicyFile(PolicyResponse policy)
        {
            Guid policyFileId = Guid.NewGuid();
            PolicyFile policyFile = new PolicyFile
            {
                ID = policyFileId,
                PolicyFileByte = policy.PolicyFile
            };
            _policyFileRepository.Insert(policyFile);
            //get policy entity 
            Policy policyEntity = _policyRepository.Table.FirstOrDefault(p => p.CheckOutDetailsId == policy.ReferenceId);
            //update policy fileId 
            policyEntity.PolicyFileId = policyFileId;
            //save the updates into db
            _policyRepository.Update(policyEntity);
            return policyFileId;
        }

        #region Private Methods

        /// <summary>
        /// Log Policy Response error if the policy has errors
        /// </summary>
        /// <param name="policyResponseMessage"></param>

        private void LogPolicyResponseErrorsIfExist(PolicyResponse policyResponseMessage)
        {
            if (policyResponseMessage != null)
            {
                if (policyResponseMessage.Errors != null)
                {
                    string errorsMessagesAsString = string.Empty;
                    foreach (var error in policyResponseMessage.Errors)
                    {
                        errorsMessagesAsString = error.Message + Environment.NewLine;
                    }
                    _logger.Log($"Policy Response Errors {errorsMessagesAsString}");
                }
            }
        }


        #endregion

    }
}

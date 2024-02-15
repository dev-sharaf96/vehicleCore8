using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Common.Utilities;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Resources.Checkout;
using Tameenk.Services.Core;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyModificationContext: IPolicyModificationContext
    {
       
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<PolicyModification> _policyModificationRepository;
        private readonly IRepository<QuotationResponse> _quotationResponse;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IPolicyFileContext _policyFileContext;
        private readonly IPolicyEmailService _policyEmailService;
        private readonly ICheckoutContext _checkoutContext;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueue;
        private readonly IRepository<Endorsment> _endorsmentRepository;
        private readonly IRepository<EndorsmentBenefit> _endorsmentBenefitRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderItemBenefit> _orderItemBenefitRepository;
        private readonly IRepository<PolicyAdditionalBenefit> _policyAdditionalBenefitRepository;
        private readonly IAutoleasingUserService _autoleasingUserService;

        public PolicyModificationContext(IRepository<CheckoutDetail> checkoutDetailrepository
            , IRepository<NajmStatus> najmStatusRepo
              , IYakeenClient yakeenClient
            , IInsuranceCompanyService insuranceCompanyService
            , IRepository<CheckoutDetail> checkoutDetailRepository
            , IRepository<PolicyModification> policyModificationRepository
            , IRepository<QuotationResponse> quotationResponse
            , IPolicyFileContext policyFileContext, IPolicyEmailService policyEmailService,
            ICheckoutContext checkoutContext,
             IRepository<PolicyProcessingQueue> policyProcessingQueue,
             IRepository<Endorsment> endorsmentRepository,
             TameenkConfig tameenkConfig, IRepository<QuotationRequest> quotationRequestRepository
            , IRepository<OrderItem> orderItemRepository, IRepository<OrderItemBenefit> orderItemBenefitRepository
            , IRepository<PolicyAdditionalBenefit> policyAdditionalBenefitRepository
            , IRepository<EndorsmentBenefit> endorsmentBenefitRepository
            , IAutoleasingUserService autoleasingUserService)
        {
            _checkoutDetailRepository = checkoutDetailrepository;
            _yakeenClient = yakeenClient;
            _insuranceCompanyService = insuranceCompanyService;
            _policyModificationRepository = policyModificationRepository;
            _quotationResponse = quotationResponse;
            _policyFileContext = policyFileContext;
            _policyEmailService = policyEmailService;
            _checkoutContext = checkoutContext;
            _policyProcessingQueue = policyProcessingQueue;
            _endorsmentRepository = endorsmentRepository;
            _endorsmentBenefitRepository = endorsmentBenefitRepository;
            _tameenkConfig = tameenkConfig;
            _quotationRequestRepository = quotationRequestRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemBenefitRepository = orderItemBenefitRepository;
            _policyAdditionalBenefitRepository = policyAdditionalBenefitRepository;
            _autoleasingUserService = autoleasingUserService;
        }

        public AddDriverOutput PurchaseVechileDriver(Models.Checkout.PurchaseDriverModel model, string UserId, string userName)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.MethodName = model?.Channel.ToString().ToLower() == "autoleasing" ? "PurchaseDriver" : "PurchaseVechileDriver";
            log.UserId = UserId.ToString();
            log.UserName = userName;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Request Model is Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var policyModificationInfo = _policyModificationRepository.Table.Where(x => x.QuotationReferenceId == model.ReferenceId|| x.ReferenceId == model.ReferenceId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (policyModificationInfo == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(policyModificationInfo.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Insurance company Id is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                var quotationResponse = _quotationResponse.TableNoTracking.Where(x => x.ReferenceId == policyModificationInfo.QuotationReferenceId).FirstOrDefault();
                if (quotationResponse == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "quotationResponseis is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                QuotationRequest quotationRequest = _quotationRequestRepository.Table.Where(a => a.ID == quotationResponse.RequestId).FirstOrDefault();
                if (quotationRequest == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "quotationRequest is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (quotationRequest.AdditionalDriverIdOne.HasValue && quotationRequest.AdditionalDriverIdTwo.HasValue)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "policy reached the maximum allowed drivers";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                CheckoutDetail checkoutDetail = _checkoutDetailRepository.Table.Where(x => x.ReferenceId == policyModificationInfo.QuotationReferenceId && x.IsCancelled == false).FirstOrDefault();
                if (checkoutDetail == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "checkoutDetail is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (checkoutDetail.AdditionalDriverIdOne.HasValue && checkoutDetail.AdditionalDriverIdTwo.HasValue)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "policy reached the maximum allowed drivers";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = policyModificationInfo.QuotationReferenceId
                };
                predefinedLogInfo.DriverNin = policyModificationInfo.Nin;
                PurchaseDriverRequest serviceRequest = new PurchaseDriverRequest();
                serviceRequest.PolicyNo = policyModificationInfo.PolicyNo;
                serviceRequest.ReferenceId = policyModificationInfo.ReferenceId;
                serviceRequest.PaymentAmount = policyModificationInfo.TotalAmount.Value;
                serviceRequest.PaymentBillNumber = policyModificationInfo.InvoiceNo.ToString();
                IInsuranceProvider provider = GetProvider(insuranceCompany, policyModificationInfo.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                PurchaseDriverResponse results = null;

                bool isAutoleasingPolicy = false;
                if (checkoutDetail.Channel.ToLower() == "autoleasing")
                {
                    isAutoleasingPolicy = true;
                    results = provider.PurchaseDriver(serviceRequest, predefinedLogInfo);
                }
                else
                {
                    isAutoleasingPolicy = false;
                    results = provider.PurchaseVechileDriver(serviceRequest, predefinedLogInfo);
                }
                if (results == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (!quotationRequest.AdditionalDriverIdOne.HasValue)
                {
                    quotationRequest.AdditionalDriverIdOne = policyModificationInfo.DriverId;
                }
                else if (!quotationRequest.AdditionalDriverIdTwo.HasValue)
                {
                    quotationRequest.AdditionalDriverIdTwo = policyModificationInfo.DriverId;
                }
                _quotationRequestRepository.Update(quotationRequest);

                if (!checkoutDetail.AdditionalDriverIdOne.HasValue)
                {
                    checkoutDetail.AdditionalDriverIdOne = policyModificationInfo.DriverId;
                }
                else if (!checkoutDetail.AdditionalDriverIdTwo.HasValue)
                {
                    checkoutDetail.AdditionalDriverIdTwo = policyModificationInfo.DriverId;
                }
                string filePath = string.Empty;
                string exception = string.Empty;
                Endorsment endorsment = new Endorsment();
                byte[] bytes = null;
                bool generatedFromBCare = false;
                if(string.IsNullOrEmpty(results.EndorsementFileUrl)&& results.EndorsementFile==null)
                {
                    var processingqueue = _policyProcessingQueue.Table.Where(x => x.ReferenceId == checkoutDetail.ReferenceId && x.IsCancelled == false).FirstOrDefault();
                    if (processingqueue == null)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "processingqueue is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    checkoutDetail.PolicyStatusId = 7;
                    checkoutDetail.ModifiedDate = DateTime.Now;
                    _checkoutDetailRepository.Update(checkoutDetail);

                    processingqueue.ProcessedOn = null;
                    processingqueue.ProcessingTries = 0;
                    _policyProcessingQueue.Update(processingqueue);
                    generatedFromBCare = true;
                }
                else if (!string.IsNullOrEmpty(results.EndorsementFileUrl))
                {
                    string fileURL = results.EndorsementFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        bytes = client.DownloadData(fileURL);
                    }
                }
                else if (results.EndorsementFile != null)
                {
                    bytes = results.EndorsementFile;
                }
                if (!generatedFromBCare)
                {
                    filePath = Utilities.SaveCompanyFileFromDashboard(policyModificationInfo.ReferenceId, bytes, insuranceCompany.Key, true,
                            _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                            _tameenkConfig.RemoteServerInfo.DomainName,
                            _tameenkConfig.RemoteServerInfo.ServerIP,
                            _tameenkConfig.RemoteServerInfo.ServerUserName,
                            _tameenkConfig.RemoteServerInfo.ServerPassword,
                            isAutoleasingPolicy,
                            out exception);
                    if (!string.IsNullOrWhiteSpace(exception))
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "failed to save file";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to save file duo to exception : " + exception;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    endorsment.FilePath = filePath;
                }
                endorsment.InsurranceCompanyId = insuranceCompany.InsuranceCompanyID;
                endorsment.PolicyModificationRequestId = policyModificationInfo.Id;
                endorsment.ReferenceId = policyModificationInfo.ReferenceId;
                endorsment.Channel = log.Channel;
                endorsment.ServerIP = log.ServerIP;
                endorsment.UserAgent = log.UserAgent;
                endorsment.UserIP = log.UserIP;
                endorsment.CreatedDate = DateTime.Now;
                endorsment.QuotationReferenceId = policyModificationInfo.QuotationReferenceId;
                _endorsmentRepository.Insert(endorsment);

                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }

        public AddBenefitOutput PurchaseVechileBenefit(Models.Checkout.PurchaseBenefitModel model, string UserId, string userName)
        {
            AddBenefitOutput output = new AddBenefitOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.MethodName = model?.Channel.ToString().ToLower()== "autoleasing" ? "PurchaseBenefit" : "PurchaseVechileBenefit";
            log.UserId = UserId;
            log.UserName = userName;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.InvaildRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var policyModificationInfo = _policyModificationRepository.Table.Where(x => x.QuotationReferenceId == model.ReferenceId|| x.ReferenceId == model.ReferenceId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (policyModificationInfo == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(policyModificationInfo.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Insurance company Id is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = policyModificationInfo.QuotationReferenceId
                };
                //Calculate total ammount of benefits 
                CheckoutDetail checkoutDetail = _checkoutDetailRepository.Table.FirstOrDefault(x => x.ReferenceId == policyModificationInfo.QuotationReferenceId);
                if (checkoutDetail == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "checkoutDetail is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.Channel = checkoutDetail.Channel;
                var orderItem = _orderItemRepository.TableNoTracking.Where(x => x.CheckoutDetailReferenceId == policyModificationInfo.QuotationReferenceId).FirstOrDefault();
                if (orderItem == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "orderItems is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                decimal totalAmmount = 0;

                var orderBenefits = _orderItemBenefitRepository.Table.Where(x => x.OrderItemId == orderItem.Id);
                if (model.Benefits != null && model.Benefits.Count() > 0)
                {
                    foreach (var benefit in model.Benefits)
                    {
                        if (orderBenefits.Any(x => x.BenefitExternalId == benefit.BenefitId))
                        {
                            model.Benefits.Remove(benefit);
                        }
                        else
                        {
                            totalAmmount += _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.BenefitId == benefit.BenefitId).Select(x => x.BenefitPrice).FirstOrDefault().Value * 1.15M;
                        }
                    }
                }
                PurchaseBenefitRequest serviceRequest = new PurchaseBenefitRequest();
                serviceRequest.PolicyNo = policyModificationInfo.PolicyNo;
                serviceRequest.ReferenceId = policyModificationInfo.ReferenceId;
                serviceRequest.PaymentAmount = (double)totalAmmount;
                serviceRequest.PaymentBillNumber = policyModificationInfo.InvoiceNo.ToString();
                serviceRequest.Benefits = model.Benefits;

                IInsuranceProvider provider = GetProvider(insuranceCompany, policyModificationInfo.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                PurchaseBenefitResponse results =null;

                bool isAutoleasingPolicy = false;
                if (checkoutDetail.Channel.ToLower() == "autoleasing")
                {
                    isAutoleasingPolicy = true;
                    if (policyModificationInfo.InsuranceCompanyId.Value == 12)
                    {
                        serviceRequest.LesseeID = checkoutDetail.Driver?.NIN;
                        serviceRequest.LessorID = _autoleasingUserService.GetUser(UserId)?.BankId.ToString();
                    }
                    results = provider.AutoleasingPurchaseBenefit(serviceRequest, predefinedLogInfo);
                }
                else
                {
                    isAutoleasingPolicy = false;
                    results = provider.PurchaseVechileBenefit(serviceRequest, predefinedLogInfo);
                }
                if (results == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                string filePath = string.Empty;
                string exception = string.Empty;
                Endorsment endorsment = new Endorsment();
                byte[] bytes = null;
                bool generatedFromBCare = false;
                if (string.IsNullOrEmpty(results.EndorsementFileUrl) && results.EndorsementFile == null)
                {
                    foreach (var benefit in model.Benefits)
                    {
                        var benfitIfo = _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.BenefitId == benefit.BenefitId).FirstOrDefault();
                        if (benfitIfo == null)
                            continue;

                        OrderItemBenefit ob = new OrderItemBenefit();
                        ob.BenefitId =(short)benfitIfo.BenefitCode.Value;
                        ob.BenefitExternalId = benfitIfo.BenefitId;
                        ob.OrderItemId = orderItem.Id;
                        ob.Price = benfitIfo.BenefitPrice.HasValue ? benfitIfo.BenefitPrice.Value : 0;
                        _orderItemBenefitRepository.Insert(ob);
                    }
                    var processingqueue = _policyProcessingQueue.Table.Where(x => x.ReferenceId == checkoutDetail.ReferenceId && x.IsCancelled == false).FirstOrDefault();
                    if (processingqueue == null)
                    {
                        output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "processingqueue is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    checkoutDetail.PolicyStatusId = 7;
                    checkoutDetail.ModifiedDate = DateTime.Now;
                    _checkoutDetailRepository.Update(checkoutDetail);

                    processingqueue.ProcessedOn = null;
                    processingqueue.ProcessingTries = 0;
                    _policyProcessingQueue.Update(processingqueue);
                    generatedFromBCare = true;
                }
                else if (!string.IsNullOrEmpty(results.EndorsementFileUrl))
                {
                    string fileURL = results.EndorsementFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        bytes = client.DownloadData(fileURL);
                    }
                }
                else if (results.EndorsementFile != null)
                {
                    bytes = results.EndorsementFile;
                }
                if (!generatedFromBCare)
                {
                    filePath = Utilities.SaveCompanyFileFromDashboard(policyModificationInfo.ReferenceId, bytes, insuranceCompany.Key, true,
                            _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                            _tameenkConfig.RemoteServerInfo.DomainName,
                            _tameenkConfig.RemoteServerInfo.ServerIP,
                            _tameenkConfig.RemoteServerInfo.ServerUserName,
                            _tameenkConfig.RemoteServerInfo.ServerPassword,
                            isAutoleasingPolicy,
                            out exception);
                    if (!string.IsNullOrWhiteSpace(exception))
                    {
                        output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "failed to save file";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to save file duo to exception : " + exception;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    endorsment.FilePath = filePath;
                }
                endorsment.InsurranceCompanyId = insuranceCompany.InsuranceCompanyID;
                endorsment.PolicyModificationRequestId = policyModificationInfo.Id;
                endorsment.ReferenceId = policyModificationInfo.ReferenceId;
                endorsment.Channel = log.Channel;
                endorsment.ServerIP = log.ServerIP;
                endorsment.UserAgent = log.UserAgent;
                endorsment.UserIP = log.UserIP;
                endorsment.CreatedDate = DateTime.Now;
                endorsment.QuotationReferenceId = policyModificationInfo.QuotationReferenceId;
                _endorsmentRepository.Insert(endorsment);
                if (endorsment.Id < 1)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "Failed to insert Endorsment request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to insert Endorsment request";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                List<EndorsmentBenefit> endorsmentBenefits = new List<EndorsmentBenefit>();
                foreach (var benefit in model.Benefits)
                {
                    EndorsmentBenefit endorsmentBenefit = new EndorsmentBenefit()
                    {
                        BenefitId = benefit.BenefitId,
                        CreatedDate = DateTime.Now,
                        EndorsmentId = endorsment.Id,
                        QuotationReferenceId = endorsment.QuotationReferenceId,
                        ReferenceId = endorsment.ReferenceId
                    };
                    endorsmentBenefits.Add(endorsmentBenefit);
                }
                _endorsmentBenefitRepository.Insert(endorsmentBenefits);
                output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Purchase benefits";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private IInsuranceProvider GetProvider(InsuranceCompany insuranceCompany, int type)
        {
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + type);
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
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    throw new Exception("Unable to find provider.");
                }
                if (insuranceCompany.Key != "Tawuniya")
                    Utilities.AddValueToCache("instance_" + providerFullTypeName + type, instance, 1440);

                scope.Dispose();
            }
            return provider;
        }

    }
}

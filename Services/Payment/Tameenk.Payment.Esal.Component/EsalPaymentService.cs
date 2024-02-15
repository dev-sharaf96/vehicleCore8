using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Esal;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Payment.Esal.Component.Model.Settlement;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalPaymentService : IEsalPaymentService
    {
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;
        private readonly IRepository<EsalRequest> _esalRequestRepository;
        private readonly IRepository<EsalSettlement> _esalSettlementRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<ProfitMarginRequest> _profitMarginRequestRepository;

        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;



        private readonly IRepository<EsalResponse> _esalResponseRepository;
        private readonly IRepository<Invoice> _invoiceRepository;

        private readonly IHttpClient _httpClient;

        public EsalPaymentService(TameenkConfig config, ILogger logger,
            IRepository<EsalRequest> esalRequestRepository,
            IRepository<Customer> customerRepository, IRepository<LineItem> lineItemRepository, IRepository<Supplier> supplierRepository,
            IRepository<ProfitMarginRequest> profitMarginRequestRepository, IRepository<CheckoutDetail> checkoutDetailRepository,
            IPolicyProcessingService policyProcessingService, IShoppingCartService shoppingCartService,
            IRepository<Invoice> invoiceRepository, IRepository<EsalResponse> esalResponseRepository, IRepository<EsalSettlement> esalSettlementRepository, IHttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
            _esalRequestRepository = esalRequestRepository;
            _customerRepository = customerRepository;
            _lineItemRepository = lineItemRepository;
            _supplierRepository = supplierRepository;
            _profitMarginRequestRepository = profitMarginRequestRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _policyProcessingService = policyProcessingService;
            _shoppingCartService = shoppingCartService;
            _invoiceRepository = invoiceRepository;
            _esalResponseRepository = esalResponseRepository;
            _esalSettlementRepository = esalSettlementRepository;
        }

        public EsalOutput UploadEsalInvoice(EsalRequestDto esalRequest, string channel, int companyId, string companyName, string driverNin, Guid userId,
            string referenceId, string vehicleId, string externalId)
        {
            //if (userId.ToString().ToLower() == "06501157-46af-4639-a85e-f29060e21fce")
            //{
            //    string InvoiceNumber = esalRequest.Invoices.FirstOrDefault().InvoiceNumber;
            //    esalRequest.Invoices.FirstOrDefault().InvoiceNumber = "TEST" + InvoiceNumber;
            //    esalRequest.Invoices.FirstOrDefault().GrandTotal = 1;
            //    esalRequest.Invoices.FirstOrDefault().TotalBeforeVAT = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().Amount = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().AmountAfterDiscount = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().DiscountAmount = 0;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().DiscountPercent = 0;

            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().Price = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().Quantity = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().PriceAfterVat = 1;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().TotalVat = 0;
            //    esalRequest.Invoices.FirstOrDefault().LineItems.FirstOrDefault().VatPercent = 0;
            //    esalRequest.Invoices.FirstOrDefault().OutstandingAmount = 1;
            //}

            EsalOutput output = new EsalOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(esalRequest);
            log.Method = "UploadEsalInvoice";
            log.ReferenceId = referenceId;
            log.VehicleId = vehicleId;
            log.UserID = userId;
            log.DriverNin = driverNin;
            log.CompanyID = companyId;
            log.CompanyName = companyName;
            log.ExternalId = externalId;
            try
            {
                if (esalRequest == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "esalRequest is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (esalRequest.Invoices == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "esalRequest.Invoices is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ReferenceId = esalRequest.Invoices.FirstOrDefault().InvoiceReference;
                string esaluploadInvoices = Utilities.GetAppSetting("EsaluploadInvoicesURL");
                log.ServiceURL = esaluploadInvoices;

                if (string.IsNullOrEmpty(esaluploadInvoices))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "esaluploadInvoices is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (_esalRequestRepository.TableNoTracking.Any(r => r.ReferenceId == referenceId&& r.HasEsalInvoice))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }
                string exception = string.Empty;
                bool retVal = SaveEsalRequestDB(esalRequest, companyId, companyName, driverNin, userId, referenceId, vehicleId, out exception);
                if (!retVal)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Failed to Save to DB due to " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var tokenOutput = GetEsalToken(channel, log);
                if (tokenOutput.ErrorCode != EsalOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.AuthorizationFailed;
                    output.ErrorDescription = "GetEsalToken Authorization Failed " + tokenOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.Method = "UploadEsalInvoice";
                HttpStatusCode httpStatusCode = new HttpStatusCode();
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    //NamingStrategy = new CamelCaseNamingStrategy()
                };
                DateTime dtBeforeCalling = DateTime.Now;
                var response = Utilities.SendRequestJson(esaluploadInvoices, JsonConvert.SerializeObject(esalRequest, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                }), out httpStatusCode, tokenOutput.Token);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.UnAuthoriztation;
                    output.ErrorDescription = " HttpStatusCode is Unauthorized";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.Forbidden)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = " HttpStatusCode is Access Denied";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = " HttpStatusCode is Invalid request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                //update esal request 
                var request = _esalRequestRepository.Table.Where(r => r.ReferenceId == referenceId && !r.HasEsalInvoice).OrderByDescending(r=>r.Id).FirstOrDefault();
                if(request!=null)
                {
                    request.HasEsalInvoice = true;
                    request.ModifiedDate = DateTime.Now;
                    _esalRequestRepository.Update(request);
                }
                output.ErrorCode = EsalOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ReferenceId = esalRequest.Id;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
        }

        public EsalOutput CancelEsalInvoice(EsalCancelRequestDto esalRequest)
        {
            string channel = "Portal";
            EsalOutput output = new EsalOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = esalRequest.supplierId;
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(esalRequest);
            log.Method = "CancelEsalInvoice";

            try
            {
                if (esalRequest == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "esalRequest is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (esalRequest.invoices == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "esalRequest.Invoices is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                log.ReferenceId = esalRequest.invoices.FirstOrDefault().invoiceNumber;
                string esalcancelInvoices = Utilities.GetAppSetting("EsalcancelInvoicesURL");
                log.ServiceURL = esalcancelInvoices;

                if (string.IsNullOrEmpty(esalcancelInvoices))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Esal cancel Invoices URL is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var tokenOutput = GetEsalToken(channel, log);
                if (tokenOutput.ErrorCode != EsalOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.AuthorizationFailed;
                    output.ErrorDescription = tokenOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.Method = "CancelEsalInvoice";
                var request = _httpClient.PostAsync(esalcancelInvoices, esalRequest, tokenOutput.Token);
                request.Wait();
                var response = request.Result;

                if (response == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.UnAuthoriztation;
                    output.ErrorDescription = " HttpStatusCode is Unauthorized";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = " HttpStatusCode is Invalid request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + response.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var result = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = result;
                if (result == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content.ReadAsStringAsync().Result is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var outputInfo = JsonConvert.DeserializeObject<EsalCancelResponseDto>(result);
                if (outputInfo == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "output is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var invoiceNumbers = esalRequest.invoices.Select(i => i.invoiceNumber).ToList();
                foreach (var invoice in _esalResponseRepository.Table.Where(r => invoiceNumbers.Contains(r.InvoiceNumber)).ToList())
                {
                    invoice.IsCancelled = true;
                    _esalResponseRepository.Update(invoice);
                }
                output.CancelInvoiceResponse = outputInfo;
                output.ErrorCode = EsalOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
        }
        private EsalOutput GetEsalToken(string channel, ServiceRequestLog log)
        {
            EsalOutput output = new EsalOutput();
            ServiceRequestLog tokenlog = new ServiceRequestLog();
            tokenlog.Method = "GetEsalToken";
            tokenlog.Channel = log.Channel;
            tokenlog.ServerIP = log.ServerIP;
            tokenlog.ReferenceId = log.ReferenceId;
            tokenlog.VehicleId = log.VehicleId;
            tokenlog.UserID = log.UserID;
            tokenlog.DriverNin = log.DriverNin;
            tokenlog.CompanyID = log.CompanyID;
            tokenlog.CompanyName = log.CompanyName;
            try
            {
                string userName = Utilities.GetAppSetting("EsalUserName");
                string password = Utilities.GetAppSetting("EsalPassword");
                string esalLoginURL = Utilities.GetAppSetting("EsalLoginURL");
                tokenlog.ServiceURL = esalLoginURL;
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "credential is null";
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }
                if (string.IsNullOrEmpty(esalLoginURL))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "esalLoginURL is null";
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }

                var formParamters = new Dictionary<string, string>();
                formParamters.Add("username", userName);
                formParamters.Add("password", password);
                tokenlog.ServiceRequest = JsonConvert.SerializeObject(formParamters);

                HttpStatusCode httpStatusCode = new HttpStatusCode();
                DateTime dtBeforeCalling = DateTime.Now;
                var response = Utilities.SendRequestJson(esalLoginURL, tokenlog.ServiceRequest, out httpStatusCode);
                DateTime dtAfterCalling = DateTime.Now;
                tokenlog.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                tokenlog.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "HttpStatusCode is Internal Server Error";
                    tokenlog.ErrorCode = (int)output.ErrorCode;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.ServiceError;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }
                //var result = response.Content.ReadAsStringAsync().Result;
               
                if (response == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content.ReadAsStringAsync().Result is null";
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }
                dynamic outputInfo = JsonConvert.DeserializeObject(response);
                if (outputInfo == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "output is null";
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }
                if (string.IsNullOrEmpty((string)outputInfo.authorization.token))
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "token is null";
                    tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    tokenlog.ErrorDescription = output.ErrorDescription;
                    tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                    tokenlog.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                    return output;
                }

                output.ErrorCode = EsalOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.Success;
                tokenlog.ErrorDescription = "Success";
                tokenlog.ServiceErrorCode = tokenlog.ErrorCode.ToString();
                tokenlog.ServiceErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);

                output.Token = (string)outputInfo.authorization.token;
                return output;
            }
            catch (Exception e)
            {
                output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = e.ToString();
                tokenlog.ErrorCode = (int)EsalOutput.ErrorCodes.ServiceException;
                tokenlog.ErrorDescription = e.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(tokenlog);
                return output;
            }
        }


        public EsalError UpdateInvoiceWithSadatID(EsalUploadInvoiceNotification invoiceNotification,string requestIPAddress)
        {
            EsalResponse esalResponse = new EsalResponse();
            esalResponse.Method = "UpdateInvoiceWithSadatID";
            esalResponse.ServerIP = ServicesUtilities.GetServerIP();
            esalResponse.RequestIPAddress = requestIPAddress;
            esalResponse.CreatedDate = DateTime.Now;
            esalResponse.ServerRequest = JsonConvert.SerializeObject(invoiceNotification);
            EsalError output = new EsalError();
            foreach (var invoice in invoiceNotification.Invoices)
            {
                //var invoiceRequest = _invoiceRequestRepository.Table.FirstOrDefault(x => x.InvoiceNumber == invoice.InvoiceNumber);
                var esalRequest = _esalRequestRepository.Table.FirstOrDefault(x => x.InvoiceNumber == invoice.InvoiceNumber);

                if (esalRequest == null)
                {
                    output.Code = EsalOutput.ErrorCodes.NullResponse.ToString();
                    output.Message = "Esal request is null";
                    esalResponse.ErrorCode = output.Code;
                    esalResponse.ErrorMessage = output.Message;
                    _esalResponseRepository.Insert(esalResponse);
                    return output;
                }
                esalResponse.ReferenceId = esalRequest.ReferenceId;
                esalResponse.InvoiceNumber = invoice.InvoiceNumber;
                esalResponse.Status = invoice.Status;
                esalResponse.SadadBillsId = invoice.SadadBillsId;
                esalResponse.RequestID = esalRequest.RequestID;
                esalResponse.CurAmt = esalRequest.GrandTotal.ToString();
                esalResponse.IsPaid = false;
                StringBuilder errorCodeBuilder = new StringBuilder();
                StringBuilder errorMessageBuilder = new StringBuilder();
                if (invoice.Errors != null)
                {
                    foreach (var error in invoice.Errors)
                    {
                        errorMessageBuilder.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        errorCodeBuilder.AppendLine(error.Code);
                    }
                }

                esalResponse.ErrorCode = errorCodeBuilder.ToString();
                esalResponse.ErrorMessage = errorMessageBuilder.ToString();
                _esalResponseRepository.Insert(esalResponse);
                output.Code = EsalOutput.ErrorCodes.Success.ToString();
                output.Message = "Success";
                return output;
            }
            output.Code = EsalOutput.ErrorCodes.NullResponse.ToString();
            output.Message = "Invoices is empty";
            return output;
        }

        public EsalError UpdateInvoicePayment(EsalPaymentNotification esalPaymentNotification, out CheckoutDetail checkoutDetails)
        {

            checkoutDetails = new CheckoutDetail();
            EsalError output = new EsalError();
            var esalResponse = _esalResponseRepository.Table.FirstOrDefault(x => x.SadadBillsId == esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumber);
            try
            {
                if (esalResponse == null)
                {
                    output.Code = EsalOutput.ErrorCodes.NullResponse.ToString();
                    output.Message = "Cannot find Esal with SadadBill " + esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumber;
                    return output;
                }
                esalResponse.Method = "PaymentNotification";
                esalResponse.ServerIP = ServicesUtilities.GetServerIP();
                esalResponse.ModifiedDate = DateTime.Now;
                esalResponse.ServerRequest = JsonConvert.SerializeObject(esalPaymentNotification);
                esalResponse.RequestIPAddress = Utilities.GetUserIPAddress();

                if (esalPaymentNotification.Payment.BankDetails != null)
                {
                    esalResponse.BankDetailbankId = esalPaymentNotification.Payment.BankDetails.BankId;
                    esalResponse.BankDetailBranchCode = esalPaymentNotification.Payment.BankDetails.BranchCode;
                    esalResponse.BankDetailDistrictCode = esalPaymentNotification.Payment.BankDetails.DistrictCode;
                    esalResponse.BankDetailAccessChannel = esalPaymentNotification.Payment.BankDetails.AccessChannel;
                }

                esalResponse.BillerId = esalPaymentNotification.Payment.BillerId;
                esalResponse.CurAmt = esalPaymentNotification.Payment.CurAmt;
                esalResponse.PmtRefInfo = esalPaymentNotification.Payment.PmtRefInfo;

                if (esalPaymentNotification.Payment.Payor != null)
                {
                    esalResponse.PayorOfficialId = esalPaymentNotification.Payment.Payor.OfficialId;
                    esalResponse.PayorOfficialIdType = esalPaymentNotification.Payment.Payor.OfficialIdType;
                }

                esalResponse.PrcDt = esalPaymentNotification.Payment.PrcDt;
                esalResponse.DueDt = esalPaymentNotification.Payment.DueDt;
                esalResponse.EfftDt = esalPaymentNotification.Payment.EfftDt;
                esalResponse.SadadPmtId = esalPaymentNotification.Payment.SadadPmtId;
                esalResponse.BankPmtId = esalPaymentNotification.Payment.BankPmtId;
                esalResponse.BankReversalId = esalPaymentNotification.Payment.BankReversalId;
                esalResponse.PmtMethod = esalPaymentNotification.Payment.PmtMethod;

                if (esalPaymentNotification.Payment.ProxyPayor != null)
                {
                    esalResponse.ProxyPayorOfficialId = esalPaymentNotification.Payment.ProxyPayor.OfficialId;
                    esalResponse.ProxyPayorOfficialIdType = esalPaymentNotification.Payment.ProxyPayor.OfficialIdType;
                }
                esalResponse.PaymentStatus = esalPaymentNotification.Payment.Status;

                if (esalPaymentNotification.Payment.PrePaidPaymentRef != null)
                {
                    esalResponse.PrePaidPaymentRefBllingAcct = esalPaymentNotification.Payment.PrePaidPaymentRef.BillingAcct;
                    esalResponse.PrePaidPaymentRefBeneficiaryPhoneNum = esalPaymentNotification.Payment.PrePaidPaymentRef.BeneficiaryPhoneNum;
                    esalResponse.PrePaidPaymentRefServiceCode = esalPaymentNotification.Payment.PrePaidPaymentRef.ServiceCode;
                    if (esalPaymentNotification.Payment.PrePaidPaymentRef.BeneficiaryId != null)
                    {
                        esalResponse.PrePaidPaymentRefOfficialId = esalPaymentNotification.Payment.PrePaidPaymentRef.BeneficiaryId.OfficialId;
                        esalResponse.PrePaidPaymentRefOfficialIdType = esalPaymentNotification.Payment.PrePaidPaymentRef.BeneficiaryId.OfficialIdType;
                    }
                    esalResponse.PrePaidPaymentRefChkDigit = esalPaymentNotification.Payment.PrePaidPaymentRef.ChkDigit;
                }

                if (esalPaymentNotification.Payment.PostPaidPaymentRef != null)
                {
                    esalResponse.PostPaidPaymentRefBllingAcct = esalPaymentNotification.Payment.PostPaidPaymentRef.BillingAcct;
                    esalResponse.PostPaidPaymentRefBillNumber = esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumber;
                    if (esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumberWithAccount != null)
                    {
                        esalResponse.PostPaidPaymentRefBillNumberWithAccountBillingAcct = esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumberWithAccount.BillingAcct;
                        esalResponse.PostPaidPaymentRefBillNumberWithAccountBillNumber = esalPaymentNotification.Payment.PostPaidPaymentRef.BillNumberWithAccount.BillNumber;
                    }
                    esalResponse.PostPaidPaymentRefBillCycle = esalPaymentNotification.Payment.PostPaidPaymentRef.BillCycle;
                    esalResponse.PostPaidPaymentRefServiceCode = esalPaymentNotification.Payment.PostPaidPaymentRef.ServiceCode;
                    esalResponse.PostPaidPaymentRefChkDigit = esalPaymentNotification.Payment.PostPaidPaymentRef.ChkDigit;
                }
                if (esalResponse.CurAmt != esalPaymentNotification.Payment.CurAmt)
                {
                    output.Code = EsalOutput.ErrorCodes.NullResponse.ToString();
                    output.Message = "payment amount are not the same as we sent " + esalResponse.CurAmt + " and recieved " + esalPaymentNotification.Payment.CurAmt;
                    _esalResponseRepository.Update(esalResponse);
                    return output;
                }
                if (esalResponse.InvoiceNumber != esalPaymentNotification.Payment.PmtRefInfo)
                {
                    output.Code = EsalOutput.ErrorCodes.NullResponse.ToString();
                    output.Message = "InvoiceNumber are not the same as we sent " + esalResponse.InvoiceNumber + " and recieved " + esalPaymentNotification.Payment.PmtRefInfo;
                    _esalResponseRepository.Update(esalResponse);
                    return output;
                }
                //var invoiceNumber = int.Parse(esalResponse.InvoiceNumber);
                //var invoice = _invoiceRepository.Table.FirstOrDefault(i => i.InvoiceNo == invoiceNumber);
                //if (invoice == null || string.IsNullOrEmpty(invoice.ReferenceId))
                //{
                //    esalResponse.ErrorCode = EsalOutput.ErrorCodes.ServiceError.ToString();
                //    esalResponse.ErrorMessage = "Unable to find invoice with Invoic No: " + invoiceNumber;
                //    _esalResponseRepository.Update(esalResponse);
                //    output.Code = EsalOutput.ErrorCodes.ServiceError.ToString();
                //    output.Message = esalResponse.ErrorMessage;
                //    return output;
                //}


                // update checkout details status to be PaymentSuccess
                checkoutDetails = _checkoutDetailRepository.Table.First(c => c.ReferenceId == esalResponse.ReferenceId);
                if (checkoutDetails == null)
                {
                    esalResponse.ErrorCode = EsalOutput.ErrorCodes.ServiceError.ToString();
                    esalResponse.ErrorMessage = "checkoutDetails is null as ReferenceId is "+ esalResponse.ReferenceId;
                    _esalResponseRepository.Update(esalResponse);
                    output.Code = EsalOutput.ErrorCodes.ServiceError.ToString();
                    output.Message = esalResponse.ErrorMessage;
                    return output;
                }
                esalResponse.IsPaid = true;
                _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId.ToString(), checkoutDetails.ReferenceId);
                if (checkoutDetails.PolicyStatusId == 1 || checkoutDetails.PolicyStatusId == 3)
                {
                    if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Available)
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                    checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.Esal;

                    _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, checkoutDetails.InsuranceCompanyName, checkoutDetails.Channel);
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    _checkoutDetailRepository.Update(checkoutDetails);
                }

                output.Code = EsalOutput.ErrorCodes.Success.ToString();
                output.Message = "Success";
                esalResponse.ErrorCode = output.Code;
                esalResponse.ErrorMessage = output.Message;
                _esalResponseRepository.Update(esalResponse);
                return output;
            }
            catch(Exception exp)
            {
                output.Code = EsalOutput.ErrorCodes.ServiceException.ToString();
                output.Message = exp.ToString();
                esalResponse.ErrorCode = output.Code;
                esalResponse.ErrorMessage = output.Message;
               _esalResponseRepository.Update(esalResponse);
                return output;
            }
        }

        private bool SaveEsalRequestDB(EsalRequestDto esalRequestDto, int companyId, string companyName, string driverNin, Guid userId,
            string referenceId, string vehicleId, out string exception)
        {
            try
            {
                exception = string.Empty;
                EsalRequest esalRequest = PrepareEsalRequestDB(esalRequestDto, companyId, companyName, driverNin, userId, referenceId, vehicleId);
                _esalRequestRepository.Insert(esalRequest);
                var invoiceDto = esalRequestDto.Invoices.FirstOrDefault();
                if (invoiceDto != null)
                {
                    if (invoiceDto.ProfitMargin != null)
                    {
                        ProfitMarginRequest profitMarginRequest = PrepareProfitMarginRequestDB(invoiceDto.ProfitMargin, referenceId);
                        _profitMarginRequestRepository.Insert(profitMarginRequest);
                    }
                    if (invoiceDto.Supplier != null)
                    {
                        Supplier supplier = PrepareSupplierDB(invoiceDto.Supplier, referenceId);
                        _supplierRepository.Insert(supplier);
                    }
                    if (invoiceDto.Customer != null)
                    {
                        Customer customer = PrepareCustomerDB(invoiceDto.Customer, referenceId);
                        _customerRepository.Insert(customer);
                    }
                    if (invoiceDto.LineItems.FirstOrDefault() != null)
                    {
                        LineItem lineItem = PrepareLineItemDB(invoiceDto.LineItems.FirstOrDefault(), referenceId);
                        _lineItemRepository.Insert(lineItem);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        private EsalRequest PrepareEsalRequestDB(EsalRequestDto esalRequestDto, int companyId, string companyName, string driverNin, Guid userId,
            string referenceId, string vehicleId)
        {
            InvoiceRequestDto invoiceRequestDto = esalRequestDto.Invoices.FirstOrDefault();
            EsalRequest esalRequest = new EsalRequest
            {
                CompanyID = companyId,
                CompanyName = companyName,
                ReferenceId = referenceId,
                DriverNin = driverNin,
                UserID = userId,
                SequenceNumber = vehicleId,
                RequestID = esalRequestDto.Id,
                SupplierId = esalRequestDto.SupplierId,
                CreatedDate = DateTime.Now,
                ModifiedDate=DateTime.Now,
                AdvanceAmount = invoiceRequestDto.AdvanceAmount,
                AmountNotSubjToTaxation = invoiceRequestDto.AmountNotSubjToTaxation,
                Currency = invoiceRequestDto.Currency,
                DateOfDelivery = null,
                DueDate = DateTime.Parse(invoiceRequestDto.DueDate),
                ExemptedAmount = invoiceRequestDto.ExemptedAmount,
                GrandTotal = invoiceRequestDto.GrandTotal,
                InvoiceIssueDate = DateTime.Parse(invoiceRequestDto.InvoiceIssueDate),
                InvoiceNumber = invoiceRequestDto.InvoiceNumber,
                InvoiceReference = invoiceRequestDto.InvoiceReference,
                InvoiceType = invoiceRequestDto.InvoiceType,
                MilestonePayments = invoiceRequestDto.MilestonePayments,
                NarrationArabic = invoiceRequestDto.NarrationArabic,
                NarrationEnglish = invoiceRequestDto.NarrationEnglish,
                OutstandingAmount = invoiceRequestDto.OutstandingAmount,
                PercentOfCompletion = invoiceRequestDto.PercentOfCompletion,
                RemarksArabic = invoiceRequestDto.RemarksArabic,
                RemarksEnglish = invoiceRequestDto.RemarksEnglish,
                SalespersonArabic = invoiceRequestDto.SalespersonArabic,
                SalespersonEnglish = invoiceRequestDto.SalespersonEnglish,
                SelfAccountForTax = invoiceRequestDto.SelfAccountForTax,
                ShipmentRefNumber = invoiceRequestDto.ShipmentRefNumber,
                TotalBeforeVAT = invoiceRequestDto.TotalBeforeVAT,
                TotalVAT = invoiceRequestDto.TotalVAT
            };
            return esalRequest;
        }

        private ProfitMarginRequest PrepareProfitMarginRequestDB(ProfitMarginRequestDto profitMarginRequestDto, string referenceId)
        {
            ProfitMarginRequest profitMargin = new ProfitMarginRequest
            {
                CreatedDate = DateTime.Now,
                Applied = profitMarginRequestDto.Applied,
                //Description = profitMarginRequestDto.Description,
                ProcedureArabic = profitMarginRequestDto.ProcedureArabic,
                procedureEnglish = profitMarginRequestDto.procedureEnglish,
                ReferenceId = referenceId,
            };
            return profitMargin;
        }
        private Supplier PrepareSupplierDB(SupplierDto supplierDto, string referenceId)
        {
            Supplier supplier = new Supplier
            {
                CreatedDate = DateTime.Now,
                BankSwiftCode = supplierDto.BankSwiftCode,
                BranchArabic = supplierDto.BranchArabic,
                BranchEnglish = supplierDto.BranchEnglish,
                CityArabic = supplierDto.CityArabic,
                CityEnglish = supplierDto.CityEnglish,
                Iban = supplierDto.Iban,
                LocalityArabic = supplierDto.LocalityArabic,
                LocalityEnglish = supplierDto.LocalityEnglish,
                PlantArabic = supplierDto.PlantArabic,
                PlantEnglish = supplierDto.PlantEnglish,
                RegionArabic = supplierDto.RegionArabic,
                RegionEnglish = supplierDto.RegionEnglish,
                SupplierId = supplierDto.Id,
                ReferenceId = referenceId,
            };
            return supplier;
        }
        private Customer PrepareCustomerDB(CustomerDto customerDto, string referenceId)
        {
            Customer customer = new Customer
            {
                CreatedDate = DateTime.Now,
                AddressArabic1 = customerDto.AddressArabic1,
                AddressArabic2 = customerDto.AddressArabic2,
                AddressEnglish1 = customerDto.AddressEnglish1,
                AddressEnglish2 = customerDto.AddressEnglish2,
                Branch = customerDto.Branch,
                BranchArabic = customerDto.BranchArabic,
                CityArabic = customerDto.CityArabic,
                CityEnglish = customerDto.CityEnglish,
                Code = customerDto.Code,
                ContactNo = customerDto.ContactNo,
                DivisionArabic = customerDto.DivisionArabic,
                DivisionEnglish = customerDto.DivisionEnglish,
                Email = customerDto.Email,
                LocalityArabic = customerDto.LocalityArabic,
                LocalityEnglish = customerDto.LocalityEnglish,
                NameArabic = customerDto.NameArabic,
                NameEnglish = customerDto.NameEnglish,
                PlantArabic = customerDto.PlantArabic,
                PlantEnglish = customerDto.PlantEnglish,
                ProdLineArabic = customerDto.ProdLineArabic,
                ProdLineEnglish = customerDto.ProdLineEnglish,
                RegionArabic = customerDto.RegionArabic,
                RegionEnglish = customerDto.RegionEnglish,
                SegmentArabic = customerDto.SegmentArabic,
                SegmentEnglish = customerDto.SegmentEnglish,
                VatRegisterationNumber = customerDto.VatRegisterationNumber,
                ReferenceId = referenceId,
            };
            return customer;
        }
        private LineItem PrepareLineItemDB(LineItemDto lineItemDto, string referenceId)
        {
            LineItem lineItem = new LineItem
            {
                CreatedDate = DateTime.Now,
                Amount = lineItemDto.Amount,
                AmountAfterDiscount = lineItemDto.AmountAfterDiscount,
                Code = lineItemDto.Code,
                DescriptionArabic = lineItemDto.DescriptionArabic,
                DescriptionEnglish = lineItemDto.DescriptionEnglish,
                DiscountAmount = lineItemDto.DiscountAmount,
                DiscountPercent = lineItemDto.DiscountPercent,
                Price = lineItemDto.Price,
                PriceAfterVat = lineItemDto.PriceAfterVat,
                Quantity = lineItemDto.Quantity,
                SerialNumber = lineItemDto.SerialNumber,
                TotalVat = lineItemDto.TotalVat,
                UomArabic = lineItemDto.UomArabic,
                UomEnglish = lineItemDto.UomEnglish,
                VatPercent = lineItemDto.VatPercent,
                ReferenceId = referenceId,
            };
            return lineItem;
        }




        public EsalOutput SaveEsalSettlement(EsalSettlementModel _esalSettlement)
        {
            EsalOutput output = new EsalOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            //log.ReferenceId = esalRequest.SupplierId;
            //log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(_esalSettlement);
            log.Method = "SaveEsalSettlement";

            try
            {

                if (_esalSettlement == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "esalSettlement is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (_esalSettlement.BillerReconUploadRq == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "_esalSettlement.BillerReconUploadRq is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (_esalSettlement.BillerReconUploadRq.PmtBankRec == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "_esalSettlement.BillerReconUploadRq.PmtBankRec is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (_esalSettlement.BillerReconUploadRq.PmtBankRec.FirstOrDefault() == null)
                {
                    output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "_esalSettlement.BillerReconUploadRq.PmtBankRec.FirstOrDefault is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var PmtBankRec = _esalSettlement.BillerReconUploadRq.PmtBankRec.FirstOrDefault();

                EsalSettlement esalSettlement = new EsalSettlement
                {
                    StatusCode = _esalSettlement?.Status?.StatusCode,
                    ShortDesc = _esalSettlement?.Status?.ShortDesc,
                    SeverityType = _esalSettlement?.Status?.SeverityType,

                    RqUID = _esalSettlement?.RqUID,

                    BillerReconUploadRqPrcDt = _esalSettlement?.BillerReconUploadRq?.PrcDt,
                    BillerReconUploadRqCollectPmtAmt = _esalSettlement?.BillerReconUploadRq?.CollectPmtAmt,
                    BillerReconUploadRqReconPmtAmt = _esalSettlement?.BillerReconUploadRq?.ReconPmtAmt,
                    BillerReconUploadRqUnReconPmtAmt = _esalSettlement?.BillerReconUploadRq?.UnReconPmtAmt,
                    BillerReconUploadRqTransFees = _esalSettlement?.BillerReconUploadRq?.TransFees,

                    PmtBankRecBankId = PmtBankRec.BankId,
                    PmtBankRecCurAmt = PmtBankRec.CurAmt,

                    BranchCode = PmtBankRec.PmtBranchRec.FirstOrDefault().BranchCode,
                    PmtBranchRecCurAmt = PmtBankRec.PmtBranchRec.FirstOrDefault().CurAmt,

                    PmtRecPmtTransIdPmtId = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtTransId
                                 .FirstOrDefault()?.PmtId,
                    PmtRecPmtTransIdPmtIdType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtTransId
                                 .FirstOrDefault()?.PmtIdType,

                    CustIdOfficialId = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.CustId?.OfficialId,
                    CustIdofficialIdType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.CustId?.OfficialIdType,

                    PmtStatusPmtStatusCode = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtStatus?.StatusCode,
                    PmtStatusEffDt = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtStatus?.EffDt,
                    StatusStatusCode = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtStatus?.status?.StatusCode,
                    StatusShortDesc = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtStatus?.status?.ShortDesc,
                    StatusSeverityType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtStatus?.status?.SeverityType,

                    PmtInfoCurAmt = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.CurAmt,
                    PmtInfoPrcDt = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.PrcDt,
                    PmtInfoDueDt = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.DueDt,
                    PmtInfoBillCycle = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.BillCycle,
                    PmtInfoBillNumber = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.BillNumber,
                    BankId = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.BankId,
                    DistrictCode = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.DistrictCode,
                    AccessChannel = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.AccessChannel,
                    PmtMethod = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.PmtMethod,
                    PmtType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.PmtType,
                    ChkDigit = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.ChkDigit,
                    ServiceType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.ServiceType,
                    PmtRefInfo = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.PmtRefInfo,

                    AccountIdBillerId = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.AccountId?.BillerId,
                    AccountIdBillingAcct = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.AccountId?.BillingAcct,

                    IncludeInfoPlusIncPaymentRanges = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.IncludeInfoPlus?.IncPaymentRanges,

                    BeneficiaryIdOfficialId = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.BeneficiaryId?.OfficialId,
                    BeneficiaryIdOfficialIdType = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.BeneficiaryId?.OfficialIdType,
                    PhoneNumBeneficiaryPhoneNum = PmtBankRec.PmtBranchRec.FirstOrDefault().PmtRec.FirstOrDefault()?.PmtInfo?.PhoneNum?.BeneficiaryPhoneNum,

                };

                //insert To DB
                _esalSettlementRepository.Insert(esalSettlement);

                output.ErrorCode = EsalOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            catch (Exception ex)
            {
                output.ErrorCode = EsalOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }

        }

        public List<string> GetExpiredEsalInvoices()        {

            try            {
                var dataProviderSettings = new DataSettingsManager()
                                       .LoadSettings((ConfigurationManager.GetSection("TameenkConfig") as TameenkConfig)
                                                      .Settings.Path);
                using (SqlConnection connection = new SqlConnection(dataProviderSettings.DataConnectionString))                {                    SqlCommand sqlCommand = new SqlCommand("GetExpiredEsalInvoices", connection);                    sqlCommand.CommandType = CommandType.StoredProcedure;                    try                    {                        connection.Open();                        using (var reader = sqlCommand.ExecuteReader())                        {                            var queueItems = new List<string>();                            while (reader.Read())                            {                                queueItems.Add(reader.GetString(0));                            }                            return queueItems;                        }                    }                    finally                    {                        if (connection.State == ConnectionState.Open)                            connection.Close();                    }                }            }            catch (Exception exp)            {                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetExpiredEsalInvoices" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return null;            }
        }        public void CancelExpiredInvoices()
        {
            try
            {
                var invoicesList = GetExpiredEsalInvoices();
                if (invoicesList == null || invoicesList.Count() == 0)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetExpiredEsalInvoices_count" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "count is 0 ");
                    return;
                }
                else
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetExpiredEsalInvoices_count_is" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "count is " + invoicesList.Count());
                }
                foreach (string invoiceNo in invoicesList)
                {
                    var response = _esalResponseRepository.TableNoTracking.Where(r => r.InvoiceNumber == invoiceNo && r.Method== "PaymentNotification").FirstOrDefault();
                    if (response != null)
                        continue;
                    if (response != null&&response.CreatedDate.AddHours(16)>DateTime.Now)
                        continue;
                    EsalCancelRequestDto invoices = new EsalCancelRequestDto();
                    invoices.supplierId = "258";
                    invoices.invoices = new List<InvoiceCanceledDto>();
                    InvoiceCanceledDto invoice = new InvoiceCanceledDto();
                    invoice.invoiceNumber = invoiceNo;
                    invoices.invoices.Add(invoice);
                    var result = CancelEsalInvoice(invoices);
                }
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\CancelExpiredInvoices" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());

            }
        }
        public bool IsSadadBillIdExistBefore(string referenceId)
        {
           var request= _esalResponseRepository.TableNoTracking.Where(r =>r.ReferenceId == referenceId && r.SadadBillsId != null).FirstOrDefault();
            if (request != null)
                return true;
            else
                return false;
        }
        public string GetSadadNumber(string invoiceNo,string referenceId)
        {
            var response= _esalResponseRepository.TableNoTracking.Where(r => r.InvoiceNumber == invoiceNo&&r.ReferenceId==referenceId&&r.SadadBillsId!=null).OrderByDescending(r=>r.ID).FirstOrDefault();
            if (response != null)
                return response.SadadBillsId;
            else
                return string.Empty;
        }
        public bool CheckIfRequestExceededOneMinutes(string invoiceNo, string referenceId)
        {
            var request = _esalRequestRepository.TableNoTracking.Where(r => r.InvoiceNumber == invoiceNo && r.ReferenceId == referenceId).OrderByDescending(r => r.Id).FirstOrDefault();
            if (request != null)
            {
                if (request.CreatedDate.AddMinutes(1.5)<DateTime.Now)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

    }
}

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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto;
using Tameenk.Integration.Dto.Payment;
using Tameenk.Integration.Dto.Payment.CreateOrder;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Checkout;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tameenk.Integration.Dto.Payment;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Net.Security;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Services.Implementation.Payments
{
    public class HyperpayPaymentService : IHyperpayPaymentService
    {
        #region Fields

        private readonly IRepository<HyperpayRequest> hyperpayRequestRepository;
        private readonly IRepository<HyperpayResponse> hyperpayResponseRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IInsuranceCompanyService _inuranceCompanyService;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<CompanyBankAccounts> _companyBankAccountRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ICheckoutsService _checkoutsService;
        private readonly IRepository<HyperPayUpdateOrder> _hyperPayUpdateOrderRepository;

        private readonly IRepository<HyperPayCreateOrder> _hyperPayCreateOrderRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        #endregion

        #region Ctor

        public HyperpayPaymentService(IRepository<HyperpayRequest> hyperpayRequestRepository,
            IRepository<HyperpayResponse> hyperpayResponseRepository,
            IPolicyProcessingService policyProcessingService,
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            TameenkConfig tameenkConfig,
            ILogger logger, INotificationService notificationService,
            IInsuranceCompanyService inuranceCompanyService,
            IRepository<Invoice> invoiceRepository, IRepository<CompanyBankAccounts> companyBankAccountRepository,
            IRepository<Product> productRepository, ICheckoutsService checkoutsService,
            IRepository<HyperPayUpdateOrder> hyperPayUpdateOrderRepository, IRepository<HyperPayCreateOrder> hyperPayCreateOrderRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository)
        {
            this.hyperpayRequestRepository = hyperpayRequestRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<HyperpayRequest>));
            this.hyperpayResponseRepository = hyperpayResponseRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<HyperpayResponse>));
            _policyProcessingService = policyProcessingService ?? throw new TameenkArgumentNullException(nameof(IPolicyProcessingService));
            _shoppingCartService = shoppingCartService ?? throw new TameenkArgumentNullException(nameof(IShoppingCartService));
            _orderService = orderService ?? throw new TameenkArgumentNullException(nameof(IOrderService));
            _config = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));
            _notificationService = notificationService;
            _inuranceCompanyService = inuranceCompanyService;
            _invoiceRepository = invoiceRepository;
            _companyBankAccountRepository = companyBankAccountRepository;
            _productRepository = productRepository;
            _checkoutsService = checkoutsService;
            _hyperPayUpdateOrderRepository = hyperPayUpdateOrderRepository;
            _hyperPayCreateOrderRepository = hyperPayCreateOrderRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate riyadh bank request url.
        /// </summary>
        /// <param name="HyperpayRequest">The RiyadBank request parameters</param>
        /// <returns></returns>
        public string CreateRiyadBankRequestUrl(HyperpayRequest hyperpayRequest)
        {
            if (hyperpayRequest == null)
            {
                throw new TameenkArgumentNullException(nameof(hyperpayRequest));
            }
            _logger.Log($"HyperpayPaymentService -> CreateRiyadBankRequestUrl, (Riyad Bank Migs Request : {JsonConvert.SerializeObject(hyperpayRequest.Amount)})");


            ////var VPC_URL = "https://migs.mastercard.com.au/vpcpay";
            //var vpcUrl = _config.RiyadBank.Url;
            //HyperpayRequest.AccessCode = _config.RiyadBank.AccessCode;
            //HyperpayRequest.MerchantId = _config.RiyadBank.MerchantId;
            //HyperpayRequest.MerchTxnRef = CreateMerchantReference();
            //HyperpayRequest.Version = "1";


            //string hashSecret = _config.RiyadBank.SecureHashSecret;

            //// Create signature hash to secure request. 
            //var transactionData = GetParameters(HyperpayRequest).OrderBy(t => t.Key, new VPCStringComparer());

            //if (!string.IsNullOrEmpty(hashSecret))
            //{
            //    HyperpayRequest.SecureHash = CreateSHA256Signature(transactionData, hashSecret);
            //}
            //HyperpayRequest.SecureHashType = "SHA256";
            //var urlParamters = string.Join("&", transactionData.Select(item => $"{HttpUtility.UrlEncode(item.Key)}={HttpUtility.UrlEncode(item.Value)}"));
            //var redirectUrl = $"{vpcUrl}?{urlParamters}";

            //_HyperpayRequestRepository.Insert(HyperpayRequest);
            //// Return the url
            //return $"{redirectUrl}&vpc_SecureHash={HyperpayRequest.SecureHash}&vpc_SecureHashType={HyperpayRequest.SecureHashType}";
            return "";
        }

        /// <summary>
        /// Validate the response of parameter
        /// </summary>
        /// <param name="secureHash">The secure hash (signature) send from MIGS portal in response parameter.</param>
        /// <param name="list">The list of parameters without the secure hash and secure hash type.</param>
        /// <returns></returns>
        //public bool ValidateResponse(string secureHash, IEnumerable<KeyValuePair<string, string>> list)
        //{
        //    var signature = CreateSHA256Signature(list, _config.RiyadBank.SecureHashSecret);
        //    return signature == secureHash;
        //}

        /// <summary>
        /// Process payment according to MIGS response.
        /// </summary>
        /// <param name="hyperpayResponse">The riyadbank MIGS response.</param>
        public bool ProcessPayment(HyperpayResponse hyperpayResponse, LanguageTwoLetterIsoCode culture, string channel, int paymentMethodId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var hyperpayRequest = GetHyperpayRequestByReferenceId(hyperpayResponse.ReferenceId, out exception);
                if (hyperpayRequest == null)
                {
                    exception = "There is no hyperpayRequest " + exception;
                    return false;
                }
                _shoppingCartService.EmptyShoppingCart(hyperpayRequest.UserId.ToString(), hyperpayRequest.ReferenceId);
                var paymentSucceded = false;
                if (Regex.IsMatch(hyperpayResponse.ResponseCode, "^(000.000.|000.100.1|000.[36])", RegexOptions.IgnoreCase) || Regex.IsMatch(hyperpayResponse.ResponseCode, "(000.400.0|000.400.100)"))
                {
                    paymentSucceded = true;
                }
                if (paymentSucceded && hyperpayRequest.Amount != hyperpayResponse.Amount)
                {
                    exception = "Payment response was not the same amount as request";
                    throw new TameenkException("Payment response was not the same amount as request");
                }
                //There should be only one checkout related to this payment request
                //var checkoutDetail = hyperpayRequest.CheckoutDetails.FirstOrDefault();
                var checkoutDetail = _checkoutsService.GetFromCheckoutDetailsByReferenceId(hyperpayResponse.ReferenceId, out exception);
                if (checkoutDetail == null)
                {
                    exception = "There is no checkoutDetail " + exception;
                    throw new TameenkArgumentException("There is no checkoutDetail related to this payment request");
                }
                if (paymentMethodId == 0 && checkoutDetail.PaymentMethodId.HasValue)
                {
                    paymentMethodId = checkoutDetail.PaymentMethodId.Value;
                }
                if (checkoutDetail.IsCancelled)
                {
                    exception = "Policy is cancelled";
                    return true;
                }
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.Available)
                {
                    exception = "Success Before";
                    return true;
                }
                if (checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment && checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure)
                {
                    exception = "policy status is not PendingPayment it's " + checkoutDetail.PolicyStatusId;
                    return true;
                }
                var hyperpayResponses = hyperpayResponseRepository.TableNoTracking.FirstOrDefault(req => req.Message == "Transaction succeeded" && req.HyperpayRequestId == hyperpayRequest.Id && req.HyperpayResponseId == hyperpayResponse.HyperpayResponseId && req.Amount == hyperpayResponse.Amount);

                if (hyperpayResponses != null)
                {
                    if (UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                    {
                        UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    }
                    return true;
                }
                if (paymentSucceded)
                {
                    try
                    {
                        var product = _productRepository.TableNoTracking.Where(a => a.Id == checkoutDetail.SelectedProductId).FirstOrDefault();
                        var companyName = product == null
                                    ? string.Empty
                                    : _inuranceCompanyService.GetInsuranceCompanyName(product.ProviderId.Value, culture);
                        string productType = string.Empty;
                        if (product.ProviderId.Value == 12) // tawuniya
                        {
                            if (product != null)
                            {
                                var lang = culture == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";
                                if (product.InsuranceTypeCode == 1)
                                {
                                    productType = CheckoutResources.ResourceManager.GetString("TPL", CultureInfo.GetCultureInfo(lang));
                                }
                                else if (product.InsuranceTypeCode == 2)
                                {
                                    productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
                                }
                                else if (product.InsuranceTypeCode == 7)
                                {
                                    productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
                                }
                            }
                        }
                        else if (product != null && product.ProviderId.Value == 20 && product.InsuranceTypeCode == 8) // Al Rajhi & Wafi Smart
                        {
                            var lang = culture == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";
                            productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
                        }
                        else
                        {
                            productType = product == null
                                    ? string.Empty
                                    : culture == LanguageTwoLetterIsoCode.Ar ? product.QuotationResponse.ProductType.ArabicDescription : product.QuotationResponse.ProductType.EnglishDescription;
                        }
                        var amount = Math.Round(hyperpayResponse.Amount, 2);
                        var message = string.Format(Tameenk.Resources.WebResources.WebResources.ProcessPayment_SendingSMS,
                            productType, companyName, amount);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = checkoutDetail.Phone,
                            MessageBody = message,
                            Method = SMSMethod.HyperPayPaymentNotification.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = channel
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    }
                    catch
                    {

                    }
                }
                if (UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                {
                    hyperpayResponse.HyperpayRequestId = hyperpayRequest.Id;
                    hyperpayResponseRepository.Insert(hyperpayResponse);
                    UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                }
                return paymentSucceded;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        #endregion



        //public HyperpayRequest GetHyperPayCheckoutId(HyperpayRequest hyperpayRequest, out string exception)
        //{
        //    Dictionary<string, dynamic> responseData;
        //    exception = string.Empty;
        //    try
        //    {
        //        string data = "entityId=" + _config.Hyperpay.EntityId +
        //       "&amount=" + hyperpayRequest.Amount +
        //       "&currency=" + _config.Hyperpay.Currency +
        //       "&paymentType=" + _config.Hyperpay.PaymentType +
        //       "&notificationUrl=" + _config.Hyperpay.NotificationURL;

        //        if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
        //            data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
        //        else
        //            data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

        //        data += "&customer.email=" + hyperpayRequest.UserEmail;

        //        byte[] buffer = Encoding.ASCII.GetBytes(data);

        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_config.Hyperpay.Url);
        //        request.Method = "POST";
        //        request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
        //        request.ContentType = "application/x-www-form-urlencoded";

        //        Stream PostData = request.GetRequestStream();
        //        PostData.Write(buffer, 0, buffer.Length);
        //        PostData.Close();
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            Stream dataStream = response.GetResponseStream();
        //            StreamReader reader = new StreamReader(dataStream);
        //            var s = new JavaScriptSerializer();
        //            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //            reader.Close();
        //            dataStream.Close();

        //            hyperpayRequest.ResponseId = responseData["id"];
        //            hyperpayRequest.ResponseNdc = responseData["ndc"];
        //            hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
        //            hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
        //            hyperpayRequest.ResponseCode = responseData["result"]["code"];
        //            hyperpayRequest.ResponseDescription = responseData["result"]["description"];
        //            hyperpayRequestRepository.Insert(hyperpayRequest);
        //        }
        //        return hyperpayRequest;
        //    }
        //    catch (Exception exp)
        //    {
        //        exception = exp.ToString();
        //        return null;
        //    }
        //}


        public HyperpayRequest RequestHyperpayUrl(HyperpayRequest hyperpayRequest, out string exception)
        {
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;
                string url = _config.Hyperpay.Url;
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                request.ContentType = "application/x-www-form-urlencoded";
                string exp = "";
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                    hyperpayRequest.ResponseId = responseData["id"];
                    hyperpayRequest.ResponseNdc = responseData["ndc"];
                    hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                    hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                    hyperpayRequest.ResponseCode = responseData["result"]["code"];
                    hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                }
                hyperpayRequestRepository.Insert(hyperpayRequest);
                //return responseData;
                return hyperpayRequest;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                return null;
            }

        }

        public HyperSplitOutput RequestHyperpayUrlWithSplitOption(HyperpayRequest hyperpayRequest,int companyId,string companyName,string channel,Guid merchantTransactionId, out string exception)
        {
            DateTime beforeCalling = DateTime.Now;
            hyperpayRequest.MerchantTransactionId = merchantTransactionId;
            HyperSplitOutput output = new HyperSplitOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServiceURL = _config.Hyperpay.Url;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "RequestHyperpayUrlWithSplitOption";
            log.ReferenceId = hyperpayRequest.ReferenceId;
            log.CompanyID =companyId;
            log.CompanyName = companyName;
            log.Channel = channel;
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == hyperpayRequest.ReferenceId);
                if (invoice == null)
                {
                    exception = " invoice is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (invoice.TotalCompanyAmount <= 0)
                {
                    exception = "invoice.TotalCompanyAmount<=0 as it's " + invoice.TotalCompanyAmount;
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.TotalCompanyAmountIsLessZero;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;

                }
                if (!invoice.InsuranceCompanyId.HasValue)
                {
                    exception = " invoice.InsuranceCompanyId is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.InsuranceCompanyIdIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var bcareBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == 15);
                if (bcareBankAccount == null)
                {
                    exception = "bcare Bank Account is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.BCareBankAccountIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    exception = "companyBankAccount is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.CompanyBankAccountIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var bcareAmount = (invoice.TotalBCareFees + invoice.TotalBCareCommission);                BeneficiaryModel odBeneficiary = null;                if (invoice.InsuranceTypeCode == 1 && !string.IsNullOrEmpty(invoice.ODReference))                {                    Invoice odInvoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == invoice.ODReference);                    if (odInvoice != null)                    {                        bcareAmount += odInvoice.TotalBCareFees + odInvoice.TotalBCareCommission;                        var odCompanyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == odInvoice.InsuranceCompanyId);                        if (odCompanyBankAccount != null)                        {                            odBeneficiary = new BeneficiaryModel()                            {                                name = odCompanyBankAccount.BeneficiaryName,                                accountId = odCompanyBankAccount.IBAN,                                address1 = odCompanyBankAccount.Address1,                                address2 = odCompanyBankAccount.Address2,                                address3 = odCompanyBankAccount.Address3,                                bankIdBIC = odCompanyBankAccount.SWIFTCODE,                                debitCurrency = "SAR",                                transferAmount = odInvoice.TotalCompanyAmount?.ToString(),                                transferCurrency = "SAR"                            };                        }                    }                }
                var beneficiaryBcare = new BeneficiaryModel()
                {
                    name = bcareBankAccount.BeneficiaryName,
                    accountId = bcareBankAccount.IBAN,
                    address1 = bcareBankAccount.Address1,
                    address2 = bcareBankAccount.Address2,
                    address3 = bcareBankAccount.Address3,
                    bankIdBIC = bcareBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = bcareAmount.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiary = new BeneficiaryModel()
                {
                    name = companyBankAccount.BeneficiaryName,
                    accountId = companyBankAccount.IBAN,
                    address1 = companyBankAccount.Address1,
                    address2 = companyBankAccount.Address2,
                    address3 = companyBankAccount.Address3,
                    bankIdBIC = companyBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = invoice.TotalCompanyAmount?.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiaries = new List<BeneficiaryModel>();
                beneficiaries.Add(beneficiaryBcare);
                beneficiaries.Add(beneficiary);
                if(odBeneficiary!=null)
                {
                    beneficiaries.Add(odBeneficiary);
                }
                var payOut = new PayoutModel()
                {
                    transferOption = "7", // ‘0’ --> real time; 7 batch
                    period = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                    configId = Utilities.GetAppSetting("HyperpaykeyConfigId"), // "b29db28c3834dceece73c41ff39f1737", // sent before by mail
                    beneficiary = beneficiaries
                };

                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonSerializer.Serialize(payOut);

                if (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0)
                {
                    hyperpayRequest.Amount = hyperpayRequest.Amount - invoice.TotalBCareDiscount.Value;
                }

                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&merchantInvoiceId={hyperpayRequest.ReferenceId}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}" +
                    $"&customParameters[payout]={json}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode={_config.Hyperpay.TestMode}&merchantTransactionId={hyperpayRequest.MerchantTransactionId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.MerchantTransactionId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;

                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";

                string url = _config.Hyperpay.Url;
                string exp = string.Empty;
                log.ServiceRequest = data;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers["Authorization"] = $"Bearer { _config.Hyperpay.AccessToken}";
                Utilities.InitiateSSLTrust();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();
                    }
                }
                if (string.IsNullOrEmpty(responseString))
                {
                    exception = "responseString is null or empty and status code is "+ httpResponse.StatusCode;
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.ResponseIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = responseString;
                var serializer = new JavaScriptSerializer();
                responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                hyperpayRequest.ResponseId = responseData["id"];
                hyperpayRequest.ResponseNdc = responseData["ndc"];
                hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                hyperpayRequest.ResponseCode = responseData["result"]["code"];
                hyperpayRequest.ResponseDescription = responseData["result"]["description"];

                hyperpayRequest.ServiceRequest = jsonSerializer.Serialize(data);
                hyperpayRequestRepository.Insert(hyperpayRequest);
                output.ErrorCode = HyperSplitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.HyperpayRequest = hyperpayRequest;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (WebException webException)
            {
                using (WebResponse response = webException.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream value = response.GetResponseStream())
                    {
                        var s = new JavaScriptSerializer();
                        Dictionary<string, dynamic> responseData;
                        using (var reader = new StreamReader(value))
                        {
                            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                            reader.Close();
                        }
                        responseString = s.Serialize(responseData);
                    }
                }
                exception +=";"+ webException.ToString() + " And responseString: " + responseString;
                output.ErrorCode = HyperSplitOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = exception;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                output.ErrorCode = HyperSplitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exception;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public HyperpayRequest RequestHyperpayUrlForSTCPayWithSplitOption(HyperpayRequest hyperpayRequest, out string exception)
        {
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == hyperpayRequest.ReferenceId);
                if (invoice == null || invoice.TotalCompanyAmount <= 0)
                {
                    exception = " invoice is null or invoice.TotalCompanyAmount<=0 ";
                    return hyperpayRequest;
                }
                if (!invoice.InsuranceCompanyId.HasValue)
                {
                    exception = " invoice.InsuranceCompanyId is null ";
                    return hyperpayRequest;
                }
                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    exception = "companyBankAccount is null ";
                    return hyperpayRequest;
                }
                var beneficiary = new BeneficiaryModel()
                {
                    name = companyBankAccount.BeneficiaryName,
                    accountId = companyBankAccount.IBAN,
                    address1 = companyBankAccount.Address1,
                    address2 = companyBankAccount.Address2,
                    address3 = companyBankAccount.Address3,
                    bankIdBIC = companyBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = invoice.TotalCompanyAmount?.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiaries = new List<BeneficiaryModel>();
                beneficiaries.Add(beneficiary);

                var payOut = new PayoutModel()
                {
                    transferOption = "0", // ‘0’ --> real time
                    configId = Utilities.GetAppSetting("HyperpaykeyConfigId"), // "b29db28c3834dceece73c41ff39f1737", // sent before by mail
                    beneficiary = beneficiaries
                };

                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonSerializer.Serialize(payOut);

                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}" +
                    $"&customParameters[payout]={json}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;

                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";

                string url = _config.Hyperpay.Url;
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                string exp = string.Empty;
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                    hyperpayRequest.ResponseId = responseData["id"];
                    hyperpayRequest.ResponseNdc = responseData["ndc"];
                    hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                    hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                    hyperpayRequest.ResponseCode = responseData["result"]["code"];
                    hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                }
                hyperpayRequestRepository.Insert(hyperpayRequest);
                return hyperpayRequest;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                return null;
            }

        }
        public HyperpayRequest RequestHyperpayUrlForSTCPayForMobileWithSplitOption(HyperpayRequest hyperpayRequest, string phoneNumber, bool isQrCode, out string exception)
        {
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == hyperpayRequest.ReferenceId);
                if (invoice == null || invoice.TotalCompanyAmount <= 0)
                {
                    exception = " invoice is null or invoice.TotalCompanyAmount<=0 ";
                    return hyperpayRequest;
                }
                if (!invoice.InsuranceCompanyId.HasValue)
                {
                    exception = " invoice.InsuranceCompanyId is null ";
                    return hyperpayRequest;
                }
                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    exception = "companyBankAccount is null ";
                    return hyperpayRequest;
                }
                var beneficiary = new BeneficiaryModel()
                {
                    name = companyBankAccount.BeneficiaryName,
                    accountId = companyBankAccount.IBAN,
                    address1 = companyBankAccount.Address1,
                    address2 = companyBankAccount.Address2,
                    address3 = companyBankAccount.Address3,
                    bankIdBIC = companyBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = invoice.TotalCompanyAmount?.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiaries = new List<BeneficiaryModel>();
                beneficiaries.Add(beneficiary);

                var payOut = new PayoutModel()
                {
                    transferOption = "0", // ‘0’ --> real time
                    configId = Utilities.GetAppSetting("HyperpaykeyConfigId"), // "b29db28c3834dceece73c41ff39f1737", // sent before by mail
                    beneficiary = beneficiaries
                };

                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonSerializer.Serialize(payOut);

                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}" +
                    $"&customParameters[payout]={json}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;

                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";
                if (isQrCode)
                    data += $"&customParameters[SHOPPER_payment_mode]=qr_code";
                else
                    data += $"&customParameters[SHOPPER_payment_mode]=mobile&customer.mobile=" + phoneNumber; // STCPAY mobile number 05xxxxxxxx;

                string url = _config.Hyperpay.Url;
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                request.ContentType = "application/x-www-form-urlencoded";
                string exp = "";
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                    hyperpayRequest.ResponseId = responseData["id"];
                    hyperpayRequest.ResponseNdc = responseData["ndc"];
                    hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                    hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                    hyperpayRequest.ResponseCode = responseData["result"]["code"];
                    hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                }
                hyperpayRequestRepository.Insert(hyperpayRequest);
                //return responseData;
                return hyperpayRequest;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                return null;
            }

        }

        public HyperpayRequest RequestHyperpayUrlForSTCPay(HyperpayRequest hyperpayRequest, out string exception)
        {
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;

                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";

                string url = _config.Hyperpay.Url;
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                request.ContentType = "application/x-www-form-urlencoded";
                string exp = "";
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                    hyperpayRequest.ResponseId = responseData["id"];
                    hyperpayRequest.ResponseNdc = responseData["ndc"];
                    hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                    hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                    hyperpayRequest.ResponseCode = responseData["result"]["code"];
                    hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                }
                hyperpayRequestRepository.Insert(hyperpayRequest);
                //return responseData;
                return hyperpayRequest;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                return null;
            }

        }
        public HyperpayRequest RequestHyperpayUrlForSTCPayForMobile(HyperpayRequest hyperpayRequest, string phoneNumber, bool isQrCode, out string exception)
        {
            exception = "";
            string responseString = string.Empty;
            string data = string.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
            {
                exception = "The payment request not linked to checkout details.";
            }
            try
            {
                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode=EXTERNAL&merchantTransactionId={hyperpayRequest.ReferenceId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.ReferenceId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;

                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";
                if (isQrCode)
                    data += $"&customParameters[SHOPPER_payment_mode]=qr_code";
                else
                    data += $"&customParameters[SHOPPER_payment_mode]=mobile&customer.mobile=" + phoneNumber; // STCPAY mobile number 05xxxxxxxx;

                string url = _config.Hyperpay.Url;
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                request.ContentType = "application/x-www-form-urlencoded";
                string exp = "";
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                    hyperpayRequest.ResponseId = responseData["id"];
                    hyperpayRequest.ResponseNdc = responseData["ndc"];
                    hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                    hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                    hyperpayRequest.ResponseCode = responseData["result"]["code"];
                    hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                }
                hyperpayRequestRepository.Insert(hyperpayRequest);
                //return responseData;
                return hyperpayRequest;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                return null;
            }

        }
        public Dictionary<string, dynamic> RequestHyperpayToValidResponse(string id, out string exception)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServiceURL = _config.Hyperpay.Url;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "HyperpayToValidateResponse";
            log.ReferenceId = id;
            log.ServiceURL = _config.Hyperpay.Url;
            log.ServerIP = Utilities.GetInternalServerIP();
            Dictionary<string, dynamic> responseData;
            exception = string.Empty;
            try
            {
                string data = $"entityId={_config.Hyperpay.EntityId}";
                string url = $"{_config.Hyperpay.Url}/{id}/payment?" + data;
                log.ServiceRequest = id+"/payment?" + data;
 

                 HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                //request.ContentType = "application/x-www-form-urlencoded";
                //responseData = SendRequest(url, _config.Hyperpay.AccessToken);
                //return responseData;

                //Stream PostData = request.GetRequestStream();
                //PostData.Write(buffer, 0, buffer.Length);
                //PostData.Close();

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    var s = new JavaScriptSerializer();
                    responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }
                log.ServiceResponse = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                log.ErrorCode = 1;
                log.ErrorDescription = "Succcess";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return responseData;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            var s = new JavaScriptSerializer();
                            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                            reader.Close();

                        }
                    }
                }
                log.ServiceResponse = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                log.ErrorCode = 500;
                log.ErrorDescription = exception;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (responseData !=null&& responseData.ContainsKey("result")&& responseData["result"]["code"]== "200.300.404")
                {
                   var dbResponse =hyperpayResponseRepository.TableNoTracking.Where(a => a.Ndc == id).OrderByDescending(a => a.Id).FirstOrDefault();
                   if(dbResponse!=null)
                    {
                        var s = new JavaScriptSerializer();
                        responseData = s.Deserialize<Dictionary<string, dynamic>>(dbResponse.ServiceResponse);
                    }
                }
                return responseData;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                log.ErrorCode = 500;
                log.ErrorDescription = exception;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return null;
            }

        }
        //public Dictionary<string, dynamic> Search(string transactionId, out string exception)
        //{
        //    Dictionary<string, dynamic> responseData;
        //    exception = string.Empty;
        //    try
        //    {
        //        string data = $"entityId={_config.Hyperpay.EntityId}" + "&merchantTransactionId=" + transactionId;
        //        string url = $"{_config.Hyperpay.QueryURL}" + data;

        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Method = "GET";
        //        request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            Stream dataStream = response.GetResponseStream();
        //            StreamReader reader = new StreamReader(dataStream);
        //            var s = new JavaScriptSerializer();
        //            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //            reader.Close();
        //            dataStream.Close();
        //        }
        //        return responseData;
        //    }
        //    catch (WebException e)
        //    {
        //        using (WebResponse response = e.Response)
        //        {
        //            HttpWebResponse httpResponse = (HttpWebResponse)response;
        //            using (Stream data = response.GetResponseStream())
        //            {
        //                using (var reader = new StreamReader(data))
        //                {
        //                    var s = new JavaScriptSerializer();
        //                    responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //                    reader.Close();
        //                }
        //            }
        //        }
        //        return responseData;
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex.ToString();
        //        return null;
        //    }
        //}
        public Dictionary<string, dynamic> Search(string transactionId,string channel, out string exception)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServiceURL = _config.Hyperpay.Url;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "HyperpaySearch";
            log.ReferenceId = transactionId;
            log.ServiceURL = _config.Hyperpay.QueryURL;
            Dictionary<string, dynamic> responseData;
            exception = string.Empty;
            string serviceResponse = string.Empty;
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                string data = $"entityId={_config.Hyperpay.EntityId}" + "&merchantTransactionId=" + transactionId;
                log.ServiceRequest = data;
                string url = $"{_config.Hyperpay.QueryURL}" + data;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
                dtBeforeCalling = DateTime.Now;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    var s = new JavaScriptSerializer();
                    responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                serviceResponse = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                if (responseData == null || responseData.Count() == 0)
                {
                    log.ErrorDescription = "response is null or count is 0";
                    log.ErrorCode = 2;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return null;
                }
                if (responseData["result"]["code"] == "700.400.580")
                {
                    log.ServiceResponse = serviceResponse;
                    log.ErrorCode = 404;
                    log.ErrorDescription = "Failed";
                    log.ServiceErrorCode = responseData["result"]["code"];
                    log.ServiceErrorDescription = responseData["result"]["description"];
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return responseData;
                }
                if (responseData["payments"][0].ContainsKey("result"))
                {
                    log.ServiceResponse = responseData["payments"][0]["result"]["description"];
                    log.ServiceErrorCode = responseData["payments"][0]["result"]["code"];
                }
                var paymentSucceded = false;
                if (Regex.IsMatch(responseData["payments"][0]["result"]["code"], "^(000.000.|000.100.1|000.[36])", RegexOptions.IgnoreCase) || Regex.IsMatch(responseData["result"]["code"], "(000.400.0|000.400.100)"))
                {
                    paymentSucceded = true;
                }
                if (paymentSucceded)
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Succcess";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                }
                else
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "Failed";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                }
                return responseData;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            var s = new JavaScriptSerializer();
                            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                            reader.Close();
                        }
                    }
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                log.ErrorCode = 500;
                log.ErrorDescription = exception;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return responseData;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                log.ServiceResponse = serviceResponse;
                log.ErrorCode = 500;
                log.ErrorDescription = exception;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return null;
            }
        }
        //public Dictionary<string, dynamic> RequestHyperpayToValidResponse(string id, out string exception)
        //{
        //    Dictionary<string, dynamic> responseData;
        //    exception = string.Empty;
        //    try
        //    {

        //        string data = $"entityId={_config.Hyperpay.EntityId}";
        //        string url = $"{_config.Hyperpay.Url}/{id}/payment?" + data;


        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Method = "GET";
        //        request.Headers["Authorization"] = $"Bearer {_config.Hyperpay.AccessToken}";
        //        //request.ContentType = "application/x-www-form-urlencoded";
        //        //responseData = SendRequest(url, _config.Hyperpay.AccessToken);
        //        //return responseData;

        //        //Stream PostData = request.GetRequestStream();
        //        //PostData.Write(buffer, 0, buffer.Length);
        //        //PostData.Close();

        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            Stream dataStream = response.GetResponseStream();
        //            StreamReader reader = new StreamReader(dataStream);
        //            var s = new JavaScriptSerializer();
        //            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //            reader.Close();
        //            dataStream.Close();
        //        }
        //        return responseData;


        //    }
        //    catch (WebException e)
        //    {
        //        using (WebResponse response = e.Response)
        //        {
        //            HttpWebResponse httpResponse = (HttpWebResponse)response;
        //            using (Stream data = response.GetResponseStream())
        //            {
        //                using (var reader = new StreamReader(data))
        //                {
        //                    var s = new JavaScriptSerializer();
        //                    responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //                    reader.Close();

        //                }
        //            }
        //        }
        //        return responseData;
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex.ToString();
        //        return null;
        //    }

        //}
        public List<HyperpayRequest> GetAllMissingTransactionsFromHyperPayRequest(DateTime startDate, DateTime endDate,out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMissingTransactionsFromHyperPayRequest";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter startDateParam = new SqlParameter() { ParameterName = "@startDate", Value = startDate };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "@endDate", Value = endDate };
                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<HyperpayRequest> hyperpayRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequest>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return hyperpayRequest;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
            //try
            //{
            //    var listOfRef = hyperpayRequestRepository.TableNoTracking.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate && !string.IsNullOrEmpty(c.ResponseId) && string.IsNullOrEmpty(c.StatusDescription)).OrderByDescending(c => c.CreatedDate).ToList();
            //    if (listOfRef != null && listOfRef.Count > 0)
            //        return listOfRef;
            //    else
            //    {
            //        return null;
            //    }
            //}
            //catch (Exception exp)
            //{
            //    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\log22.txt", exp.ToString());
            //    return null;
            //}
        }

        #region LeasingHyperPay

        public HyperSplitOutput RequestHyperpayForLeasing(HyperpayRequest hyperpayRequest, int companyId, string companyName, string channel, Guid merchantTransactionId, out string exception)
        {
            File.WriteAllText(@"C:\inetpub\WataniyaLog\RequestHyperpayForLeasing.txt", "enter");

            exception = String.Empty;
            if (hyperpayRequest.CheckoutDetails.Count < 1)
                exception = "The payment request not linked to checkout details.";

            hyperpayRequest.MerchantTransactionId = merchantTransactionId;
            HyperSplitOutput output = new HyperSplitOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServiceURL = _config.Hyperpay.Url;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "RequestHyperpayForLeasing";
            log.ReferenceId = hyperpayRequest.ReferenceId;
            log.CompanyID = companyId;
            log.CompanyName = companyName;
            log.Channel = channel;
            string responseString = string.Empty;
            string data = string.Empty;

            try
            {
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == hyperpayRequest.ReferenceId);
                if (invoice == null)
                {
                    exception = " invoice is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (invoice.TotalCompanyAmount <= 0)
                {
                    exception = "invoice.TotalCompanyAmount<=0 as it's " + invoice.TotalCompanyAmount;
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.TotalCompanyAmountIsLessZero;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;

                }
                if (!invoice.InsuranceCompanyId.HasValue)
                {
                    exception = " invoice.InsuranceCompanyId is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.InsuranceCompanyIdIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0)
                    hyperpayRequest.Amount = hyperpayRequest.Amount - invoice.TotalBCareDiscount.Value;

                var bcareBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == 15); // need to change it to new BCare account IBAN
                if (bcareBankAccount == null)
                {
                    exception = "bcare Bank Account is null ";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.BCareBankAccountIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var amount = invoice.TotalPrice; // (invoice.TotalCompanyAmount + invoice.TotalBCareFees + invoice.TotalBCareCommission + invoice.ActualBankFees);
                var beneficiaryBcare = new BeneficiaryModel()
                {
                    name = bcareBankAccount.BeneficiaryName,
                    accountId = bcareBankAccount.IBAN,
                    address1 = bcareBankAccount.Address1,
                    address2 = bcareBankAccount.Address2,
                    address3 = bcareBankAccount.Address3,
                    bankIdBIC = bcareBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = amount?.ToString(),
                    transferCurrency = "SAR"
                };

                var beneficiaries = new List<BeneficiaryModel>() { beneficiaryBcare };
                var payOut = new PayoutModel()
                {
                    transferOption = "7", // ‘0’ --> real time; 7 batch
                    period = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                    configId = Utilities.GetAppSetting("HyperpaykeyConfigId"), // "b29db28c3834dceece73c41ff39f1737", // sent before by mail
                    beneficiary = beneficiaries
                };

                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonSerializer.Serialize(payOut);

                Dictionary<string, dynamic> responseData;
                data = $"entityId={_config.Hyperpay.EntityId}" +
                    $"&amount={hyperpayRequest.Amount}" +
                    $"&merchantInvoiceId={hyperpayRequest.ReferenceId}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}" +
                    $"&customParameters[payout]={json}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode={_config.Hyperpay.TestMode}&merchantTransactionId={hyperpayRequest.MerchantTransactionId}";
                else
                    data += $"&merchantTransactionId={hyperpayRequest.MerchantTransactionId}";

                data += "&customer.email=" + hyperpayRequest.UserEmail;
                data += "&customParameters[branch_id]=1";
                data += "&customParameters[teller_id]=1";
                data += "&customParameters[device_id]=1";
                data += $"&customParameters[bill_number]={hyperpayRequest.ReferenceId}";

                string url = _config.Hyperpay.Url;
                string exp = "";
                log.ServiceRequest = data;
                responseString = SendRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                log.ServiceResponse = responseString;

                if (string.IsNullOrEmpty(responseString))
                {
                    exception = "responseString is null or empty";
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.ResponseIsNull;
                    output.ErrorDescription = exception;
                    output.HyperpayRequest = hyperpayRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var serializer = new JavaScriptSerializer();
                responseData = serializer.Deserialize<Dictionary<string, dynamic>>(responseString);
                hyperpayRequest.ResponseId = responseData["id"];
                hyperpayRequest.ResponseNdc = responseData["ndc"];
                hyperpayRequest.ResponseBuildNumber = responseData["buildNumber"];
                hyperpayRequest.ResponseTimestamp = responseData["timestamp"];
                hyperpayRequest.ResponseCode = responseData["result"]["code"];
                hyperpayRequest.ResponseDescription = responseData["result"]["description"];
                hyperpayRequest.ServiceRequest = jsonSerializer.Serialize(data);
                hyperpayRequestRepository.Insert(hyperpayRequest);

                output.ErrorCode = HyperSplitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.HyperpayRequest = hyperpayRequest;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                exception += exp.ToString() + " And responseString: " + responseString;
                output.ErrorCode = HyperSplitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exception;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        #endregion

        #region Private Methods

        private string SendRequest(string url, string request, string AccessToken, out string ex)
        {
            ex = "";
            var statusCode = new HttpStatusCode();
            try
            {
                string response = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers["Authorization"] = $"Bearer {AccessToken}";
                try
                {
                    Utilities.InitiateSSLTrust();
                }
                catch
                {

                }
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                statusCode = httpResponse.StatusCode;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }
                return response;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        var s = new JavaScriptSerializer();
                        Dictionary<string, dynamic> responseData;
                        using (var reader = new StreamReader(data))
                        {
                            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                            reader.Close();
                        }
                        ex = s.Serialize(responseData);
                    }
                }
                return null;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                ex = exp.ToString();
                return null;
            }
        }

        private Dictionary<string, dynamic> SendRequest(string url, string AccessToken)
        {
            var statusCode = new HttpStatusCode();
            try
            {
                Dictionary<string, dynamic> responseData;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = $"Bearer {AccessToken}";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    var serializer = new JavaScriptSerializer();
                    responseData = serializer.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }
                return responseData;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

        /// <summary>
        /// Update checkout payment status
        /// </summary>
        /// <param name="checkoutDetailsId">Reference ID</param>
        /// <param name="response">payment response</param>
        public bool UpdateCheckoutPaymentStatus(CheckoutDetail checkoutDetail, bool paymentSuccessfull, string channel, int paymentMethodId)
        {
            string exception = string.Empty;
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.ReferenceId = checkoutDetail?.ReferenceId;
            log.PaymentMethod = Enum.GetName(typeof(PaymentMethodCode), paymentMethodId);
            log.CompanyId = checkoutDetail?.InsuranceCompanyId;
            log.CompanyName = checkoutDetail?.InsuranceCompanyName;
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.MethodName = "UpdateCheckoutPaymentStatus";
            DateTime dtBefore = DateTime.Now;
            try
            {
                if (paymentSuccessfull) // Payment succeeded 
                {
                    if (checkoutDetail.PolicyStatusId != (int)EPolicyStatus.Available)
                    {
                        _checkoutsService.UpdateCheckoutWithPaymentStatus(checkoutDetail.ReferenceId, (int)EPolicyStatus.PaymentSuccess, paymentMethodId, out exception);
                        if (!string.IsNullOrEmpty(exception))
                        {
                            var checkoutdetatils = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == checkoutDetail.ReferenceId && c.IsCancelled == false);
                            if (checkoutdetatils != null && checkoutdetatils.PolicyStatusId != (int)EPolicyStatus.Available)
                            {
                                checkoutdetatils.PolicyStatusId = paymentSuccessfull ? (int)EPolicyStatus.PaymentSuccess : (int)EPolicyStatus.PaymentFailure;
                                checkoutdetatils.ModifiedDate = DateTime.Now;
                                _checkoutDetailRepository.Update(checkoutdetatils);
                            }
                            log.CreatedDate = DateTime.Now;
                            log.ErrorCode = 2;
                            log.ErrorDescription = exception;
                            log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBefore).TotalSeconds;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        }
                    }
                    exception = string.Empty;
                    _policyProcessingService.InsertIntoProcessingQueue(checkoutDetail.ReferenceId, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.InsuranceCompanyName, channel, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetail.ReferenceId, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.InsuranceCompanyName, channel);
                        log.CreatedDate = DateTime.Now;
                        log.ErrorCode = 3;
                        log.ErrorDescription = exception;
                        log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBefore).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                }
                else
                {
                    _checkoutsService.UpdateCheckoutWithPaymentStatus(checkoutDetail.ReferenceId, (int)EPolicyStatus.PaymentFailure, paymentMethodId, out exception);
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                log.CreatedDate = DateTime.Now;
                log.ErrorCode = 5;
                log.ErrorDescription = exception;
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBefore).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return false;
            }
        }

        /// <summary>
        /// Get riyadbank request from database by MerchantTxtRef & reference Id of checkout
        /// </summary>
        /// <param name="referenceId">The checkout details reference identifier.</param>
        /// <param name="merchantReferenceTxt">The merchant reference text.</param>
        /// <returns></returns>
        public HyperpayRequest HyperpayRequestByMerchantReference(string referenceId)
        {
            //return hyperpayRequestRepository.Table.Include(e => e.CheckoutDetails.Select(c => c.OrderItems.Select(o => o.Product.QuotationResponse.ProductType)))
            //    .FirstOrDefault(req => req.ResponseBuildNumber == buildNumber && req.CheckoutDetails.Any(c => c.ReferenceId == referenceId));
            return hyperpayRequestRepository.Table.Where(e => e.ReferenceId == referenceId).OrderByDescending(e => e.Id).FirstOrDefault();
        }

        public void UpdateHyperRequest(HyperpayRequest hyperpayRequest)
        {
            hyperpayRequestRepository.Update(hyperpayRequest);
        }


        public HyperpayRequest GetById(int Id)
        {
            return hyperpayRequestRepository.Table.Include(x => x.CheckoutDetails).FirstOrDefault(x => x.Id == Id);
        }

        public List<HyperPayNotificationInfo> GetFailedPaymentTransferredOperation(DateTime startDate, DateTime endDate, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFailedPaymentTransferredOperation";
                command.CommandType = CommandType.StoredProcedure;
                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-2);
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-1);
                command.CommandTimeout = 600;
                SqlParameter startDateParam = new SqlParameter() { ParameterName = "startDate", Value = start };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "endDate", Value = end };
                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<HyperPayNotificationInfo> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperPayNotificationInfo>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public void RetryFailedSplitOperation(out string exception)
        {
            exception = string.Empty;
            try
            {
                DateTime startDate = DateTime.Now.AddHours(-5);
                DateTime endDate = DateTime.Now.AddMinutes(-10);

                var failedTransactions = GetFailedPaymentTransferredOperation(startDate, endDate, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\PayoutRetrialMechanism.txt", exception);
                    return;
                }
                if (failedTransactions == null || failedTransactions.Count() == 0)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\PayoutRetrialMechanismCount.txt", "count is zero");
                    return;
                }

                foreach (var transaction in failedTransactions)
                {
                    var hyperpayRequest = JsonConvert.DeserializeObject<HyperpayRequestModel>(transaction.ServiceRequest);
                    if (hyperpayRequest == null)
                        continue;
                    if (hyperpayRequest.CustomParametersPayout == null)
                        continue;
                    var beneficiaryInfo = hyperpayRequest.CustomParametersPayout.Beneficiary.FirstOrDefault();
                    if (beneficiaryInfo == null)
                        continue;
                    SplitOrdersModel ordermodel = new SplitOrdersModel();
                    ordermodel.ConfigId = Utilities.GetAppSetting("HyperpaykeyConfigId");
                    ordermodel.MerchantTransactionId = transaction.ReferenceId;
                    ordermodel.TransferOption = "0";
                    ordermodel.MerchantRequestId = transaction.ReferenceId;

                    SplitBeneficiaryModel beneficiary = new SplitBeneficiaryModel();
                    beneficiary.AccountId = beneficiaryInfo.AccountId;
                    beneficiary.Name = beneficiaryInfo.Name;
                    beneficiary.TransferAmount = beneficiaryInfo.TransferAmount;
                    beneficiary.TransferCurrency = beneficiaryInfo.TransferCurrency;
                    beneficiary.DebitCurrency = beneficiaryInfo.DebitCurrency;
                    beneficiary.Address1 = beneficiaryInfo.Address1;
                    beneficiary.Address2 = beneficiaryInfo.Address2;
                    beneficiary.Address3 = beneficiaryInfo.Address3;
                    ordermodel.Beneficiary.Add(beneficiary);
                    var output = SendHyperpaySplitRequest(ordermodel, transaction);
                    if (output.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }
        public HyperSplitOutput SendHyperpaySplitRequest(SplitOrdersModel orderModel, HyperPayNotificationInfo request)
        {
            string exception = string.Empty;
            HyperSplitOutput output = new HyperSplitOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = request.ReferenceId;
            log.CompanyID = request.InsuranceCompanyId;
            log.CompanyName = request.InsuranceCompanyName;
            log.Channel = request.Channel;
            log.ServiceURL = _config.Hyperpay.SplitLoginUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "HyperpaySplit";
            log.ReferenceId = request.ReferenceId;
            try
            {
                if (orderModel == null)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "Model Is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (orderModel.Beneficiary == null || orderModel.Beneficiary.Count == 0)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "Beneficiary Is Null or empty or count is 0";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.ConfigId))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "ConfigId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.MerchantTransactionId))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "merchantTransactionId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.TransferOption))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "TransferOption Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(_config.Hyperpay.SplitOrderUrl))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "split order url Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var loginInfo = SendSplitLoginRequest(request);
                if (loginInfo.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "failed to login due to " + loginInfo.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(orderModel);
                log.ServiceRequest = stringPayload;
                DateTime dtBeforeCalling = DateTime.Now;
                var response = SendRequest(_config.Hyperpay.SplitOrderUrl, stringPayload, loginInfo.AccessToken, out exception);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "an exception occured while sending request to split login which is " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "login service response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var responseInfo = JsonConvert.DeserializeObject<SplitOrdersModel>(response);
                if (string.IsNullOrEmpty(responseInfo.UniqueId))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "UniqueId is null to login";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseInfo.Beneficiary == null || responseInfo.Beneficiary.Count() == 0)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "responseInfo.Beneficiary is null or count is 0";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ErrorCode = HyperSplitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = HyperSplitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public HyperSplitOutput SendSplitLoginRequest(HyperPayNotificationInfo request)
        {
            string exception = string.Empty;
            HyperSplitOutput output = new HyperSplitOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = request.ReferenceId;
            log.CompanyID = request.InsuranceCompanyId;
            log.CompanyName = request.InsuranceCompanyName;
            log.Channel = request.Channel;
            log.ServiceURL = _config.Hyperpay.SplitLoginUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "SplitLogin";
            log.ReferenceId = request.ReferenceId;
            try
            {
                if (string.IsNullOrEmpty(_config.Hyperpay.SplitLoginUrl))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "split Login url Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(_config.Hyperpay.SplitLoginEmail))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "Split Login Email Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(_config.Hyperpay.SplitLoginPassword))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "Split Login Password Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                SplitLoginModel loginModel = new SplitLoginModel();
                loginModel.Email = _config.Hyperpay.SplitLoginEmail;
                loginModel.Password = _config.Hyperpay.SplitLoginPassword;

                //var stringPayload = JsonConvert.SerializeObject(loginModel);
                //log.ServiceRequest = stringPayload;
                //DateTime dtBeforeCalling = DateTime.Now;
                //var loginResponse = SendRequest(_config.Hyperpay.SplitLoginUrl, stringPayload,string.Empty, out exception);
                //DateTime dtAfterCalling = DateTime.Now;
                //log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;


                var stringPayload = JsonConvert.SerializeObject(loginModel);
                log.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64Autoleasing);
                Utilities.InitiateSSLTrust();
                var postTask = client.PostAsync(_config.Hyperpay.SplitLoginUrl, httpContent);
                postTask.Wait();

                var response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                //if (!string.IsNullOrEmpty(exception))
                //{
                //    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                //    output.ErrorDescription = "an exception occured while sending request to split login which is "+exception;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return output;
                //}
                if (response.Content == null)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "login service response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "login service response Content is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var loginInfo = JsonConvert.DeserializeObject<SplitLoginResponseModel>(response.Content.ReadAsStringAsync().Result);
                if (!loginInfo.Status)
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "failed to login";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (loginInfo.data == null || string.IsNullOrEmpty(loginInfo.data.AccessToken))
                {
                    output.ErrorCode = HyperSplitOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "login Info data or accesstoken is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.AccessToken = loginInfo.data.AccessToken;
                output.ErrorCode = HyperSplitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = HyperSplitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }


        public int UpdateHyperpayRequestStatus(int Id, string referenceId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateHyperpayRequestStatus";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                SqlParameter IdParam = new SqlParameter() { ParameterName = "Id", Value = Id };
                command.Parameters.Add(referenceIdParam);
                command.Parameters.Add(IdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }            catch            {
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }
        public HyperpayRequest GetHyperpayRequestByReferenceId(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetHyperpayRequestByReferenceId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var hyperpayRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequest>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return hyperpayRequest;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public HyperpayUpdateOrderOutput UpdateOrder(HyperpayOrderModel orderModel, string referenceId, string channel, string accessToken, string paymentBrand, bool isBcareUpdateOrder, CheckoutDetail checkoutDetail)
        {
            string exception = string.Empty;
            HyperpayUpdateOrderOutput output = new HyperpayUpdateOrderOutput();
            HyperPayUpdateOrder hyperPayUpdateOrder = new HyperPayUpdateOrder();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = referenceId;
            log.Channel = channel;
            log.ServiceURL = _config.Hyperpay.UpdateOrderUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "HayperPayUpdateOrder";
            log.CompanyID = checkoutDetail.InsuranceCompanyId;
            log.CompanyName = checkoutDetail.InsuranceCompanyName;
            try
            {
                if (string.IsNullOrEmpty(_config.Hyperpay.UpdateOrderUrl))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "UpdateOrderUrl url Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "referenceId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.Amount))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "amount Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.BeneficiaryAccountId))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "beneficiaryAccountId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.ConfigId))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "ConfigId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.UniqueId))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "UniqueId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(orderModel);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                log.ServiceRequest = stringPayload;
                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);

                Utilities.InitiateSSLTrust();
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, _config.Hyperpay.UpdateOrderUrl);
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = httpContent;
                var postTask = client.SendAsync(requestMessage);
                postTask.Wait();

                var response = postTask.Result;

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                // Hyperpay Update Order Request
                hyperPayUpdateOrder.IsBcareUpdateOrder = isBcareUpdateOrder;
                hyperPayUpdateOrder.ServiceRequest = log.ServiceRequest;
                hyperPayUpdateOrder.RequestAmount = orderModel.Amount;
                hyperPayUpdateOrder.RequestConfigId = orderModel.ConfigId;
                hyperPayUpdateOrder.RequestUniqueId = orderModel.UniqueId;
                hyperPayUpdateOrder.RequestBeneficiaryAccountId = orderModel.BeneficiaryAccountId;
                hyperPayUpdateOrder.RequestPaymentBrand = paymentBrand;
                hyperPayUpdateOrder.Channel = channel;
                hyperPayUpdateOrder.ReferenceId = referenceId;
                hyperPayUpdateOrder.ServiceResponse = log.ServiceResponse;
                hyperPayUpdateOrder.CreatedDate = DateTime.Now;

                //if (!string.IsNullOrEmpty(exception))
                //{
                //    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.ServiceException;
                //    output.ErrorDescription = "an exception occured while sending request to update order which is " + exception;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return output;
                //}
                if (response.Content == null)
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "update order service response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayUpdateOrderRepository.Insert(hyperPayUpdateOrder);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "update order service response Content is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayUpdateOrderRepository.Insert(hyperPayUpdateOrder);
                    return output;
                }
                var OrderInfo = JsonConvert.DeserializeObject<UpdateOrderResponseModel>(response.Content.ReadAsStringAsync().Result);
                // Hyperpay Update Order Response
                hyperPayUpdateOrder.ResponseStatus = OrderInfo.Status;
                hyperPayUpdateOrder.ResponseMessage = OrderInfo.Message;

                if (!OrderInfo.Status)
                {
                    output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.StatusFailed;
                    output.ErrorDescription = "update order service status is false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayUpdateOrderRepository.Insert(hyperPayUpdateOrder);
                    return output;
                }

                if (OrderInfo.data != null && OrderInfo.data.Any())
                {
                    var data = OrderInfo.data.FirstOrDefault();
                    hyperPayUpdateOrder.ResponseAmount = data.Amount;
                    hyperPayUpdateOrder.PayoutStatus = data.PayoutStatus;
                    hyperPayUpdateOrder.PayoutBeneficiaryName = data.PayoutBeneficiaryName;
                    hyperPayUpdateOrder.PaymentType = data.PaymentType;
                }

                output.ResponseModel = OrderInfo;
                output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                _hyperPayUpdateOrderRepository.Insert(hyperPayUpdateOrder);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = HyperpayUpdateOrderOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                _hyperPayUpdateOrderRepository.Insert(hyperPayUpdateOrder);
                return output;
            }
        }
        public HyperpayResponse GetFromHyperpayResponseSuccessTransaction(int HyperpayRequestId, string hyperpayResponseId, decimal amount)
        {
            return hyperpayResponseRepository.TableNoTracking.FirstOrDefault(req => req.Message == "Transaction succeeded" && req.HyperpayRequestId == HyperpayRequestId && req.HyperpayResponseId == hyperpayResponseId && req.Amount == amount);
        }
        public bool InsertHyperpayResponse(HyperpayResponse hyperpayResponse)
        {
            hyperpayResponseRepository.Insert(hyperpayResponse);
            return true;

        }

        public HyperpayCreateOrderOutput CreateOrder(HyperpayCreateOrderModel orderModel, string referenceId, string channel, string accessToken, string paymentBrand)
        {
            string exception = string.Empty;
            HyperpayCreateOrderOutput output = new HyperpayCreateOrderOutput();
            HyperPayCreateOrder hyperPayCreateOrder = new HyperPayCreateOrder();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = referenceId;
            log.Channel = channel;
            log.ServiceURL = _config.Hyperpay.CreateOrderUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "HayperPayCreateOrder";
            try
            {
                if (string.IsNullOrEmpty(_config.Hyperpay.CreateOrderUrl))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "CreateOrderUrl url Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "referenceId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (string.IsNullOrEmpty(orderModel.MerchantTransactionId))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "MerchantTransactionId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.ConfigId))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "ConfigId Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(orderModel.TransferOption))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.MissingInput;
                    output.ErrorDescription = "TransferOption Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(orderModel);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                log.ServiceRequest = stringPayload;
                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);

                Utilities.InitiateSSLTrust();
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _config.Hyperpay.CreateOrderUrl);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = httpContent;
                var postTask = client.SendAsync(requestMessage);
                postTask.Wait();

                var response = postTask.Result;

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                // Hyperpay Create Order Request
                hyperPayCreateOrder.ServiceRequest = log.ServiceRequest;
                hyperPayCreateOrder.MerchantTransactionId = orderModel.MerchantTransactionId;
                hyperPayCreateOrder.RequestConfigId = orderModel.ConfigId;
                hyperPayCreateOrder.Period = orderModel.Period;
                hyperPayCreateOrder.TransferOption = orderModel.TransferOption;
                hyperPayCreateOrder.RequestPaymentBrand = paymentBrand;
                hyperPayCreateOrder.Channel = channel;
                hyperPayCreateOrder.ReferenceId = referenceId;
                hyperPayCreateOrder.ServiceResponse = log.ServiceResponse;
                hyperPayCreateOrder.CreatedDate = DateTime.Now;

                if (response.Content == null)
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "create order service response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayCreateOrderRepository.Insert(hyperPayCreateOrder);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "create order service response Content is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayCreateOrderRepository.Insert(hyperPayCreateOrder);
                    return output;
                }
                var OrderInfo = JsonConvert.DeserializeObject<CreateOrderResponseModel>(response.Content.ReadAsStringAsync().Result);
                // Hyperpay Create Order Response
                hyperPayCreateOrder.ResponseStatus = OrderInfo.Status;
                hyperPayCreateOrder.ResponseMessage = OrderInfo.Message;

                if (!OrderInfo.Status)
                {
                    output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.StatusFailed;
                    output.ErrorDescription = "update order service status is false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    _hyperPayCreateOrderRepository.Insert(hyperPayCreateOrder);
                    return output;
                }

                if (OrderInfo.data != null)
                {
                    hyperPayCreateOrder.UniqueId = OrderInfo.data.UniqueId;
                }

                output.ResponseModel = OrderInfo;
                output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                _hyperPayCreateOrderRepository.Insert(hyperPayCreateOrder);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = HyperpayCreateOrderOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                _hyperPayCreateOrderRepository.Insert(hyperPayCreateOrder);
                return output;
            }
        }
        public ApplePayOutput StartApplePaySession(string referenceId, string companyName, int companyId, string channel)
        {
            ApplePayOutput output = new ApplePayOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = referenceId;
            log.CompanyName = companyName;
            log.CompanyID = companyId;
            log.Channel = channel;
            log.ServiceURL = _config.Hyperpay.ApplePaySessionUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "StartApplePaySession";
            try
            {
                var stringPayload = "{\"merchantIdentifier\" : \"" + _config.Hyperpay.ApplePayMerchantIdentifier + "\", \"domainName\" : \"" + _config.Hyperpay.ApplePayDomainName + "\",  \"displayName\" : \"BCare\" }";
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                log.ServiceRequest = stringPayload;
                X509Certificate2 SSLCERT = new X509Certificate2(_config.Hyperpay.ApplePaySslCertPath, _config.Hyperpay.ApplePaySslCertPWD);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(SSLCERT);

                var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpClient _httpClient = new HttpClient(handler);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;

                var postTask = _httpClient.PostAsync(_config.Hyperpay.ApplePaySessionUrl, content);
                    postTask.Wait();
                 var response = postTask.Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                if (response.Content == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "create order service response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.EmptyResponse;
                    output.ErrorDescription = "create order service response Content is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var value = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ApplePaySessionResponseModel>(value);
                if(!string.IsNullOrEmpty(result.StatusCode)&& result.StatusCode=="400")
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.invalidDomain;
                    output.ErrorDescription = "invalid domain";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!string.IsNullOrEmpty(result.StatusCode) && result.StatusCode == "417")
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidMerchantIdentifier;
                    output.ErrorDescription = "invalid Merchant Identifier";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!string.IsNullOrEmpty(result.StatusMessage) )
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = result.StatusMessage;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString()+ "--- ApplePaySslCertPath:" + _config.Hyperpay.ApplePaySslCertPath;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        public bool InsertHyperpayReuest(HyperpayRequest hyperpayRequest)
        {
            hyperpayRequestRepository.Insert(hyperpayRequest);
            return true;

        }
        public ApplePayOutput ApplePayPayment(ApplePayRequestInfo requestInfo, ApplePayPaymnetTokenModel paymentToken)
        {
            ApplePayOutput output = new ApplePayOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServiceURL = _config.Hyperpay.ApplePayPaymentUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "ApplePayPayment";
            log.ReferenceId = requestInfo.ReferenceId;
            log.CompanyID = requestInfo.InsuranceCompanyId;
            log.CompanyName = requestInfo.InsuranceCompanyName;
            log.Channel = requestInfo.Channel;
            string responseString = string.Empty;
            string data = string.Empty;
            try
            {
                var bcareBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == 15);
                if (bcareBankAccount == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.BCareBankAccountIsNull;
                    output.ErrorDescription = "bcareBankAccount is null";
                    return output;
                }
                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == requestInfo.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.CompanyBankAccountIsNull;
                    output.ErrorDescription = "companyBankAccount is null";
                    return output;
                }
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == requestInfo.ReferenceId);
                if (invoice == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = "invoice is null";
                    return output;
                }

                var beneficiaryBcare = new BeneficiaryModel()
                {
                    name = bcareBankAccount.BeneficiaryName,
                    accountId = bcareBankAccount.IBAN,
                    address1 = bcareBankAccount.Address1,
                    address2 = bcareBankAccount.Address2,
                    address3 = bcareBankAccount.Address3,
                    bankIdBIC = bcareBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = (invoice.TotalBCareFees + invoice.TotalBCareCommission)?.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiary = new BeneficiaryModel()
                {
                    name = companyBankAccount.BeneficiaryName,
                    accountId = companyBankAccount.IBAN,
                    address1 = companyBankAccount.Address1,
                    address2 = companyBankAccount.Address2,
                    address3 = companyBankAccount.Address3,
                    bankIdBIC = companyBankAccount.SWIFTCODE,
                    debitCurrency = "SAR",
                    transferAmount = invoice.TotalCompanyAmount?.ToString(),
                    transferCurrency = "SAR"
                };
                var beneficiaries = new List<BeneficiaryModel>();
                beneficiaries.Add(beneficiaryBcare);
                beneficiaries.Add(beneficiary);

                var payOut = new PayoutModel()
                {
                    transferOption = "7", // ‘0’ --> real time; 7 batch
                    period = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                    configId = Utilities.GetAppSetting("HyperpaykeyConfigId"), 
                    beneficiary = beneficiaries
                };

                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonSerializer.Serialize(payOut);

                data = $"entityId={_config.Hyperpay.ApplePayEntityId}" +
                    $"&amount={Math.Round(requestInfo.HyperpayRequestAmount.Value,2)}" +
                    $"&currency={_config.Hyperpay.Currency }" +
                    $"&paymentType={_config.Hyperpay.PaymentType}" +
                    $"&notificationUrl={_config.Hyperpay.NotificationURL}" +
                    $"&customParameters[payout]={json}";

                if (!string.IsNullOrEmpty(_config.Hyperpay.TestMode))
                    data += $"&testMode={_config.Hyperpay.TestMode}&merchantTransactionId={requestInfo.ReferenceId}";
                else
                    data += $"&merchantTransactionId={requestInfo.ReferenceId}";

                data += "&customer.email=" + requestInfo.Email;
                data += "&paymentBrand=APPLEPAY";
                data += "&applePay.source=web";
                data += "&shopperResultUrl=https://bcare.com.sa";
                data += "&applePay.paymentToken="+ HttpUtility.UrlEncode(JsonConvert.SerializeObject(paymentToken.PaymentData));

                string url = _config.Hyperpay.ApplePayPaymentUrl;
                string exp = string.Empty;
                log.ServiceRequest = data;
                responseString = SendPaymentRequest(url, data, _config.Hyperpay.AccessToken, out exp);
                log.ServiceResponse = responseString;
                if (!string.IsNullOrEmpty(exp))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = exp;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponse = responseString;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(responseString))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.ResponseIsNull;
                    output.ErrorDescription = "responseString is null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var result = JsonConvert.DeserializeObject<ApplePayPaymnetResponseModel>(responseString);
                output.PaymnetResponseModel = result;
                output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        public ApplePayRequestInfo GetRequestInfoForApplePay(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetRequestInfoForApplePay";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var requestInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<ApplePayRequestInfo>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return requestInfo;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public bool UpdateCheckoutWithPaymentStatus(string referenceId,int policyStatusId,int insuranceCompanyId,string insuranceCompanyName,bool paymentSuccessfull, string channel, int paymentMethodId)
        {
            string exception = string.Empty;
            try
            {
                if (paymentSuccessfull) // Payment succeeded 
                {
                    if (policyStatusId != (int)EPolicyStatus.Available)
                    {
                        _checkoutsService.UpdateCheckoutWithPaymentStatus(referenceId, (int)EPolicyStatus.PaymentSuccess, paymentMethodId, out exception);
                    }
                    _policyProcessingService.InsertPolicyProcessingQueue(referenceId, insuranceCompanyId, insuranceCompanyName, channel);
                }
                else
                {
                    _checkoutsService.UpdateCheckoutWithPaymentStatus(referenceId, (int)EPolicyStatus.PaymentFailure, paymentMethodId, out exception);
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                if (!string.IsNullOrEmpty(exception))
                {
                    CheckoutRequestLog log = new CheckoutRequestLog();
                    log.ReferenceId = referenceId;
                    log.PaymentMethod = Enum.GetName(typeof(PaymentMethodCode), paymentMethodId);
                    log.CompanyId = insuranceCompanyId;
                    log.CompanyName = insuranceCompanyName;
                    log.Channel = channel;
                    log.ServerIP = ServicesUtilities.GetServerIP();
                    log.MethodName = "UpdateCheckoutWithPaymentStatus";
                    log.CreatedDate = DateTime.Now;
                    log.ServiceRequest = $"paymentSuccessfull: {paymentSuccessfull}, paymentMethodId: {paymentMethodId}";
                    log.ErrorCode = 5;
                    log.ErrorDescription = exception;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                    var checkoutdetatils = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == referenceId && c.IsCancelled == false);
                    if (checkoutdetatils != null)
                    {
                        if (paymentSuccessfull)
                        {
                            checkoutdetatils.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                            _policyProcessingService.InsertPolicyProcessingQueue(referenceId, insuranceCompanyId, insuranceCompanyName, channel);
                        }
                        else
                        {
                            checkoutdetatils.PolicyStatusId = (int)EPolicyStatus.PaymentFailure;
                        }
                        if (policyStatusId != (int)EPolicyStatus.Available)
                        {
                            _checkoutDetailRepository.Update(checkoutdetatils);
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        private string SendPaymentRequest(string url, string request, string AccessToken, out string exception)
        {
            exception = string.Empty;
            string responseValue = string.Empty;
            var statusCode = new HttpStatusCode();
            try
            {
                string response = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers["Authorization"] = $"Bearer {AccessToken}";
                try
                {
                    Utilities.InitiateSSLTrust();
                }
                catch
                {

                }
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                statusCode = httpResponse.StatusCode;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }
                return response;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        var s = new JavaScriptSerializer();
                        Dictionary<string, dynamic> responseData;
                        using (var reader = new StreamReader(data))
                        {
                            responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                            reader.Close();
                        }
                        responseValue = s.Serialize(responseData);
                    }
                }
                return responseValue;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return null;
            }
        }
        public HyperpayRequest GetHyperpayRequestByMerchantTransactionId(string merchantTransactionId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetHyperpayRequestByMerchantTransactionId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter merchantTransactionIdParam = new SqlParameter() { ParameterName = "@merchantTransactionId", Value = merchantTransactionId };
                command.Parameters.Add(merchantTransactionIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var hyperpayRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequest>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return hyperpayRequest;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public HyperpayRequestInfo GetHyperpayRequestInfo(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetHyperpayRequestInfo";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter merchantTransactionIdParam = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(merchantTransactionIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var hyperpayRequestInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequestInfo>(reader).FirstOrDefault();
                if (hyperpayRequestInfo != null)
                {
                    reader.NextResult();
                    var orderItemBenefit = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefit>(reader).ToList();
                    if(orderItemBenefit != null)
                    {
                        hyperpayRequestInfo.OrderItemBenefit = orderItemBenefit;
                    }
                    reader.NextResult();
                   var priceDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList();
                    if (priceDetail != null)
                    {
                        hyperpayRequestInfo.PriceDetail = priceDetail;
                    }
                }
                idbContext.DatabaseInstance.Connection.Close();
                return hyperpayRequestInfo;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        #endregion
    }
}

    


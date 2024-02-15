using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Data.Entity;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tameenk.Core.Domain.Enums.Payments;
using Microsoft.AspNetCore.Http;

namespace Tameenk.Services.Implementation.Payments
{
    public class SadadPaymentService : ISadadPaymentService
    {
        private readonly TameenkConfig _config;
        private readonly HttpContext _HttpContextBase;
        private readonly ILogger _logger;
        private readonly IRepository<SadadRequest> _sadadRequestRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;

        public SadadPaymentService(TameenkConfig config, HttpContext HttpContextBase, ILogger logger,
            IRepository<SadadRequest> sadadRequestRepository, IRepository<CheckoutDetail> checkoutDetailRepository,
            IPolicyProcessingService policyProcessingService, IShoppingCartService shoppingCartService)
        {
            _config = config;
            _HttpContextBase = HttpContextBase;
            _logger = logger;
            _sadadRequestRepository = sadadRequestRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _policyProcessingService = policyProcessingService;
            _shoppingCartService = shoppingCartService;
        }
        public SadadResponse ExecuteSadadPayment(SadadRequest sadadRequest, bool isActive, int companyId, string companyName, string referenceId,string externalId)
        {
            var output = SubmitSadadRequest(sadadRequest, isActive, companyId, companyName, referenceId,externalId);
            if (output.ErrorCode != SadadOutput.ErrorCodes.Success)
            {
                SadadResponse sadadResponse = new SadadResponse
                {
                    Status = "Failed",
                    ErrorCode = 5,
                    Description = output.ErrorDescription,
                    SadadRequestId = sadadRequest.Id
                };
                return sadadResponse;
            }
            output.Output.ErrorCode = 0;
            return output.Output;
        }




        private SadadOutput SubmitSadadRequest(SadadRequest sadadRequest, bool isActive, int companyId, string companyName, string referenceId,string externalId)
        {
            SadadOutput output = new SadadOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ReferenceId = referenceId;
            log.CompanyID = companyId;
            log.CompanyName = companyName;
            log.Channel = "Portal";
            log.ServiceURL = _config.Sadad.Url;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ExternalId = externalId;
            if (isActive)
            {
                log.Method = "SadadActive";
                _sadadRequestRepository.Insert(sadadRequest);
            }
            else
            {
                log.Method = "SadadDeactive";
            }
            try
            {
                byte[] bytesToWrite = GenerateRequestXML(sadadRequest, isActive);
                log.ServiceRequest = Encoding.UTF8.GetString(bytesToWrite);
                if (bytesToWrite == null)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.GenerateRequestXMLFailed;
                    output.ErrorDescription = "Failed to Generate Request XML";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var request = HttpWebRequest.Create(_config.Sadad.Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentLength = bytesToWrite.Length;
                request.ContentType = "text/xml; charset=utf-8";

                if (isActive)
                {
                    //By Niaz Upgrade-Assistant todo
                    //request.ClientCertificates.Add(
                    //    new X509Certificate2(
                    //        _HttpContextBase.Server.MapPath(_config.Sadad.KeyRelativePath),
                    //        "b2b123",
                    //        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet));
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
                }

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                requestStream.Close();
                DateTime dtBeforeCalling = DateTime.Now;
                HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
                Stream dataStream = httpResponse.GetResponseStream();
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                XmlSerializer serializer = new XmlSerializer(typeof(SadadResponse));
                SadadResponse response = null;
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    XDocument xDoc = XDocument.Load(reader);
                    XNamespace ab = _config.Sadad.SoapEnvelopNameSpace;
                    var unwrappedResponse = xDoc.Descendants(ab + "Body").First().FirstNode;
                    response = serializer.Deserialize(unwrappedResponse.CreateReader()) as SadadResponse;
                }

                if (response == null)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = Utilities.SerializeObject(response);


                if (response.ErrorCode == 1 && isActive)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = response.Description;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.ErrorCode == 1 && !isActive)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.Success;
                    output.ErrorDescription = response.Description;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Description.ToLower().Contains("BILL MODIFY NOT ALLOWED ON REJECTED BILL".ToLower()) && !isActive)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.Success;
                    output.ErrorDescription = response.Description;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Description.ToLower().Contains("NO HISTORY FOUND, ONLY ACTIVE STATUS ALLOWED".ToLower()) && !isActive)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.Success;
                    output.ErrorDescription = response.Description;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.ErrorCode == 99995 && !isActive)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.Success;
                    output.ErrorDescription = response.Description;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.ErrorCode == 2)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Daily Limit Exceeded";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.ErrorCode == 3)
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Internal Timeout";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Status.ToLower().Trim() != "success")
                {
                    output.ErrorCode = SadadOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service response content return no success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = response;
                output.ErrorCode = SadadOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                if (isActive)
                {
                    sadadRequest.SadadResponses.Add(response);
                    _sadadRequestRepository.Update(sadadRequest);
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SadadOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                SadadResponse sadadResponse = new SadadResponse
                {
                    Status = "Failed",
                    ErrorCode = 5,
                    Description = ex.ToString(),
                    SadadRequestId = sadadRequest.Id
                };
                sadadRequest.SadadResponses.Add(sadadResponse);
                _sadadRequestRepository.Update(sadadRequest);
                return output;

            }
        }





        /// <summary>
        /// Update checkout payment status
        /// </summary>
        /// <param name="checkoutDetailsId">Reference ID</param>
        public CheckoutDetail UpdateCheckoutPaymentStatus(string checkoutDetailsId)
        {
            var checkoutDetails = _checkoutDetailRepository.Table
                .Include(x=>x.OrderItems.Select(y=>y.Product.InsuranceCompany))
                .Include(x => x.OrderItems.Select(y => y.Product.QuotationResponse.ProductType))
                .FirstOrDefault(c => c.ReferenceId == checkoutDetailsId);
            if (checkoutDetails != null)
            {
                if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Available)
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
             
                checkoutDetails.ModifiedDate = DateTime.Now;
                checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.Sadad;
                string companyName = string.Empty;
                if(checkoutDetails.InsuranceCompany!=null&&!string.IsNullOrEmpty(checkoutDetails.InsuranceCompany.Key))
                {
                    companyName = checkoutDetails.InsuranceCompany.Key;
                }
                else
                {
                    companyName = checkoutDetails.InsuranceCompanyName;
                }
                _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, companyName, "Portal");
                _checkoutDetailRepository.Update(checkoutDetails);
                _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId, checkoutDetails.ReferenceId);
            }
            return checkoutDetails;
        }

        private byte[] GenerateRequestXML(SadadRequest sadadRequest, bool isActive = true)
        {
            StringBuilder xmlsb = new StringBuilder();
            xmlsb.AppendLine("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:bil='http://sadadonlinepayment.com/billreq' >");
            xmlsb.AppendLine("<soapenv:Header/>");
            xmlsb.AppendLine("<soapenv:Body>");
            xmlsb.AppendLine("<SadadPmtReq>");
            xmlsb.AppendLine("<BillingAcct>");
            xmlsb.AppendLine($"<BillerId>{sadadRequest.BillerId}</BillerId>");
            xmlsb.AppendLine($"<ExactPmtRq>{sadadRequest.ExactFlag}</ExactPmtRq>");
            xmlsb.AppendLine($"<CustomerAccount>{sadadRequest.CustomerAccountNumber}</CustomerAccount>");
            if(string.IsNullOrEmpty(sadadRequest.CustomerAccountName.Replace("-", " ")))
                xmlsb.AppendLine($"<CustomerName>InsuredName</CustomerName>");
            else
                xmlsb.AppendLine($"<CustomerName>{sadadRequest.CustomerAccountName.Replace("-", " ")}</CustomerName>");

           if(isActive)
                xmlsb.AppendLine($"<Status>{_config.Sadad.BillslAccountStatus}</Status>");
            else
                xmlsb.AppendLine($"<Status>INACTIVE</Status>");

            xmlsb.AppendLine($"<AmountDue>{sadadRequest.BillAmount}</AmountDue>");
            xmlsb.AppendLine($"<BillOpenDate>{sadadRequest.BillOpenDate.ToString("yyyy-MM-dd")}</BillOpenDate>");
            xmlsb.AppendLine($"<BillDueDate>{sadadRequest.BillDueDate.ToString("yyyy-MM-dd")}</BillDueDate>");
            xmlsb.AppendLine($"<BillExpiryDate>{sadadRequest.BillExpiryDate.ToString("yyyy-MM-dd")}</BillExpiryDate>");
            xmlsb.AppendLine($"<BillCloseDate>{sadadRequest.BillCloseDate.ToString("yyyy-MM-dd")}</BillCloseDate>");
            //param += "<MaxAdvanceAmount>" + MaxAdvanceAmount + "</MaxAdvanceAmount>";
            //param += "<MinAdvanceAmount>" + MinAdvanceAmount + "</MinAdvanceAmount>";
            //param += "<MinPartialAmount>" + MinPartialAmount + "</MinPartialAmount>";
            xmlsb.AppendLine("</BillingAcct>");
            xmlsb.AppendLine("</SadadPmtReq>");
            xmlsb.AppendLine("</soapenv:Body>");
            xmlsb.AppendLine("</soapenv:Envelope>");

            string xmlString = xmlsb.ToString();
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(xmlString);
        }

        private bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

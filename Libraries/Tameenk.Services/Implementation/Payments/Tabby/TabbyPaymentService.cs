using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Tabby;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyPaymentService: ITabbyPaymentService
    {
       
        private readonly IRepository<TabbyRequest> _tabbyRequestRepository;
        private readonly IRepository<TabbyResponse> _tabbyResponseRepository;
        private readonly IRepository<TabbyNotification> _tabbyNotificationRepository;
        private readonly IRepository<TabbyNotificationDetails> _tabbyNotificationDetailsRepository;
        private readonly IRepository<CheckoutDetail> checkoutDetailsRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
       // private readonly IPaymentNotificationSmsSender _smsSender;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly IRepository<TabbyCaptureRequest> _tabbyCaptureRequestRepository;
        private readonly IRepository<TabbyCaptureResponse> _tabbyCaptureResponseRepository;
        private readonly IRepository<TabbyCaptureResponseDetails> _tabbyCaptureResponseDetailsRepository;
        private readonly IRepository<TabbyWebHook> _tabbyWebHookRepository;
        private readonly IRepository<TabbyWebHookDetails> _tabbyWebHookDetailsRepository;
        public TabbyPaymentService(IRepository<TabbyRequest> tabbyRequestRepository,
            IRepository<TabbyResponse> tabbyResponseRepository,
            IRepository<TabbyNotification> tabbyNotificationRepository,
             IRepository<TabbyNotificationDetails> tabbyNotificationDetailsRepository,
             IRepository<CheckoutDetail> _checkoutDetailsRepository,
             IRepository<Invoice> invoiceRepository,
             IPolicyProcessingService policyProcessingService,
             IShoppingCartService shoppingCartService,
             IOrderService orderService,
              IRepository<TabbyCaptureRequest> tabbyCaptureRequestRepository,
              IRepository<TabbyCaptureResponse> tabbyCaptureResponseRepository,
              IRepository<TabbyCaptureResponseDetails> tabbyCaptureResponseDetailsRepository,
               IRepository<TabbyWebHook> tabbyWebHookRepository,
               IRepository<TabbyWebHookDetails> tabbyWebHookDetailsRepository)
        {
            _tabbyRequestRepository = tabbyRequestRepository ?? throw new ArgumentNullException(nameof(IRepository<TabbyRequest>));
            _tabbyResponseRepository = tabbyResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<TabbyResponse>));
            _tabbyNotificationRepository = tabbyNotificationRepository ?? throw new ArgumentNullException(nameof(IRepository<TabbyNotification>));
            _tabbyNotificationDetailsRepository = tabbyNotificationDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<TabbyNotificationDetails>));
             checkoutDetailsRepository = _checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _policyProcessingService = policyProcessingService ?? throw new ArgumentNullException(nameof(IPolicyProcessingService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(IShoppingCartService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(IOrderService));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(IRepository<Invoice>));
            _tabbyCaptureResponseRepository = tabbyCaptureResponseRepository;
            _tabbyCaptureRequestRepository = tabbyCaptureRequestRepository;
            _tabbyCaptureResponseDetailsRepository = tabbyCaptureResponseDetailsRepository;
            _tabbyWebHookRepository = tabbyWebHookRepository;
            _tabbyWebHookDetailsRepository = tabbyWebHookDetailsRepository;
        }

      

        public TabbyResponseStatus CheckResponseObject(dynamic response)
        {
            TabbyResponseStatus tabbyResponseStatus = new TabbyResponseStatus();
            if (response == null)
            {
                return tabbyResponseStatus;
            }
            if (response.status != null)
            {
                tabbyResponseStatus.status = response.status.ToString().ToLower();
                tabbyResponseStatus.IsErrors = true;
            }
            if (response.errorType != null)
            {
                tabbyResponseStatus.errorType = response.errorType.ToString();
                tabbyResponseStatus.IsErrors = true;
            }
            if (response.error != null)
            {
                tabbyResponseStatus.error = response.error.ToString();
                tabbyResponseStatus.IsErrors = true;
            }
            JArray errors = (JArray)response["errors"];
            if(errors != null && errors.Count > 0)
            {
                tabbyResponseStatus.errors = new List<TabbyErrorStatus>();
                foreach (var item in errors)
                {
                    tabbyResponseStatus.errors.Add(new TabbyErrorStatus()
                    {
                        field = item.Value<string>("field"),
                        code = item.Value<string>("code"),
                        message = item.Value<string>("message")
                    });
                }
                tabbyResponseStatus.IsErrors = true;
            }
            return tabbyResponseStatus;
        }
        public TabbyOutput SubmitTabbyRequest(TabbyRequestModel tabbyrequest, TabbyConfig tabbysetting,
          CheckoutDetail checkoutDetail, string channel, string externalId, string nin)
        {
            TabbyOutput output = new TabbyOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "SubmitTabbyRequest";
            log.ReferenceId = checkoutDetail.ReferenceId;
            Guid userId = Guid.Empty;
            Guid.TryParse(checkoutDetail.UserId, out userId);
            log.UserID = userId;
            log.VehicleId = checkoutDetail.VehicleId.ToString();
            log.DriverNin = nin;
            log.CompanyID = checkoutDetail.InsuranceCompanyId;
            log.CompanyName = checkoutDetail.InsuranceCompanyName;
            log.ExternalId = externalId;
            DateTime date = DateTime.Now;
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(tabbyrequest);
                log.ServiceRequest = jsonRequest;
                log.ServiceURL = tabbysetting.CheckoutUrl;
                HttpStatusCode httpStatusCode = new HttpStatusCode();
                var response = Utilities.SendRequestJson(tabbysetting.CheckoutUrl, jsonRequest, out httpStatusCode, tabbysetting.Pk);

                log.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "HttpStatusCode is Unauthorized ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is BadRequest";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = "Service Response is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(response);

                if (responseAfterDeserialize == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = "Deserialize Response is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                TabbyResponseStatus ResponseTabbyErrors = CheckResponseObject(responseAfterDeserialize);
                if (ResponseTabbyErrors.IsErrors && ResponseTabbyErrors.status == "error")
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = ResponseTabbyErrors.errorType + " : ";
                    if (ResponseTabbyErrors.errors != null && ResponseTabbyErrors.errors.Count() > 0)
                    {
                        for (int i = 0; i < ResponseTabbyErrors.errors.Count(); i++)
                        {
                            output.ErrorDescription += "[" + i + "] " + JsonConvert.SerializeObject(ResponseTabbyErrors.errors[i]);
                        }

                    }
                    if (!string.IsNullOrEmpty(ResponseTabbyErrors.error))
                    {
                        output.ErrorDescription += "Error : " + ResponseTabbyErrors.error;
                    }
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                TabbyResponseModel responseObject = JsonConvert.DeserializeObject<TabbyResponseModel>(responseAfterDeserialize.ToString());
                output.TabbyResponseHandler = new TabbyResponseHandler
                {
                    ResponseBody = responseObject,
                    Status = ResponseTabbyErrors
                };
                output.ErrorCode = TabbyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }


        public TabbyOutput SubmitTabbyNotification(string PaymentId,TabbyResponse tabbyResponse,TabbyConfig tabbyConfig)
        {
            TabbyOutput output = new TabbyOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "SubmitTabbyNotification";
            log.CreatedDate = DateTime.Now;
            log.CompanyID = tabbyResponse.TabbyRequest.CompanyId;
            log.CompanyName = tabbyResponse.TabbyRequest.CompanyName;
            log.ReferenceId = tabbyResponse.ReferenceId;
            log.Channel = tabbyResponse.Channel;
            Guid userId = Guid.Empty;
            Guid.TryParse(tabbyResponse.UserId, out userId);
            log.UserID = userId;
            var date = DateTime.Now;
            TabbyNotification tabbyNotificationEntity = new TabbyNotification();
            var tabbyNotification = new TabbyResponse();
            try
            {
               
                string paymentNotificationURL = tabbyConfig.PaymentUrl + PaymentId;
                log.ServiceURL = tabbyConfig.PaymentUrl;
                log.ServiceRequest = paymentNotificationURL;
                HttpStatusCode httpStatusCode = new HttpStatusCode();
                string response = Utilities.SendRequestJson(paymentNotificationURL, "", out httpStatusCode, tabbyConfig.SK, "GET");
                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ResponseIsNull;
                    output.ErrorDescription = "Service Response is Null";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.Unauthorized;
                    output.ErrorDescription = "HttpStatusCode is Unauthorized ";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InternalServerError;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.BadRequest;
                    output.ErrorDescription = " HttpStatusCode is BadRequest";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.BadRequest;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(response);
                output.TabbyNotificationResponseModel = JsonConvert.DeserializeObject<TabbyNotificationResponseModel>(responseAfterDeserialize.ToString());

                output.ErrorCode = TabbyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public TabbyOutput SubmitTabbyCaptureRequest(TabbyConfig tabbyConfig, TabbyResponse tabbyResponseObject, string PaymentId)
        {
            var date = DateTime.Now;
            TabbyOutput output = new TabbyOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "SubmitTabbyCaptureRequest";
            log.ReferenceId = tabbyResponseObject.ReferenceId;
            log.CompanyID = tabbyResponseObject.TabbyRequest.CompanyId;
            log.CompanyName = tabbyResponseObject.TabbyRequest.CompanyName;

            log.Channel = tabbyResponseObject.Channel;
            Guid userIdCapture = Guid.Empty;
            Guid.TryParse(tabbyResponseObject.UserId, out userIdCapture);
            log.UserID = userIdCapture;
            string CapturURL = tabbyConfig.CaptureUrl.Replace("@paymentid", PaymentId);
            log.ServiceURL = CapturURL;
            try
            {
                TabbyCaptureRequestViewModel tabbyCaptureRequestViewModel = new TabbyCaptureRequestViewModel()
                {
                    amount = tabbyResponseObject.TabbyRequest.Amount.ToString(),
                    discount_amount = "0.00",
                    shipping_amount = "0.00",
                    tax_amount = "0.00",
                    items = null
                };
                var captureRequest = JsonConvert.SerializeObject(tabbyCaptureRequestViewModel);
                log.ServiceRequest = captureRequest;
                HttpStatusCode httpStatusCode = new HttpStatusCode();
                //string response = Utilities.SendRequestJson(CapturURL, captureRequest, out httpStatusCode, tabbyConfig.SK, "POST");

                string response = string.Empty;                var httpWebRequest = (HttpWebRequest)WebRequest.Create(CapturURL);                httpWebRequest.ContentType = "application/json";                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + tabbyConfig.SK);                Utilities.InitiateSSLTrust();                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(captureRequest);
                        streamWriter.Flush();
                    }                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();                httpStatusCode = httpResponse.StatusCode;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))                    {                        response = streamReader.ReadToEnd();                    }                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.ResponseIsNull;
                    output.ErrorDescription = "Service Response is Null and status code is "+ httpStatusCode.ToString();
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.Unauthorized;
                    output.ErrorDescription = "HttpStatusCode is Unauthorized ";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InternalServerError;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.BadRequest;
                    output.ErrorDescription = " HttpStatusCode is BadRequest";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.BadRequest;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                dynamic responseCaptureAfterDeserialize = JsonConvert.DeserializeObject(response);
                output.TabbyCaptureResponseViewModel= JsonConvert.DeserializeObject<TabbyCaptureResponseViewModel>(responseCaptureAfterDeserialize.ToString());

                output.ErrorCode = TabbyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode =(int) output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode =(int) output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        public bool InsertIntoTabbyNotification(TabbyNotification tabbyNotification)
        {
            try
            { 
            _tabbyNotificationRepository.Insert(tabbyNotification);
            return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertIntoTabbyNotificationDetails(TabbyNotificationDetails tabbyNotificationDetails)
        {
            try
            { 
            _tabbyNotificationDetailsRepository.Insert(tabbyNotificationDetails);
            return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertIntoTabbyCaptureRequest(TabbyCaptureRequest tabbyCaptureRequest)
        {
            try
            {
                _tabbyCaptureRequestRepository.Insert(tabbyCaptureRequest);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertIntoTabbyCaptureResponse(TabbyCaptureResponse tabbyCaptureResponse)
        {
            try
            {
                _tabbyCaptureResponseRepository.Insert(tabbyCaptureResponse);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertIntoTabbyCaptureResponseDetails(TabbyCaptureResponseDetails tabbyCaptureResponseDetails)
        {
            try
            {
                _tabbyCaptureResponseDetailsRepository.Insert(tabbyCaptureResponseDetails);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertIntoTabbyWebhook(TabbyWebHook tabbyWebHook)
        {
            try
            {
                _tabbyWebHookRepository.Insert(tabbyWebHook);
                return true;
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\InsertIntoTabbyWebhook.txt", JsonConvert.SerializeObject(ex));
                return false;
            }
        }
        public bool InsertIntoTabbyWebhookDetails(TabbyWebHookDetails tabbyWebHookDetails)
        {
            try
            {
                _tabbyWebHookDetailsRepository.Insert(tabbyWebHookDetails);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public TabbyItems GetTabbyItemsByUserId(string userId)        {            List<BuyerHistoryData> BuyerHistoryData = null;            TabbyItems tabbyItems = new TabbyItems();            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetTabbyItems";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParameter = new SqlParameter() { ParameterName = "userId", Value = userId };
                command.Parameters.Add(userIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                BuyerHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BuyerHistoryData>(reader).ToList();
                reader.NextResult();
                UserData UserData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<UserData>(reader).FirstOrDefault();
                reader.NextResult();
                int TotalPolicies = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                tabbyItems.BuyerHistoryData = BuyerHistoryData;
                tabbyItems.Buyer = UserData;
                tabbyItems.TotalPolicies = TotalPolicies;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return tabbyItems;        }

    }
}

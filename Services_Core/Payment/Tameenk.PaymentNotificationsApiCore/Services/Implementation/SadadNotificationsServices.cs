using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Enums;
using Tameenk.PaymentNotificationsApi.Contexts;
using Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Repository;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class SadadNotificationsServices : ISadadNotificationsServices
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<SadadNotificationMessage> _sadadNotificationRepository;
        private readonly IPaymentNotificationSmsSender _smsSender;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IRepository<SadadRequest> _sadadRequest;

        public SadadNotificationsServices(IRepository<Invoice> invoiceRepository,
            IRepository<CheckoutDetail> checkoutDetailsRepository,
            IRepository<SadadNotificationMessage> sadadNotificationRepository,
            IPaymentNotificationSmsSender smsSender,
            ISadadPaymentService sadadPaymentService,
            ILogger logger, IOrderService orderService, IRepository<SadadRequest> sadadRequest)
        {
            _invoiceRepository = invoiceRepository;
            _checkoutDetailsRepository = checkoutDetailsRepository;
            _sadadNotificationRepository = sadadNotificationRepository;
            _smsSender = smsSender;
            _sadadPaymentService = sadadPaymentService;
            _logger = logger;
            _orderService = orderService;
            _sadadRequest = sadadRequest;
        }

        public ResponseMessage SaveAndProcessSadadNotification(NotificationMessage message)
        {
            WafierAndWafferDbContext db = null;
            var saveDB = new SadadApiMessage();
            if (message != null && message.Header != null && message.Body != null)
            {
                var header = new NotificationMessageHeader();
                header.Sender = message.Header.Receiver;
                header.Receiver = message.Header.Sender;
                header.MessageType = RepositoryConstants.SadadResponseMessageType;
                string timestmp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                header.TimeStamp = DateTime.ParseExact(timestmp, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                if (message.Header.Receiver != "" && message.Header.Sender != "" && message.Header.MessageType != "" &&
                    message.Body.AccountNo != "" && message.Body.CustomerRefNo != "" && message.Body.TransType != "" && message.Body.Description != "" &&
                    message.Body.CustomerRefNo != "" && message.Body.TransType != "" && message.Body.Description != "")
                {
                    saveDB.HeadersSender = message.Header.Sender.ToString();
                    saveDB.HeadersReceiver = message.Header.Receiver;
                    saveDB.HeadersMessageType = message.Header.MessageType;
                    saveDB.HeadersTimeStamp = message.Header.TimeStamp;
                    saveDB.BodysAccountNo = message.Body.AccountNo;
                    saveDB.BodysAmount = message.Body.Amount;
                    saveDB.BodysCustomerRefNo = message.Body.CustomerRefNo;
                    saveDB.BodysTransType = message.Body.TransType;
                    saveDB.BodysDescription = message.Body.Description;
                    saveDB.ErrorMessage = string.Empty;
                    saveDB.CreatedDate = DateTime.Now;

                    var sadadCustomerRefNo = new SadadCustomerReference(message.Body.CustomerRefNo);
                    if (sadadCustomerRefNo.ApplicationId == RepositoryConstants.TameenkApplicationId)
                    {
                        return SaveAndProcessSadadNotificationForTameenk(message, sadadCustomerRefNo);
                    }
                    else
                    {
                        var aplicationName = Applications.GetName(sadadCustomerRefNo.ApplicationId);
                        db = new WafierAndWafferDbContext(aplicationName);

                        var payRefCode = db.Payments.Where(c => c.PaymentReferenceCode == sadadCustomerRefNo.DynamicPart).SingleOrDefault();

                        if (payRefCode != null && !payRefCode.PaymentStatus)
                        {
                            var paymentresponse = new PaymentResponse() { CreatedDate = DateTime.Now, UserId = payRefCode.PaymentUserID, PaymentMethodsID = payRefCode.PaymentMethodsID, PaymentsID = payRefCode.ID };
                            db.PaymentResponse.Add(paymentresponse);
                            var paymentesponsedetails = new List<PaymentResponseDetails>();
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "Sender", Value = message.Header.Sender.ToString() });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "Receiver", Value = message.Header.Receiver });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "MessageType", Value = message.Header.MessageType });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "TimeStamp", Value = message.Header.TimeStamp.ToString() });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "AccountNo", Value = message.Body.AccountNo });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "Amount", Value = message.Body.Amount });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "CustomerRefNo", Value = message.Body.CustomerRefNo });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "TransType", Value = message.Body.TransType });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "Description", Value = message.Body.Description });
                            paymentesponsedetails.Add(new PaymentResponseDetails() { Name = "Status", Value = RepositoryConstants.WafierAndWafferSadadApiOkMessageResponsesStatus });
                            paymentresponse.PaymentResponseDetails = paymentesponsedetails;

                            payRefCode.PaymentStatus = true;
                            payRefCode.PaidDate = DateTime.Now;
                            var MessageresponseOK = new ResponseMessage() { Header = header, Status = RepositoryConstants.WafierAndWafferSadadApiOkMessageResponsesStatus };
                            saveDB.MessageResponsesStatus = MessageresponseOK.Status;
                            db.MessageWebsrevices.Add(saveDB);

                            var paymentrequest = new PaymentRequests() { CreatedDate = DateTime.Now, UserId = payRefCode.PaymentUserID, PaymentMethodsID = payRefCode.PaymentMethodsID, PaymentsID = payRefCode.ID };
                            db.PaymentRequests.Add(paymentrequest);
                            var paymentrequestdetails = new List<PaymentRequestDetails>();
                            paymentrequestdetails.Add(new PaymentRequestDetails() { Name = "Sender", Value = header.Sender });
                            paymentrequestdetails.Add(new PaymentRequestDetails() { Name = "Receiver", Value = header.Receiver });
                            paymentrequestdetails.Add(new PaymentRequestDetails() { Name = "MessageType", Value = header.MessageType });
                            paymentrequestdetails.Add(new PaymentRequestDetails() { Name = "TimeStamp", Value = header.TimeStamp.ToString() });
                            paymentrequestdetails.Add(new PaymentRequestDetails() { Name = "Status", Value = MessageresponseOK.Status });
                            paymentrequest.PaymentRequestDetails = paymentrequestdetails;

                            db.SaveChanges();

                            try
                            {
                                var user = db.Users.Where(x => x.Id.ToString() == payRefCode.PaymentUserID).FirstOrDefault();
                                var response = new HttpClient().GetAsync("https://www.bcare.com.sa/" + aplicationName + "/Payments/SendNotificationToUser?phoneNumber=" + user.PhoneNumber + "&email=" + user.Email).Result;
                            }
                            catch { }

                            return MessageresponseOK;
                        }
                        else
                        {
                            var MessageresponseERRR = new ResponseMessage() { Header = header, Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus };
                            saveDB.ErrorMessage = "check  payment refrance code is not correct or null or allready is payment";
                            saveDB.MessageResponsesStatus = MessageresponseERRR.Status;
                            db.MessageWebsrevices.Add(saveDB);
                            db.SaveChanges();

                            return MessageresponseERRR;
                        }
                    }
                }
                else
                {
                    var sadadMessage = saveSadadNotificationMessageToTameenk(message);
                    var responseMessage = new ResponseMessage() { Header = null, Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus };
                    saveSadadResponseToTameenk(responseMessage, sadadMessage.ID);
                    return responseMessage;
                }
            }
            else
            {
                var sadadMessage = saveSadadNotificationMessageToTameenk(message);
                var responseMessage = new ResponseMessage() { Header = null, Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus };
                saveSadadResponseToTameenk(responseMessage, sadadMessage.ID);
                return responseMessage;
            }
        }

        private ResponseMessage SaveAndProcessSadadNotificationForTameenk(NotificationMessage message, SadadCustomerReference sadadCustomerReference)
        {
            _logger.Log($"SadadNotificationsServices -> SaveAndProcessSadadNotificationForTameenk >>> Start <<< (Message : {JsonConvert.SerializeObject(message)}, Sadad customer reference : {JsonConvert.SerializeObject(sadadCustomerReference)})");
            string timestmp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            var responseMessage = new ResponseMessage();
            responseMessage.Header = new NotificationMessageHeader()
            {
                Receiver = message.Header.Sender,
                Sender = message.Header.Receiver,
                MessageType = RepositoryConstants.SadadResponseMessageType,
                TimeStamp = DateTime.ParseExact(timestmp, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
            };

            if (message != null && message.Body != null)
            {
                decimal amount = 0;
                decimal.TryParse(message.Body.Amount, out amount);
                var tameenkSadadNotificationMessage = _sadadNotificationRepository.Table.
                 Where(s => s.BodysCustomerRefNo == message.Body.CustomerRefNo && s.BodysAmount == amount).FirstOrDefault();
                if (tameenkSadadNotificationMessage != null)
                {
                    responseMessage.Status = RepositoryConstants.WafierAndWafferSadadApiOkMessageResponsesStatus;
                    return responseMessage;
                }
            }

            var sadadMessage = saveSadadNotificationMessageToTameenk(message);
            //check if there is sadadNotificationMessage in Tameenk
            var invoice = _invoiceRepository.Table.FirstOrDefault(i => i.InvoiceNo == sadadCustomerReference.TameenkInvoiceNo);
            string referenceId = string.Empty;
            if (invoice == null || string.IsNullOrEmpty(invoice.ReferenceId))
            {
                var sadadRequest=_sadadRequest.TableNoTracking.Where(a => a.CustomerAccountNumber == "03" + sadadCustomerReference.TameenkInvoiceNo).FirstOrDefault();
                if(sadadRequest!=null)
                {
                    var checkout = _checkoutDetailsRepository.TableNoTracking.Where(a => a.ReferenceId == sadadRequest.ReferenceId).FirstOrDefault();
                    if(checkout!=null)
                    {
                        invoice = _invoiceRepository.Table.FirstOrDefault(i => i.ReferenceId == checkout.ReferenceId);
                        if(invoice!=null)
                        {
                            string exception = string.Empty;
                            _orderService.DeleteInvoiceByRefrenceId(checkout.ReferenceId, checkout.UserId, out exception);
                        }
                    }
                    _orderService.CreateInvoice(sadadRequest.ReferenceId, checkout.SelectedInsuranceTypeCode.Value, sadadRequest.CompanyId, sadadCustomerReference.TameenkInvoiceNo);
                    referenceId = sadadRequest.ReferenceId;
                }
               // responseMessage.Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus;
            }
            else
            {
                referenceId = invoice.ReferenceId;
            }
            //if (invoice == null || string.IsNullOrEmpty(invoice.ReferenceId))
            //{
            //    _logger.Log($"SadadNotificationsServices -> SaveAndProcessSadadNotificationForTameenk >>> Unable to find invoice <<< (Invoic No : {sadadCustomerReference.TameenkInvoiceNo}, Message : {JsonConvert.SerializeObject(message)}, Sadad customer reference : {JsonConvert.SerializeObject(sadadCustomerReference)})", LogLevel.Warning);
            //    responseMessage.Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus;
            //}
            //else
            {
                // update checkout details status to be PaymentSuccess
                var checkoutDetails = _sadadPaymentService.UpdateCheckoutPaymentStatus(referenceId);
                responseMessage.Status = RepositoryConstants.WafierAndWafferSadadApiOkMessageResponsesStatus;

                LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                        LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;

                _smsSender.SendSms(checkoutDetails, sadadMessage.BodysAmount.HasValue ? sadadMessage.BodysAmount.Value : 0, culture);
            }
            saveSadadResponseToTameenk(responseMessage, sadadMessage.ID);
            return responseMessage;
        }

        private SadadNotificationMessage saveSadadNotificationMessageToTameenk(NotificationMessage message)
        {

            var sadadNotificationMessage = new SadadNotificationMessage();
            sadadNotificationMessage.CreatedDate = DateTime.Now;

            if (message != null)
            {
                sadadNotificationMessage.SadadRequestJson = JsonConvert.SerializeObject(message);
                if (message.Header != null)
                {
                    sadadNotificationMessage.HeadersSender = message.Header.Sender;
                    sadadNotificationMessage.HeadersReceiver = message.Header.Receiver;
                    sadadNotificationMessage.HeadersMessageType = message.Header.MessageType;
                    sadadNotificationMessage.HeadersTimeStamp = message.Header.TimeStamp;
                }

                if (message.Body != null)
                {
                    sadadNotificationMessage.BodysAccountNo = message.Body.AccountNo;
                    decimal paidAmount = 0;
                    if (decimal.TryParse(message.Body.Amount, out paidAmount))
                    {
                        sadadNotificationMessage.BodysAmount = paidAmount;
                    }
                    sadadNotificationMessage.BodysCustomerRefNo = message.Body.CustomerRefNo;
                    sadadNotificationMessage.BodysTransType = message.Body.TransType;
                    sadadNotificationMessage.BodysDescription = message.Body.Description;
                }
            }
            _sadadNotificationRepository.Insert(sadadNotificationMessage);

            return sadadNotificationMessage;
        }

        private void saveSadadResponseToTameenk(ResponseMessage message, int notificationMessageId)
        {
            var notificationMsg = _sadadNotificationRepository.Table.First(s => s.ID == notificationMessageId);

            SadadNotificationResponse sadadNotificationResponse = new SadadNotificationResponse();
            sadadNotificationResponse.NotificationMessageId = notificationMessageId;
            sadadNotificationResponse.HeadersSender = message.Header.Sender;
            sadadNotificationResponse.HeadersReceiver = message.Header.Receiver;
            sadadNotificationResponse.HeadersMessageType = message.Header.MessageType;
            sadadNotificationResponse.HeadersTimeStamp = message.Header.TimeStamp;
            sadadNotificationResponse.Status = message.Status;
            notificationMsg.SadadNotificationResponses.Add(sadadNotificationResponse);
            _sadadNotificationRepository.Update(notificationMsg);
        }
    }
}
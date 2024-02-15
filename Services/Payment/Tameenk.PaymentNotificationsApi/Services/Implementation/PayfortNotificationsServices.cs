using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.PaymentNotificationApi;
using Tameenk.PaymentNotificationsApi.Contexts;
using Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Repository;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Helpers;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class PayfortNotificationsServices : IPayfortNotificationsServices
    {
        private readonly IRepository<PayfortPaymentRequest> _payfortPaymentRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IPaymentNotificationSmsSender _smsSender;

        public PayfortNotificationsServices(IRepository<PayfortPaymentRequest> payfortPaymentRepository,
            IRepository<CheckoutDetail> checkoutDetailsRepository, IPaymentNotificationSmsSender smsSender, IPayfortPaymentService payfortPaymentService)
        {
            _payfortPaymentRepository = payfortPaymentRepository ?? throw new ArgumentNullException(nameof(payfortPaymentRepository));
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(checkoutDetailsRepository));
            _payfortPaymentService = payfortPaymentService ?? throw new ArgumentNullException(nameof(payfortPaymentService));
            _smsSender = smsSender;
        }

        public void SaveAndProcessPayfortNotification(HttpRequest request)
        {
            string merchantReference = string.Empty;
            string messagecode = string.Empty;
            //string response_message_ = string.Empty;
            var paymentresponsedetails = new List<PaymentResponseDetails>();
            //var signatureResponse = request["signature"].ToString();
            //foreach (var item in request.Form.AllKeys)
            //{
            //    string name = item.ToString();
            //    string value = request[item.ToString()];
            //    if (name == "__RequestVerificationToken")
            //    {
            //        break;
            //    }
            //}
            //var paymentresult = paymentresponsedetails.Select(x => new { x.Name, x.Value }).OrderBy(x => x.Name).ToList();

            //string signatureValues = PayfortConfig.SHAResponsePharse;
            //foreach (var items in paymentresult)
            //{
            //    if (items.Name.ToString() == "signature")
            //        continue;
            //    signatureValues += items.Name + "=" + items.Value;
            //    if (items.Name == "merchant_reference")
            //        merchantReference = items.Value;
            //    if (items.Name == "response_code")
            //        messagecode = items.Value;
            //    if (items.Name == "response_message")
            //        response_message_ = items.Value;
            //}

            //signatureValues += PayfortConfig.SHAResponsePharse;
            //string signature = SHA.Sha256HashRequest(signatureValues);

            var formParams = request.Form.AllKeys.OrderBy(k => k).ToDictionary(k => k, v => request.Form[v]);
            paymentresponsedetails = formParams.Select(x=>new PaymentResponseDetails() { Name = x.Key, Value = x.Value }).ToList();

            merchantReference = formParams["merchant_reference"];
            messagecode = formParams["response_code"];

            if (_payfortPaymentService.ValidatedResponse(formParams))
            {
                string[] splits = merchantReference.Split('-');
                if (splits.Length > 2 && splits[0] == RepositoryConstants.TameenkApplicationId)
                {
                    SaveAndProcessPayfortNotificationForTameenk(merchantReference, request);
                }
                //case of policy update request payment
                else if (splits.Length > 2 && splits[0] == RepositoryConstants.PolicyUpdateRequestId)
                {
                    ProcessPolicyUpdPayment(merchantReference, request);
                }
                else
                {
                    var aplicationName = splits.Count() > 2 ? Applications.GetName(splits[0]) : Applications.GetName(Applications.Wafier);
                    //if(aplicationName == RepositoryConstants.TameenkApplicationId)
                    WafierAndWafferDbContext db = new WafierAndWafferDbContext(aplicationName);

                    PaymentRequests PaymentRequest = (from paymentreqDetails in db.PaymentRequestDetails
                                                      join paymentreq in db.PaymentRequests
                                                      on paymentreqDetails.PaymentRequestsID equals paymentreq.ID
                                                      where paymentreqDetails.Value == merchantReference
                                                      select paymentreq).SingleOrDefault();

                    var paymentresponse = new PaymentResponse() { PaymentResponseDetails = paymentresponsedetails, CreatedDate = DateTime.Now, PaymentMethodsID = PaymentRequest.PaymentMethodsID, PaymentsID = PaymentRequest.PaymentsID, SignatureMatch = true };
                    db.PaymentResponse.Add(paymentresponse);
                    if (db.SaveChanges() > 0)//but if statment if payment respons success go to success actionresult els go to failure with the error message details.xxx
                    {
                        if (messagecode == "14000")
                        {
                            int invoiceid = splits.Count() > 2 ? int.Parse(splits[1]) : int.Parse(splits[0]);
                            var invoice = db.Invoices.Find(invoiceid);
                            var payment = db.Payments.Find(invoice.PaymentsID);
                            payment.PaymentReferenceCode = merchantReference;
                            payment.PaymentStatus = true;
                            payment.PaidDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new Exception("error in handling your payment");
                    }
                }
            }
            else
            {
                throw new Exception("error validating payment response.");
            }
        }

        #region Private Methods
        private void SaveAndProcessPayfortNotificationForTameenk(string merchantReference, HttpRequest request)
        {
            var response = CreatePayfortPaymentResponse(request);
            _payfortPaymentService.ProcessPayment(merchantReference, response);
        }

        private void ProcessPolicyUpdPayment(string merchantReference, HttpRequest request)
        {
            var response = CreatePayfortPaymentResponse(request);
            _payfortPaymentService.ProcessPolicyUpdPayment(merchantReference, response);

        }

        /// <summary>
        /// Create PayfortPaymentResponse entity from payfort request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private PayfortPaymentResponse CreatePayfortPaymentResponse(HttpRequest request)
        {
            return new PayfortPaymentResponse()
            {
                ResponseCode = int.Parse(request.Form["response_code"]),
                ResponseMessage = request.Form["response_message"],
                Amount = string.IsNullOrEmpty(request.Form["payfortResponse.amount"]) ? null : (decimal?)(decimal.Parse(request.Form["amount"]) / 100),
                PaymentOption = request.Form["payment_option"],
                CardNumber = request.Form["card_number"],
                CardHolerName = request.Form["card_holder_name"],
                CardExpiryDate = request.Form["expiry_date"],
                CustomerIP = request.Form["customer_ip"],
                FortId = request.Form["fort_id"],
                status = string.IsNullOrEmpty(request.Form["status"]) ? null : (short?)short.Parse(request.Form["status"])
            };
        }

        #endregion  
    }
}


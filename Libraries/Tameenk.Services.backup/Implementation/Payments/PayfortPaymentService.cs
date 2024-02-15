using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Implementation.Payments
{
    public class PayfortPaymentService : IPayfortPaymentService
    {
        private readonly TameenkConfig _config;
        private readonly IRepository<PayfortPaymentRequest> _payfortPaymentRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPolicyService _policyService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public PayfortPaymentService(TameenkConfig config, IRepository<PayfortPaymentRequest> payfortPaymentRepository,
            IRepository<CheckoutDetail> checkoutDetailsRepository,
            IPolicyProcessingService policyProcessingService,
            IShoppingCartService shoppingCartService,
            IPolicyService policyService,
            INotificationService notificationService
            , ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(TameenkConfig));
            _payfortPaymentRepository = payfortPaymentRepository ?? throw new ArgumentNullException(nameof(IRepository<PayfortPaymentRequest>));
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _policyProcessingService = policyProcessingService ?? throw new ArgumentNullException(nameof(IPolicyProcessingService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(IShoppingCartService));
            _policyService = policyService ?? throw new ArgumentNullException(nameof(IPolicyService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(INotificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));

        }
        public void ProcessPayment(string merchantReference, PayfortPaymentResponse response)
        {
            var paymentRequest = _payfortPaymentRepository.Table
                .Include(e => e.CheckoutDetails).FirstOrDefault(p => p.ReferenceNumber == merchantReference);
            if (paymentRequest != null)
            {
                //There should be only one checkout related to this payment request
                var checkoutDetail = paymentRequest.CheckoutDetails.FirstOrDefault();
                if (checkoutDetail == null)
                {
                    _logger.Log($"PayfortPaymentService-> ProcessPayment couldnt find the checkout.");
                    throw new TameenkArgumentException("There is no checkoutDetail related to this payment request");
                }

                InsertPaymentResponse(response, paymentRequest);
                UpdateCheckoutPaymentStatus(checkoutDetail.ReferenceId, response);
                _logger.Log($"PayfortPaymentService-> ProcessPayment completed.");

            }
            else
            {
                _logger.Log($"PayfortPaymentService-> ProcessPayment there was not paymentRequest with this merchantReference: {merchantReference}");
            }

        }

        /// <summary>
        /// Reprocess payment by regenerating payfortPayment response and update checkout status
        /// </summary>
        /// <param name="merchantReference">Merchent Reference</param>
        public void ReProcessPayment(string merchantReference)
        {
            //using reference id get merchantReference from db
            var paymentRequest = _payfortPaymentRepository.Table
                .Include(e => e.CheckoutDetails)
                .FirstOrDefault(x => x.ReferenceNumber == merchantReference);
            if (paymentRequest == null)
                throw new ArgumentException($"There is no payment request with this merchant id = {merchantReference}", "referenceId");
            //each payment request should have only one CheckoutDetail
            var checkoutDetail = paymentRequest.CheckoutDetails.FirstOrDefault();
            if (checkoutDetail == null) throw new TameenkArgumentException("There is no checkoutDetail related to this payment request.");

            var response = new PayfortPaymentResponse();
            response.ResponseCode = 14000;
            response.ResponseMessage = "عملية ناجحة";
            response.Amount = paymentRequest.Amount * 100;
            response.PaymentOption = "VISA";
            response.CardNumber = "Manually created.";
            response.CardHolerName = "Manually created.";
            response.CardExpiryDate = "2020";
            response.CustomerIP = "Manually created.";
            response.FortId = "Manually created.";
            InsertPaymentResponse(response, paymentRequest);
            UpdateCheckoutPaymentStatus(checkoutDetail.ReferenceId, response);

        }


        /// <summary>
        /// Process policy update payment
        /// </summary>
        /// <param name="merchantReference">Merchant reference</param>
        /// <param name="response">Payfort response</param>
        public void ProcessPolicyUpdPayment(string merchantReference, PayfortPaymentResponse response)
        {
            //1-save response
            //1.1- get paymentRequest
            var paymentRequest = _payfortPaymentRepository.Table
               .Include(e => e.PolicyUpdateRequests)
               //.Include(e=>e.PolicyUpdateRequests.First().Policy)
               .FirstOrDefault(p => p.ReferenceNumber == merchantReference);
            if (paymentRequest == null) throw new TameenkArgumentException("There is no payment request for this merchant reference.");
            //1.2 save response
            InsertPaymentResponse(response, paymentRequest);

            //2-change policy upd request status from awaiting payment to payment done
            //2.1 get policy update request entity
            //there is only policy update request for Payment
            var policyUpdRequest = paymentRequest.PolicyUpdateRequests.FirstOrDefault();
            if (policyUpdRequest == null) throw new TameenkArgumentException("There is no policy update request related to payment request");
            //2.2 update policy upd request status
            _policyService.UpdatePolicyUpdRequestStatus(policyUpdRequest, PolicyUpdateRequestStatus.PaidAwaitingApproval);
            //3-insert notification for admin that policy upd payment is done
            _notificationService.AddPolicyUpdRequestNotificationForInsurance(policyUpdRequest.Policy.InsuranceCompanyID.Value, policyUpdRequest.PolicyId, NotificationType.PolicyUpdateRequestPaid);
        }
        public bool ValidatedResponse(Dictionary<string, string> payFortParams)
        {
            var excludedParameter = new List<string>() { "signature",
                "integration_type",
                "r"
            };

            var signatureValues = new StringBuilder(_config.PayFort.SHAResponsePharse);
            foreach (var item in payFortParams)
            {
                if (!excludedParameter.Contains(item.Key))
                {
                    signatureValues.Append($"{item.Key}={item.Value}");
                }
            }

            signatureValues.Append(_config.PayFort.SHAResponsePharse);
            string signature = Sha256HashRequest(signatureValues.ToString());

            if (!payFortParams.ContainsKey("signature"))
                return false;
            return signature == payFortParams["signature"];
        }

        #region Private Methods

        /// <summary>
        /// Link payment response to payment request and save it
        /// </summary>
        /// <param name="response">payment response</param>
        /// <param name="payfortPaymentRequest">payment request</param>
        private void InsertPaymentResponse(PayfortPaymentResponse response, PayfortPaymentRequest paymentRequest)
        {
            response.RequestId = paymentRequest.ID;
            paymentRequest.PayfortPaymentResponses.Add(response);

            _payfortPaymentRepository.Update(paymentRequest);
        }


        /// <summary>
        /// Update checkout payment status
        /// </summary>
        /// <param name="checkoutDetailsId">Reference ID</param>
        /// <param name="response">payment response</param>
        private void UpdateCheckoutPaymentStatus(string checkoutDetailsId, PayfortPaymentResponse response)
        {
            var checkoutDetails = _checkoutDetailsRepository.Table.First(c => c.ReferenceId == checkoutDetailsId);
            if (response.ResponseCode == 14000) // Payment succeeded 
            {
                _logger.Log($"PayfortPaymentService-> ProcessPayment responseCode 1400-success.");
                if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Available)
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;

                _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, checkoutDetails.InsuranceCompanyName, "Portal");
                _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId, checkoutDetailsId);
                _logger.Log($"PayfortPaymentService-> ProcessPayment shoping cart cleared.");
            }
            else
            {
                _logger.Log($"PayfortPaymentService-> ProcessPayment responseCode {response.ResponseCode} -failed.");
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentFailure;
            }

            _checkoutDetailsRepository.Update(checkoutDetails);
        }

        private string Sha256HashRequest(string signature)
        {
            StringBuilder Sb = new StringBuilder();
            using (System.Security.Cryptography.SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(signature));
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        /// <summary>
        /// Build payfort payment request paramater that will be posted to payfort.
        /// </summary>
        /// <param name="payfortPaymentRequestId">The payfort payment request identifier</param>
        /// <param name="language">The user language</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> BuildPaymentRequestParameters(int payfortPaymentRequestId, string language, string baseUrl, string returnUrl = null)
        {
            _logger.Log($"PayfortPaymentService -> BuildPaymentRequestParameters <<start>> payfortPaymentRequestId:{payfortPaymentRequestId}");
            var paymentRequest = _payfortPaymentRepository.Table.Include(pr => pr.CheckoutDetails.Select(cd => cd.AspNetUser)).FirstOrDefault(pr => pr.ID == payfortPaymentRequestId);
            returnUrl = string.IsNullOrEmpty(baseUrl) ? _config.Quotatoin.Url + _config.PayFort.ReturnUrl : @"/" + _config.PayFort.ReturnUrl;
            int amount = decimal.ToInt32(paymentRequest.Amount.GetValueOrDefault() * 100);
            var checkoutDetail = paymentRequest.CheckoutDetails.FirstOrDefault();
            if (checkoutDetail == null)
            {
                _logger.Log($"PayfortPaymentService -> BuildPaymentRequestParameters checkout details is null for this payment. payfortPaymentRequestId:{payfortPaymentRequestId}");
                throw new TameenkException("Missing checkout details for this payment");
            }
            var paymentRequestParameters = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("access_code", _config.PayFort.AccessCode),
                new KeyValuePair<string, string>("amount",  amount.ToString()),
                new KeyValuePair<string, string>("command", "PURCHASE"),
                new KeyValuePair<string, string>("currency", _config.PayFort.Currency),
                new KeyValuePair<string, string>("customer_email", checkoutDetail.AspNetUser.Email),
                new KeyValuePair<string, string>("language", language),
                new KeyValuePair<string, string>("merchant_identifier", _config.PayFort.MerchantIdentifier),
                new KeyValuePair<string, string>("merchant_reference", paymentRequest.ReferenceNumber),
                new KeyValuePair<string, string>("return_url", baseUrl + returnUrl)
            };


            var paymentresult = paymentRequestParameters.OrderBy(x => x.Key).ToList();
            var signaterBuilder = new StringBuilder(_config.PayFort.SHARequestPhrase);
            foreach (var item in paymentresult)
            {
                signaterBuilder.Append(item.Key);
                signaterBuilder.Append("=");
                signaterBuilder.Append(item.Value);
            }
            signaterBuilder.Append(_config.PayFort.SHARequestPhrase);
            string signature = Sha256HashRequest(signaterBuilder.ToString());
            paymentRequestParameters.Add(new KeyValuePair<string, string>("signature", signature));

            return paymentRequestParameters;

        }


        /// <summary>
        /// Get checkout details reference identifier from payfort response
        /// </summary>
        /// <param name="payfortResponse">The pay fort response object.</param>
        /// <returns></returns>
        public string GetCheckoutReferenceIdFromPayfortResponse(PayfortResponse payfortResponse)
        {
            string result = string.Empty;
            //get the payment request, then get checkout that has this payment request
            var request = _payfortPaymentRepository.Table
                .Include(p => p.PayfortPaymentResponses)
                .Include(p => p.CheckoutDetails)
                .FirstOrDefault(p => p.ReferenceNumber == payfortResponse.merchant_reference);
            if (payfortResponse.response_code == "14000" && request != null)
            {
                /// each payment request should be related to one checkoutDetail
                var checkoutDetail = request.CheckoutDetails.FirstOrDefault();
                if (checkoutDetail == null) throw new ArgumentException("There is not checkout related to this payment request");
                result = checkoutDetail.ReferenceId;
            }
            return result;
        }


        /// <summary>
        /// Build the payfort payment response entity 
        /// </summary>
        /// <param name="payfortResponse">The payfort response.</param>
        /// <returns></returns>
        public PayfortPaymentResponse BuildPayfortPaymentResponse(PayfortResponse payfortResponse)
        {
            return new PayfortPaymentResponse()
            {
                ResponseCode = int.Parse(payfortResponse.response_code),
                ResponseMessage = payfortResponse.response_message,
                Amount = string.IsNullOrEmpty(payfortResponse.amount) ? null : (decimal?)(decimal.Parse(payfortResponse.amount) / 100),
                PaymentOption = payfortResponse.payment_option,
                CardNumber = payfortResponse.card_number,
                CardHolerName = payfortResponse.card_holder_name,
                CardExpiryDate = payfortResponse.expiry_date,
                CustomerIP = payfortResponse.customer_ip,
                FortId = payfortResponse.fort_id,
                status = string.IsNullOrEmpty(payfortResponse.status) ? null : (short?)short.Parse(payfortResponse.status),
                CustomerEmail = payfortResponse.customer_email,
                Signature = payfortResponse.signature
            };
        }
        #endregion


    }
}

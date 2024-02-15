using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Payment;

namespace Tameenk.Services.Core.Payments
{
    public interface IHyperpayPaymentService
    {
        /// <summary>
        /// Generate riyadh bank request url.
        /// </summary>
        /// <param name="riyadBankMigsRequest">The RiyadBank request parameters</param>
        /// <returns></returns>
        string CreateRiyadBankRequestUrl(HyperpayRequest hyperpayRequest);


        /// <summary>
        /// Validate the response of parameter
        /// </summary>
        /// <param name="secureHash">The secure hash (signature) send from MIGS portal in response parameter.</param>
        /// <param name="list">The list of parameters without the secure hash and secure hash type.</param>
        /// <returns></returns>
        //bool ValidateResponse(string secureHash, IEnumerable<KeyValuePair<string, string>> list);

        /// <summary>
        /// Process payment according to MIGS response.
        /// </summary>
        /// <param name="riyadBankMigsResponse">The riyadbank MIGS response.</param>
        bool ProcessPayment(HyperpayResponse hyperpayResponse, LanguageTwoLetterIsoCode culture, string channel,int paymentMethodId, out string exception);
        HyperpayRequest RequestHyperpayUrl(HyperpayRequest hyperpayRequest, out string exception);
        HyperpayRequest RequestHyperpayUrlForSTCPay(HyperpayRequest hyperpayRequest, out string exception);
        // Dictionary<string, dynamic> RequestHyperpayToValidResponse(string id);
        Dictionary<string, dynamic> RequestHyperpayToValidResponse(string id, out string exception);
        // HyperpayRequest GetHyperPayCheckoutId(HyperpayRequest hyperpayRequest, out string exception);
        void UpdateHyperRequest(HyperpayRequest hyperpayRequest);
        HyperpayRequest GetById(int Id);
        Dictionary<string, dynamic> Search(string transactionId,string channel, out string exception);
        List<HyperpayRequest> GetAllMissingTransactionsFromHyperPayRequest(DateTime startDate, DateTime endDate, out string exception);
        HyperpayRequest RequestHyperpayUrlForSTCPayForMobile(HyperpayRequest hyperpayRequest, string phoneNumber, bool isQrCode, out string exception);

        HyperSplitOutput RequestHyperpayUrlWithSplitOption(HyperpayRequest hyperpayRequest, int companyId, string companyName, string channel,Guid merchantTransactionId, out string exception);

        void RetryFailedSplitOperation(out string exception);
        HyperpayRequest RequestHyperpayUrlForSTCPayWithSplitOption(HyperpayRequest hyperpayRequest, out string exception);
        HyperpayRequest RequestHyperpayUrlForSTCPayForMobileWithSplitOption(HyperpayRequest hyperpayRequest, string phoneNumber, bool isQrCode, out string exception);
        HyperpayRequest GetHyperpayRequestByReferenceId(string referenceId, out string exception);
        HyperpayUpdateOrderOutput UpdateOrder(HyperpayOrderModel orderModel, string referenceId, string channel, string accessToken, string paymentBrand, bool isBcareUpdateOrder, CheckoutDetail checkoutDetail);

        bool UpdateCheckoutPaymentStatus(CheckoutDetail checkoutDetail, bool paymentSuccessfull, string channel, int paymentMethodId);
        int UpdateHyperpayRequestStatus(int Id, string referenceId);
        HyperpayResponse GetFromHyperpayResponseSuccessTransaction(int HyperpayRequestId, string hyperpayResponseId, decimal amount);
        bool InsertHyperpayResponse(HyperpayResponse hyperpayResponse);
        HyperSplitOutput SendSplitLoginRequest(HyperPayNotificationInfo request);
        HyperpayCreateOrderOutput CreateOrder(HyperpayCreateOrderModel orderModel, string referenceId, string channel, string accessToken, string paymentBrand);
        ApplePayOutput StartApplePaySession(string referenceId, string companyName, int companyId, string channel);
        bool InsertHyperpayReuest(HyperpayRequest hyperpayRequest);
        ApplePayOutput ApplePayPayment(ApplePayRequestInfo requestInfo, ApplePayPaymnetTokenModel paymentToken);
        ApplePayRequestInfo GetRequestInfoForApplePay(string referenceId, out string exception);
        bool UpdateCheckoutWithPaymentStatus(string referenceId, int policyStatusId, int insuranceCompanyId, string insuranceCompanyName, bool paymentSuccessfull, string channel, int paymentMethodId);
        HyperpayRequest GetHyperpayRequestByMerchantTransactionId(string merchantTransactionId, out string exception);

        HyperpayRequestInfo GetHyperpayRequestInfo(string referenceId, out string exception);
        HyperSplitOutput RequestHyperpayForLeasing(HyperpayRequest hyperpayRequest, int companyId, string companyName, string channel, Guid merchantTransactionId, out string exception);
    }
}

using System;
using System.Collections.Generic;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Models.Checkout;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core.Payments;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation;
using Tameenk.Core;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Core.IVR;

namespace Tameenk.Services.Checkout.Components
{
    public interface ICheckoutContext
    {

        CheckoutOutput CheckoutDetails(string ReferenceId, string QtRqstExtrnlId, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang, string productId, string selectedProductBenfitId, string hashed);
        CheckoutOutput SubmitCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang);
        CheckoutOutput AddItemToCart(Tameenk.Core.Domain.Dtos.AddItemToCartModel model, CheckoutRequestLog log, string lang);
        void SubmitMissingTransactions();
        bool ManagePhoneVerification(Guid userId, string phoneNumber, string code,string channel);
        PolicyOutput GetUserPolicies(string nin, string sequenceNumber, string customCardNumber, out string exception);
        QuotationOutput ValidateTawuniyaQuotation(QuotationRequestDriverModel quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, List<string> selectedBenefits = null, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false);
        CheckoutOutput ActivateUserEmailToReceivePolicy(string referenceId, string email, string userId, string channel, out string exception);
        CheckoutDetail GetCheckoutDetailByReferenceIdAndUserId(string referenceId, string userId);
        bool SendActivationEmail(string referenceId, string userId, out string exception);
        NumberOfAccidentOutput ValidateNumberOfAccident(string userId, string userName, ShoppingCartItemDB item, QuotationRequestDriverModel quotationRequest, string channel, List<long> selectedProductBenfitId);
        string GetCarPlateInfo(string plateText1, string plateText2, string plateText3, int plateNumber, string lang);
        List<RenewalPolicyInfo> GetRenewalPolicies(DateTime start, DateTime end, int notificationNo, out string exception);
        PolicyOutput GetUserFailedPoliciesByReference(string referenceId, out string exception);
        CheckoutOutput UserHasActivePolicy(string nin);
        List<PolicyInformation> GetPolicyInformationForRoadAssistance();
        CheckoutOutput SubmitAutoleasingCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang);
        CheckoutOutput AddAutoleasingItemToCart(Tameenk.Core.Domain.Dtos.AddItemToCartModel model, CheckoutRequestLog log, string lang, bool autoRenewal = false);
        List<FailedMorniRequests> GetAllFailedMorniRequests();
        bool GetCheckOutDetailsTemp3(string externalId);
        PolicyOutput SendWataniyaDraftpolicy(QuotationRequestDriverModel quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false);
        List<EdaatNotificationOutput> GetAllEdaatNotificationPayment(EdaatFilter filter, out int count, out string exception, int pageIndex = 0, int pageSize = int.MaxValue);
        CheckoutOutput PaymentUsingHyperpay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId);
        HyperPayOutput ProcessHyperpayPayment(string id, string lang, string channel, Guid userId, int paymentMethodId);
        HyperPayOutput HyperpayUpdateOrder(CheckoutDetail checkoutDetail, HyperpayResponse hyperpayResponse, int? paymentId, bool? paymentSucceded);
        ApplePayOutput PaymentUsingApplePay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId);
        ApplePayOutput ApplePayProcessPayment(string referenceId, string paymentToken, string lang, string userId);
        CheckoutOutput ResendVerifyCode(string userId, string phoneNumber, string lang);
        List<EdaatRequest> GetEdaatRequestsByNationalID(string nationalID,DateTime  startDate,DateTime endDate, out string exception);

        CheckoutDiscountOutput CheckDiscountCode(Tameenk.Core.Domain.Dtos.CheckotDiscountModel model, string userId);
        AddBenefitOutput PurchaseVechileBenefit(Models.Checkout.PurchaseBenefitModel model, string UserId, string userName);
        AddDriverOutput PurchaseVechileDriver(Models.Checkout.PurchaseDriverModel model, string UserId, string userName);
        int UpdateCheckoutWithPolicyStatus(CheckoutDetail checkoutDetail, out string exception);
        TabbyOutput TabbyPaymentNotification(string PaymentId, string channel, string userId);
        TabbyOutput TabbyPaymentWebhook(TabbyWebhookModel tabbyWebHookModel);
        string LogApplePayError(string userId, string referenceId, string errorDescription);
        CheckoutOutput PrePaymentChecks(string userId, PrePaymentCheckModel model);

        CheckoutOutput AddLeasingItemToCart(Tameenk.Core.Domain.Dtos.AddILeasingtemToCartModel model, CheckoutRequestLog log, string lang);
        CheckoutOutput SubmitLeasingCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang);
        bool LeasingHandleAutoleasingPoliciesToSendSmsPortalLink(AutoleasingPortalLinkModel data, out string exception);
        CheckoutOutput PaymentUsingHyperpayLeasing(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId);
        CheckoutOutput AddIVRItemToChart(RenewalAddItemToChartModel checkOutModel, IVRServicesLog log);
        IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> HandleIVRSendSadadNumber(RenewalAddItemToChartUserModel lowestProduct, bool sendToCallerPhone, string phoneNumber, IVRServicesLog log);
    }
}

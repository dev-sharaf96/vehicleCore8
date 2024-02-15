using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Payment.Esal.Component;
using Tameenk.Payment.Esal.Component.Model;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Implementation.Payments.Tabby;

namespace Tameenk.Services.Checkout.Components.Output
{
    public class CheckoutOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            EmptyReturnObject = 6,
            InvalidRetrunObject = 7,
            InvalidIBAN = 8,
            InvalildInputParameter = 9,
            ErrorWithRedirectSearchResult = 10,
            FailedToAddItemToCart = 11,
            InvalidQuotation = 12,
            InvalidQuotationNumberOfAccident = 13,
            InvalidPayment = 14,
            MissingImages = 16,
            IBANUsedForOtherDriver = 15,
            PhoneNoWithAnotherDriver = 16,
            InvalidEmail = 17,
            InvalidPhone = 18,
            QuotationRequestExpired=19,
            ShoppingCartItemBenefitsIsNull=20,
            EmailReachedTheMaximumPurchase=21,
            InsuredCityDoesNotMatchAddressCity=22,
            MissingAddress=40,
            EmailAlreadyUsed = 23,
            PhoneAlreadyUsed = 24,
            PhoneIsNotVerified = 25,
            InvalidProduct = 26,
            UserLockedOut=27,
            AnonymousUser=28,
            UserNotFound=29,
            HashedNotMatched=30,
            InvalidCity=31,
            InvalidVerifyCode = 32,            UserExceedsInsuranceNumberLimitPerYear = 33,
            MailNotSent = 34,
            ConfilictProduct=35,
            SadadError = 36,
            IBANValidationError=37,
            VehicleExceededSadadLimit=38,
            HaveAccidents=39,
            FailedToDeleteOldInvoice=40,
            FailedToCancelSadadnvoice=41,
            CommissionsAndFeesNull = 42,
            UserIsNotAutoLeasing = 43,
            InvalidBank = 44,
            initialOptionPurchasedError=45,
            InvalidPaymentMethods=46,
            HasActivePolicies=47,
            //new for edaat
            AuthorizationFailed = 48,
            Failed = 49,
            PaidBefore=50,
            UserIsNotCorporate = 51,
            FailedToWalletUpdateOrder = 52,
            AccountIsNotCorporate = 53,
            InvalidVerificationCode=54,
            CheckoutDetailsNotFound = 55,
            EdaatNumberReachedToMaximum = 56,
            CompanyDriverExceedsInsuranceNumberLimitPerYear =57,
            FailedToAddODItemToCart=58,
            ShoppingCartItemIsNull = 59,
            InvalidService,
            NoQuotationForm,
            YakeenMobileVerificationError
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        [JsonProperty("checkoutModel")]
        public CheckoutModel CheckoutModel { get; set; }

        //[JsonProperty("payfortResponse")]
        //public PayfortResponse PayfortResponse { get; set; }

        [JsonProperty("sadadPaymentResponseModel")]
        public SadadPaymentResponseModel SadadPaymentResponseModel { get; set; }
        [JsonProperty("esalPaymentResponseModel")]
        public EsalPaymentResponseModel EsalPaymentResponseModel { get; set; }

        [JsonProperty("edaatPaymentResponseModel")]
        public EdaatPaymentResponseModel EdaatPaymentResponseModel { get; set; }


        [JsonProperty("payfortRequestId")]
        public int PayfortRequestId { get; set; }

        [JsonProperty("hyperpaycheckoutid")]
        public string HyperPayCheckoutId { get; set; }

        [JsonProperty("hyperpayRequestId")]
        public int HyperpayRequestId { get; set; }

        //[JsonProperty("riyadBankMigsResponse")]
        //public RiyadBankMigsResponse RiyadBankMigsResponse { get; set; }

        //[JsonProperty("purchasedProductModel")]
        //public PurchasedProductModel PurchasedProductModel { get; set; }

        //[JsonProperty("riyadBankRequestUrl")]
        //public string RiyadBankRequestUrl { get; set; }

        //[JsonProperty("payfortRequestParameters")]
        //public List<KeyValuePair<string, string>> PayfortRequestParameters { get; set; }


        // for add item to cart
        [JsonProperty("qtRqstExtrnlId")]
        public string qtRqstExtrnlId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("typeOfInsurance")]
        public short? TypeOfInsurance { get; set; }

        [JsonProperty("vehicleAgencyRepair")]
        public bool? VehicleAgencyRepair { get; set; }

        [JsonProperty("deductibleValue")]
        public int? DeductibleValue { get; set; }

        public bool InvalidQoutation { get; set; }
        public double ResponseTimeInSeconds { get; set; }
        [JsonProperty("isUserPhoneConfirmed")]
        public bool IsUserPhoneConfirmed { get; set; }

        [JsonProperty("isCheckoutEmailVerified")]
        public bool IsCheckoutEmailVerified { get; set; }


        [JsonProperty("productId")]
        public string ProductId { get; set; }


        [JsonProperty("activePolicyData")]        public ActivePolicyData ActivePolicyData { get; set; }

        [JsonProperty("driverNin")]
        public string DriverNin { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        public string _hValue
        {
            get;
            set;
        }

        [JsonProperty("walletPaymentResponseModel")]
        public WalletPaymentResponseModel WalletPaymentResponseModel { get; set; }
        public string FirstExpiryDateForEdaat
        {
            get;
            set;
        }
        public string _c
        {
            get;
            set;
        }
        [JsonProperty("tabbyResponseModel")]
        public TabbyResponseHandler tabbyResponseHandlerModel { get; set; }

        [JsonProperty("selectedProductBenfitId")]
        public List<int> SelectedProductBenfitId { get; set; }

        [JsonProperty("loginResult")]
        public RenewalLoginResponseModel LoginResult { get; set; }

        [JsonIgnore]
        public string LogDescription { get; set; }
    }
}

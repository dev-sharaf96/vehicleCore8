using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Implementation.Payments
{
    public class RiyadBankMigsPaymentService : IRiyadBankMigsPaymentService
    {
        #region Fields

        private readonly IRepository<RiyadBankMigsRequest> _riyadBankMigsRequestRepository;
        private readonly IRepository<RiyadBankMigsResponse> _riyadBankMigsResponseRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IInsuranceCompanyService _inuranceCompanyService;
        #endregion

        #region Ctor

        public RiyadBankMigsPaymentService(IRepository<RiyadBankMigsRequest> riyadBankMigsRequestRepository,
            IRepository<RiyadBankMigsResponse> riyadBankMigsResponseRepository,
            IPolicyProcessingService policyProcessingService,
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            TameenkConfig tameenkConfig,
            ILogger logger, INotificationService notificationService,
            IInsuranceCompanyService inuranceCompanyService)
        {
            _riyadBankMigsRequestRepository = riyadBankMigsRequestRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<RiyadBankMigsRequest>));
            _riyadBankMigsResponseRepository = riyadBankMigsResponseRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<RiyadBankMigsResponse>));
            _policyProcessingService = policyProcessingService ?? throw new TameenkArgumentNullException(nameof(IPolicyProcessingService));
            _shoppingCartService = shoppingCartService ?? throw new TameenkArgumentNullException(nameof(IShoppingCartService));
            _orderService = orderService ?? throw new TameenkArgumentNullException(nameof(IOrderService));
            _config = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));
            _notificationService = notificationService;
            _inuranceCompanyService = inuranceCompanyService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate riyadh bank request url.
        /// </summary>
        /// <param name="riyadBankMigsRequest">The RiyadBank request parameters</param>
        /// <returns></returns>
        public string CreateRiyadBankRequestUrl(RiyadBankMigsRequest riyadBankMigsRequest)
        {
            if (riyadBankMigsRequest == null)
            {
                throw new TameenkArgumentNullException(nameof(riyadBankMigsRequest));
            }
            if (riyadBankMigsRequest.CheckoutDetails.Count < 1)
            {
                throw new TameenkArgumentException("The migs payment request not linked to checkout details.");
            }
            _logger.Log($"RiyadBankMigsPaymentService -> CreateRiyadBankRequestUrl, (Riyad Bank Migs Request : {JsonConvert.SerializeObject(riyadBankMigsRequest.Amount)})");


            //var VPC_URL = "https://migs.mastercard.com.au/vpcpay";
            var vpcUrl = _config.RiyadBank.Url;
            riyadBankMigsRequest.AccessCode = _config.RiyadBank.AccessCode;
            riyadBankMigsRequest.MerchantId = _config.RiyadBank.MerchantId;
            riyadBankMigsRequest.MerchTxnRef = CreateMerchantReference();
            riyadBankMigsRequest.Version = "1";


            string hashSecret = _config.RiyadBank.SecureHashSecret;

            // Create signature hash to secure request. 
            var transactionData = GetParameters(riyadBankMigsRequest).OrderBy(t => t.Key, new VPCStringComparer());

            if (!string.IsNullOrEmpty(hashSecret))
            {
                riyadBankMigsRequest.SecureHash = CreateSHA256Signature(transactionData, hashSecret);
            }
            riyadBankMigsRequest.SecureHashType = "SHA256";
            var urlParamters = string.Join("&", transactionData.Select(item => $"{HttpUtility.UrlEncode(item.Key)}={HttpUtility.UrlEncode(item.Value)}"));
            var redirectUrl = $"{vpcUrl}?{urlParamters}";

            _riyadBankMigsRequestRepository.Insert(riyadBankMigsRequest);
            // Return the url
            return $"{redirectUrl}&vpc_SecureHash={riyadBankMigsRequest.SecureHash}&vpc_SecureHashType={riyadBankMigsRequest.SecureHashType}";
        }

        /// <summary>
        /// Validate the response of parameter
        /// </summary>
        /// <param name="secureHash">The secure hash (signature) send from MIGS portal in response parameter.</param>
        /// <param name="list">The list of parameters without the secure hash and secure hash type.</param>
        /// <returns></returns>
        public bool ValidateResponse(string secureHash, IEnumerable<KeyValuePair<string, string>> list)
        {
            var signature = CreateSHA256Signature(list, _config.RiyadBank.SecureHashSecret);
            return signature == secureHash;
        }

        /// <summary>
        /// Process payment according to MIGS response.
        /// </summary>
        /// <param name="riyadBankMigsResponse">The riyadbank MIGS response.</param>
        public bool ProcessPayment(RiyadBankMigsResponse riyadBankMigsResponse, LanguageTwoLetterIsoCode culture)
        {
            var riyadBankMigsRequest = GetRiyadBankMigsRequestByMerchantReference(riyadBankMigsResponse.MerchTxnRef, riyadBankMigsResponse.OrderInfo);
            if (riyadBankMigsRequest == null)
            {
                _logger.Log($"RiyadBankMigsPaymentService -> ProcessPayment there was not riyadBankMigsRequest with this response: {JsonConvert.SerializeObject(riyadBankMigsResponse)}");
                return false;
            }
            if (riyadBankMigsRequest.Amount != riyadBankMigsResponse.Amount)
            {
                _logger.Log($"RiyadBankMigsPaymentService -> ProcessPayment the response was not the same amount as request : (request amount : {riyadBankMigsRequest.Amount}, response amount : {riyadBankMigsResponse.Amount})");
                throw new TameenkException("RiyadBank (MIGS) Payment response was not the same amount as request");
            }

            var riyadBankMigsResponses = _riyadBankMigsResponseRepository.Table.FirstOrDefault(req => req.OrderInfo == riyadBankMigsResponse.OrderInfo && req.TxnResponseCode == "0" && req.RiyadBankMigsRequestId == riyadBankMigsRequest.Id && req.TransactionNo == riyadBankMigsResponse.TransactionNo && req.Amount == riyadBankMigsResponse.Amount);
            if (riyadBankMigsResponses != null)
                return true;

            riyadBankMigsRequest.RiyadBankMigsResponses.Add(riyadBankMigsResponse);
            //There should be only one checkout related to this payment request
            var checkoutDetail = riyadBankMigsRequest.CheckoutDetails.FirstOrDefault();
            if (checkoutDetail == null)
            {
                _logger.Log($"RiyadBankMigsPaymentService -> ProcessPayment couldnt find the checkout.");
                throw new TameenkArgumentException("There is no checkoutDetail related to this payment request");
            }
            var paymentSucceded = riyadBankMigsResponse.TxnResponseCode == "0";
            if (paymentSucceded)
            {
                try
                {
                    var product = checkoutDetail.OrderItems != null && checkoutDetail.OrderItems.Count > 0 ? checkoutDetail.OrderItems.FirstOrDefault().Product : null;
                    var companyName = product == null
                                ? string.Empty
                                : _inuranceCompanyService.GetInsuranceCompanyName(product.ProviderId.Value, culture);
                    var productType = product == null
                                ? string.Empty
                                : culture == LanguageTwoLetterIsoCode.Ar ? product.QuotationResponse.ProductType.ArabicDescription : product.QuotationResponse.ProductType.EnglishDescription;

                    var amount = Math.Round(riyadBankMigsResponse.Amount / 100, 2);
                    var message = string.Format(Tameenk.Resources.WebResources.WebResources.ProcessPayment_SendingSMS,
                        productType, companyName, amount);
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = checkoutDetail.Phone,
                        MessageBody = message,
                        Method = SMSMethod.RiyadBankPaymentNotification.ToString(),
                        Module = Module.Vehicle.ToString()
                    };
                    _notificationService.SendSmsBySMSProviderSettings(smsModel);
                }
                catch
                {

                }
            }
            UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded);
            riyadBankMigsRequest.ModifiedDate = DateTime.Now;
            _riyadBankMigsRequestRepository.Update(riyadBankMigsRequest);
            return paymentSucceded;
        }


        #endregion


        #region Private Methods

        /// <summary>
        /// Update checkout payment status
        /// </summary>
        /// <param name="checkoutDetailsId">Reference ID</param>
        /// <param name="response">payment response</param>
        private void UpdateCheckoutPaymentStatus(CheckoutDetail checkoutDetail, bool paymentSuccessfull)
        {
            if (paymentSuccessfull) // Payment succeeded 
            {
                if (checkoutDetail.PolicyStatusId != (int)EPolicyStatus.Available)
                    checkoutDetail.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;

                _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetail.ReferenceId);
                _shoppingCartService.EmptyShoppingCart(checkoutDetail.UserId, checkoutDetail.ReferenceId);
            }
            else
            {
                checkoutDetail.PolicyStatusId = (int)EPolicyStatus.PaymentFailure;
            }
        }

        /// <summary>
        /// Get riyadbank request from database by MerchantTxtRef & reference Id of checkout
        /// </summary>
        /// <param name="referenceId">The checkout details reference identifier.</param>
        /// <param name="merchantReferenceTxt">The merchant reference text.</param>
        /// <returns></returns>
        private RiyadBankMigsRequest GetRiyadBankMigsRequestByMerchantReference(string merchantReferenceTxt, string referenceId)
        {
            return _riyadBankMigsRequestRepository.Table.Include(e => e.CheckoutDetails.Select(c => c.OrderItems.Select(o => o.Product.QuotationResponse.ProductType)))
                .FirstOrDefault(req => req.MerchTxnRef == merchantReferenceTxt && req.CheckoutDetails.Any(c => c.ReferenceId == referenceId));
        }
        /// <summary>
        /// Get parameter from riydbank request.
        /// </summary>
        /// <param name="riyadBankMigsRequest">The riydbank reuest parameter object.</param>
        /// <returns></returns>
        private Dictionary<string, string> GetParameters(RiyadBankMigsRequest riyadBankMigsRequest)
        {
            var parameters = new Dictionary<string, string> {
                { "vpc_Version" ,riyadBankMigsRequest.Version},
                { "vpc_Command",riyadBankMigsRequest.Command},
                { "vpc_Merchant" ,riyadBankMigsRequest.MerchantId},
                { "vpc_AccessCode" ,riyadBankMigsRequest.AccessCode},
                { "vpc_MerchTxnRef" ,riyadBankMigsRequest.MerchTxnRef},
                { "vpc_OrderInfo",riyadBankMigsRequest.OrderInfo},
                { "vpc_Amount" ,riyadBankMigsRequest.Amount.ToString()},
                { "vpc_ReturnURL", riyadBankMigsRequest.ReturnUrl},
                { "vpc_Locale",riyadBankMigsRequest.Locale}
            };
            return parameters;
        }

        /// <summary>
        /// Create SHA256 signature.
        /// </summary>
        /// <param name="list">The list of orderd parameters.</param>
        /// <param name="secureHashKey">The secure hash key used in hashing.</param>
        /// <returns></returns>
        private string CreateSHA256Signature(IEnumerable<KeyValuePair<string, string>> list, string secureHashKey)
        {
            string _secureSecret = secureHashKey;
            // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
            // hex decoding eliminates this source of error as it is independent of the character encoding
            // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
            byte[] convertedHash = new byte[_secureSecret.Length / 2];
            for (int i = 0; i < _secureSecret.Length / 2; i++)
            {
                convertedHash[i] = (byte)Int32.Parse(_secureSecret.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            // Build string from collection in preperation to be hashed
            var parameterStrings = new List<string>();
            foreach (KeyValuePair<string, string> kvp in list)
            {
                if (kvp.Key.StartsWith("vpc_") || kvp.Key.StartsWith("user_"))
                    parameterStrings.Add($"{kvp.Key}={kvp.Value}");
            }
            // Create secureHash on string
            var paramString = string.Join("&", parameterStrings);
            string hexHash = "";
            using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
            {
                byte[] utf8bytes = Encoding.UTF8.GetBytes(paramString);
                byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
                byte[] hashValue = hasher.ComputeHash(iso8859bytes);

                foreach (byte b in hashValue)
                {
                    hexHash += b.ToString("X2");
                }
            }
            return hexHash;

        }
        /// <summary>
        /// Create merchant reference number to identify the payment request.
        /// </summary>
        /// <returns></returns>
        private string CreateMerchantReference()
        {
            return $"RIYADBANKMIGS{_riyadBankMigsRequestRepository.Table.Count()}{new Random((int)DateTime.Now.Ticks).Next(111111111, 999999999)}";
        }

        #endregion


    }

    /// <summary>
    /// The VPC string comparer to set the order of parameters.
    /// </summary>
    public class VPCStringComparer : IComparer<string>
    {
        /// <summary>
        /// Compare two strings.
        /// </summary>
        /// <param name="x">The first string to compare with.</param>
        /// <param name="y">The second string to compare with.</param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            var myComparer = CompareInfo.GetCompareInfo("en-US");
            return myComparer.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}

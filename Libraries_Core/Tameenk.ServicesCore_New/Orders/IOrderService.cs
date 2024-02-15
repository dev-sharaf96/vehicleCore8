using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Orders;

namespace Tameenk.Services.Orders
{
    public interface IOrderService
    {




        /// <summary>
        /// Get all bank codes
        /// </summary>
        /// <returns></returns>
        IEnumerable<BankCode> GetBankCodes();


        /// <summary>
        /// Get all bank to specific user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<UserBank> GetUserBanks(string id, int pageIndx = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// get all IBANs to specific user
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns></returns>
        IEnumerable<string> GetUserIBANs(string userId);

        /// <summary>
        /// Get checkout details by refernce identifier.
        /// </summary>
        /// <param name="referenceId">The checkout details reference identifier.</param>
        /// <returns></returns>
        CheckoutDetail GetCheckoutDetailByReferenceId(string referenceId);

        /// <summary>
        /// Update checkout details.
        /// </summary>
        /// <param name="checkoutDetail">The checkout details object.</param>
        /// <returns></returns>
        CheckoutDetail UpdateCheckout(CheckoutDetail checkoutDetail);

        /// <summary>
        /// Create invoice of checkout details.
        /// </summary>
        /// <param name="referenceId">The checkout details refernce identifier.</param>
        /// <returns></returns>
        Invoice CreateInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0,string odReferenceId = null);

        /// <summary>
        /// Create new checkout details.
        /// </summary>
        /// <param name="checkoutDetail">The checkout details object.</param>
        /// <returns></returns>
        CheckoutDetail CreateCheckoutDetails(CheckoutDetail checkoutDetail);

        /// <summary>
        /// Calculate the total amount of checkout details
        /// </summary>
        /// <param name="checkoutDetail">The checkout details object.</param>
        /// <returns></returns>
        decimal CalculateCheckoutDetailTotal(CheckoutDetail checkoutDetail);


        /// <summary>
        /// Create order items from shopping cart items
        /// </summary>
        /// <param name="shoppingCartItems">The shopping cart items</param>
        /// <param name="referenceId">The checkout reference identifier.</param>
        /// <returns></returns>
        List<OrderItem> CreateOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId);

        /// <summary>
        /// Get user last used IBAN
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        string GetLatestUsedIBANByUser(string userId);

        /// <summary>
        /// Get invoice if it exits
        /// </summary>
        /// <param name="refrenceId">reference identifier</param>
        /// <returns></returns>
        Invoice GetInvoiceByRefrenceId(string refrenceId);

        CheckoutUsers CreateCheckoutUser(CheckoutUsers checkoutUser);

        bool IsUserHaveVerifiedPhoneNumbers(Guid UserId, string phoneNumber);

        bool IsUserHaveVerifiedPhoneNumbersByNIN(string driverNIN, string phoneNumber, string referenceId);
        CheckoutUsers GetByUserIdAndPhoneNumber(Guid UserId, string phoneNumber);

        CheckoutUsers UpdateCheckoutUser(CheckoutUsers checkoutUser);

        DateTime GetFirstPolicyIssueDate(string userEmail);

        int GetPurchasedCheckoutCount(string userEmail, DateTime durationDate);
        CheckoutDetail GetFromCheckoutDeatilsbyPhoneNo(string phone, DateTime durationDate);
        List<CheckoutUsers> GetByCheckoutUsersByUserId(Guid userId, DateTime durationDate);
        List<CheckoutDetail> GetFromCheckoutDeatilsbyPhoneNoList(Guid mainDriverId, string phone, DateTime durationDate);
        bool DeleteInvoiceByRefrenceId(string refrenceId, string userID, out string exception);
        bool DeleteOrderItemByRefrenceId(string refrenceId, out string exception);
        bool SaveOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId, out string exception);
        CheckoutDetail GetFromCheckoutDeatilsbyReferenceId(string referenceId);
        int GetCountFromCheckoutDeatilsbyPhoneNoList(Guid mainDriverId, string phone, DateTime durationDate);
        CheckoutDetail GetFromCheckoutDetailByPhoneNumber(string phoneNumber, Guid driverId);
        CheckoutDetail GetFromCheckoutDetailByEmail(string email, Guid driverId);
        CheckoutDetail GetFromCheckoutDetailByIBAN(string iban, Guid driverId);
        CheckoutDetailInfo GetCheckoutDetail(string phoneNumber, string email, string iban, Guid driverId,string driverNin);

        bool ManagePhoneVerificationForCheckoutUsers(Guid UserId, string phoneNumber, string code);        CheckoutUsers GetByUserIdAndPhoneNumberWithCodeNotVerified(Guid UserId, string phoneNumber);
        bool ConfirmCheckoutDetailEmail(string referenceId, string userId, string email, out string exception);
        List<CommissionsAndFees> GetCommissionsAndFees(int insuranceTypeCode, int companyId);
        Invoice UpdateInvoiceCommissionsAndFees(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int paymentMethodId);

        List<CommissionsAndFees> GetFeesOfPaymentMethod(int insuranceTypeCode, int companyId, int paymentMethodId);
        Invoice UpdateInvoiceFeesCalculationDetails(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int paymentMethodId);
        CheckoutDetailInfo CheckIfIbanAlreadyUsed(string iban, string driverNin);
        CheckoutDetailInfo CheckIfEmailAlreadyUsed(string email, string driverNin);
        CheckoutDetailInfo CheckIfPhoneAlreadyUsed(string phoneNumber, string driverNin);
        bool UpdateDiscountCodeToBeConsumed(Guid? vehicleId, string discountCode,string referenceId);
        bool IsUserHaveVerifiedPhoneNumbersByNINAndSequenceNumber(string driverNIN, string phoneNumber, string sequenceNumber);
        bool IsUserHaveVerifiedPhoneByYakeen(string driverNIN, string phoneNumber);

        Invoice CreateLeasingInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0);
        decimal CalculateLeasingCheckoutDetailTotal(CheckoutDetail checkoutDetail);
        List<OrderItem> CreateLeasingOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId, out string exception);
        LeasingOrderItemDetails GetLeasingOrderItemByReferenceId(string referenceId, out string exception);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Checkouts;

namespace Tameenk.Services.Core.Checkouts
{
    public interface ICheckoutsService
    {
        /// <summary>
        /// Get all Checkouts with filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<CheckoutDetail> GetCheckoutsWithFilter(IQueryable<CheckoutDetail> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ReferenceId", bool sortOrder = false);
        IQueryable<CheckoutDetail> PrepareCheckoutDetailsQueryWithFilter(CheckoutsFilter checkoutsFilter);
        CheckoutDetail GetCheckoutDetailsByReferenceId(string referenceId);
        CheckoutDriverInfo GetAllCheckedoutPoliciesBasedOnFilter(PolicyCheckoutFilter policyFilter);
        IPagedList<CheckoutDriverInfo> GetAllCheckedoutPoliciesWithFilter(IQueryable<CheckoutDriverInfo> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ReferenceId", bool sortOrder = false);
        CheckoutDriverInfo GetDriverInfo(string nin);
        bool UpdateCheckedoutPolicy(CheckoutDriverInfo checkoutDriverInfo);
        bool AddCheckedoutPolicy(CheckoutDriverInfo checkoutDriverInfo);
        bool DeleteCheckedoutPolicy(CheckoutDriverInfo checkoutDriverInfo, out string nin, out string request);
        CheckoutDriverInfo GetCheckoutDriverInfo(string nin, string phone, string email, string iban);
        IEnumerable<Channels> GetAllChannel();
        List<UserPolicyModel> GetUserPolicies(string nin, string sequenceNumber, string customCardNumber, out string exception);
        List<FailedPolicyModel> GetUserFailedPolicies(string nin, string sequenceNumber, string customCardNumber, string referenceId, out string exception);
        int GetUserSuccessPolicies(string driverNin, out string exception);
        CheckoutDetail GetCheckoutDetailByReferenceIdAndUserId(string referenceId, string userId);
        int GetVerifiedEmailCheckoutDetail(string driverNin, string email, out string exception);
      
        int GetTotalVehicleSadadRequestsPerDay(string vehicleId, out string exception);
        List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp(out string exception);
        List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp2(out string exception);
        List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp3(out string exception);
        List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp4(out string exception);

        CheckoutDetail GetComprehansiveCheckoutDetail(string referenceId);
        CheckoutCarImage AddChekoutCarImages(CheckoutCarImage carImage, out string exception);
        bool UpdateCheckOut(CheckoutDetail details, out string exception);
        bool UpdateCheckOutDetails(string referenceId);
        List<RenewalPolicyInfo> GetRenewalPolicies(DateTime start, DateTime end, int notificationNo, out string exception);
        CheckoutDetail GetFromCheckoutDeatilsbyReferenceId(string referenceId);
        ActivePolicyData UserHasActivePolicy(string driverNin, out string exception);

        List<PolicyInformation> GetPolicyInformationForRoadAssistance(out string exception);
        List<FailedMorniRequests> GetAllFailedMorniRequests(out string exception);
        int GetActivePoliciesByNinAndVehicleId(string driverNin, string vehicleId, out string exception);

        List<string> GetAssignedDriverNinsByEmail(string email, out string exception);

        CheckoutDetail GetCheckoutDetails(string referenceId);
        CheckoutDetail GetFromCheckoutDetailsByReferenceId(string referenceId, out string exception);
        int UpdateCheckoutWithPaymentStatus(string referenceId, int policyStatusId, int paymentMethodId, out string exception);
        CheckoutDetail GetLastPruchasedCheckoutDetailsByNIN(string driverNin, out string exception);
        Policy GetOLdTplPolicyData(string driverNin, string vehicleId, out string exception);
        List<InsuredPolicyInfo> GetUserSuccessPoliciesDetails(string driverNin, out string exception);
        List<InsuredPolicyDetails> GetUserSuccessPoliciesInfo(string driverNin, out string exception);
        CheckoutDetail GetUserActivePoliciesByNin(string driverNin, out string exception);
        CorporateModel GetUserSuccessPoliciesDetailsForCorprate(string driverNin, out string exception);
        bool CheckIfQuotationIsRenewalByReferenceId(string referenceId, out string exception);
    }
}

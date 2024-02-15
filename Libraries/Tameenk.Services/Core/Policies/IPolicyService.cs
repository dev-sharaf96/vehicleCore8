using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Attachments.Models;
using Tameenk.Services.Core.Policies.Renewal;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Core.Policies
{
    public interface IPolicyService
    {


        /// <summary>
        /// Get details to success policies
        /// </summary>
        /// <param name="ReferenceId">Reference Id</param>
        /// <returns></returns>
        Policy GetDetailsToSuccessPolicies(string ReferenceId);

        /// <summary>
        /// Re-Generate Policy File Pdf
        /// </summary>
        /// <param name="ReferenceId">Reference Id</param>
        /// <returns></returns>
        //Task<byte[]> ReGeneratePolicyFilePdf(string ReferenceId);

        /// <summary>
        /// Get policy File From URL by reference Id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <returns></returns>
        Task<byte[]> GetPolicyFileFromUrlByReferenceId(string referenceId);

        /// <summary>
        /// Get All Policy Status Except Avaliable
        /// </summary>
        /// <returns></returns>
        List<PolicyStatus> GetAllPolicyStatusExceptAvaliable();

        /// <summary>
        /// Get details to Fail policy by reference Id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <returns></returns>
        FailPolicy GetDetailsToFailPolicyByReferenceId(string referenceId);

        /// <summary>
        /// Edit in Fail policy
        /// </summary>
        /// <param name="failPolicy">Fail policy</param>
        /// <returns></returns>
        FailPolicy EditFailPolicy(FailPolicy failPolicy);

        /// <summary>
        /// update processing tries in policy processing queue for specific policy
        /// by reference id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <param name="processingTries">processing Tries</param>
        /// <returns></returns>
        bool UpdateMaxTriesInPolicy(string referenceId, int processingTries = 0);


        /// <summary>
        /// Get Count of fail policies based on filter .
        /// </summary>
        /// <param name="policyFilter">policy filter</param>
        /// <returns></returns>
        IQueryable<FailPolicy> GetCountFailPoliciesWithFilter(FailPolicyFilter policyFilter);

        /// <summary>
        /// Get fail policies based on filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<FailPolicy> GetFailPoliciesWithFilter(IQueryable<FailPolicy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "CheckoutDetail.ReferenceId", bool sortOrder = false);
        

            /// <summary>
            /// Get all Najm Status Lookup
            /// </summary>
            /// <param name="pageIndex">page index</param>
            /// <param name="pageSize">page size</param>
            /// <param name="sortField">sort field</param>
            /// <param name="sortOrder">sort order</param>
            /// <returns></returns>

       IPagedList<NajmStatus> GetNajmStatuses(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false);
      

        /// <summary>
        /// Get Count of ( success policies based on filter
        /// </summary>
        /// <param name="policyFilter">policy Filter</param>
        /// <returns></returns>
            IQueryable<Policy> GetCountSuccessPoliciesWithFilter(SuccessPoliciesFilter policyFilter);

        /// <summary>
        /// Get Success Policies With Filter
        /// </summary>
        /// <param name="policyFilter">policy filter</param>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">sort Field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<Policy> GetSuccessPoliciesWithFilter(IQueryable<Policy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false);
            /// <summary>
            /// Get policy by id
            /// </summary>
            /// <param name="policyId">policy ID</param>
            /// <returns></returns>
        Policy GetPolicy(int policyId);

        /// <summary>
        /// Get number of Update Request by user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetPolicyUpdateRequestByUserCount(string id);

        /// <summary>
        /// Get number of expire policies for specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetUserExpirePoliciesCount(string id);

        /// <summary>
        /// download policy file return byte[]
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns></returns>
        PolicyFile DownloadPolicyFile(string fileId);

        /// <summary>
        /// Get number of vaild policies for specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetUserPoliciesCount(string id);


      
        

        /// <summary>
        /// get all policies for specific user
        /// </summary>
        /// <returns></returns>
        IPagedList<Policy> GetUserPolicies(string id, int pageIndx = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// get all najm policies
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        IPagedList<Policy> GetNajmPolicies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false);

        


        /// <summary>
        /// Get all policies
        /// </summary>
        /// <param name="statusId">status ID</param>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <returns></returns>
        IPagedList<Policy> GetPolicies(int statusId, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// Return polies that match given filter
        /// </summary>
        /// <param name="policyFilter"></param>
        /// <returns></returns>
        IPagedList<Policy> GetPoliciesDetail(PolicyFilter policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false);
        IPagedList<NajmStatusHistory> GetNajmStatusHistories(int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Policy> GetExceededMaxTriesPolicies(int maxTries, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Policy> GetPoliciesWithFileDownloadFailureStatus(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get policies that are on ending status from najm.(not submitted or rejected.)
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size</param>
        /// <returns></returns>
        IPagedList<Policy> GetNajmPendingPolicies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false);
        /// <summary>
        /// Get the najm statistics including 
        /// the count of each policy based on its status
        /// </summary>
        /// <returns>An object of Najm statistics.</returns>
        NajmStatistics GetNajmStatistics();
        IPagedList<Policy> GetPoliciesWithFileGenerationFailureStatus(int pageIndex = 0, int pageSize = int.MaxValue);

       


        /// <summary>
        /// Create policy update request
        /// </summary>
        /// <param name="policyId">Policy Id</param>
        /// <param name="policyUpdateRequestTypeId">Update request type Id</param>
        /// <param name="attachmentsModel">Required attachments</param>
        /// <returns>Guid of the created update request</returns>
        string CreatePolicyUpdateRequest(int policyId, PolicyUpdateRequestType policyUpdateRequestType, List<PolicyUpdateFileDetails> attachmentsModel);

        /// <summary>
        /// Get the policy update requests.
        /// </summary>
        /// <param name="insuranceProviderId">Filter the result by the insurance provider identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="type">The policy update request type.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns></returns>
        IPagedList<PolicyUpdateRequest> GetPolicyUpdateRequests(int? insuranceProviderId = null, string userId = null, PolicyUpdateRequestType? type = null, PolicyUpdateRequestStatus? status = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// Get Policy update request by Guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Return Policy update request that match this guid</returns>
        PolicyUpdateRequest GetPolicyUpdateRequestByGuid(string guid);

        /// <summary>
        /// Change the policy update request status
        /// </summary>
        /// <param name="policyUpdateRequestId">The policy update request identifier.</param>
        /// <param name="status">The policy update request status.</param>
        /// <returns>The updated polci update request.</returns>
        PolicyUpdateRequest ChangePolicyUpdateRequestStatus(int policyUpdateRequestId, PolicyUpdateRequestStatus status);

        /// <summary>
        /// Update policy Update request status with the given status
        /// </summary>
        /// <param name="policyUpdateRequest">Entity to update</param>
        /// <param name="status">New Status</param>
        void UpdatePolicyUpdRequestStatus(PolicyUpdateRequest policyUpdateRequest, PolicyUpdateRequestStatus status);


        /// <summary>
        /// Add new policy update request payment.
        /// </summary>
        /// <param name="policyUpdatePayment">The policy update payment.</param>
        /// <returns>The updated policy update request.</returns>
        PolicyUpdateRequest AddPolicyUpdatePayment(PolicyUpdatePayment policyUpdatePayment);

        /// <summary>
        /// Update policy file document.
        /// </summary>
        /// <param name="policyId">The policy identifier.</param>
        /// <param name="policyFile">The policy file binary.</param>
        void UpdatePolicyFile(int policyId, Byte[] policyFile);

        void UpdatePolicyUpdateRequest(PolicyUpdateRequest request);

        void UpdatePolicyUpdateRequests(List<PolicyUpdateRequest> requests);

        /// <summary>
        /// Get all policy update requests that have expired waiting payment
        /// </summary>
        /// <returns></returns>
        IPagedList<PolicyUpdateRequest> GetPolicyUpdateRequestsWithExpiredPayment(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Set Policy's is refunded to given status
        /// </summary>
        /// <param name="referenceId">Checkout Reference Id</param>
        /// <param name="isRefunded">is refunded new status</param>
        Policy SetPolicyIsRefunded(string referenceId, bool isRefunded);

        PolicyOutput GeneratePolicyManually(PolicyData policyInfo);
        IPagedList<Policy> GetAllPoliciesWithFilter(IQueryable<Policy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "policyIssueDate", bool sortOrder = false);

     IQueryable<Policy> GetAllPoliciesWithFilter(SuccessPoliciesFilter policyFilter);
        Policy GetPolicyByReferenceId(string ReferenceId);

        List<AutoleaseAllInvoices> GetAutoleaseAllInvoices(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int commandTimeout, out string exception);


        /// <summary>
        /// Get invoice file 
        /// </summary>
        /// <param name="refrenceId">refrence id</param>
        /// <returns></returns>
        InvoiceFile GetInvoiceFileByRefrenceId(string refrenceId);
        bool CancelPolicy(string referenceId, bool isCancelled, string userName);
        List<FailPolicy> GetFailedPoliciesThatReachedMaxTrials();
        List<IncomeReportDBModel> GetIncomeReportWithFilter(SuccessPoliciesFilter policyFilter);
        bool RegeneratePolicy(string referenceId, out string exception);
        string GetUserEmailByPolicyNo(string policyNo, string userId);
        Policy GetPolicyByPolicyNo(string policyNo);
        PolicyFile GetPolicyFileByFileID(string fileId);
        bool UpdatePolicyFileAfterReUploadPolicy(PolicyFile policyfile);
        CheckDiscountOutput CheckDiscountByNIN(string nin);
        #region autoleasing
        List<PoliciesForClaimsListingModel> GetAllPoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int bankId, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<AutoleaseCancelledPoliciesListing> GetAutoleaseSuccessPoliciesFromDBWithFilter(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, bool export, int commandTimeout, out int totalCount, out string exception);
        AutoleasingBankStatisticsListing GetAllAutoleasingBankStatisticsFromDBWithFilter(AutoleasingBankStatisticsFilter policyFilter, int bankId, int commandTimeout, out int totalCount, out string exception);
        List<InsuranceProposalPoliciesFromDBModel> GetAllInsuranceProposalPoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int bankId, int commandTimeout, out int totalCount, out string exception);
        List<ShadowAccountOutput> GetShadowAccountDetails(ShadowAccountModel request, int bankId, out int totalCount);
        List<AutoleaseCancelledPoliciesListing> GetAllAutoleaseCancelledPoliciesFromDBWithFilter(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, bool export, int commandTimeout, out int totalCount, out string exception);
        List<InsuranseCompaniesNajmResponseTime> GetCompaniesWithNajmResponseTime();
        List<ComapanyGrade> GetComapaniesGrade();
        List<AutoleaseCancelledPoliciesListing> GetAutoleaseSuccessPoliciesForAddDriver(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception);

        #endregion

        #region Notifications
        void NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel);
        Tameenk.Core.Domain.Entities.Policy GetPolicyWithReferenceIdAndPolicyNumber(PolicyUploadNotificationModel policyUploadNotificationModel);
        void SavePolicyWithNajmStatus(Policy policyUploadNotificationModel);
        bool IsPolicyExist(string referenceId, string policyNo);
        bool CheckPolicyExistenceByReferenceIdOrPolicyNo(string referenceId, string policyNo);
        WataniyaMotorPolicyInfo CheckWataniyaInitialPolicyExistenceByReferenceId(string referenceId);
        bool UpdateWataniyaPolicyInfoCallback(WataniyaMotorPolicyInfo policyInfo, out string exception);
        #endregion
        Invoice GetInvoiceById(int invoiceId);
        RenewalDataOutput GetAllRenewalPoliciesFromDBWithFilter(RenewalFiltrationModel filterModel, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<RenewalDiscountListingModel> GetAllRenewalDiscountFromDBWithFilter(RenewalDiscountFilterModel filterModel, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception);
        bool AddNewRenewalDiscount(RenewalDiscount model, string lang, out bool isServiceException, out string exception);
        bool ManageRenewalDiscountActivation(RenewalDiscountActionModel model, out string exception);
        bool DeleteRenewalDiscount(int id, out string exception);

        bool UpdatePolicyFile(Guid fileId, string filePath, out string exception);
        PolicyDataModel GetPolicyByPolicyNoAndReferenceId(string policyNo, string referenceId, out string exception);
        List<PoliciesForCancellationListingModel> GetAllCancellationPoliciesFromDBWithFilter(PoliciesForCancellationFilterModel policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<SMSSkippedNumbersModel> GetFromSMSSkippedNumbers(SMSSkippedNumbersFilterModel filterModel, int commandTimeout, out int totalCount, out string exception, int pageIndex = 0, int pageSize = int.MaxValue);        bool AddNewRenewalStopSMSPhon(SMSSkippedNumbers model, out string exception);        bool DeleteRenewalStopSMSPhon(string phone, out string exception);
        List<CheckOutInfo> GetChekoutDetailsWithFilter(CheckOutDetailsFilter CheckoutFilter, out int totalcount, out string exception);
        PolicyFilterOutput RenewalMessageFilter(RenewalMessageFilter model, out string exception);
        List<RenewalDiscount> getDiscountType(out string exception);
        List<OwnDamagePolicyInfo> GetOwnDamagePolicyForSMS(OwnDamageFilter OwnDamageFilter, out int totalcount, out string exception);
        List<policyDetailForOD> GetpolicyForOD(OwnDamageFilter OwnDamageFilter, out string ex);
        List<BcareWithdrawalListingModel> GetBcareWithdrawalListWithFilter(BcareWithdrawalFilterModel filterModel, int commandTimeout, out string exception);
        bool InsertIntoWinnersTable(BcareWithdrawalFilterModel model, List<BcareWithdrawalListingModel> dataList, string userId, out string exception);
        BcareWithdrawalStatisticsModel GetBcareWithdrawalStatistics(out string exception);
        List<ProcessingQueueInfo> GetProcessingQueue(ProcessingQueueFilter processingQueueFilter, out int totalcount, out string exception);
        SuccessPolicystatisticsModel AdminGetDetailsToSuccessPolicies(string ReferenceId);

        bool UnCancelPolicy(string referenceId, out string exception);
        List<DriverswithPolicyDetails> GetOverFivePolicies(DriverWithOverFivePoliciesFilter Filter, out int totalcount, out string exception);
        List<PoliciesDuplicationModel> GetAllPoliciesDuplicationService(PoliciesDuplicationFilter policyFilter, int commandTimeout, out int totalCount, out string exception);
        AspNetUser GetUserBySadadNo(string sadadNo, out string exception);
    }
}

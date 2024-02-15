using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Core.Policies
{
    public interface IPolicyProcessingService
    {
        IPagedList<PolicyProcessingQueue> GetPolicyProcessingQueue(DateTime? createdFrom, DateTime? createdTo,
            bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<PolicyProcessingQueue> GetQueueForFailedFilesPolicyProcessingTask(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithOutPdfTemplate(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithPdfTemplate(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<PolicyProcessingQueue> GetFailedPolicyFromProcessingQueue(DateTime? createdFrom, DateTime? createdTo,
            bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<CheckoutDetail> GetFailedPolicyItems(IList<string> failedCheckoutReferenceIds);

        void UpdatePolicyProcessingQueue(PolicyProcessingQueue policyProcessingQueue);

        void InsertPolicyProcessingQueue(string referenceId);
        void InsertIntoPolicyProcessingQueue(string referenceId, string channel, string userID);
        void InsertPolicyProcessingQueue(string referenceId,int CompanyId , string CompanyName, string channel);
        IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithOutPdfTemplateForSpecificCompany(DateTime? createdFrom, DateTime? createdTo,
          bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue);
        List<PolicyProcessingQueue> GetQueueForPolicyProcessingTask(int maxProcessingTries, string companyKey);
      IPagedList<PolicyProcessingQueue> GetQueueFailedItemsForPolicyProcessingTask(DateTime? createdFrom, DateTime? createdTo,
      bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<PolicyProcessingQueue> GetQueuetemsForPolicyProcessingTaskWithZeroTrials(DateTime? createdFrom, DateTime? createdTo,
     bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue);
        List<PolicyProcessingQueue> GetQueuetemsForPolicyProcessingTaskWithZeroTrialsByCompanyKey(string companyKey);
        List<PolicyProcessingQueue> GetQueueFailedItemsForPolicyProcessingTaskByCompanyKey(int maxProcessingTries, string companyKey);
        List<PolicyProcessingQueue> GetFailedItemsForPolicyProcessingTaskWithLessThan3Tries(int maxProcessingTries, string companyName);
        List<PolicyProcessingQueue> GetFailedItemsForPolicyProcessingTaskWithMoreThan3Tries(int maxProcessingTries, string companyName);
        List<PolicyProcessingQueue> GetProcessingQueue(string companyKey);
        PolicyProcessingQueue GetProcessQueueItemById(int id, PolicyProcessingQueue item, string serverIP);
        bool GetAndUpdatePolicyProcessingQueue(int id, PolicyProcessingQueue policy, string serverIP,string driverNin, string vehicleId, out string exception);
        List<WalaaPolicies> GetWalaaPoliciesProcessingQueue(out string exception);
        void UpdateWalaaPolicy(WalaaPolicies policy, out string exception);
        bool InsertIntoProcessingQueue(string referenceId, int CompanyId, string CompanyName, string channel, out string exception);
        List<PolicyProcessingQueue> GetPolicyYearlyMaximumPurchaseQueue(int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue);

        void ExcuteHandleMissingTransaction();
        //List<MissingPolicyPolicyProcessingQueue> GetMissingPolicyTransactions();
        //bool UpdateMissingTransactionPolicyProcessingQueue(MissingPolicyPolicyProcessingQueue policy, out string exception);
        //MissingPoliciesOutput HandleMergeMissingPolicyTransactions(MissingPolicyPolicyProcessingQueue missingPolicy);
        void ExcuteHandleMissingCheckoutTransaction();
        void HandleMissingProcessingQueueTransactions();
        void HandleMissingPriceDetails();
        void HandleCheckoutInsuredMappingTemp();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Policy.Components
{
    public interface IPolicyContext
    {
        RemoteServerInfo remoteServerInfo { get; set; }
        PolicyOutput SubmitPolicy(string referenceId, LanguageTwoLetterIsoCode userLanguage, string serverIP,string channel);
        PdfGenerationOutput GetFailedPolicyFile(string referenceId, string serverIP, string channel);

        PolicyOutput GeneratePolicyManually(PolicyData policyInfo, string serverIP, string channel, string userName, bool isPdfServer, string domain, string remoteServerIP, string adminUsername, string adminPassword);
        bool GenerateInvoicePdf(string referenceId, out string exception);
        bool SendSmsRenewalNotifications(DateTime start, DateTime end, int notificationNo,string taskName);
        bool ResendPolicyFileByMail(byte[] file, string email, string policyNo, string referenceId, string companyKey, int langCode);
        PdfGenerationOutput ReplacePolicyFile(byte[] policyFile, string filePath, string serverIP, bool isPdfServer, string adminUsername, string adminDomain, string adminPassword);
        PdfGenerationOutput GeneratePolicyFileFromPolicyDetails(PolicyResponse policy, int iCompanyId, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log);
        void GetAndSendPolicyInformationForRoadAssistance();
        void SendMorniSMS();
        void GetAndSubmitAllFailedMorniRequests();
        bool GnerateInvoiceRecord(string referenceId, int paymentMethodId, short insuranceTypeCode, int insuranceCompanyId, out string exception);
        bool CheckQuotationStatusAndSendNotification(int insuranceTypeCode, bool isAutoLease, string method);
        bool CheckServicesStatusAndSendNotification(string method);
        List<AutoleasingPolicyReportInfoModel> GetAutoleasingPolicyReport(AutoleasingPolicyReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception);
       // PdfGenerationOutput GetWalaaFailedPolicyFile(CheckoutDetail checkoutDetails, string referenceId, string url);
        List<PreviousPolicyDetails> GetPolicyInfoForRenewal();
        void HandlePolicyRenewalInfo(PreviousPolicyDetails policy);
        NajmResponseOutput GetNajmResponseTimeForConnectWithPolicy(NajmResponseTimeFilter NajmPolicyFilter, int pageIndex, int pageSize, int commandTimeout, out string exception);
        PolicyNotificationOutput GetpolicyNotificationLog(PolicyNotificationFilter NajmNotificationPolicyFilter, int pageIndex, int pageSize, int commandTimeout, out string exception);
        List<VehicleSuccessPoliciesForAddBenefitsListing> GetVehicleSuccessPoliciesWithFilterForAddBenefits(VehicleSuccessPoliciesFilterForAddBenefits policyFilter, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<VehicleSuccessPoliciesForAddDriverListing> GetVehicleSuccessPoliciesWithFilterForAddDriver(VehicleSuccessPoliciesFilterForAddDriver policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<PoliciesForClaimsListingModel> GetAllVehiclePoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception);
        VehicleClaimNotificationOutput SendVehicleClaimNotificationRequest(ClaimNotificationRequest claimNotification, string userId);
        VehicleClaimRegistrationOutput SendVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, int companyId, string userId);
        CancellPolicyOutput SendCancellationRequest(CancelVechilePolicyRequestDto cancelRequest, string userid, string username);
        VehicleDiscounts LinkVechWithDiscountCode(VehicleDiscounts VehicleDiscountmodel, out string exception);
        SMSOutput SendRenewalSmsMsg(RenewalSendSMS model, out string exception);
        SMSOutput SendOwnDamageSmsMsg(OwnDamageQueue model, out string exception, string lang);
        void ExecutePolicy(string companyName);
        PolicyOutput HandlePolicyYearlyMaximumPurchaseTask(PolicyProcessingQueue queueItem);
        CompanyAvgResponseTimeOutput HandleInsuranceCompanyOrderTask();
        bool GetPurchaseStatisticsTask();
        bool GetPaymentMethodsStatisticsInfo();
        bool RehitSmsRenewalNotifications(int notificationNo);
        void GenerateMissingInvoicePdf();

        bool SendSmsRenewalNotificationsNew(DateTime start, DateTime end, int notificationNo, string taskName);

    }
}

using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.BlockNins;

namespace Tameenk.Services.Core.Excel
{
    public interface IExcelService
    {

        byte[] GenerateServiceRequest(List<ServiceRequestLog> request, string name);
        byte[] GenerateServiceRequest(List<IServiceRequestLog> request, string name);
        byte[] GenerateExcelPoliciesStatisticsInfo(List<PolicyStatisticsDataModel> policies);
        byte[] GenericGenerationExcelSheet<T>(List<T> data, string nameOfExcelSheet, DateTime? dateTime = null);


        /// <summary>
        /// Generate Excel sheet to Fail Policies
        /// </summary>
        /// <param name="failedPolicies"></param>
        /// <returns></returns>
        byte[] GenerateExcelFailPolicies(List<FailPolicy> failedPolicies);

        /// <summary>
        /// Generate Excel sheet to success policies
        /// </summary>
        /// <param name="policies"></param>
        /// <returns></returns>
        byte[] GenerateExcelSuccessPolicies(List<PolicyListingModel> policies);

        byte[] GenerateExcelSamaReport(List<SamaReport> sama);

        /// <summary>
        /// Generate Excel Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="nameOfExcelSheet"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        byte[] GenerateExcelSheet<T>(List<T> data, string nameOfExcelSheet, DateTime? dateTime = null);
        byte[] GenerateFailPoliciesExcel(List<PolicyListingModel> failedPolicies);
        byte[] ExportInquiryRequestLog(List<InquiryRequestLog> request, string name);
        byte[] ExportTicketLog(List<TicketLogModel> logs, string name);
        byte[] ExportVehicleMakers(List<VehicleMakerModel> makers, string name);
        byte[] ExportVehicleMakerModels(List<VehicleMakerModelsModel> makers, string name);
        byte[] ExportYakeenCityCenters(List<YakeenCityCenterModel> makers, string name);
        byte[] GenerateOffersExcel(List<DeservingDiscount> policies);
        byte[] GenerateExcelSuccessPoliciesInfo(List<SuccessPoliciesInfoListingModel> policies, string lang);

        byte[] GenerateSmsServiceLogExcel(List<SMSLog> smss);
        byte[] GeneratePolicyInformationForRoadAssistance(List<PolicyInformation> policies);

        byte[] GenerateSamaReportPoliciesExcel(List<SamaReportPoliciesListingModel> policies, string lang);
        byte[] ExportOccupations(List<Occupation> occupations, string name);
        //byte[] ExportAllTicket(List<TicketModel> request, string name);
        byte[] ExportAllTicket(List<ExcelTiketModel> request, string name);

        byte[] GenerateExcelAutoleasingQuotationReport(List<AutoleasingQuotationReportInfoModel> quotationReport);
        byte[] GenerateExcelAutoleasingPolicyReport(List<AutoleasingPolicyReportInfoModel> policyReport);
        byte[] GenerateExcelCorporatePolicies(List<CorporatePolicyModel> corporatePolicies);
        byte[] GenerateSamaStatisticsReportExcel(SamaStatisticsCountReport report);
        byte[] ExportRenewalData(List<RenewalDataModel> data, string name);
        byte[] ExportCheckoutRequestLog(List<CheckoutRequestLog> request, string name);
        byte[] GenerateExcelGetAutoleasingWalletReport(List<AutoleasingWalletReportModel> walletReport);
        byte[] GenerateExcelCommissionAndFees(List<CommissionAndFeesModel> commissions);
        byte[] GeneratePoliciesDetailsExcel(List<CheckOutInfo> CheckOutDetails);
        byte[] GetNajmResponseTimeForConnectWithPolicyExcel(List<NajmResponseTimeModel> NajmResponseTimeModel);
        byte[] GenerateExcelPendingPolicies(List<ProcessingQueueInfo> pendingPolicies);
        byte[] GenearetOldQuotation(List<OldQuotationDetails> oldQuotationDetails);
        byte[] GenearetBlockedUsers(List<BlockedUsersDTO> blockedUsersDTO);
        byte[] GenerateServiceRequestForTabbby(List<TabbyWebHookServiceRequestLogModel> request, string name);
        byte[] GenerateAppnotificationsServiceLogExcel(List<FirebaseNotificationLog> request, string name);
        byte[] GenerateExcelDriverswithPolicyDetails(List<DriverswithPolicyDetails> PolicyDetails);
        byte[] GenerateExcelRepeatedPolicies(List<PoliciesDuplicationModel> policies, string lang);
    }
}

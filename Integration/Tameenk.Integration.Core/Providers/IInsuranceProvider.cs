using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;

namespace Tameenk.Integration.Core.Providers
{
    /// <summary>
    /// The interface for insurance company that provide quotation and policy generation
    /// </summary>
    public interface IInsuranceProvider
    {
        /// <summary>
        /// Generate insurance policy from 3rd-party service provider
        /// </summary>
        /// <param name="policy">The policy request details required for policy genration</param>
        /// <returns>A policy response details</returns>
        PolicyResponse GetPolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        FileServiceOutput PolicySchedule(string policyNo, string referenceId);
        /// <summary>
        /// Generate quotations from 3rd-party service provider.
        /// </summary>
        /// <param name="quotation">The quotation request details required for quoting.</param>
        /// <returns>Quotation response message.</returns>
        QuotationServiceResponse GetQuotation(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);

        /// <summary>
        /// Validate quotation before checkout process
        /// </summary>
        /// <param name="quotationRequest"></param>
        /// <returns></returns>
        bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors);
        //bool ValidateNumberOfAccidentBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors);


        ComprehensiveImagesOutput UploadComprehansiveImages(ComprehensiveImagesRequest request, ServiceRequestLog log);

        ServiceOutput GetTawuniyaQuotation(QuotationServiceRequest quotationServiceRequest, Product product, string proposalNumber, ServiceRequestLog log, List<string> selectedbenefits);        ServiceOutput GetQuotationServiceResponse(QuotationServiceRequest quotation, Product product, string proposalNumber, ServiceRequestLog predefinedLogInfo, List<string> selectedbenefits);

        ClaimRegistrationServiceOutput SendClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo);

        ClaimNotificationServiceOutput SendClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo);
        CancelPolicyOutput SubmitAutoleasingCancelPolicyRequest(CancelPolicyRequestDto policy, int bankId, ServiceRequestLog predefinedLogInfo);
        CancelPolicyOutput SubmitCancelPolicyRequest(CancelPolicyRequestDto policy, ServiceRequestLog predefinedLogInfo);

        QuotationServiceResponse GetQuotationAutoleasing(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo);

        PolicyResponse GetAutoleasingPolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        ServiceOutput GetTawuniyaAutoleasingQuotation(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo);
        ServiceOutput GetWataniyaAutoleasingDraftpolicy(QuotationServiceRequest quotation, Product selectedProduct, ServiceRequestLog predefinedLogInfo);
        ServiceOutput GetWataniyaMotorDraftpolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo);
        AddDriverResponse AddDriver(AddDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        PurchaseDriverResponse PurchaseDriver(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        AddBenefitResponse AutoleasingAddBenefit(AddBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        PurchaseBenefitResponse AutoleasingPurchaseBenefit(PurchaseBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);

        CustomCardServiceOutput UpdateCustomCard(UpdateCustomCardRequest request, ServiceRequestLog predefinedLogInfo);
        CustomCardServiceOutput AutoleaseUpdateCustomCard(UpdateCustomCardRequest request, ServiceRequestLog predefinedLogInfo);
        AddBenefitResponse AddVechileBenefit(AddVechileBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false );
        PurchaseBenefitResponse PurchaseVechileBenefit(PurchaseBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        ClaimRegistrationServiceOutput SendVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo);
        ClaimRegistrationServiceOutput SubmitVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo);
        ClaimNotificationServiceOutput SendVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo);
        CancelPolicyOutput SubmitVehicleCancelPolicyRequest(CancelVechilePolicyRequestDto policy, ServiceRequestLog predefinedLogInfo);
        ClaimNotificationServiceOutput SubmitVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo);
        ClaimNotificationServiceOutput GetVehicleClaimNotificationResponseObject(object response, ClaimNotificationRequest claim);
        AddDriverResponse AddVechileDriver(AddDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        PurchaseDriverResponse PurchaseVechileDriver(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest = false);
        ServiceOutput WataniyaNajmStatus(string policyNo, string referenceId, string customId, string sequenceNo);
    }
}

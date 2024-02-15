using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.BlockNins;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Quotations;

namespace Tameenk.Services.Core.Quotations
{
    public interface IQuotationService
    {


        /// <summary>
        /// Get quotation Request by external id 
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        QuotationRequest GetQuotationRequest(string externalId);

        /// <summary>
        /// get number of offers fro specific user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        int GetUserOffersCount(string id);

        /// <summary>
        /// Get quotation response by reference identifier.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        QuotationResponse GetQuotationResponseByReferenceId(string referenceId);
        QuotationResponseDBModel GetQuotationResponseByReferenceIdDB(string ReferenceId, string ProductId);

        /// <summary>
        /// Set quotation request user.
        /// </summary>
        /// <param name="qtRqstExtrnlId">The quotation request externa identifier.</param>
        /// <param name="userId">The user identifier.</param>
        QuotationRequest SetQuotationRequestUser(string qtRqstExtrnlId, string userId);

        QuotationRequest GetQuotationRequestDrivers(string qtRqstExtrnlId);

        /// <summary>
        /// Pormotion From TPL To Comperhensive for specific external Id
        /// </summary>
        /// <param name="qtRqstExtrnlId"></param>
        /// <param name="vehicleAgencyRepair"></param>
        /// <param name="deductibleValue"></param>
        /// <param name="productTypeId">product type id</param>
        /// <returns></returns>
        Product GetLowestProductByPrice(string qtRqstExtrnlId, bool vehicleAgencyRepair = false, int? deductibleValue = 2000, int productTypeId = 2);


        /// <summary>
        /// Check if quotation request is still valid
        /// </summary>
        /// <param name="quotationExternalId">Quotation request external id</param>
        /// <returns></returns>
        bool IsQuotationStillValid(string quotationExternalId);

        string GetNCDFreeYearsDescription(int code, string lang);


        /// <summary>
        /// Update give quotation to the database
        /// </summary>
        /// <param name="quotationRequest">Quotation request to be updated</param>
        /// <returns>Quotation request after being updated in the db</returns>
        QuotationRequest UpdateQuotationRequest(QuotationRequest quotationRequest);
        bool ValidateUniqueIBAN(string referenceId, string strIBAN, Guid mainDriverId);

        QuotationServiceRequest GetQuotationRequestData(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue);


        /// <summary>
        /// Invalidate all quotation responses for given user
        /// </summary>
        /// <param name="userId"></param>
        void InvalidateUserQuotationResponses(string userId);
        Guid GetDriverIdByReferenceId(string referenceId);

        QuotationDetailsModel GetQuotationDetailsByReferenceId(string referenceId);
        QuotationResponseCache GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        bool InsertIntoQuotationResponseCache(QuotationResponseCache quotationResponseCache, out string exception);
        bool DeleteFromQuotationResponseCache(string externalId, out string exception);
        bool GetQuotationResponseCacheAndDelete(int insuranceCompanyId, int insuranceTypeCode, string externalId, out string exception);
        CheckoutDetail GetQuotationRequestByExternal(string externalId,out string exception);
        QuotationRequest GetQuotationRequestDriversByExternalAndRef(string qtRqstExtrnlId, string referenceId);
        int UpdateQuotationResponseToBeCheckedout(long quotationResponseId, Guid productid);
        bool IsQuotationResponseExist(string referenceId);
        QuotationResponse GetInvoiceDataFromQuotationResponseByReferenceId(string referenceId);
        QuotationResponse GetQuotationByReference(string referenceId);
        bool GetAutoleaseQuotationResponseCacheAndDelete(int insuranceCompanyId, string externalId, string initialExternalId, out string exception);
        QuotationResponseDBModel GetAutoleasingQuotationResponseByReferenceIdDB(string ReferenceId, string ProductId);
        QuotationRequest GetQuotationRequestData(string externalId);
        InsuranceProposalInfoModel GetInsuranceProposalDetails(string externalId, out string exception);
        QuotationInfoModel GetBulkQuotationsDetails(string externalId);
        QuotationInfoModel GetQuotationsDetails(string externalId, bool agencyRepair, int deductible, out string exception);
        AutoleasingQuotationResponseCache GetFromAutoleasingQuotationResponseCache(int insuranceCompanyId, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId);
        QuotationResponse GetQuotationResponseByExternalAndCompanyId(string external, int companyId, bool agencyRepair, int deductible);
        bool InsertIntoAutoleasingQuotationResponseCache(AutoleasingQuotationResponseCache autoleasingQuotationResponseCache, out string exception);
        WataniyaDraftPolicy GetWataniyaDraftPolicyInitialData(string referenceId);
        QuotationResponse GetWataniyaQuotationResponseByReferenceId(string referenceId);
        LicenseType GetWataniyaDriverLicenseType(string licenseType);

        void InsertOrupdateWataniyaMotorPolicyInfo(WataniyaMotorPolicyInfo initialPolicyInfo, out string exception);
        WataniyaMotorPolicyInfo GetWataniyaMotorPolicyInfoByReference(string reference);

        void InsertOrupdateWataniyaAutoleasePolicyInfo(WataniyaDraftPolicy initialPolicyInfo, out string exception);
        WataniyaDraftPolicy GetWataniyaAutoleasePolicyInfoByReference(string reference);

        QuotationRequest GetQuotationRequestVehicleInfo(string qtRqstExtrnlId);

        NCDFreeYear GetNCDFreeYearsInfo(int code);
        int GetcountFromQuotationSharesByExternalId(string externalId, string shareType);
        int RemoveUserIdFromQuotationRequest(string userId, out string exception);

        QuotationRequest GetQuotationRequesByPreviousReferenceId(string previousReferenceId);
        QuotationInfoModel GetAutoleasingRenewalQuotationsDetailsForHistorySettings(string externalId, bool agencyRepair, int deductible, out string exception);
        QuotationRequest GetQuotaionDriversForODByExternailId(string externalId);
        QuotationRequest GetQuotationRequestDriversInfo(string qtRqstExtrnlId);
        QuotationRequestDriverModel GetQuotationRequestAndDriversInfo(string ReferenceId, string externalId, out string exception);
        int UpdateQuotationRequestWithNOA(int quotationRequestId, int insuredId, int noOfAccident, string najmResponse);
        int ExpireQuotationResponses(int quotationRequestId, int insuredIde);
        QuotationRequest GetQuotationRequestFromCachingWithExternalId(string externalId);
        List<OldQuotationDetails> GetOldQuotationDetails(OldQuotationDetailsFilter oldQuotationDetailsFilter, out int totalcount, out string exception);
        QuotationResponse GetODResponseDetailsByExternalId(string externalId, out string exception);
        QuoteRequestVehicleInfo GetQuotationRequestAndVehicleInfo(string externalId);

        bool AddBlockedUser(AddBlockedNinModel addBlockedUserModel, string userName, out string exception);

        List<BlockedUsersDTO> GetQuotationBlockFilterService(BlockedNinFilter filter, out int totalcount, out string exception);
        bool RemoveBlockNin(int Id, out string nin, out string exception);
        AutoleasingQuotationFormSettingModel GetFristYearPolicyDetails(string SequenceNumber);
        QuotationRequest GetQuotationRequestByExternal(string externalId);
        RenewalPolicesData GetQuotationRequestByExternalNew(string externalId, out string exception);

    }
}

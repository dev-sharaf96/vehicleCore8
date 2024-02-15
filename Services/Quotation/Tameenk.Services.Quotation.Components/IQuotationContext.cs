using System;
using System.Collections.Generic;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.Quotation.Components
{
    public interface IQuotationContext
    {
        QuotationOutput GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false);

        QuotationOutput GetQuotation(InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false, string policyNo = null, string policyExpiryDate = null, bool OdQuotation = false);

        QuotationServiceRequest GetQuotationRequest(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId);

        int GetUserOffersCount(string id);

        IPagedList<QuotationRequest> GetQuotationRequestsByUserId(string userId, int pageIndx = 0, int pageSize = int.MaxValue);

        string ExportAutomatedTestResultToExcel(bool Quotation);

        List<Benefit> GetBenefits();
        QuotationOutput GetTawuniyaQuotation(int quotationRequestId, string referenceId, Guid currentProduct, string qtRqstExtrnlId, InsuranceCompany insuranceCompanyProvider, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false);
        QuotationOutput GetNOAQuotationRequest(string qtRqstExtrnlId, int insuranceCompanyID, string userId, string channel, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId);
        QuotationsFormOutput GetBulkQuotaionInsuranceProposalDetailsByExternalId(string externalId, string lang, out string exception);
        QuotationsFormOutput GetQuotaionInsuranceProposalDetailsByExternalId(QuotationFormWithSelectedBenfitsViewModel model, out string exception);
        QuotationsFormOutput GetQuotaionsFormDetailsByExternalId(string externalId, string lang, out string exception);
        List<QuotationResponseModel> GetAllQuotationsByExternalId(string qtRqstExtrnlId, Channel channel, Guid userId, string userName, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue);
        QuotationsFormOutput GetQuotaionsInfoByExternalId(QuotationRequestLog predefinedLog, QuotationFormWithSelectedBenfitsViewModel model, out string exception);
        List<IntialQuotationOutput> GetIntialQuotations(string nin, out string exception);
        Output<RepairMethodModel> GetRepairMethodFromBankSettings(int bankId, string externalId, int vehicleValue);
        Output<bool> UpdateQuotationRepairMethodSettingsHistory(string externalId, string currentUserId, bool isAgency);
        QuotationOutput GetQuoteAutoleasing(int insuranceCompanyId, string qtRqstExtrnlId, Channel channel, Guid userId, string userName, int bankId, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null);
        QuotationServiceRequest GetAutoleasingQuotationRequest(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId);
        QuotationOutput GetTawuniyaAutoleasingQuotation(int quotationRequestId, string referenceId, Guid currentProduct, string qtRqstExtrnlId, InsuranceCompany insuranceCompanyProvider, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false);
        QuotationOutput GetWataniyaQuotation(int quotationRequestId, string referenceId, Guid currentProduct, string qtRqstExtrnlId, InsuranceCompany insuranceCompanyProvider, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false);
        List<AutoleasingQuotationReportInfoModel> GetAutoleasingQuotaionReport(AutoleasingQuotationReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception);
        ShareQuotationOutput ShareQuotation(string phone, string email, string externalId, string userId, QuotationShareTypes ShareType, string url, string channel,string lang);
        AddVehicleBenefitOutput AddVehicleBenefit(AddVehicleBenefitModel model, string userId, string userName);
        QuotationsFormOutput GetRenewalQuotaionsInfoByExternalId(QuotationRequestLog predefinedLog, QuotationFormWithSelectedBenfitsViewModel model, out string exception);

        QuotationResponseModelForProfile GetVehicleInfo(string qtRqstExtrnlId, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool saveQuotResponseWithOldDate = false, string lang = "ar");
    }
}

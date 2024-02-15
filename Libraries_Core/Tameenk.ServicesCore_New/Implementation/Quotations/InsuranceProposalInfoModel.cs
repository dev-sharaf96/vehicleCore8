using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class InsuranceProposalInfoModel
    {
        #region Insured Data

        public string?InsuredNameAr { get; set; }
        public string?InsuredNameEn { get; set; }
        public string?InsuredNationalId { get; set; }

        #endregion

        #region Main Driver Data

        public string?MainDriverNameEn { get; set; }
        public string?MainDriverNameAr { get; set; }
        public string?DriverNationalId { get; set; }
        public string?MobileNumber { get; set; }
        public string?MainDriverAddressEn { get; set; }
        public string?MainDriverAddressAr { get; set; }

        #endregion

        #region Vehicle Data

        public string?CarPlateText1 { get; set; }
        public string?CarPlateText2 { get; set; }
        public string?CarPlateText3 { get; set; }
        public short? CarPlateNumber { get; set; }
        public int? VehicleValue { get; set; }
        public string?VehicleMaker { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string?VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public string?VehicleId { get; set; }
        public string?ChassisNumber { get; set; }
        public string?MajorColor { get; set; }
        public byte VehicleBodyCode { get; set; }
        public string?BodyTypeEn { get; set; }
        public string?BodyTypeAr { get; set; }
        public string?VehicleLicenseExpiryDate { get; set; }
        public short? VehicleYear { get; set; }

        #endregion

        #region Additional Drivers

        public string?AdditionalDriverOneNameEn { get; set; }
        public string?AdditionalDriverOneNameAr { get; set; }

        public string?AdditionalDriverTwoNameEn { get; set; }
        public string?AdditionalDriverTwoNameAr { get; set; }

        #endregion

        #region Quotations Responses

        public List<InsuranceProposalQuotationResponsesInfoModel> QuotationResponses { get; set; }

        #endregion

        #region Bank Data

        public BankData Bank { get; set; }

        #endregion

        #region Depreciation Settings

        public DepreciationSettingHistory DepreciationSettingHistory { get; set; }
        public DepreciationSetting DepreciationSetting { get; set; }

        #endregion

        #region Repair Method Settings

        public RepairMethodeSettingHistory RepairMethodeSettingHistory { get; set; }
        public RepairMethodeSetting RepairMethodeSetting { get; set; }

        #endregion

        public int? ContractDuration { get; set; }
    }

    public class InsuranceProposalQuotationResponsesInfoModel
    {
        public long Id { get; set; }
        public string? ReferenceId { get; set; }
        public string? VehicleAgencyRepair { get; set; }
        public int? Deductible { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }

        public List<QuotationProductInfoModel>? Products { get; set; }
    }

    public class BankData
    {
        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? IBAN { get; set; }
        public string? NationalAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class DepreciationSettingHistory
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string? ExternalId { get; set; }
        public int MakerCode { get; set; }
        public int ModelCode { get; set; }
        public string? MakerName { get; set; }
        public string? ModelName { get; set; }
        public decimal Percentage { get; set; }
        public bool IsDynamic { get; set; }
        public decimal FirstYear { get; set; }
        public decimal SecondYear { get; set; }
        public decimal ThirdYear { get; set; }
        public decimal FourthYear { get; set; }
        public decimal FifthYear { get; set; }
        public string? AnnualDepreciationPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class DepreciationSetting
    {
        public int? Id { get; set; }
        public int? BankId { get; set; }
        public string? ExternalId { get; set; }
        public int? MakerCode { get; set; }
        public int? ModelCode { get; set; }
        public string? MakerName { get; set; }
        public string? ModelName { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsDynamic { get; set; }
        public decimal? FirstYear { get; set; }
        public decimal? SecondYear { get; set; }
        public decimal? ThirdYear { get; set; }
        public decimal? FourthYear { get; set; }
        public decimal? FifthYear { get; set; }
        public string? AnnualDepreciationPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class RepairMethodeSettingHistory
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string? ExternalId { get; set; }
        public string? FirstYear { get; set; }
        public string? SecondYear { get; set; }
        public string? ThirdYear { get; set; }
        public string? FourthYear { get; set; }
        public string? FifthYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class RepairMethodeSetting
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string? FirstYear { get; set; }
        public string? SecondYear { get; set; }
        public string? ThirdYear { get; set; }
        public string? FourthYear { get; set; }
        public string? FifthYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MinimumPremiumSettingHistory
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string?ExternalId { get; set; }
        public decimal FirstYear { get; set; }
        public decimal SecondYear { get; set; }
        public decimal ThirdYear { get; set; }
        public decimal FourthYear { get; set; }
        public decimal FifthYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class MinimumPremiumSetting
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public decimal FirstYear { get; set; }
        public decimal SecondYear { get; set; }
        public decimal ThirdYear { get; set; }
        public decimal FourthYear { get; set; }
        public decimal FifthYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

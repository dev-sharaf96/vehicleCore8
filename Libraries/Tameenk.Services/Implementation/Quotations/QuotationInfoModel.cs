using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class QuotationInfoModel
    {
        #region Insured Data

        public string InsuredNameAr { get; set; }
        public string InsuredNameEn { get; set; }
        public string InsuredNationalId { get; set; }

        #endregion

        #region Main Driver Data
    
        public string MainDriverNameEn { get; set; }
        public string MainDriverNameAr { get; set; }
        public string DriverNationalId { get; set; }
        public string MainDriverAddressEn { get; set; }
        public string MainDriverAddressAr { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public int? VehicleValue { get; set; }
        public string VehicleMaker { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public string VehicleId { get; set; }
        public short? VehicleYear { get; set; }

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

        #region Minimum Premium Setting

        public MinimumPremiumSettingHistory MinimumPremiumSettingHistory { get; set; }
        public MinimumPremiumSetting MinimumPremiumSetting { get; set; }

        #endregion

        public int? ContractDuration { get; set; }

        public List<QuotationProductInfoModel> Products { get; set; }
    }

    public class QuotationProductInfoModel
    {
        public long? QuotationResponseId { get; set; }
        public string CompanyKey { get; set; }
        public int InsuranceCompanyID { get; set; }
        public Guid ProductID { get; set; }
        public string ProductDescAr { get; set; }
        public string ProductDescEn { get; set; }
        public decimal ProductPrice { get; set; }
        public int? DeductableValue { get; set; }
        public string VehicleRepairType { get; set; }
        public string ProductName { get; set; }
        public string ReferenceId { get; set; }
        public decimal? InsurancePercentage { get; set; }
        public decimal? ShadowAmount { get; set; }

        public List<ProductPriceDetailsInfoModel> PriceDetails { get; set; }
        public List<ProductBenfitDetailsInfoModel> Benfits { get; set; }
    }

    public class ProductPriceDetailsInfoModel
    {
        public Guid DetailId { get; set; }
        public Guid ProductID { get; set; }
        public byte PriceTypeCode { get; set; }
        public decimal PriceValue { get; set; }
        public decimal? PercentageValue { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime CreateDateTime { get; set; }
        public byte Code { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
    }

    public class ProductBenfitDetailsInfoModel
    {
        public long Id { get; set; }
        public Guid? ProductId { get; set; }
        public short? BenefitId { get; set; }
        public bool? IsSelected { get; set; }
        public decimal? BenefitPrice { get; set; }
        public string BenefitExternalId { get; set; }
        public bool IsReadOnly { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
    }
}

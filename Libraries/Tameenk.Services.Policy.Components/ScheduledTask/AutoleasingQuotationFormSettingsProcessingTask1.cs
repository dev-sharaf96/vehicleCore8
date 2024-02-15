using System;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using System.Collections.Generic;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Services.Core.Quotations;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Tameenk.Services.Policy.Components
{
    public class AutoleasingQuotationFormSettingsProcessingTask1 : ITask
    {
        #region Fields
        private readonly IRepository<AutoleasingQuotationFormSettings> _autoleasingQuotationFormSettingsRepository;
        private readonly IQuotationService _quotationService;
        private readonly IRepository<AutoleasingAgencyRepairHistory> _autoleasingRepairMethodRepository;        private readonly IRepository<AutoleasingDepreciationSettingHistory> _autoleasingDepreciationSettingHistoryRepository;        private readonly IRepository<AutoleasingMinimumPremiumHistory> _autoleasingMinimumPremiumHistoryRepository;        #endregion

        #region Ctor
        public AutoleasingQuotationFormSettingsProcessingTask1(
            IRepository<AutoleasingQuotationFormSettings> autoleasingQuotationFormSettingsRepository,
            IQuotationService quotationService,
            IRepository<AutoleasingAgencyRepairHistory> autoleasingRepairMethodRepository,
            IRepository<AutoleasingDepreciationSettingHistory> autoleasingDepreciationSettingHistoryRepository,
            IRepository<AutoleasingMinimumPremiumHistory> autoleasingMinimumPremiumHistoryRepository)
        {
            _autoleasingQuotationFormSettingsRepository = autoleasingQuotationFormSettingsRepository;
            _quotationService = quotationService;
            _autoleasingRepairMethodRepository = autoleasingRepairMethodRepository;
            _autoleasingDepreciationSettingHistoryRepository = autoleasingDepreciationSettingHistoryRepository;
            _autoleasingMinimumPremiumHistoryRepository = autoleasingMinimumPremiumHistoryRepository;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            var data = GetFromAutoleasingQuotationFormSettings();
            if (data == null)
                return;

            foreach (var item in data)
            {
                string exception = string.Empty;
                var deductible = item.DeductableValue.HasValue ? item.DeductableValue.Value : 2000;
                QuotationInfoModel quotationInfo = _quotationService.GetAutoleasingRenewalQuotationsDetailsForHistorySettings(item.ExternalId, item.AgencyRepair, deductible, out exception);
                if (!string.IsNullOrEmpty(exception) || quotationInfo == null
                    || (quotationInfo.DepreciationSettingHistory == null && quotationInfo.DepreciationSetting == null)
                    || (quotationInfo.RepairMethodeSetting == null && quotationInfo.RepairMethodeSettingHistory == null))
                {
                    if (!string.IsNullOrEmpty(exception))
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\_quotationService.GetQuotationsDetails" + item.ExternalId + ".txt", " Exception is:" + exception.ToString());
                    return;
                }

                var result = HandleAutoleasingQuotationFormSettings(quotationInfo, item.ExternalId, item.DeductableValue.Value);
                if (result)
                    item.IsDone = true;
                else
                {
                    item.IsLocked = false;
                    item.IsDone = false;
                }

                UpdateAutoleasingQuotationFormSettings(item);
            }
        }

        private bool HandleAutoleasingQuotationFormSettings(QuotationInfoModel quotationInfo, string externalId, int deductableValue)
        {
            string exception = string.Empty;
            try
            {
                AutoleasingQuotationFormSettings autoleasingQuotationFormSettings = new AutoleasingQuotationFormSettings();
                autoleasingQuotationFormSettings.VehicleId = quotationInfo.VehicleId;
                autoleasingQuotationFormSettings.ExternalId = externalId;
                autoleasingQuotationFormSettings.BankId = quotationInfo.Bank.Id;
                autoleasingQuotationFormSettings.CreateDate = DateTime.Now;

                #region Deprecation Data

                var contractDuration = quotationInfo.ContractDuration;
                AutoleasingAgencyRepairHistory bankRepairMethodSettingsHistory = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id && r.ExternalId == externalId).FirstOrDefault();
                if (bankRepairMethodSettingsHistory == null)
                {
                    var _bankRepairMethodSettings = quotationInfo.RepairMethodeSetting;
                    bankRepairMethodSettingsHistory = new AutoleasingAgencyRepairHistory();
                    bankRepairMethodSettingsHistory.BankId = _bankRepairMethodSettings.BankId;
                    bankRepairMethodSettingsHistory.FirstYear = _bankRepairMethodSettings.FirstYear;
                    bankRepairMethodSettingsHistory.SecondYear = _bankRepairMethodSettings.SecondYear;
                    bankRepairMethodSettingsHistory.ThirdYear = _bankRepairMethodSettings.ThirdYear;
                    bankRepairMethodSettingsHistory.FourthYear = _bankRepairMethodSettings.FourthYear;
                    bankRepairMethodSettingsHistory.FifthYear = _bankRepairMethodSettings.FifthYear;
                }

                AutoleasingDepreciationSettingHistory depreciationSettingHistory = _autoleasingDepreciationSettingHistoryRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id && r.ExternalId == externalId).FirstOrDefault();
                if (depreciationSettingHistory == null)
                {
                    var deprecationSetting = quotationInfo.DepreciationSetting;
                    depreciationSettingHistory = new AutoleasingDepreciationSettingHistory();
                    depreciationSettingHistory.BankId = quotationInfo.Bank.Id;
                    depreciationSettingHistory.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSettingHistory.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSettingHistory.MakerName = deprecationSetting.MakerName;
                    depreciationSettingHistory.ModelName = deprecationSetting.ModelName;
                    depreciationSettingHistory.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSettingHistory.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSettingHistory.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSettingHistory.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSettingHistory.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSettingHistory.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSettingHistory.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSettingHistory.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
                }

                AutoleasingMinimumPremiumHistory bankMinimumPremiumSettingsHistory = _autoleasingMinimumPremiumHistoryRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id && r.ExternalId == externalId).FirstOrDefault();
                if (bankMinimumPremiumSettingsHistory == null)
                {
                    var minimumPremiumSetting = quotationInfo.MinimumPremiumSetting;
                    bankMinimumPremiumSettingsHistory = new AutoleasingMinimumPremiumHistory();
                    bankMinimumPremiumSettingsHistory.BankId = quotationInfo.Bank.Id;
                    bankMinimumPremiumSettingsHistory.FirstYear = minimumPremiumSetting.FirstYear;
                    bankMinimumPremiumSettingsHistory.SecondYear = minimumPremiumSetting.SecondYear;
                    bankMinimumPremiumSettingsHistory.ThirdYear = minimumPremiumSetting.ThirdYear;
                    bankMinimumPremiumSettingsHistory.FourthYear = minimumPremiumSetting.FourthYear;
                    bankMinimumPremiumSettingsHistory.FifthYear = minimumPremiumSetting.FifthYear;
                }

                autoleasingQuotationFormSettings.RepairMethod = bankRepairMethodSettingsHistory.FirstYear;

                List<string> annualPercentages = new List<string>();
                if (contractDuration >= 1)
                {
                    if (!depreciationSettingHistory.FirstYear.Equals(null))
                        annualPercentages.Add("0 %");
                }

                if (contractDuration >= 2)
                {
                    if (!depreciationSettingHistory.SecondYear.Equals(null))
                        annualPercentages.Add(depreciationSettingHistory.SecondYear.ToString().Split('.')[0] + " %");
                }

                if (contractDuration >= 3)
                {
                    if (!depreciationSettingHistory.ThirdYear.Equals(null))
                        annualPercentages.Add(depreciationSettingHistory.ThirdYear.ToString().Split('.')[0] + " %");
                }

                if (contractDuration >= 4)
                {
                    if (!depreciationSettingHistory.FourthYear.Equals(null))
                        annualPercentages.Add(depreciationSettingHistory.FourthYear.ToString().Split('.')[0] + " %");
                }

                if (contractDuration >= 5)
                {
                    if (!depreciationSettingHistory.FifthYear.Equals(null))
                        annualPercentages.Add(depreciationSettingHistory.FifthYear.ToString().Split('.')[0] + " %");
                }

                autoleasingQuotationFormSettings.Depreciation = (depreciationSettingHistory.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSettingHistory.Percentage.ToString().Split('.')[0] + " %";

                var vehicleValue = quotationInfo.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSettingHistory.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSettingHistory.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSettingHistory.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSettingHistory.SecondYear / 100);

                        if (depreciationSettingHistory.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSettingHistory.ThirdYear / 100);

                        if (depreciationSettingHistory.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSettingHistory.FourthYear / 100);

                        if (depreciationSettingHistory.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSettingHistory.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSettingHistory.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSettingHistory.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSettingHistory.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSettingHistory.Percentage / 100);
                    }
                }
                else if (depreciationSettingHistory.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSettingHistory.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSettingHistory.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSettingHistory.SecondYear / 100);

                        if (depreciationSettingHistory.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSettingHistory.ThirdYear / 100);

                        if (depreciationSettingHistory.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSettingHistory.FourthYear / 100);

                        if (depreciationSettingHistory.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSettingHistory.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSettingHistory.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSettingHistory.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSettingHistory.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSettingHistory.Percentage * 4 / 100);
                    }
                }

                List<PremiumReference> premiumReference = new List<PremiumReference>();
                //List<string> repairMethodList = new List<string>();
                //if (contractDuration >= 1)
                //    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                //if (contractDuration >= 2)
                //    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                //if (contractDuration >= 3)
                //    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                //if (contractDuration >= 4)
                //    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                //if (contractDuration >= 5)
                //    repairMethodList.Add(bankRepairMethodSettings.FifthYear);

                List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();

                //int countAgency = repairMethodList.Where(r => r == "Agency").Count();
                //int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();
                //if (repairMethodList.Count() == countAgency)
                //    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Agency").ToList();
                //else if (repairMethodList.Count() == countWorkShop)
                //    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Workshop").ToList();
                //else
                //    products = quotationInfo.Products;

                products = quotationInfo.Products;
                foreach (var _product in products)
                {
                    if (_product == null || _product.PriceDetails == null)
                        continue;

                    decimal? basicPremium = _product.PriceDetails.Where(p => p.PriceTypeCode == 7)?.FirstOrDefault()?.PriceValue;
                    decimal? vat = _product.PriceDetails.Where(p => p.PriceTypeCode == 8)?.FirstOrDefault()?.PriceValue;
                    decimal? otherTypeCodes = _product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                                                                || p.PriceTypeCode == 6 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);

                    List<short?> selectedBenfitsIds = new List<short?>();
                    if (_product.Benfits.Any(a => a.IsSelected == true))
                        selectedBenfitsIds = _product.Benfits.Where(a => a.IsSelected == true).Select(a => a.BenefitId).ToList();

                    decimal? benefits = 0;
                    decimal? clientBenefitsAmount = 0;
                    decimal? clientBenefitsAmountWithVAT = 0;
                    if (selectedBenfitsIds != null && selectedBenfitsIds.Count > 0)
                    {
                        benefits = _product.Benfits.Where(b => b.BenefitId.HasValue && selectedBenfitsIds.Contains(b.BenefitId.Value))?.Sum(b => b.BenefitPrice.Value);
                        clientBenefitsAmount = benefits;
                        clientBenefitsAmountWithVAT = benefits * 1.15M;
                        benefits *= 1.15M;
                    }

                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        foreach (var benefit in _product.Benfits)
                        {
                            if (selectedBenfitsIds != null && selectedBenfitsIds.Any() && selectedBenfitsIds.Contains(benefit.BenefitId))
                                continue;

                            if (benefit.IsReadOnly && benefit.IsSelected.HasValue && benefit.IsSelected.Value)
                                benefits += benefit.BenefitPrice.Value;
                        }
                    }

                    var otherCodesAndBenifits = otherTypeCodes + benefits;
                    var premium = basicPremium + vat + otherCodesAndBenifits;// - discounts;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        premiumReference.Add(
                        new PremiumReference
                        {
                            Premium = premium ?? 0,
                            ReferenceId = _product.ReferenceId,
                            BasicPremium = basicPremium ?? 0,
                            VehicleRepairType = _product.VehicleRepairType,
                            VAT = vat ?? 0,
                            InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / vehicleValue,
                            OtherCodesAndBenifits = otherTypeCodes + clientBenefitsAmount
                        });
                    }
                    else
                    {
                        premiumReference.Add(
                        new PremiumReference
                        {
                            Premium = premium ?? 0,
                            ReferenceId = _product.ReferenceId,
                            BasicPremium = basicPremium ?? 0,
                            VehicleRepairType = _product.VehicleRepairType,
                            VAT = vat ?? 0,
                            InsurancePercentage = _product.InsurancePercentage,
                            OtherCodesAndBenifits = otherCodesAndBenifits
                        });
                    }
                }

                //var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType == "Agency").OrderBy(p => p.Premium).FirstOrDefault();
                //var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType == "Workshop").OrderBy(p => p.Premium).FirstOrDefault();
                var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType.ToLower() == bankRepairMethodSettingsHistory.FirstYear.ToLower()).OrderBy(p => p.Premium).FirstOrDefault();
                var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType.ToLower() == bankRepairMethodSettingsHistory.FirstYear.ToLower()).OrderBy(p => p.Premium).FirstOrDefault();
                var lowestPremiumAlYusr = premiumReference.Where(p => p.VehicleRepairType.ToLower() == bankRepairMethodSettingsHistory.FirstYear.ToLower()).OrderBy(p => p.Premium).FirstOrDefault();

                Decimal? InsurancePercentage1 = 0;
                Decimal? InsurancePercentage2 = 0;
                Decimal? InsurancePercentage3 = 0;
                Decimal? InsurancePercentage4 = 0;
                Decimal? InsurancePercentage5 = 0;

                Decimal? Premium1 = 0;
                Decimal? Premium2 = 0;
                Decimal? Premium3 = 0;
                Decimal? Premium4 = 0;
                Decimal? Premium5 = 0;
                if (contractDuration >= 1)
                {
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage1 = lowestPremiumAlYusr?.InsurancePercentage;
                        Premium1 = lowestPremiumAlYusr?.Premium;
                        if (Premium1 < bankMinimumPremiumSettingsHistory.FirstYear)
                        {
                            Premium1 = bankMinimumPremiumSettingsHistory.FirstYear;
                        }
                    }
                    else if (bankRepairMethodSettingsHistory.FirstYear == "Agency")
                    {
                        InsurancePercentage1 = lowestPremiumAgency?.InsurancePercentage;
                        Premium1 = lowestPremiumAgency?.Premium;
                    }
                    else
                    {
                        InsurancePercentage1 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium1 = lowestPremiumWorkshop?.Premium;
                    }

                    InsurancePercentage1 = InsurancePercentage1 ?? 0;
                    Premium1 = Premium1 ?? 0;

                    autoleasingQuotationFormSettings.InsurancePercentage = Math.Round(InsurancePercentage1.Value, 2).ToString();
                    autoleasingQuotationFormSettings.Premium = Math.Round(Premium1.Value, 2).ToString();
                    autoleasingQuotationFormSettings.Deductible = deductableValue.ToString();
                    autoleasingQuotationFormSettings.MinimumPremium = bankMinimumPremiumSettingsHistory.FirstYear.ToString().Replace(".00", "");
                }
                if (contractDuration >= 2)
                {
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage2 = lowestPremiumAlYusr?.InsurancePercentage;
                        Premium2 = (InsurancePercentage2 * DepreciationValue2) / 100;
                        if (Premium2 < bankMinimumPremiumSettingsHistory.SecondYear)
                        {
                            Premium2 = bankMinimumPremiumSettingsHistory.SecondYear;
                        }
                    }
                    else if (bankRepairMethodSettingsHistory.SecondYear == "Agency")
                    {
                        InsurancePercentage2 = lowestPremiumAgency?.InsurancePercentage;
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumAgency?.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage2 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumWorkshop?.OtherCodesAndBenifits) * 1.15M;
                    }
                }
                if (contractDuration >= 3)
                {
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage3 = lowestPremiumAlYusr?.InsurancePercentage;
                        Premium3 = (InsurancePercentage3 * DepreciationValue3) / 100;
                        if (Premium3 < bankMinimumPremiumSettingsHistory.ThirdYear)
                        {
                            Premium3 = bankMinimumPremiumSettingsHistory.ThirdYear;
                        }
                    }
                    else if (bankRepairMethodSettingsHistory.ThirdYear == "Agency")
                    {
                        InsurancePercentage3 = lowestPremiumAgency?.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumAgency?.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage3 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumWorkshop?.OtherCodesAndBenifits) * 1.15M;
                    }
                }
                if (contractDuration >= 4)
                {
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage4 = lowestPremiumAlYusr?.InsurancePercentage;
                        Premium4 = (InsurancePercentage4 * DepreciationValue4) / 100;
                        if (Premium4 < bankMinimumPremiumSettingsHistory.FourthYear)
                        {
                            Premium4 = bankMinimumPremiumSettingsHistory.FourthYear;
                        }
                    }
                    else if (bankRepairMethodSettingsHistory.FourthYear == "Agency")
                    {
                        InsurancePercentage4 = lowestPremiumAgency?.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumAgency?.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage4 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumWorkshop?.OtherCodesAndBenifits) * 1.15M;
                    }
                }
                if (contractDuration >= 5)
                {
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage5 = lowestPremiumAlYusr?.InsurancePercentage;
                        Premium5 = (InsurancePercentage5 * DepreciationValue5) / 100;
                        if (Premium5 < bankMinimumPremiumSettingsHistory.FifthYear)
                        {
                            Premium5 = bankMinimumPremiumSettingsHistory.FifthYear;
                        }
                    }
                    else if (bankRepairMethodSettingsHistory.FifthYear == "Agency")
                    {
                        InsurancePercentage5 = lowestPremiumAgency?.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumAgency?.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage5 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumWorkshop?.OtherCodesAndBenifits) * 1.15M;
                    }
                }

                autoleasingQuotationFormSettings.Total5YearsPremium = Math.Round((Premium1 ?? 0) + (Premium2 ?? 0) + (Premium3 ?? 0) + (Premium4 ?? 0) + (Premium5 ?? 0), 2).ToString();
                #endregion

                if (!_autoleasingQuotationFormSettingsRepository.TableNoTracking.Any(a => a.VehicleId == autoleasingQuotationFormSettings.VehicleId && a.ExternalId == autoleasingQuotationFormSettings.ExternalId))
                    _autoleasingQuotationFormSettingsRepository.Insert(autoleasingQuotationFormSettings);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleAutoleasingQuotationFormSettings_" + quotationInfo.VehicleId + ".txt", " Exception is:" + exception.ToString());
                return false;
            }
        }

        private static List<AutoleasingQuotationFormSettingModel> GetFromAutoleasingQuotationFormSettings()
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            string exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingQuotationFormSettings";
                command.CommandType = CommandType.StoredProcedure;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AutoleasingQuotationFormSettingModel>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetFromAutoleasingQuotationFormSettings" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        private static bool UpdateAutoleasingQuotationFormSettings(AutoleasingQuotationFormSettingModel item)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            string exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateAutoleasingQuotationForm_OldHistory";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter idParameter = new SqlParameter() { ParameterName = "id", Value = item.Id };
                command.Parameters.Add(idParameter);

                SqlParameter isDoneParameter = new SqlParameter() { ParameterName = "isDone", Value = item.IsDone };
                command.Parameters.Add(isDoneParameter);

                SqlParameter isLockedParameter = new SqlParameter() { ParameterName = "isLocked", Value = item.IsLocked };
                command.Parameters.Add(isLockedParameter);

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<bool>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetFromAutoleasingQuotationFormSettings" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                idbContext.DatabaseInstance.Connection.Close();
                return false;
            }
        }

        #endregion

        private class AutoleasingQuotationFormSettingModel
        {
            public int Id { get; set; }
            public string ExternalId { get; set; }
            public string VehicleId { get; set; }
            public int? DeductableValue { get; set; }
            public bool AgencyRepair { get; set; }
            public bool IsLocked { get; set; }
            public bool IsDone { get; set; }
        }

        private class PremiumReference
        {
            public string ReferenceId { get; set; }
            public Decimal? Premium { get; set; }
            public decimal BasicPremium { get; set; }
            public string VehicleRepairType { get; set; }
            public decimal VAT { get; internal set; }
            public decimal? InsurancePercentage { get; internal set; }
            public decimal? OtherCodesAndBenifits { get; internal set; }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.Leasing;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Policies.leasingportal;

namespace Tameenk.Services.Implementation.Leasing
{
    public class LeasingProfileService: ILeasingProfileService
    {
        private readonly IRepository<AutoleasingQuotationFormSettings> _autoleasingQuotationFormSettingsRepository;
        private readonly IRepository<Invoice> _invoice;
        private readonly IRepository<PriceDetail> _priceDetail;
        private readonly IRepository<AutoleasingAgencyRepair> _autoleasingRepairMethodRepository;
        private readonly IRepository<AutoleasingRenewalPolicyStatistics> _autoleasingRenewalPolicyStatistics;
        public LeasingProfileService(IRepository<AutoleasingQuotationFormSettings> autoleasingQuotationFormSettingsRepository
            ,IRepository<Invoice> invoice, IRepository<PriceDetail> priceDetail,IRepository<AutoleasingAgencyRepair> autoleasingRepairMethodRepository
            , IRepository<AutoleasingRenewalPolicyStatistics> autoleasingRenewalPolicyStatistics


            )
        {
            _autoleasingQuotationFormSettingsRepository = autoleasingQuotationFormSettingsRepository;
            _invoice = invoice;
            _priceDetail = priceDetail; 
            _autoleasingRepairMethodRepository = autoleasingRepairMethodRepository;
            _autoleasingRenewalPolicyStatistics = autoleasingRenewalPolicyStatistics;
    }

    public List<policyStatistics> GetLeasingProfilePolicies(string currentUserId, string nationalId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetleasingprofilePolicyData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter currentUserIdParameter = new SqlParameter() { ParameterName = "UserId", Value = currentUserId };
                command.Parameters.Add(currentUserIdParameter);
                SqlParameter nationalIdParameter = new SqlParameter() { ParameterName = "nationalId", Value = nationalId };
                command.Parameters.Add(nationalIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<policyStatistics>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        //public AllPolicyDetailsModel GetClientPolicyLogDetailsService(string referenceId, string externalId, out string exception)
        //{
        //    exception = string.Empty;
        //    int commandTimeout = 120;
        //    var dbContext = EngineContext.Current.Resolve<IDbContext>();
        //    try
        //    {
        //        var command = dbContext.DatabaseInstance.Connection.CreateCommand();
        //        command.CommandText = "ClientPolicyLogDetailsSP";
        //        command.CommandType = CommandType.StoredProcedure;
        //        dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
        //        SqlParameter referanceIdParameter = new SqlParameter() { ParameterName = "referanceId", Value = referenceId };
        //        command.Parameters.Add(referanceIdParameter);

        //        SqlParameter externalIdParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
        //        command.Parameters.Add(externalIdParameter);

        //        dbContext.DatabaseInstance.Connection.Open();
        //        var reader = command.ExecuteReader();
        //        PolicyLogDetailsDTO policyDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogDetailsDTO>(reader).FirstOrDefault();
        //        if (policyDetails == null)
        //            return null;

        //        reader.NextResult();
        //        List<PolicyLogPricesDetailsDTO> priceDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogPricesDetailsDTO>(reader).ToList();
        //        if (priceDetails == null)
        //            return null;

        //        reader.NextResult();
        //        decimal benefitsPrice = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<decimal>(reader).FirstOrDefault();

        //        reader.NextResult();
        //        DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();

        //        reader.NextResult();
        //        DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();

        //        reader.NextResult();
        //        AutoleasingQuotationFormSettings AutoleasingQuotationFormSettings = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationFormSettings>(reader).FirstOrDefault();

        //        AllPolicyDetailsModel response = new AllPolicyDetailsModel();
        //        response.PolicyDetails = policyDetails;
        //        response.PolicyPricesDetails = PolicyDetailsCalculations(priceDetails, benefitsPrice, depreciationSettingHistoryData, depreciationSettingData,AutoleasingQuotationFormSettings);
        //        return response;
        //    }
        //    catch (Exception exp)
        //    {
        //        exception = exp.ToString();
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
        //            dbContext.DatabaseInstance.Connection.Close();
        //    }
        //}
        public List<AllPolicyDetailsModel> GetClientPolicyLogDetailsService(ClientDataModel clientDataModel, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
           string basicExternal = (clientDataModel.ParentExternalId == null) ? clientDataModel.ExternalId : clientDataModel.ParentExternalId;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var renwalPolicies = _autoleasingRenewalPolicyStatistics.TableNoTracking.Where(a => a.ParentExternalId == basicExternal).ToList();
                List<ClientPolicyData> clientPolicies = new List<ClientPolicyData>();
                if (clientDataModel.ParentExternalId == null || string.IsNullOrEmpty(clientDataModel.ParentExternalId))
                {
                    clientPolicies.Add(new ClientPolicyData
                    {
                        ExternalId = clientDataModel.ExternalId,
                        ReferenceId = clientDataModel.ReferenceId
                    });
                }

                else
                {
                        clientPolicies.Add(new ClientPolicyData
                        {
                            ExternalId = clientDataModel.ParentExternalId,
                            ReferenceId = clientDataModel.ParentReferenceId
                        });
                }
               
                if (renwalPolicies!=null && clientPolicies.Count() > 0)
                {
                    foreach (var renwalPolicy in renwalPolicies)
                    {
                        clientPolicies.Add(new ClientPolicyData
                        {
                            ExternalId = renwalPolicy.ExternalId,
                            ReferenceId = renwalPolicy.ReferenceId
                        });
                    }
                }
                var mainQuotationForm = _autoleasingQuotationFormSettingsRepository.TableNoTracking.FirstOrDefault(a => a.ExternalId == basicExternal);

                //var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                List<AllPolicyDetailsModel> response = new List<AllPolicyDetailsModel>();
                dbContext.DatabaseInstance.Connection.Open();
                foreach (var Policy in clientPolicies)
                {
                    var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                    command.CommandText = "ClientPolicyLogDetailsSP";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter referanceIdParameter = new SqlParameter() { ParameterName = "referanceId", Value = Policy.ReferenceId };
                    command.Parameters.Add(referanceIdParameter);

                    SqlParameter externalIdParameter = new SqlParameter() { ParameterName = "externalId", Value = Policy.ExternalId };
                    command.Parameters.Add(externalIdParameter);

                    var reader = command.ExecuteReader();
                    PolicyLogDetailsDTO policyDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogDetailsDTO>(reader).FirstOrDefault();
                    if (policyDetails == null)
                        return null;

                    reader.NextResult();
                    List<PolicyLogPricesDetailsDTO> priceDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogPricesDetailsDTO>(reader).ToList();
                    if (priceDetails == null)
                        return null;

                    reader.NextResult();
                    decimal benefitsPrice = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<decimal>(reader).FirstOrDefault();

                    reader.NextResult();
                    DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();

                    reader.NextResult();
                    DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();

                    reader.NextResult();
                    AutoleasingQuotationFormSettings AutoleasingQuotationFormSettings = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationFormSettings>(reader).FirstOrDefault();
                    reader.NextResult();
                    AutoleasingRenewalPolicyStatistics autoleasingRenewalPolicyStatistics = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingRenewalPolicyStatistics>(reader).FirstOrDefault();

                    AllPolicyDetailsModel policy = new AllPolicyDetailsModel();
                    policy.PolicyDetails = policyDetails;
                    policy.PolicyPricesDetails = PolicyDetailsCalculations(priceDetails, benefitsPrice, depreciationSettingHistoryData, depreciationSettingData, AutoleasingQuotationFormSettings,autoleasingRenewalPolicyStatistics, policyDetails, mainQuotationForm);
                    response.Add(policy);
                }
                return response;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        public DriverInsuranceRecord GetDriverInsuranceRecordService(ClientDataModel clientDataModel, ref string exception)
        {
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            string BasicReference = (clientDataModel.ParentReferenceId != null || !string.IsNullOrEmpty(clientDataModel.ParentReferenceId)) ? clientDataModel.ParentReferenceId : clientDataModel.ReferenceId; 
            string BasicExternal = (clientDataModel.ParentExternalId != null || !string.IsNullOrEmpty(clientDataModel.ParentExternalId)) ? clientDataModel.ParentExternalId : clientDataModel.ExternalId;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetDriverInsuranceRecordData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter referanceIdParameter = new SqlParameter()
                {
                    ParameterName = "ReferenceId",
                    Value = BasicReference
                };
                SqlParameter basicExternalParameter = new SqlParameter()
                {
                    ParameterName = "BasicExternalId",
                    Value = BasicExternal
                };
                command.Parameters.Add(basicExternalParameter);
                command.Parameters.Add(referanceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                DriverInsuranceRecordFromDB driverInsuranceRecordFromDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverInsuranceRecordFromDB>(reader).FirstOrDefault();
                DriverInsuranceRecord driverInsuranceRecord = new DriverInsuranceRecord();
                DriverData driverData = new DriverData();
                VehicleData vehicleData = new VehicleData();
                ContractData contractData = new ContractData();
                CustomerBalanceDetails balanceDetails = new CustomerBalanceDetails();
                //reader.NextResult();
                //balanceDetails.PayedAmount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<decimal>(reader).FirstOrDefault().ToString();
              
                reader.NextResult();
                var BalancAmount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<decimal>(reader).FirstOrDefault().ToString();
               
                reader.NextResult();
                RepairMethodeSettingHistory repairMethodeSettingHistory = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSettingHistory>(reader).FirstOrDefault();

                reader.NextResult();
                RepairMethodeSetting repairMethodeSetting = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSetting>(reader).FirstOrDefault();

                reader.NextResult();
                MinimumPremiumSettingHistory minimumPremiumSettingHistory = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSettingHistory>(reader).FirstOrDefault();
                driverInsuranceRecord.contractData = contractData;

                reader.NextResult();
                MinimumPremiumSetting minimumPremiumSetting = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSetting>(reader).FirstOrDefault();
                driverInsuranceRecord.contractData = contractData;

                reader.NextResult();
                DepreciationSettingHistory depreciationSettingHistory = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();
                driverInsuranceRecord.contractData = contractData;
                
                reader.NextResult();
                DepreciationSetting depreciationSetting = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();
                driverInsuranceRecord.contractData = contractData;
                reader.NextResult();
                List<QuotationProductInfoModel> productsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationProductInfoModel>(reader).ToList();
                reader.NextResult();
                List<PriceDetail> PriceDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PriceDetail>(reader).ToList();
                reader.NextResult();
             AutoleasingQuotationFormSettings autoleasingQuotationFormSettings = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationFormSettings>(reader).FirstOrDefault();
                reader.NextResult();
                Invoice invoice = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();

                driverData.FullNameAr = driverInsuranceRecordFromDB.FullNameAr;
                driverData.FullNameEn = driverInsuranceRecordFromDB.FullNameEn;
                driverData.NIN = driverInsuranceRecordFromDB.NIN;
                driverData.MobileNumber = driverInsuranceRecordFromDB.MobileNumber;
                driverData.DateOfBirth = driverInsuranceRecordFromDB.DateOfBirth;
                driverData.Email = driverInsuranceRecordFromDB.Email;
                driverInsuranceRecord.driverData = driverData;
                vehicleData.ModelYear = driverInsuranceRecordFromDB.ModelYear;
                vehicleData.VehicleValue = driverInsuranceRecordFromDB.VehicleValue;
                vehicleData.VehicleModel = driverInsuranceRecordFromDB.VehicleModel;
                driverInsuranceRecord.vehicleData = vehicleData;
                contractData.StartDate = driverInsuranceRecordFromDB.StartDate;
                contractData.Remains = driverInsuranceRecordFromDB.Remains;
                contractData.Duration = driverInsuranceRecordFromDB.Duration;
                driverInsuranceRecord.contractData = contractData;

                if (autoleasingQuotationFormSettings != null)
                    driverInsuranceRecord.balanceData = calculatedValue (autoleasingQuotationFormSettings, BalancAmount, PriceDetails,invoice, BasicExternal);

                //balanceDetails.ChargedData = CalculateChargedData(driverInsuranceRecordFromDB,repairMethodeSettingHistory,repairMethodeSetting,minimumPremiumSettingHistory,minimumPremiumSetting,autoleasingQuotationFormSettings,depreciationSettingHistory, depreciationSetting, productsData,PriceDetails).ToString();
                return driverInsuranceRecord;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private CustomerBalanceDetails calculatedValue(AutoleasingQuotationFormSettings autoleasingQuotationFormSettings, string BalancAmount, List<PriceDetail> PriceDetails,Invoice invoice,string basicExternal)
        {
            List<AutoleasingRenewalPolicyStatistics> Policies = new List<AutoleasingRenewalPolicyStatistics>();
            //if (autoleasingQuotationFormSettings != null && autoleasingQuotationFormSettings.ExternalId != null)
            //{
                 Policies = _autoleasingRenewalPolicyStatistics.TableNoTracking.Where(a => a.ParentExternalId == basicExternal).ToList();
           // }
            
            CustomerBalanceDetails data = new CustomerBalanceDetails();
            decimal total5YearsPremium = 0;
            decimal.TryParse(autoleasingQuotationFormSettings.Total5YearsPremium, out total5YearsPremium);
            decimal? premimAmount = 0;
            decimal? PayedAmount = 0;
            decimal? Ncd = (invoice != null) ? invoice.Discount : 0;//840.74
            foreach (var item in Policies)
            {
                Ncd += _invoice.TableNoTracking.FirstOrDefault(a => a.ReferenceId == item.ReferenceId).Discount;
            }
            switch (Policies.Count)
            {
                case 0:
                    premimAmount = autoleasingQuotationFormSettings.Premium1;
                    PayedAmount = autoleasingQuotationFormSettings.Premium1;
                    break;
                case 1:
                    premimAmount = autoleasingQuotationFormSettings.Premium1 + autoleasingQuotationFormSettings.Premium2;
                    PayedAmount = autoleasingQuotationFormSettings.Premium1 + Policies[0]?.PaymentAmount;
                    break;
                case 2:
                    premimAmount = autoleasingQuotationFormSettings.Premium1 + autoleasingQuotationFormSettings.Premium2 + autoleasingQuotationFormSettings.Premium3;
                    PayedAmount = autoleasingQuotationFormSettings.Premium1 + Policies[0]?.PaymentAmount+ Policies[1]?.PaymentAmount;
                    break;
                case 3:
                    premimAmount = autoleasingQuotationFormSettings.Premium1 + autoleasingQuotationFormSettings.Premium2 + autoleasingQuotationFormSettings.Premium3 + autoleasingQuotationFormSettings.Premium4;
                    PayedAmount = autoleasingQuotationFormSettings.Premium1 + Policies[0]?.PaymentAmount + Policies[1]?.PaymentAmount+ Policies[2]?.PaymentAmount;
                    break;
                case 4:
                    premimAmount = autoleasingQuotationFormSettings.Premium1 + autoleasingQuotationFormSettings.Premium2 + autoleasingQuotationFormSettings.Premium3 + autoleasingQuotationFormSettings.Premium4+ autoleasingQuotationFormSettings.Premium4;
                    PayedAmount = autoleasingQuotationFormSettings.Premium1 + Policies[0]?.PaymentAmount + Policies[1]?.PaymentAmount + Policies[2]?.PaymentAmount + +Policies[3]?.PaymentAmount;
                    break;
            }
            data.BalancAmount = premimAmount + Ncd - PayedAmount ;
            data.ChargedData = decimal.Round(total5YearsPremium, 2);
            data.PayedAmount = PayedAmount;

            return data;
        }
        private Decimal CalculateChargedData(DriverInsuranceRecordFromDB driverInsuranceRecordFromDB, RepairMethodeSettingHistory repairMethodeSettingHistory, RepairMethodeSetting repairMethodeSetting, MinimumPremiumSettingHistory minimumPremiumSettingHistories, MinimumPremiumSetting minimumPremiumSettings, List<AutoleasingQuotationFormSettings> autoleasingQuotationFormSettings, DepreciationSettingHistory DepreciationSettingHistory, DepreciationSetting DepreciationSetting
            ,List<QuotationProductInfoModel> productsData,List<PriceDetail> PriceDetails)
        {
            AutoleasingDepreciationSetting depreciationSetting = null;

            if (DepreciationSettingHistory != null)
            {
                var deprecationHistory =DepreciationSettingHistory;
                depreciationSetting = new AutoleasingDepreciationSetting()
                {
                    BankId = driverInsuranceRecordFromDB.BankId.Value,
                    MakerCode = deprecationHistory.MakerCode,
                    ModelCode = deprecationHistory.ModelCode,
                    MakerName = deprecationHistory.MakerName,
                    ModelName = deprecationHistory.ModelName,
                    Percentage = deprecationHistory.Percentage,
                    IsDynamic = deprecationHistory.IsDynamic,
                    FirstYear = deprecationHistory.FirstYear,
                    SecondYear = deprecationHistory.SecondYear,
                    ThirdYear = deprecationHistory.ThirdYear,
                    FourthYear = deprecationHistory.FourthYear,
                    FifthYear = deprecationHistory.FifthYear,
                    AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                };
            }
            else
            {
                var deprecationSetting = DepreciationSetting;
                depreciationSetting = new AutoleasingDepreciationSetting();
                depreciationSetting.BankId = driverInsuranceRecordFromDB.BankId.Value;
                depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                depreciationSetting.MakerName = deprecationSetting.MakerName;
                depreciationSetting.ModelName = deprecationSetting.ModelName;
                depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
            }

           var AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;
             var AnnualDeprecationPercentage = depreciationSetting.Percentage.ToString();

            List<string> annualPercentages = new List<string>();
            if (driverInsuranceRecordFromDB.Duration/12 >= 1)
            {
                if (!depreciationSetting.FirstYear.Equals(null))
                {
    
                    annualPercentages.Add("0 %");
                }
            }

            if (driverInsuranceRecordFromDB.Duration / 12 >= 2)
            {
                if (!depreciationSetting.SecondYear.Equals(null))
                {
                    annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + " %");
                }
            }

            if (driverInsuranceRecordFromDB.Duration / 12 >= 3)
            {
                if (!depreciationSetting.ThirdYear.Equals(null))
                {
                    annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %");
                }
            }

            if (driverInsuranceRecordFromDB.Duration / 12 >= 4)
            {
                if (!depreciationSetting.FourthYear.Equals(null))
                {
                    annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + " %");
                }
            }

            if (driverInsuranceRecordFromDB.Duration / 12 >= 5)
            {
                if (!depreciationSetting.FifthYear.Equals(null))
                {
                    annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + " %");
                }
            }

            var IsDynamic = depreciationSetting.IsDynamic;
            var Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + " %";

            Decimal? DepreciationValue1 = 0;
            Decimal? DepreciationValue2 = 0;
            Decimal? DepreciationValue3 = 0;
            Decimal? DepreciationValue4 = 0;
            Decimal? DepreciationValue5 = 0;
            List<Decimal?> depreciationValues = new List<Decimal?>();

            var currentVehicleValue = driverInsuranceRecordFromDB.VehicleValue;

            if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
            {
                if (depreciationSetting.IsDynamic)
                {
                    DepreciationValue1 = driverInsuranceRecordFromDB.VehicleValue;

                    if (depreciationSetting.SecondYear != 0)
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                    if (depreciationSetting.ThirdYear != 0)
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                    if (depreciationSetting.FourthYear != 0)
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                    if (depreciationSetting.FifthYear != 0)
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                }
                else
                {
                    DepreciationValue1 = currentVehicleValue;
                    DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                    DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                    DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                    DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                }
            }
            else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
            {
                if (depreciationSetting.IsDynamic)
                {
                    DepreciationValue1 = driverInsuranceRecordFromDB.VehicleValue;

                    if (depreciationSetting.SecondYear != 0)
                        DepreciationValue2 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.SecondYear / 100);

                    if (depreciationSetting.ThirdYear != 0)
                        DepreciationValue3 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.ThirdYear / 100);

                    if (depreciationSetting.FourthYear != 0)
                        DepreciationValue4 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.FourthYear / 100);

                    if (depreciationSetting.FifthYear != 0)
                        DepreciationValue5 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.FifthYear / 100);
                }
                else
                {
                    DepreciationValue1 = driverInsuranceRecordFromDB.VehicleValue;
                    DepreciationValue2 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.Percentage / 100);
                    DepreciationValue3 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.Percentage * 2 / 100);
                    DepreciationValue4 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.Percentage * 3 / 100);
                    DepreciationValue5 = driverInsuranceRecordFromDB.VehicleValue - (driverInsuranceRecordFromDB.VehicleValue * depreciationSetting.Percentage * 4 / 100);
                }
            }
            AutoleasingAgencyRepair bankRepairMethodSettings = null;
            AutoleasingMinimumPremium bankMinimumPremiumSettings = null;
            if (minimumPremiumSettings != null)
            {
                var minimumPremiumSettingHistory = minimumPremiumSettingHistories;
                bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                {
                    BankId = driverInsuranceRecordFromDB.BankId.Value,
                    FirstYear = minimumPremiumSettingHistory.FirstYear,
                    SecondYear = minimumPremiumSettingHistory.SecondYear,
                    ThirdYear = minimumPremiumSettingHistory.ThirdYear,
                    FourthYear = minimumPremiumSettingHistory.FourthYear,
                    FifthYear = minimumPremiumSettingHistory.FifthYear
                };
            }
            else
            {
                var minimumPremiumSetting = minimumPremiumSettings;
                bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                {
                    BankId = driverInsuranceRecordFromDB.BankId.Value,
                    FirstYear = minimumPremiumSetting.FirstYear,
                    SecondYear = minimumPremiumSetting.SecondYear,
                    ThirdYear = minimumPremiumSetting.ThirdYear,
                    FourthYear = minimumPremiumSetting.FourthYear,
                    FifthYear = minimumPremiumSetting.FifthYear
                };
            }
            if (repairMethodeSettingHistory != null)
                {
                    var bankRepairMethodSettingsHistory = repairMethodeSettingHistory;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = bankRepairMethodSettingsHistory.BankId,
                        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                    };
                }
                else
                {
                    var _bankRepairMethodSettings = repairMethodeSetting;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = _bankRepairMethodSettings.BankId,
                        FirstYear = _bankRepairMethodSettings.FirstYear,
                        SecondYear = _bankRepairMethodSettings.SecondYear,
                        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                        FourthYear = _bankRepairMethodSettings.FourthYear,
                        FifthYear = _bankRepairMethodSettings.FifthYear,
                    };
                    bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == driverInsuranceRecordFromDB.BankId).FirstOrDefault();
                }
            List<PremiumReferences> premiumReference = new List<PremiumReferences>();
            List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();


            List<string> repairMethodList = new List<string>();
            if (driverInsuranceRecordFromDB.Duration/12 >= 1)
            {
                repairMethodList.Add(bankRepairMethodSettings.FirstYear);
            }
            if (driverInsuranceRecordFromDB.Duration / 12 >= 2)
            {
                repairMethodList.Add(bankRepairMethodSettings.SecondYear);
            }
            if (driverInsuranceRecordFromDB.Duration / 12 >= 3)
            {
                repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
            }
            if (driverInsuranceRecordFromDB.Duration / 12 >= 4)
            {
                repairMethodList.Add(bankRepairMethodSettings.FourthYear);
            }
            if (driverInsuranceRecordFromDB.Duration / 12 >= 5)
            {
                repairMethodList.Add(bankRepairMethodSettings.FifthYear);
            }

            int countAgency = repairMethodList.Where(r => r == "Agency").Count();
            int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();
            string RepairType = string.Empty;
            if (repairMethodList.Count() == countAgency)
            {
                products = productsData.Where(q => q.VehicleRepairType == "Agency").ToList();
                RepairType = "Agency";
            }
            else if (repairMethodList.Count() == countWorkShop)
            {
                products = productsData.Where(q => q.VehicleRepairType == "Workshop").ToList();
                RepairType = "Workshop";
            }
            else
            {
                products = productsData;
                RepairType = "Mixed";
            }
            foreach (var _product in products)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount_error.txt", JsonConvert.SerializeObject(products));

                File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasing1_error.txt", JsonConvert.SerializeObject(_product));
                //var product = quotation.Products.FirstOrDefault();
                if (_product == null)
                    continue;
                if (PriceDetails == null)
                    continue;
                decimal? basicPremium = 0;
                decimal? otherTypeCodes = 0;
                decimal? vat = 0;
                List<PriceDetail> selectedPriceDetails = new List<PriceDetail>();
                selectedPriceDetails = PriceDetails.Where(p => p.ProductID == _product.ProductID).ToList();
               
                    basicPremium += selectedPriceDetails.Where(p => p.PriceTypeCode == 7)?.FirstOrDefault()?.PriceValue;

                    otherTypeCodes = selectedPriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                    || p.PriceTypeCode == 6 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);

                     vat = selectedPriceDetails.Where(p => p.PriceTypeCode == 8)?.FirstOrDefault()?.PriceValue;

                decimal? benefits = 0;
                decimal? clientBenefitsAmount = 0;
                decimal? clientBenefitsAmountWithVAT = 0;
            

                var otherCodesAndBenifits = otherTypeCodes + benefits;

                var premium = basicPremium + vat + otherCodesAndBenifits;// - discounts;

                if (driverInsuranceRecordFromDB.BankId == 2) //Yusr
                {
                    premiumReference.Add(
                    new PremiumReferences
                    {
                        Premium = premium ?? 0,
                        ReferenceId = _product.ReferenceId,
                        BasicPremium = basicPremium ?? 0,
                        VehicleRepairType = _product.VehicleRepairType,
                        VAT = vat ?? 0,
                            //InsurancePercentage = _product.InsurancePercentage,
                            //OtherCodesAndBenifits = otherCodesAndBenifits,
                            InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / driverInsuranceRecordFromDB.VehicleValue,
                        OtherCodesAndBenifits = otherTypeCodes + clientBenefitsAmount
                    });
                }
                else
                {
                    premiumReference.Add(
                    new PremiumReferences
                    {
                        Premium = premium ?? 0,
                        ReferenceId = _product.ReferenceId,
                        BasicPremium = basicPremium ?? 0,
                        VehicleRepairType = _product.VehicleRepairType,
                        VAT = vat ?? 0,
                        InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / driverInsuranceRecordFromDB.VehicleValue,
                        OtherCodesAndBenifits = otherCodesAndBenifits
                    });
                }

            }
            var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType == "Agency").OrderBy(p => p.InsurancePercentage).FirstOrDefault();
            var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType == "Workshop").OrderBy(p => p.InsurancePercentage).FirstOrDefault();
            var lowestPremiumAlYusr = premiumReference.Where(p => p.VehicleRepairType == bankRepairMethodSettings.FirstYear).OrderBy(p => p.Premium).FirstOrDefault();

            File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount10_error.txt", JsonConvert.SerializeObject(lowestPremiumAgency));
            File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount11_error.txt", JsonConvert.SerializeObject(lowestPremiumWorkshop));
            File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount12_error.txt", JsonConvert.SerializeObject(lowestPremiumAlYusr));

            var contractDuration = driverInsuranceRecordFromDB.Duration / 12;
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
               // data.RepairMethod1 = bankRepairMethodSettings.FirstYear;
                if (driverInsuranceRecordFromDB.BankId == 2) //Yusr
                {
                    InsurancePercentage1 = lowestPremiumAlYusr.InsurancePercentage;
                    Premium1 = lowestPremiumAlYusr.Premium;
                    if (Premium1 < bankMinimumPremiumSettings?.FirstYear)
                    {
                        Premium1 = bankMinimumPremiumSettings?.FirstYear;
                    }
                }
                else
                {
                    if (bankRepairMethodSettings.FirstYear == "Agency")
                    {
                        InsurancePercentage1 = lowestPremiumAgency.InsurancePercentage;
                        Premium1 = lowestPremiumAgency.Premium;
                    }
                    else
                    {
                        InsurancePercentage1 = lowestPremiumWorkshop?.InsurancePercentage;
                        Premium1 = lowestPremiumWorkshop.Premium;
                    }
                }

               InsurancePercentage1 = Math.Round(InsurancePercentage1.Value, 2);
               Premium1 = Math.Round(Premium1.Value, 2);
        
            }
            if (contractDuration >= 2)
            {
               // data.RepairMethod2 = bankRepairMethodSettings.SecondYear;
                if (driverInsuranceRecordFromDB.BankId == 2) //Yusr
                {
                    InsurancePercentage2 = lowestPremiumAlYusr.InsurancePercentage;
                    Premium2 = (InsurancePercentage2 * DepreciationValue2) / 100;
                    if (Premium2 < bankMinimumPremiumSettings.SecondYear)
                    {
                        Premium2 = bankMinimumPremiumSettings.SecondYear;
                    }
                }
                else
                {
                    if (bankRepairMethodSettings.SecondYear == "Agency")
                    {
                        InsurancePercentage2 = lowestPremiumAgency.InsurancePercentage;
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount18_error.txt", lowestPremiumAgency.InsurancePercentage.ToString());
                        InsurancePercentage2 = lowestPremiumWorkshop.InsurancePercentage;
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount19_error.txt", InsurancePercentage2.ToString());
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount20_error.txt", DepreciationValue2.ToString());
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\Leasingcount21_error.txt", lowestPremiumWorkshop.OtherCodesAndBenifits.ToString());
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                }

                InsurancePercentage2 = Math.Round(InsurancePercentage2.Value, 2);
               Premium2 = Math.Round(Premium2.Value, 2);
            }
            if (contractDuration >= 3)
            {
             
                if (driverInsuranceRecordFromDB.BankId.Value == 2) //Yusr
                {
                    InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
                    Premium3 = (InsurancePercentage3 * DepreciationValue3) / 100;
                    if (Premium3 < bankMinimumPremiumSettings.ThirdYear)
                    {
                        Premium3 = bankMinimumPremiumSettings.ThirdYear;
                    }
                }
                else
                {
                    if (bankRepairMethodSettings.ThirdYear == "Agency")
                    {
                        InsurancePercentage3 = lowestPremiumAgency.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                }

              InsurancePercentage3 = Math.Round(InsurancePercentage3.Value, 2);
              Premium3 = Math.Round(Premium3.Value, 2);
             

            }
            if (contractDuration >= 4)
            {
              
                if (driverInsuranceRecordFromDB.BankId.Value == 2) //Yusr
                {
                    InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
                    Premium4 = (InsurancePercentage4 * DepreciationValue4) / 100;
                    if (Premium4 < bankMinimumPremiumSettings.FourthYear)
                    {
                        Premium4 = bankMinimumPremiumSettings.FourthYear;
                    }
                }

                else
                {
                    if (bankRepairMethodSettings.FourthYear == "Agency")
                    {
                        InsurancePercentage4 = lowestPremiumAgency.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                }

              InsurancePercentage4 = Math.Round(InsurancePercentage4.Value, 2);
              Premium4 = Math.Round(Premium4.Value, 2);


            }
            if (contractDuration >= 5)
            {
            
                if (driverInsuranceRecordFromDB.BankId.Value == 2) //Yusr
                {
                    InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
                    Premium5 = (InsurancePercentage5 * DepreciationValue5) / 100;
                    if (Premium5 < bankMinimumPremiumSettings.FifthYear)
                    {
                        Premium5 = bankMinimumPremiumSettings.FifthYear;
                    }
                }
                else
                {
                    if (bankRepairMethodSettings.FifthYear == "Agency")
                    {
                        InsurancePercentage5 = lowestPremiumAgency.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                }

                InsurancePercentage5 = Math.Round(InsurancePercentage5.Value, 2);
                Premium5 = Math.Round(Premium5.Value, 2);
              

            }
            decimal total = 0;
            if (autoleasingQuotationFormSettings != null && autoleasingQuotationFormSettings.Count > 0)
            {
                decimal value = 0;
                autoleasingQuotationFormSettings.ForEach(x =>
                {
                    decimal.TryParse(x.Total5YearsPremium,out value);
                    total += value;
                });
            }
            
            return
                Math.Round((Premium1 ?? 0) + (Premium2 ?? 0) + (Premium3 ?? 0) + (Premium4 ?? 0) + (Premium5 ?? 0), 2)+ total;
             
        }
            private CustomerBalanceDetails GetDriverBalanceDetails(string Nin,string VehicleId, ref string exception)
        {
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingPolicy";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter VehicleIdParmter = new SqlParameter()
                {
                    ParameterName = "@VehicleId",
                    Value = VehicleId
                };
                SqlParameter NinParameter = new SqlParameter()
                {
                    ParameterName = "@NIN",
                    Value = Nin
                };
                command.Parameters.Add(VehicleIdParmter);
                command.Parameters.Add(NinParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
             List<LeasingClientPolicyModel>  Policies = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<List<LeasingClientPolicyModel>>(reader).FirstOrDefault();
                #region To be Implemented bt automapper
                var previousSettings = _autoleasingQuotationFormSettingsRepository.TableNoTracking.FirstOrDefault(a => a.VehicleId == VehicleId);
                #endregion
                CustomerBalanceDetails balanceDetails = new CustomerBalanceDetails();
                balanceDetails.ChargedData =decimal.Parse( previousSettings.Total5YearsPremium);
                decimal totolaInvoices = 0;
                List<PriceDetail> priceList = new List<PriceDetail>();
                if (Policies.Count<1)
                {
                    return null;
                }
                foreach (var policy in Policies)
                {
                    totolaInvoices += _invoice.TableNoTracking.FirstOrDefault(a => a.ReferenceId == policy.ReferenceId).TotalPrice.Value;
                    PriceDetail priceDetail = new PriceDetail();
                    priceDetail = _priceDetail.TableNoTracking.FirstOrDefault(a => a.ProductID == policy.SelectedProductId);
                    priceList.Add(priceDetail);
                }

                balanceDetails.BalancAmount = priceList.Where(p => p.PriceTypeCode == 1 || p.PriceTypeCode == 2
                 || p.PriceTypeCode == 3 || p.PriceTypeCode == 10||p.PriceTypeCode ==11|| p.PriceTypeCode == 12).Sum(p => p.PriceValue);
                balanceDetails.PayedAmount = totolaInvoices;
                return balanceDetails;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public ProfileBaiscData GetProfileBaiscDataSerivce(string ReferenceId, ref string exception)
        {
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLeasingProfileBasicData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter referanceIdParameter = new SqlParameter()
                {
                    ParameterName = "ReferenceId",
                    Value = ReferenceId
                };
                command.Parameters.Add(referanceIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                ProfileBaiscData profileBaiscData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProfileBaiscData>(reader).FirstOrDefault();

                return profileBaiscData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        //public List<policyStatistics> GetLeasingProfilePolicies(string currentUserId, out string exception)
        //{
        //    exception = string.Empty;
        //    int commandTimeout = 120;
        //    var dbContext = EngineContext.Current.Resolve<IDbContext>();
        //    try
        //    {
        //        var command = dbContext.DatabaseInstance.Connection.CreateCommand();
        //        command.CommandText = "GetleasingprofilePolicyData";
        //        command.CommandType = CommandType.StoredProcedure;
        //        dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
        //        SqlParameter currentUserIdParameter = new SqlParameter()
        //        {
        //            ParameterName = "UserId",
        //            Value = currentUserId
        //        };
        //        command.Parameters.Add(currentUserIdParameter);
        //        dbContext.DatabaseInstance.Connection.Open();
        //        var reader = command.ExecuteReader();
        //        var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<policyStatistics>(reader).ToList();
        //        dbContext.DatabaseInstance.Connection.Close();
        //        return data;
        //    }
        //    catch (Exception exp)
        //    {
        //        dbContext.DatabaseInstance.Connection.Close();
        //        exception = exp.ToString();
        //        return null;
        //    }
        //}

        //public AllPolicyDetailsModel GetClientPolicyLogDetailsService(string referenceId, out string exception)
        //{

        //    exception = string.Empty;
        //    int commandTimeout = 120;
        //    var dbContext = EngineContext.Current.Resolve<IDbContext>();
        //    try
        //    {
        //        var command = dbContext.DatabaseInstance.Connection.CreateCommand();
        //        command.CommandText = "ClientPolicyLogDetailsSP";
        //        command.CommandType = CommandType.StoredProcedure;
        //        dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
        //        SqlParameter referanceIdParameter = new SqlParameter()
        //        {
        //            ParameterName = "referanceId",
        //            Value = referenceId
        //        };
        //        command.Parameters.Add(referanceIdParameter);
        //        dbContext.DatabaseInstance.Connection.Open();
        //        var reader = command.ExecuteReader();
        //        PolicyLogDetailsDTO policyDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogDetailsDTO>(reader).FirstOrDefault();

        //        reader.NextResult();

        //        List<PolicyLogPricesDetailsDTO> priceDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyLogPricesDetailsDTO>(reader).ToList();

        //        reader.NextResult();

        //        decimal benefitsPrice = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
        //        dbContext.DatabaseInstance.Connection.Close();

        //        AllPolicyDetailsModel response = new AllPolicyDetailsModel();
        //        response.PolicyDetails = policyDetails;
        //        response.PolicyPricesDetails = PolicyDetailsCalculations(priceDetails, benefitsPrice);

        //        return response;
        //    }
        //    catch (Exception exp)
        //    {
        //        dbContext.DatabaseInstance.Connection.Close();
        //        exception = exp.ToString();
        //        return null;
        //    }
        //}

        //public LeasingPolicyPrice PolicyDetailsCalculations(List<PolicyLogPricesDetailsDTO> priceDetails, decimal totalBenefit)
        //{
        //    LeasingPolicyPrice result = new LeasingPolicyPrice();
        //    if (priceDetails == null || priceDetails.Count() < 1)
        //    {
        //        return result;
        //    }
        //    decimal _vat = 0;
        //    decimal _discounts = 0;
        //    decimal _basicPremium = 0;
        //    decimal _shadowAmount = 0;
        //    decimal _clientAmount = 0;
        //    decimal _extraPremiumPrice = 0;


        //    foreach (var price in priceDetails)
        //    {

        //        // ShadowAmount
        //        if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 || price.PriceTypeCode == 11 || price.PriceTypeCode == 12)
        //        {
        //            _shadowAmount += price.PriceValue;
        //        }


        //        // Discounts
        //        if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 || price.PriceTypeCode == 11 || price.PriceTypeCode == 12 || price.PriceTypeCode == 13)
        //        {
        //            _discounts += price.PriceValue;
        //        }

        //        //  Vat
        //        if (price.PriceTypeCode == 8)
        //        {
        //            _vat += price.PriceValue;
        //        }

        //        // BasicPremium
        //        if (price.PriceTypeCode == 7)
        //        {
        //            _basicPremium += price.PriceValue;
        //        }

        //        // ClientAmount
        //        if (price.PriceTypeCode == 4 || price.PriceTypeCode == 5 || price.PriceTypeCode == 6 || price.PriceTypeCode == 8 || price.PriceTypeCode == 9
        //        )
        //        {
        //            _clientAmount += price.PriceValue;
        //        }

        //        // ExtraPremiumPrice
        //        if (price.PriceTypeCode == 7
        //          || price.PriceTypeCode == 4
        //          || price.PriceTypeCode == 5
        //          || price.PriceTypeCode == 6
        //     )
        //        {
        //            _extraPremiumPrice += price.PriceValue;
        //        }
        //    }


        //    result.TotalBenefitWitVatPrice = Math.Round(totalBenefit + (totalBenefit * (decimal).15), 1);
        //    result.ClientTotalBalance = Math.Round(_shadowAmount, 1);
        //    result.Discounts = Math.Round(_discounts, 1);
        //    result.Vat = Math.Round(_vat + (totalBenefit * (decimal).15), 1);/*add benefit vat */
        //    /*
        //     * Get the same premium from quotation form based on contract year
        //     */
        //    result.TotalChargedValue = _basicPremium + result.TotalBenefitWitVatPrice + _vat;
        //    result.TotalValueOnClient = _clientAmount + _basicPremium + result.TotalBenefitWitVatPrice;
        //    result.ExtraPremiumPrice = _extraPremiumPrice - result.Discounts + _vat + (_vat * (decimal).15);//
        //    result.TotalPrice = _extraPremiumPrice + _vat + result.TotalBenefitWitVatPrice;
        //    result.Differences = result.TotalChargedValue - result.TotalValueOnClient;
        //    result.ClientDepositedBalance = result.Discounts + result.Differences;


        //    return result;
        //}

        #region Private Methods

        private LeasingPolicyPrice PolicyDetailsCalculations(List<PolicyLogPricesDetailsDTO> priceDetails, decimal totalBenefit, DepreciationSettingHistory depreciationhistory, DepreciationSetting depreciation, AutoleasingQuotationFormSettings autoleasingQuotationFormSettings, AutoleasingRenewalPolicyStatistics autoleasingRenewalPolicyStatistics, PolicyLogDetailsDTO policyLogDetailsDTO, AutoleasingQuotationFormSettings autoleasingQuotationForm)
        {
            LeasingPolicyPrice result = new LeasingPolicyPrice();
            decimal _vat = 0;
            decimal _discounts = 0;
            decimal _basicPremium = 0;
            decimal _shadowAmount = 0;
            decimal _clientAmount = 0;
            decimal _extraPremiumPrice = 0;
            int _depreciationPercentage = 0;
            List<string> newDep = new List<string>();
            string deprectioationPerYear = string.Empty;
            var vehicleValue = policyLogDetailsDTO.VehicleValue;
            decimal ? basicPrimum = 0;
            if (autoleasingQuotationForm != null && autoleasingQuotationForm.Depreciation!=null)
            {
                newDep = autoleasingQuotationForm.Depreciation.Replace("%","").Split(',').ToList();
                _depreciationPercentage = int.Parse(newDep[0]);
                deprectioationPerYear = (newDep[0]);
                basicPrimum = autoleasingQuotationForm.Premium1.Value;
                if (autoleasingRenewalPolicyStatistics!=null)
                {
                switch (autoleasingRenewalPolicyStatistics.Year)
                {
                    // renew 1th year
                    case 1:
                        _depreciationPercentage += int.Parse(newDep[1]);
                            deprectioationPerYear = newDep[1];
                            basicPrimum= autoleasingQuotationForm.Premium2.Value;
                            break;
                    // renew 2th year
                    case 2:
                        _depreciationPercentage += int.Parse(newDep[1])+ int.Parse(newDep[2]);
                            deprectioationPerYear = newDep[2];
                            basicPrimum = autoleasingQuotationForm.Premium3.Value;
                            break;
                    // renew 3th year

                    case 3:
                        _depreciationPercentage += int.Parse(newDep[1]) + int.Parse(newDep[2])+ int.Parse(newDep[3]);
                            deprectioationPerYear = newDep[3];
                            basicPrimum = autoleasingQuotationForm.Premium4.Value;
                            break;
                    // renew 4th year
                    case 4:
                        _depreciationPercentage += int.Parse(newDep[1]) + int.Parse(newDep[2]) + int.Parse(newDep[3])+ int.Parse(newDep[4]);
                            deprectioationPerYear = newDep[4];
                            basicPrimum = autoleasingQuotationForm.Premium5.Value;
                            break;
                    default:
                        break;
                }
            }
            }
            result.VehicleValue  = vehicleValue -( vehicleValue * _depreciationPercentage)/100;
            foreach (var price in priceDetails)
            {
                switch (price.PriceTypeCode)
                {
                    // ShadowAmount
                    // Discounts
                    case 1:
                    case 2:
                    case 3:
                    case 10:
                    case 11:
                    case 12:
                        _shadowAmount += price.PriceValue;
                        _discounts += price.PriceValue;
                        break;
                    // ClientAmount
                    // ExtraPremiumPrice
                    case 4:
                    case 5:
                    case 6:
                        _clientAmount += price.PriceValue;
                        _extraPremiumPrice += price.PriceValue;
                        break;
                    // BasicPremium
                    // ExtraPremiumPrice
                    case 7:
                        _basicPremium += price.PriceValue;
                        _extraPremiumPrice += price.PriceValue;
                        break;
                    // Vat
                    // ClientAmount
                    case 8:
                        _vat += price.PriceValue;
                        _clientAmount += price.PriceValue;
                        break;
                    // ClientAmount
                    case 9:
                        _clientAmount += price.PriceValue;
                        break;
                    // Discounts
                    case 13:
                        _discounts += price.PriceValue;
                        break;
                    default:
                        break;
                }
            }
            result.TotalBenefitWitVatPrice = Math.Round(totalBenefit + (totalBenefit * (decimal).15), 2);
            result.ClientTotalBalance = Math.Round(_shadowAmount, 2);
            result.Vat = Math.Round(_vat + (totalBenefit * (decimal).15), 2);
            result.TotalChargedValue = Math.Round((_basicPremium + (result.TotalBenefitWitVatPrice ?? 0) + _vat), 2);
            result.TotalValueOnClient = Math.Round(_clientAmount + _basicPremium + (result.TotalBenefitWitVatPrice ?? 0),2);
            result.ExtraPremiumPrice = Math.Round((_extraPremiumPrice + totalBenefit - (result.Discounts ?? 0)),2) ;
            result.TotalPrice = Math.Round((_basicPremium + _vat + (result.TotalBenefitWitVatPrice ?? 0)- _discounts), 2);
            result.DepreciationPercentage = deprectioationPerYear;
            result.BasicPrimum = basicPrimum;
            result.Differences = Math.Round(((basicPrimum ?? 0) - (result.TotalChargedValue ?? 0)), 2);
            result.Discounts = Math.Round(_discounts, 2);
            result.ClientDepositedBalance = Math.Round(((result.Discounts ?? 0) + (result.Differences ?? 0)), 2);

            return result;
        }

        private string HandleInsuranceRecordDepreciation(DepreciationSettingHistory deprecationHistory, DepreciationSetting deprecationSetting)
        {
            string annualDeprecationPercentage = string.Empty;
            AutoleasingDepreciationSetting depreciationSetting = null;

            if (deprecationHistory != null)
            {
                depreciationSetting = new AutoleasingDepreciationSetting()
                {
                    //BankId = quoteRequest.Bank.Id,
                    MakerCode = deprecationHistory.MakerCode,
                    ModelCode = deprecationHistory.ModelCode,
                    MakerName = deprecationHistory.MakerName,
                    ModelName = deprecationHistory.ModelName,
                    Percentage = deprecationHistory.Percentage,
                    IsDynamic = deprecationHistory.IsDynamic,
                    FirstYear = deprecationHistory.FirstYear,
                    SecondYear = deprecationHistory.SecondYear,
                    ThirdYear = deprecationHistory.ThirdYear,
                    FourthYear = deprecationHistory.FourthYear,
                    FifthYear = deprecationHistory.FifthYear,
                    AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                };
            }
            else
            {
                depreciationSetting = new AutoleasingDepreciationSetting();
                //depreciationSetting.BankId = quoteRequest.Bank.Id;
                depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                depreciationSetting.MakerName = deprecationSetting.MakerName;
                depreciationSetting.ModelName = deprecationSetting.ModelName;
                depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;

            }

            if (depreciationSetting == null)
                return null;

            List<string> annualPercentages = new List<string>();
            if (!depreciationSetting.FirstYear.Equals(null))
                annualPercentages.Add(depreciationSetting.FirstYear.ToString().Replace(".00", "") + "%");
            if (!depreciationSetting.SecondYear.Equals(null))
                annualPercentages.Add(depreciationSetting.SecondYear.ToString().Replace(".00", "") + "%");
            if (!depreciationSetting.ThirdYear.Equals(null))
                annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Replace(".00", "") + "%");
            if (!depreciationSetting.FourthYear.Equals(null))
                annualPercentages.Add(depreciationSetting.FourthYear.ToString().Replace(".00", "") + "%");
            if (!depreciationSetting.FifthYear.Equals(null))
                annualPercentages.Add(depreciationSetting.FifthYear.ToString().Replace(".00", "") + "%");

            return (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Replace(".00", "") + "%";
        }

        #endregion

        public List<DriverModel> GetAdditionalDriversService(string ReferenceId, ref string exception)
        {
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAdditionalDriversData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter referanceIdParameter = new SqlParameter()
                {
                    ParameterName = "ReferenceId",
                    Value = ReferenceId
                };
                command.Parameters.Add(referanceIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                DriverModel additionalDriverOne = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverModel>(reader).FirstOrDefault();
                reader.NextResult();
                DriverModel additionalDriverTwo = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverModel>(reader).FirstOrDefault();
                List<DriverModel> additionalDriversData = new List<DriverModel>() { additionalDriverOne, additionalDriverTwo};

                return additionalDriversData.Where(x => x != null).ToList();
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

    
    }
}

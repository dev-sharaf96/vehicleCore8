using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.AutoleasingDepreciation;

namespace Tameenk.Services.Implementation.AutoleasingDepreciation
{
    public class AutoleasingDepreciationSettingService : IAutoleasingDepreciationSettingService
    {

        #region Fields
        private readonly IRepository<AutoleasingDepreciationSetting> _autoleasingDepreciationSettingRepository;
        #endregion


        public AutoleasingDepreciationSettingService(IRepository<AutoleasingDepreciationSetting> autoleasingDepreciationSettingRepository)
        {
            this._autoleasingDepreciationSettingRepository = autoleasingDepreciationSettingRepository;

        }
        public AutoleasingDepreciationSetting AddDepressionSetting(AutoleasingDepreciationSetting setting, string userId)
        {
            try
            {
                AutoleasingDepreciationSetting depreciationSetting = new AutoleasingDepreciationSetting();
                depreciationSetting.BankId = setting.BankId;
                depreciationSetting.ThirdYear = setting.ThirdYear;
                depreciationSetting.AnnualDepreciationPercentage = setting.AnnualDepreciationPercentage;
                depreciationSetting.FifthYear = setting.FifthYear;
                depreciationSetting.FirstYear = setting.FirstYear;
                depreciationSetting.FourthYear = setting.FourthYear;
                depreciationSetting.IsDynamic = setting.IsDynamic;
                depreciationSetting.MakerCode = setting.MakerCode;
                depreciationSetting.ModelCode = setting.ModelCode;
                depreciationSetting.Percentage = setting.Percentage;
                depreciationSetting.SecondYear = setting.SecondYear;
                depreciationSetting.ModelName = setting.ModelName;
                depreciationSetting.MakerName = setting.MakerName;
                depreciationSetting.CreatedBy = userId;
                depreciationSetting.CreatedDate = DateTime.Now;
                _autoleasingDepreciationSettingRepository.Insert(depreciationSetting);
                return depreciationSetting;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteDepressionSetting(AutoleasingDepreciationSetting setting)
        {
            try
            {
                _autoleasingDepreciationSettingRepository.Delete(setting);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EditDepressionSetting(AutoleasingDepreciationSetting setting, string userId)
        {
            try
            {
                _autoleasingDepreciationSettingRepository.Update(setting);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AutoleasingDepreciationSetting GetDepressionSetting(int id)
        {
            return _autoleasingDepreciationSettingRepository.Table.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<AutoleasingDepreciationSetting> GetDepressionSettings(int bankId)
        {
            return _autoleasingDepreciationSettingRepository.TableNoTracking.Where(x => x.BankId == bankId).ToList();
        }

        public bool IsBankWithModelMakerDepreciationExist(int BankId, int MakerCode, int ModelCode)
        {

            var isExist = _autoleasingDepreciationSettingRepository.TableNoTracking.Where(x => x.MakerCode == MakerCode && x.ModelCode == ModelCode && x.BankId == BankId).FirstOrDefault();
            if (isExist != null)
                return true;
            return false;
        }

        public bool UpdateDepressionSetting(AutoleasingDepreciationSetting setting, string userId)
        {
            try
            {
                var settingUpdated = _autoleasingDepreciationSettingRepository.Table.Where(x => x.Id == setting.Id).FirstOrDefault();
                settingUpdated.BankId = setting.BankId == 0 ? settingUpdated.BankId : setting.BankId;
                settingUpdated.AnnualDepreciationPercentage = string.IsNullOrEmpty(setting.AnnualDepreciationPercentage) ? settingUpdated.AnnualDepreciationPercentage : setting.AnnualDepreciationPercentage;
                settingUpdated.FifthYear = setting.FifthYear == 0 ? settingUpdated.FifthYear : setting.FifthYear;
                settingUpdated.FirstYear = setting.FirstYear == 0 ? settingUpdated.FirstYear : setting.FirstYear;
                settingUpdated.FourthYear = setting.FourthYear == 0 ? settingUpdated.FourthYear : setting.FourthYear;
                settingUpdated.IsDynamic = setting.IsDynamic == null ? setting.IsDynamic : setting.IsDynamic;
                settingUpdated.MakerCode = setting.MakerCode == 0 ? settingUpdated.MakerCode : setting.MakerCode;
                settingUpdated.ModelCode = setting.ModelCode == 0 ? settingUpdated.ModelCode : setting.ModelCode;
                settingUpdated.Percentage = setting.Percentage == 0 ? settingUpdated.Percentage : setting.Percentage;
                settingUpdated.SecondYear = setting.SecondYear == 0 ? settingUpdated.SecondYear : setting.SecondYear;
                settingUpdated.ThirdYear = setting.ThirdYear == 0 ? settingUpdated.SecondYear : setting.ThirdYear;
                settingUpdated.ModifiedBy = userId;
                settingUpdated.ModifiedDate = DateTime.Now;
                _autoleasingDepreciationSettingRepository.Update(settingUpdated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

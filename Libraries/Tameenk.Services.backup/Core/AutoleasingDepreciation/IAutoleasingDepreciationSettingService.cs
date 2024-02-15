using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.AutoleasingDepreciation
{
    public interface IAutoleasingDepreciationSettingService
    {

        AutoleasingDepreciationSetting GetDepressionSetting(int id);
        AutoleasingDepreciationSetting AddDepressionSetting(AutoleasingDepreciationSetting setting, string userId);
        List<AutoleasingDepreciationSetting> GetDepressionSettings(int bankId);
        bool DeleteDepressionSetting(AutoleasingDepreciationSetting setting);
        bool EditDepressionSetting(AutoleasingDepreciationSetting setting, string userId);
        bool UpdateDepressionSetting(AutoleasingDepreciationSetting setting, string userId);
        bool IsBankWithModelMakerDepreciationExist(int BankId,int MakerCode,int ModelCode);
    }
}

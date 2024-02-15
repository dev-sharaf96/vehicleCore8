using System.Collections.Generic;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Services.Core.Settings
{
    public interface ISettingService
    {
        /// <summary>
        /// Get First Setting
        /// </summary>
        /// <returns></returns>
        Setting GetSetting();


        /// <summary>
        /// Save Setting.
        /// </summary>
        /// <param name="entity">Entity to be added or updated</param>
        /// <returns></returns>
        void Save(Setting entity);

       
    }
}

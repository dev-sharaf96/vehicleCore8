using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Settings;

namespace Tameenk.Services.Implementation.Settings
{
    public class SettingService : ISettingService
    {
        #region fields
        private readonly IRepository<Setting> repository;
        #endregion

        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="productTypeRepository">product type Repository</param>
        public SettingService(IRepository<Setting> repository)
        {
            this.repository = repository ?? throw new TameenkArgumentNullException(nameof(IRepository<ProductType>));
        }
        #endregion region 


        public Setting GetSetting()
        {
            return repository.TableNoTracking.FirstOrDefault();
        }

        public void Save(Setting entity)
        {
            if (entity.Id == 0)
                repository.Insert(entity);
            else
            {
                var data = GetSetting();
                data.MaxNumberOfPolicies = entity.MaxNumberOfPolicies;
                data.MaxNumberOfPromotionCode = entity.MaxNumberOfPromotionCode;
                repository.Update(data);
            }
               
        }
    }
}

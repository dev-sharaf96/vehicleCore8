using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Repository;

namespace Tameenk.Services.Extensions
{
    public static class YakeenEntityExtension
    {
        //static Delegate void updateMethod(YakeenEntity);

        public static bool IsValid(this YakeenEntity entity, Action<YakeenEntity> updateMethod)
        {
            if (!entity.IsDeleted)
            {
                if (entity.CreatedDateTime == null || !entity.CreatedDateTime.HasValue)
                {
                    entity.IsDeleted = true;
                    updateMethod(entity);
                    return false;
                }
                var dateTimeDiff = entity.CreatedDateTime.HasValue
                            ? (DateTime.Now - entity.CreatedDateTime.Value)
                            : TimeSpan.FromDays(RepositoryConstants.YakeenDataThresholdNumberOfDaysToInvalidate + 1);

                if (dateTimeDiff > TimeSpan.FromDays(RepositoryConstants.YakeenDataThresholdNumberOfDaysToInvalidate))
                {
                    entity.IsDeleted = true;
                    updateMethod(entity);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}

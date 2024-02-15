using System.Collections.Generic;

namespace Tameenk.Services.Administration.Identity.Core.Repositories
{
    public interface ICUDRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(List<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);
    }
}

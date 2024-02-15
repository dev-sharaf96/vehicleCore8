using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Servicies
{
    public interface ICRUDService<TEntity> : IService<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(List<TEntity> entities);

        void Update(TEntity entity);
        void Remove(int id);
        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}

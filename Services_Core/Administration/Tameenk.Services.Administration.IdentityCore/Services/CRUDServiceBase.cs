using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class CRUDServiceBase<TEntity, TRepository> : ServiceBase<TEntity, TRepository>, ICRUDService<TEntity> where TEntity : class
         where TRepository : ICUDRepository<TEntity>
    {
        public CRUDServiceBase(IUnitOfWork unitOfWork, TRepository repository) : base(unitOfWork, repository)
        {

        }

        public virtual void Add(TEntity entity)
        {
            repository.Add(entity);
        }

        public virtual void AddRange(List<TEntity> entities)
        {
            repository.AddRange(entities);
        }

        public virtual void Remove(int id)
        {
            Remove(Get(id));
        }
        public virtual void Remove(TEntity entity)
        {
            repository.Remove(entity);
        }

        public virtual void RemoveRange(List<TEntity> entities)
        {
            repository.RemoveRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            repository.Update(entity);
        }

        public virtual int SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }
        public virtual async Task<int> SaveChangesAsync()
        {
            return unitOfWork.SaveChangesAsync().Result;
        }
    }
}

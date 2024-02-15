using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public abstract class ServiceBase<TEntity, TRepository> : IService<TEntity> where TEntity : class
         where TRepository : IDataRepository<TEntity>
    {
        public readonly IUnitOfWork unitOfWork;
        public readonly TRepository repository;
        public ServiceBase(IUnitOfWork unitOfWork, TRepository repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }


        public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
            => repository.Find(predicate, orderBy, includeProperties);

        public virtual TEntity Get(int id)
            => repository.Get(id);

        public virtual List<TEntity> GetAll()
        => repository.GetAll();

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        => repository.GetSingleOrDefault(predicate);
    }
}

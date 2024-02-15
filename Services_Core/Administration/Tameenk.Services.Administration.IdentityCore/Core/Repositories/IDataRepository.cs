using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tameenk.Services.Administration.Identity.Core.Repositories
{
    public interface IDataRepository<TEntity> where TEntity : class
    {
        List<TEntity> Find(Expression<Func<TEntity, bool>> predicate,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             string includeProperties = "");
        TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        List<TEntity> GetAll();
    }
}

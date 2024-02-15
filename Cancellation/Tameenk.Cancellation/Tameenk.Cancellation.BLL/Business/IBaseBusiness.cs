using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Tameenk.Cancellation.BLL.Business
{
    public interface IBaseBusiness<TEntity>
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(int Id);
        int Count();
        List<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        List<TEntity> GetAll();
        List<TEntity> GetActive();
        List<TEntity> GetInactive();
    }
}

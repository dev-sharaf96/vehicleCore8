using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public abstract class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
    {
        protected readonly IIdentityDbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public DataRepository(IIdentityDbContext context)
        {
            _context = context;
           _entities = context.Set<TEntity>();
        }

        //protected virtual IDbSet<TEntity> Entities
        //{
        //    get
        //    {
        //        if (_entities == null)
        //            _entities = _context.Set<TEntity>();
        //        return _entities;
        //    }
        //}

        public virtual int Count()
        {
            return _entities.Count();
        }


        public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate,
               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            var query = _entities.Where(predicate);
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }

        public virtual TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public virtual List<TEntity> GetAll()
        {
            return _entities.ToList();
        }
    }
}

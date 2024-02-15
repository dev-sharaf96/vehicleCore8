using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public abstract class CUDRepository<TEntity> : DataRepository<TEntity>, ICUDRepository<TEntity> where TEntity : class
    {
        //protected readonly DbContext _context;
        //protected readonly DbSet<TEntity> _entities;

        public CUDRepository(IIdentityDbContext context) : base(context)
        {
            //_context = context;
            //_entities = context.Set<TEntity>();
        }

        public virtual void Add(TEntity entity) => _entities.Add(entity);

        public virtual void AddRange(List<TEntity> entities) => _entities.AddRange(entities);


        public virtual void Update(TEntity entity)
        {
            _entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(TEntity entity) => _entities.Remove(entity);

        public virtual void RemoveRange(List<TEntity> entities) => _entities.RemoveRange(entities);
    }
}

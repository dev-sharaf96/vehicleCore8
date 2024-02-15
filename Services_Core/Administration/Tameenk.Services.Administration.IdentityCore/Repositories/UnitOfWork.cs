using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IIdentityDbContext context;
        //private IPageRepository pages;
        //private IUserPageRepository userPages;
        public UnitOfWork(IIdentityDbContext context)
        {
            this.context = context;
        }

        //public IIdentityDbContext AdminIdentityContext { set => context = value; }
        //public IPageRepository Pages => pages == null ? pages = new PageRepository(context) : pages;
        //public IUserPageRepository UserPages => userPages == null ? userPages = new UserPageRepository(context) : userPages;


        private Dictionary<string, object> repositories;
        public T Repository<T>()
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(T);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);
                repositories.Add(type, repositoryInstance);
            }
            return (T)repositories[type];
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync().Result;
        }
    }
}

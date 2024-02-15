using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Repositories;

namespace Tameenk.Services.Administration.Identity.Core.Repositories
{
    public interface IUnitOfWork
    {
        //IIdentityDbContext AdminIdentityContext { set; }
        //IPageRepository Pages { get; }
        //IUserPageRepository UserPages { get; }
         T Repository<T>();
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}

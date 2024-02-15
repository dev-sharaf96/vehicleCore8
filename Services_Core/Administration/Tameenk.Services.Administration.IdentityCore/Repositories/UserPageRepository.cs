using System.Data.Entity;
using System.Linq;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public class UserPageRepository : CUDRepository<UserPage>, IUserPageRepository
    {
        public UserPageRepository(IIdentityDbContext context) : base(context)
        { }

        private IIdentityDbContext _appContext => (IIdentityDbContext)_context;

    }
}

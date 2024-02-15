using System.Collections.Generic;
using System.Linq;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public class ExpiredTokensRepository : CUDRepository<ExpiredTokens>, IExpiredTokensRepository
    {
        public ExpiredTokensRepository(IIdentityDbContext context) : base(context)
        { }

        private IIdentityDbContext _appContext => (IIdentityDbContext)_context;

       
    }
}

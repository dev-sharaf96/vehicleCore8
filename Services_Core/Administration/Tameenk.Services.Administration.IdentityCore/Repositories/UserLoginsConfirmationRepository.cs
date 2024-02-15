using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    public class UserLoginsConfirmationRepository : CUDRepository<UserLoginsConfirmation>, IUserLoginsConfirmationRepository
    {
        public UserLoginsConfirmationRepository(IIdentityDbContext context) : base(context)
        { }

        private IIdentityDbContext _appContext => (IIdentityDbContext)_context;
    }
}

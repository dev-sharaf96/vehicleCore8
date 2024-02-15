using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class IdentityLogService : CRUDServiceBase<IdentityLog, IIdentityLogRepository>, IIdentityLogService
    {

        public IdentityLogService(IUnitOfWork unitOfWork, IIdentityLogRepository identityLogRepository)
            : base(unitOfWork, identityLogRepository)
        {

        }


    }
}

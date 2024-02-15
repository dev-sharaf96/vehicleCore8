using System.Collections.Generic;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class ExpiredTokensService : CRUDServiceBase<ExpiredTokens, IExpiredTokensRepository>, IExpiredTokensService
    {
        private readonly IExpiredTokensRepository expiredTokensRepository;

        public ExpiredTokensService(IUnitOfWork unitOfWork, IExpiredTokensRepository expiredTokensRepository)
            : base(unitOfWork, expiredTokensRepository)
        {

            this.expiredTokensRepository = expiredTokensRepository;
        }

        public List<ExpiredTokens> GetToken(string token)
        {
            return expiredTokensRepository.Find(x => x.Token == token);
        }
    }
}

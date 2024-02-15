using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class PageService : CRUDServiceBase<Page, IPageRepository>, IPageService
    {
        private readonly IPageRepository pageRepository;

        public PageService(IUnitOfWork unitOfWork, IPageRepository pageRepository)
            : base(unitOfWork, pageRepository)
        {

            this.pageRepository = pageRepository;
        }

    }
}

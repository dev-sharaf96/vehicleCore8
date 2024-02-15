using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class UsersLocationsDeviceInfoService : CRUDServiceBase<UsersLocationsDeviceInfo, IUsersLocationsDeviceInfoRepository>, IUsersLocationsDeviceInfoService
    {

        public UsersLocationsDeviceInfoService(IUnitOfWork unitOfWork, IUsersLocationsDeviceInfoRepository usersDeviceRepository)
            : base(unitOfWork, usersDeviceRepository)
        {

        }


    }
}

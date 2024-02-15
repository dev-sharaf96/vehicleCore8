using System.Collections.Generic;
using Tameenk.Services.Administration.Identity.Core.Domain;

namespace Tameenk.Services.Administration.Identity.Core.Servicies
{
    public interface IUserPageService : ICRUDService<UserPage>
    {
        void SaveUserPages(int id, List<UserPage> userPages);
        bool IsAuthorizedUser(int pageId, string userIdString);
    }
}

using System.Collections.Generic;
using Tameenk.Services.Administration.Identity.Core.Domain;

namespace Tameenk.Services.Administration.Identity.Core.Servicies
{
    public interface IUserService : ICRUDService<AppUser>
    {
        bool SendVerificationCode(string userPone, int code, out string exception);
    }
}

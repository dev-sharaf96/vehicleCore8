using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Domain;

namespace Tameenk.Services.Administration.Identity.Core.Servicies
{
    public interface IUserLoginsConfirmationService : ICRUDService<UserLoginsConfirmation>
    {
        List<UserLoginsConfirmation> GetAll(int userId);
        UserLoginsConfirmation CheckVerificationCodeExist(int code);
        List<UserLoginsConfirmation> GetAllActiveCode(int userId);
    }
}

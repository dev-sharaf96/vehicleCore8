using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Core
{
    public interface IAutoleasingUserService
    {
        AutoleasingUser GetUserByPhone(string phoneNumber);
        AutoleasingUser GetUserByEmail(string email);
        bool UpdateUser(AutoleasingUser user);
        bool AddUser(AutoleasingUser user);
        List<AutoleasingUser> GetUsers(string parentId);
        AutoleasingUser GetUser(string id);
        List<AutoleasingUser> GetSuperAdmins();
        List<AutoleaseUserModel> GetAutoleaseUsersWithFilter(AutoleaseFilterModel filterModel, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception);
        List<AutoleasingUsersModel> GetModelUsers(int bankId);
        AutoleasingUsersModel GetModelUser(string userId);
    }
}

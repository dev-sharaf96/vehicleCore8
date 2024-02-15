using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.Leasing
{
    public interface ILeasingUserService
    {
        LeasingUser GetUserByEmail(string email);
        LeasingUser GetUserByIdForUpdate(string Id);
        LeasingUser GetUserByIdAndRefId(string Id, string referenceId);
        bool UpdateUserInfo(LeasingUser user, out string exception);
        LeasingUser GetUser(string UserId);

    }
}

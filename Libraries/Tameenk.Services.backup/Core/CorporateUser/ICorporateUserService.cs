using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core
{
    public interface ICorporateUserService
    {
        List<CorporateUserModel> GetCorporateUsersWithFilter(CorporateFilterModel filterModel, int pageIndex, int pageSize, int commandTimeout, bool export, out int totalCount, out string exception);
        List<CorporateUsers> GetAccountUsers(int accountId);
        void UpdateBulkusers(List<CorporateUsers> users, out string exception);
        List<CorporateNotificationModel> GetCorporateUsersLessThan2000(out string exception);
        bool UpdateCorporateUsersWithLastNotification(string username, out string exception);
    }
}

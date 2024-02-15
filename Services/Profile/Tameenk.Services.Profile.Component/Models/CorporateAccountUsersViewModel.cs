using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class CorporateAccountUsersViewModel
    {
        public List<CorporateUserModel> CorporateUsersList { get; set; }
        public int CorporateUsersTotalCount { get; set; }
        public int CurrentPage { get; set; }
        public string Lang { get; internal set; }
    }
}

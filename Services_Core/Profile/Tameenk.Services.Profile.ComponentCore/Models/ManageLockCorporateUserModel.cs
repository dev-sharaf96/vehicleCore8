using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component.Models
{
    public class ManageLockCorporateUserModel : BaseViewModel
    {
        public string AdminCorporateUserId { get; set; }
        public string CorporateUserId { get; set; }
        public bool Lock { get; set; }
        public int CorporateAccountId { get; set; }
    }
}

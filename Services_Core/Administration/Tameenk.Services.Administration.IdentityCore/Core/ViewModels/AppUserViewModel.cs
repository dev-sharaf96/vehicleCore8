using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.ViewModels
{
    public class AppUserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public bool ChangePasswordAfterLogin { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsActivated { get; set; } = true;

        public int? CompanyId { get; set; }
    }
}

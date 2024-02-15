using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    public interface ICustomUser
    {
        string Email { get; set; }
        string UserName { get; set; }
    }

    [Table("Users")]
    public class AppUser : IdentityUser<int, UserLogin, UserRole, UserClaim>, IUser<int>
    {
        public override string Email { get; set; }
        public override string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool ChangePasswordAfterLogin { get; set; } = true;

        [DefaultValue(false)]
        public bool IsAdmin { get; set; } = false;

        [DefaultValue(true)]
        public bool IsActivated { get; set; } = true;

        public int? CompanyId { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public List<int> PageIds { get; set; } = new List<int>();

        public AppUser()
        {
            SecurityStamp = Guid.NewGuid().ToString();
        }

    }
}

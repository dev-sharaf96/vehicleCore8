using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    
    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<int>
    {
        [ForeignKey("RoleId")]
        public virtual RoleEntity Role { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }
}

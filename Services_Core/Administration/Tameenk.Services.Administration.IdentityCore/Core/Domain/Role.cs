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
    
    [Table("Roles")]
    public class RoleEntity : IdentityRole<int, UserRole>
    {
        public string Title { get; set; }
        public string TitleEn { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}

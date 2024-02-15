using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain.Entities
{
    public class LoginSwitchAccount : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Nin { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

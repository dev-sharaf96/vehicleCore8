using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class CorporateUsers : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
        public int CorporateAccountId { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public DateTime? NotificationDate { get; set; }
    }
}

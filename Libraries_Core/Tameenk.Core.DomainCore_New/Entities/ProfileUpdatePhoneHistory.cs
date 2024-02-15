using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class ProfileUpdatePhoneHistory : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Year { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdateTypeId { get; set; }
    }
}

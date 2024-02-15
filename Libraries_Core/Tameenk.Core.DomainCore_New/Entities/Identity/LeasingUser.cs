using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class LeasingUser : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UpdatedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool? LockoutEnabled { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }    // also represent the domain
        //public bool? IsFirstLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }

        public string ReferenceId { get; set; }
        public string DriverId { get; set; }
        public string VehicleId { get; set; }
        public string DriverNin { get; set; }
        public string VehicleSequenceOrCustom { get; set; }
    }
}

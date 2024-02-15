using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class UserInfo :BaseEntity
    {
        public int Id { set; get; }
        public string NationalId { get; set; }
        public string SequenceNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDriverExist { get; set; }
        public bool IsVechileExist { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? VechileId { get; set; }
        public int BirthDateYear { get; set; }
        public int BirthDateMonth { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public bool IsActive { get; set; }
        public int? OTP { get; set; }
        public DateTime? OTPCreatedDate { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string Hashed { get; set; }

    }
}
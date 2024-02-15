using System;

namespace Tameenk.Core.Domain.Entities
{
    public class UpdateProfileInfoOtp : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int VerificationCode { get; set; }
        public bool IsCodeVerified { get; set; }
        public int ProfileInfoTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}

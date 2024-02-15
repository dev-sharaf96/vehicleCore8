using System;

namespace Tameenk.Core.Domain.Entities
{
    public class ForgotPasswordToken : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; }
        public int VerificationCode { get; set; }
        public bool IsCodeVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ForgotPasswordVerificationType { get; set; }
    }
}

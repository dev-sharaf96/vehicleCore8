using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class OtpInfo : BaseEntity
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string UserEmail { get; set; }

        public int VerificationCode { get; set; }

        public bool IsCodeVerified { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string Nin { get; set; }

    }
}

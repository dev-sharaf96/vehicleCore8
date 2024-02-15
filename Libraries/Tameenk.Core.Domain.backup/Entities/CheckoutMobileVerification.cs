using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutMobileVerification : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string NationalId { get; set; }
        public string Phone { get; set; }
        public bool IsYakeenMobileVerified { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

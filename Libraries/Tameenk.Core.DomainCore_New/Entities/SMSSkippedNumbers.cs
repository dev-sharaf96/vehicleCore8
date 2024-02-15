using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class SMSSkippedNumbers : BaseEntity
    {
        public int Id { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string CreatedBy { get; set; }
    }
}

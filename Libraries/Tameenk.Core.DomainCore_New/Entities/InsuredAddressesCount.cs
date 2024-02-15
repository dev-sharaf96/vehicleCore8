using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class InsuredAddressesCount : BaseEntity
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public int? YakeenAddressesCount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

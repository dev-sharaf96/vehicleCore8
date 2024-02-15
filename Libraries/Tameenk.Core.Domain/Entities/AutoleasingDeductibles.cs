using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingDeductibles : BaseEntity
    {

        public int Id { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
    }
}

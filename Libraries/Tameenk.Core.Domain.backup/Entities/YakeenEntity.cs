using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class YakeenEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}

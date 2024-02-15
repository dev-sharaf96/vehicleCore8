using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
  public class WareefDiscounts : BaseEntity
    {
        public int Id  { get; set; }
        public string  DescountValue { get; set; }
        public int? WareefId { get; set; }
        public string WDescountCode { get; set; }
    }
}

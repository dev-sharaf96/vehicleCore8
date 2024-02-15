using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services
{
   public class DiscountItem
    {
        public int Id { get; set; }
        public string DescountValue { get; set; }
        public int? WareefId { get; set; }
        public string WDescountCode { get; set; }
        public List<WaeefDicscountItemsDetails> WaeefDicscountItemsDetails { set; get; }
    }
}

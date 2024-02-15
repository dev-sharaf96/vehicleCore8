using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Wareefservices
{
    public class WareefDiscountsListModel
    {
        public int Id { get; set; }
        public string DiscountValue { get; set; }
        public int? WareefId { get; set; }
        public string WareefDiscountCode { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
}

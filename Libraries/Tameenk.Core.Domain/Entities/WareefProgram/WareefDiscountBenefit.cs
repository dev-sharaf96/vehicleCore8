using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
   public class WareefDiscountBenefit : BaseEntity
    {
        public int Id { set; get; }
        public string BenefitDescriptionAr { get; set; }
        public string BenefitDescriptionEn { get; set; }
        public bool IsDeleted { get; set; }
        public int DescountId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
   public class UserPurchasedPromotionPrograms: BaseEntity
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string UserId { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public PromotionProgram PromotionProgram { get; set; }
        public AspNetUser User { get; set; }
        public InsuranceCompany InsuranceCompany { set; get; }
    }
}

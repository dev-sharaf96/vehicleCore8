using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
   public class Setting : BaseEntity
    {
        public int Id { get; set; }
        public int MaxNumberOfPolicies { get; set; }
        public int MaxNumberOfPromotionCode { get; set; }
    }
}

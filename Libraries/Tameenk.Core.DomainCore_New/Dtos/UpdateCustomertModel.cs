using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class UpdateCustomertModel
    {
        public int PoliciesCount { get; set; }
        public int PromotionCodeCount{ get; set; }
        public string UserId { get; set; }
    }
}

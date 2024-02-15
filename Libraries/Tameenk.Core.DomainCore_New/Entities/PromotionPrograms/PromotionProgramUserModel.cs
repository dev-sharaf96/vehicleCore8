using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgramUserModel 
    {
      
        public int PromotionProgramId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }

    }
}

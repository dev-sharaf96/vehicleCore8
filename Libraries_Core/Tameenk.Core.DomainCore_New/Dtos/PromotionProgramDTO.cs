using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class PromotionProgramDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? DeactivatedDate { get; set; }
        public int ValidationMethodId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsPromoByEmail { get; set; }
    }
}

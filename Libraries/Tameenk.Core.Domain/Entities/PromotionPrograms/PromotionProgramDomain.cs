using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgramDomain : BaseEntity
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string Domian { get; set; }
        public string DomainNameAr { get; set; }
        public string DomainNameEn { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool? IsActive { get; set; } = true;

        public PromotionProgram PromotionProgram { get; set; }

    }
}

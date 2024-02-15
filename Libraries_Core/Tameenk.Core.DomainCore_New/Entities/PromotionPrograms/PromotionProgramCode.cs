using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgramCode : BaseEntity
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string Code { get; set; }
        public int InsuranceCompanyId { get; set; }
        public bool IsDeleted { get; set; }

        public short InsuranceTypeCode { get; set; }

        public  string CreatorId { get; set; }
        public string ModifierId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }

        public AspNetUser Modifier { get; set; }
        public AspNetUser Creator { get; set; }

        public PromotionProgram PromotionProgram { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }
        public ProductType ProductType{ get; set; }

    }
}

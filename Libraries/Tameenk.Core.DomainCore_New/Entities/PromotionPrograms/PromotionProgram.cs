using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgram : BaseEntity
    {
        public PromotionProgram()
        {
            PromotionProgramDomains = new HashSet<PromotionProgramDomain>();
            PromotionProgramUsers = new HashSet<PromotionProgramUser>();
            PromotionProgramCodes = new HashSet<PromotionProgramCode>();
            PromotionProgramNins = new HashSet<PromotionProgramNins>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Start date of promotion program
        /// </summary>
        public DateTime? EffectiveDate { get; set; }
        /// <summary>
        /// End date of promotion program
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }
        public int ValidationMethodId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsPromoByEmail { get; set; }
        public bool IsPromoByNin { get; set; }
        public bool IsPromoByAttachment { get; set; }
        public bool? EnableService { get; set; }


        public ICollection<PromotionProgramDomain> PromotionProgramDomains { get; set; }
        public ICollection<PromotionProgramUser> PromotionProgramUsers { get; set; }
        public ICollection<PromotionProgramCode> PromotionProgramCodes { get; set; }
        public ICollection<PromotionProgramNins> PromotionProgramNins { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgramAttachments : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ProgramId { get; set; }
        public string UserId { get; set; }
        public string FilePath { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}

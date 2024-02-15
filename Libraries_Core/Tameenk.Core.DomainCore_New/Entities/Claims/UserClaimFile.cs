using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class UserClaimFile : BaseEntity
    {
        public int Id { get; set; }
        public int ClaimHistoryId { get; set; }
        public string ClaimFilePath { get; set; }
        public string ClaimFileName { get; set; }
        public string ClaimFileExtension { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}

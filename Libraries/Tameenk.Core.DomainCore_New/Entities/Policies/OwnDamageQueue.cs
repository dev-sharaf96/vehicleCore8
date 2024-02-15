using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
   public class OwnDamageQueue : BaseEntity
    {
        public int Id { get; set; }
        public string PolicyNo { get; set; }
        public string ExternalId { get; set; }
        public string ErrorDescription { get; set; }
        public int? SelectedLanguage { get; set; }
        public int ProcessingTries { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Phone { get; set; }
        public string Method { get; set; }
        public bool IsLocked { get; set; }
    }
}

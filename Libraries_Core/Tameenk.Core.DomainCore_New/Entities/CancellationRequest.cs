using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Policies
{
public  class CancellationRequest :  BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? CancellationReasonCode { get; set; }
        public string CancellationAttachment { get; set; }
        public string UserName{ get; set; }
        public bool? IsAutolease { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}

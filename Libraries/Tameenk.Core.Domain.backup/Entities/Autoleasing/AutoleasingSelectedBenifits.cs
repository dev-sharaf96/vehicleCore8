using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    
    public class AutoleasingSelectedBenifits : BaseEntity
    {
        public AutoleasingSelectedBenifits(){ }

        public int Id { get; set; }
        public Guid ParentRequestId { get; set; }
        public string ExternalId { get; set; }
        public short BenifitId { get; set; }

    }
}

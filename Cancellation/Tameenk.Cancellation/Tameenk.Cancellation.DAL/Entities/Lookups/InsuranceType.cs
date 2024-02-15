using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.DAL.Entities
{
    public class  InsuranceType : IAuditableEntity
    {
        public int Code { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

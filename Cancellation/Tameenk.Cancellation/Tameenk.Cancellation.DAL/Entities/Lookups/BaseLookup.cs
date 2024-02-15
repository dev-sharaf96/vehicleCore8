using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tameenk.Cancellation.DAL.Entities
{
   public abstract class BaseLookup : IAuditableEntity
    {
        public int Code { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

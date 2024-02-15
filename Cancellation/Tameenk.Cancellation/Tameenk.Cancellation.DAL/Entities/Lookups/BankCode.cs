using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Cancellation.DAL.Entities
{
    public class BankCode : IAuditableEntity
    {
        public string Code { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

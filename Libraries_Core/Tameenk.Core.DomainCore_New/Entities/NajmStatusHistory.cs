using System;

namespace Tameenk.Core.Domain.Entities
{
    public class NajmStatusHistory : BaseEntity
    {
        public int Id { get; set; }
        
        public string ReferenceId { get; set; }
        
        public string PolicyNo { get; set; }

        public int StatusCode { get; set; }
        
        public string StatusDescription { get; set; }

        public DateTime? UploadedDate { get; set; }
        
        public string UploadedReference { get; set; }
    }
}

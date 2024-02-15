using System;

namespace Tameenk.Core.Domain.Entities
{
    public class InvoiceFile : BaseEntity
    {
        public int Id { get; set; }
        
        public byte[] InvoiceData { get; set; }

        public string FilePath { get; set; }

        public string ServerIP { get; set; }

        public Invoice Invoice { get; set; }

        public int? TemplateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CompanyInvoieFilePath { get; set; }
    }
}

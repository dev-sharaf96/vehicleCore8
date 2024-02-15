using System;

namespace Tameenk.Core.Domain.Entities
{
    public class InvoiceFileTemplates : BaseEntity
    {
        public int Id { get; set; }
        public string TemplateFilePath { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? Active { get; set; }
    }
}

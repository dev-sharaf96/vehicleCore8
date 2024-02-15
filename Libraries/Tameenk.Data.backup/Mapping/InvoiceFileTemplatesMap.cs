using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InvoiceFileTemplatesMap : EntityTypeConfiguration<InvoiceFileTemplates>
    {
        public InvoiceFileTemplatesMap()
        {
            ToTable("InvoiceFileTemplates");
            HasKey(e => e.Id);
        }
    }
}

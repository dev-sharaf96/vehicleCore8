using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InvoiceFileMap : EntityTypeConfiguration<InvoiceFile>
    {
        public InvoiceFileMap()
        {
            ToTable("InvoiceFile");
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.InvoiceData).HasColumnType("image");
        }
    }
}

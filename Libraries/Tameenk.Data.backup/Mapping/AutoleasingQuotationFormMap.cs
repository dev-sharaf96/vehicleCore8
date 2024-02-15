using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping.Quotations
{
    public class AutoleasingQuotationFormMap : EntityTypeConfiguration<AutoleasingQuotationForm>
    {
        public AutoleasingQuotationFormMap()
        {
            ToTable("AutoleasingQuotationForm");
            HasKey(e => e.Id);
        }
    }
}
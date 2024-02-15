using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping.Quotations
{
    public class TawuniyaProposalMap : EntityTypeConfiguration<TawuniyaProposal>
    {
        public TawuniyaProposalMap()
        {
            ToTable("TawuniyaProposal");
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class NajmAccidentResponseMap : EntityTypeConfiguration<NajmAccidentResponse>
    {
        public NajmAccidentResponseMap() {
            ToTable("NajmAccidentResponse");
            HasKey(c => c.Id);
        }
    }
}
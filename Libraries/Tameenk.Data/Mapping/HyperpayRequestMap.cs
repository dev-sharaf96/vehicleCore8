using Tameenk.Core.Domain.Entities.Payments.RiyadBank;

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping
{
    public class HyperpayRequestMap : EntityTypeConfiguration<HyperpayRequest>
    {
        public HyperpayRequestMap()
        {
            ToTable("HyperpayRequest");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(e => e.Amount)
                .HasPrecision(10, 4);

            //HasMany(e => e.HyperpayResponses)
            //    .WithRequired(e => e.Id)
            //    .HasForeignKey(e => e.HyperpayRequestId)
            //    .WillCascadeOnDelete(false);
        }
    }


    public class HyperpayResponseMap : EntityTypeConfiguration<HyperpayResponse>
    {
        public HyperpayResponseMap()
        {
            ToTable("HyperpayResponse");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(e => e.Amount)
                .HasPrecision(10, 4);

            //HasMany(e => e.HyperpayResponses)
            //    .WithRequired(e => e.Id)
            //    .HasForeignKey(e => e.HyperpayRequestId)
            //    .WillCascadeOnDelete(false);
        }
    }
}

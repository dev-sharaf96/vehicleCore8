using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;

namespace Tameenk.Data.Mapping.Payments.RiyadBank
{
    public class RiyadBankMigsResponseMap : EntityTypeConfiguration<RiyadBankMigsResponse>
    {
        public RiyadBankMigsResponseMap()
        {
            ToTable("RiyadBankMigsResponse");
            HasKey(e => e.Id);

            Property(e => e.Amount).HasPrecision(19, 4);

            HasRequired(e => e.RiyadBankMigsRequest).WithMany(e => e.RiyadBankMigsResponses).HasForeignKey(e => e.RiyadBankMigsRequestId);
        }
    }
}

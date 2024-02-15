using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AdditionalInfoMap: EntityTypeConfiguration<AdditionalInfo>
    {
        public AdditionalInfoMap()
        {
            ToTable("AdditionalInfo");
           // Property(e => e.CheckoutDetailsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasKey(e => e.ReferenceId);
            HasRequired(e => e.CheckoutDetail)
                .WithOptional(c => c.AdditionalInfo);
        }
    }
}

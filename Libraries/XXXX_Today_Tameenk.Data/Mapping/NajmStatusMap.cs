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
   public class NajmStatusMap : EntityTypeConfiguration<NajmStatus>
    {
        public NajmStatusMap() 
        {
            ToTable("NajmStatus");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.NameAr).IsRequired().HasMaxLength(200);
            Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            Property(e => e.Code).IsRequired().HasMaxLength(50);



            HasMany<Policy>(g => g.Policies).WithRequired(s => s.NajmStatusObj).HasForeignKey<int>(s => s.NajmStatusId);
           

        }

    }


   
}

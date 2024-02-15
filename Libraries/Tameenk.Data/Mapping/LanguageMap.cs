using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class LanguageMap : EntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            ToTable("Language");
            Property(e => e.NameAR).IsRequired().HasMaxLength(50);
            Property(e => e.NameEN).IsRequired().HasMaxLength(50);
           
            HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);
        }
    }
}

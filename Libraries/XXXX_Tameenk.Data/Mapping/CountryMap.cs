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
    public class CountryMap : EntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            ToTable("Country");
            HasKey(e => e.Code);
            Property(e => e.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.EnglishDescription).HasMaxLength(256);
            Property(e => e.ArabicDescription).HasMaxLength(256);
        }
    }
}

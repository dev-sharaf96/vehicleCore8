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
    public class BankCodeMap : EntityTypeConfiguration<BankCode>
    {
        public BankCodeMap()
        {
            ToTable("BankCode");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.Code).HasMaxLength(50);
            Property(e => e.ArabicDescription).HasMaxLength(200);
        }
    }
}

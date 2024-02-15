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
    public class ErrorCodeMap : EntityTypeConfiguration<ErrorCode>
    {
        public ErrorCodeMap()
        {
            ToTable("ErrorCode");
            HasKey(e => e.Code);
            Property(e => e.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.EnglishText).HasMaxLength(50);
            Property(e => e.ArabicText).HasMaxLength(50);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);
        }
    }
}

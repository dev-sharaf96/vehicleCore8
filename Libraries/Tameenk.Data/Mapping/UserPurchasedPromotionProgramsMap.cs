using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Data.Mapping
{
    public class UserPurchasedPromotionProgramsMap : EntityTypeConfiguration<UserPurchasedPromotionPrograms>
    {
        public UserPurchasedPromotionProgramsMap()
        {
            ToTable("UserPurchasedPromotionPrograms");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //Property(e => e.MaxNumberOfPolicies).(256);
            //Property(e => e.ArabicDescription).HasMaxLength(256);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutCarImageMap : EntityTypeConfiguration<CheckoutCarImage>
    {
        public CheckoutCarImageMap()
        {
            Property(e => e.ImageData).HasColumnType("image");

            HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.ImageBack)
                .HasForeignKey(e => e.ImageBackId);

            HasMany(e => e.CheckoutDetails1)
                .WithOptional(e => e.ImageBody)
                .HasForeignKey(e => e.ImageBodyId);

            HasMany(e => e.CheckoutDetails2)
                .WithOptional(e => e.ImageFront)
                .HasForeignKey(e => e.ImageFrontId);

            HasMany(e => e.CheckoutDetails3)
                .WithOptional(e => e.ImageLeft)
                .HasForeignKey(e => e.ImageLeftId);

            HasMany(e => e.CheckoutDetails4)
                .WithOptional(e => e.ImageRight)
                .HasForeignKey(e => e.ImageRightId);
        }
    }
}

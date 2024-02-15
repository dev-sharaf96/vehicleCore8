using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Identity
{
    public class AspNetUserMap : EntityTypeConfiguration<AspNetUser>
    {
        public AspNetUserMap() {
            ToTable("AspNetUsers");
            HasKey(e => e.Id);
            Property(u => u.Email).HasMaxLength(256);
            Property(u => u.UserName).IsRequired().HasMaxLength(256);
            
            //HasMany(e => e.AspNetClinetClaims)
            //    .WithRequired(e => e.AspNetUser)
            //    .HasForeignKey(e => e.UserId);

            HasMany(e => e.CheckoutDetails)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Invoices)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);
            

            HasMany(e => e.QuotationRequests)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            //HasMany(e => e.AspNetUserClaims)
            //   .WithRequired(e => e.AspNetUser)
            //   .HasForeignKey(e => e.UserId);

            //HasMany(e => e.AspNetUserLogins)
            //    .WithRequired(e => e.AspNetUser)
            //    .HasForeignKey(e => e.UserId);

            //HasMany(e => e.Roles)
            //    .WithMany()
            //    .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("UserId").MapRightKey("RoleId"));
        }
    }
}
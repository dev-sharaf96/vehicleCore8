using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping.Quotations
{
    public class InsuredMap : EntityTypeConfiguration<Insured>
    {
        public InsuredMap()
        {
            ToTable("Insured");
            HasKey(e => e.Id);

            Property(e => e.NationalId).IsRequired().HasMaxLength(20);
            Property(e => e.CardIdTypeId).IsRequired();
            Property(e => e.BirthDate).IsRequired();
            Property(e => e.BirthDateH).HasMaxLength(10);
            Property(e => e.NationalityCode).HasMaxLength(4);
            Property(e => e.FirstNameAr).IsRequired().HasMaxLength(500);
            Property(e => e.MiddleNameAr).HasMaxLength(50);
            Property(e => e.LastNameAr).IsRequired().HasMaxLength(50);
            Property(e => e.FirstNameEn).IsRequired().HasMaxLength(500);
            Property(e => e.MiddleNameEn).HasMaxLength(50);
            Property(e => e.LastNameEn).IsRequired().HasMaxLength(50);
            Property(e => e.ResidentOccupation).HasMaxLength(50);

            Ignore(e => e.Education);
            //Ignore(e => e.Occupation);
            Ignore(e => e.SocialStatus);
            Ignore(e => e.CardIdType);
            Ignore(e => e.Gender);

            HasOptional(e => e.IdIssueCity).WithMany().HasForeignKey(e => e.IdIssueCityId);
            HasOptional(e => e.WorkCity).WithMany().HasForeignKey(e => e.WorkCityId);
            HasOptional(e => e.City).WithMany().HasForeignKey(e => e.CityId);

            //HasOptional<Occupation>(e => e.Occupation).WithMany(x => x.Insureds).HasForeignKey<int?>(e => e.OccupationId);
        }
    }
}

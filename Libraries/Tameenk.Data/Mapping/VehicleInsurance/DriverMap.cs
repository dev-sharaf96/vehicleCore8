using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class DriverMap : EntityTypeConfiguration<Driver>
    {
        public DriverMap()
        {
            ToTable("Driver");
            HasKey(d => d.DriverId);

            Property(d => d.DateOfBirthH).HasMaxLength(100);
            Property(d => d.IdIssuePlace).HasMaxLength(50);
            Property(d => d.IdExpiryDate).HasMaxLength(50);

            //Ignore(d => d.Occupation);
            Ignore(d => d.SocialStatus);
            Ignore(d => d.Education);
            Ignore(d => d.MedicalCondition);
            Ignore(d => d.Gender);

            HasMany(e => e.DriverLicenses)
                .WithRequired(e => e.Driver)
                .WillCascadeOnDelete(false);

            HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.Driver)
                .HasForeignKey(e => e.MainDriverId)
                .WillCascadeOnDelete(false);

            HasMany(e => e.AdditionalDriverQuotationRequests)
                .WithMany(e => e.Drivers)
                .Map(m => m.ToTable("QuotationRequestAdditionalDrivers").MapLeftKey("AdditionalDriverId").MapRightKey("QuotationRequestId"));

            HasMany(e => e.CheckoutAdditionalDrivers)
                .WithRequired(e => e.Driver)
                .WillCascadeOnDelete(false);

            HasMany(e => e.CheckoutDetails)
                 .WithOptional(e => e.Driver)
                 .HasForeignKey(e => e.MainDriverId);

            HasMany(e => e.Addresses)
                 .WithOptional(e => e.Driver)
                 .HasForeignKey(e => e.DriverId);

            HasMany(e => e.DriverViolations)
             .WithRequired(e => e.Driver)
             .HasForeignKey(e => e.DriverId);

            HasOptional(e => e.City).WithMany().HasForeignKey(e => e.CityId);
            HasOptional(e => e.WorkCity).WithMany().HasForeignKey(e => e.WorkCityId);
            //HasOptional<Occupation>(e => e.Occupation).WithMany(x => x.Drivers).HasForeignKey<int?>(e => e.OccupationId);
        }
    }
}
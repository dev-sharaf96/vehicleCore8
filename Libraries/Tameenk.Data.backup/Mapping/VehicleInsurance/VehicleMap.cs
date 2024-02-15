using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleMap : EntityTypeConfiguration<Vehicle>
    {
        public VehicleMap()
        {
            HasMany(v => v.QuotationRequests).WithRequired(v => v.Vehicle).HasForeignKey(v => v.VehicleId).WillCascadeOnDelete(false);
            Property(v => v.SequenceNumber).HasMaxLength(30);
            Property(v => v.CustomCardNumber).HasMaxLength(30);
            Property(v => v.LicenseExpiryDate).HasMaxLength(20);
            Property(v => v.MajorColor).HasMaxLength(20);
            Property(v => v.MinorColor).HasMaxLength(20);
            Property(v => v.RegisterationPlace).HasMaxLength(20);
            Property(v => v.VehicleMaker).HasMaxLength(50);
            Property(v => v.VehicleModel).IsRequired().HasMaxLength(30);
            Property(v => v.ChassisNumber).HasMaxLength(30);
            Property(v => v.CarPlateText1).HasMaxLength(1);
            Property(v => v.CarPlateText2).HasMaxLength(1);
            Property(v => v.CarPlateText3).HasMaxLength(1);
            Property(v => v.ModificationDetails).HasMaxLength(200);

            Ignore(e => e.EngineSize);
            Ignore(e => e.VehicleUse);
            Ignore(e => e.TransmissionType);
            Ignore(e => e.AxlesWeight);
            Ignore(e => e.ParkingLocation);
            Ignore(e => e.MileageExpectedAnnual);
            Ignore(e => e.VehicleIdType);

            HasMany(e => e.CheckoutDetails)
                .WithRequired(e => e.Vehicle)
                .WillCascadeOnDelete(false);

            HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.Vehicle)
                .WillCascadeOnDelete(false);
            HasMany(e => e.VehicleSpecifications).WithMany(e => e.Vehicles)
                .Map(vs => vs.MapLeftKey("VehicleId")
                    .MapRightKey("VehicleSpecificationId")
                    .ToTable("Vehicle_VehicleSpecification")
                );
        }
    }
}
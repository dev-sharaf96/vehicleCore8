using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            ToTable("Address");
            HasKey(e => e.Id);

            Property(e => e.Address1).HasMaxLength(500);
            Property(e => e.Address2).HasMaxLength(500);
            Property(e => e.ObjLatLng).HasMaxLength(500);
            Property(e => e.BuildingNumber).HasMaxLength(500);
            Property(e => e.Street).HasMaxLength(500);
            Property(e => e.District).HasMaxLength(500);
            Property(e => e.City).HasMaxLength(500);
            Property(e => e.PostCode).HasMaxLength(500);
            Property(e => e.AdditionalNumber).HasMaxLength(500);
            Property(e => e.RegionName).HasMaxLength(500);
            Property(e => e.IsPrimaryAddress).HasMaxLength(500);
            Property(e => e.UnitNumber).HasMaxLength(500);
            Property(e => e.AdditionalNumber).HasMaxLength(500);
            Property(e => e.Latitude).HasMaxLength(500);
            Property(e => e.Longitude).HasMaxLength(500);
            Property(e => e.CityId).HasMaxLength(500);
            Property(e => e.RegionId).HasMaxLength(500);
            Property(e => e.Restriction).HasMaxLength(500);
            Property(e => e.PKAddressID).HasMaxLength(500);
            Property(e => e.AddressLoction).HasMaxLength(50);
            HasOptional(e => e.Driver).WithMany().HasForeignKey(e => e.DriverId);
        }
    }
}

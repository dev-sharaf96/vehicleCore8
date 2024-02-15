namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            InsuranceCompanies = new HashSet<InsuranceCompany>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        [StringLength(500)]
        public string Address1 { get; set; }

        [StringLength(500)]
        public string Address2 { get; set; }

        [StringLength(500)]
        public string ObjLatLng { get; set; }

        [StringLength(500)]
        public string BuildingNumber { get; set; }

        [StringLength(500)]
        public string Street { get; set; }

        [StringLength(500)]
        public string District { get; set; }

        [StringLength(500)]
        public string City { get; set; }

        [StringLength(500)]
        public string PostCode { get; set; }

        [StringLength(500)]
        public string AdditionalNumber { get; set; }

        [StringLength(500)]
        public string RegionName { get; set; }

        public string PolygonString { get; set; }

        [StringLength(500)]
        public string IsPrimaryAddress { get; set; }

        [StringLength(500)]
        public string UnitNumber { get; set; }

        [StringLength(500)]
        public string Latitude { get; set; }

        [StringLength(500)]
        public string Longitude { get; set; }

        [StringLength(500)]
        public string CityId { get; set; }

        [StringLength(500)]
        public string RegionId { get; set; }

        [StringLength(500)]
        public string Restriction { get; set; }

        [StringLength(500)]
        public string PKAddressID { get; set; }

        public Guid? DriverId { get; set; }

        [StringLength(50)]
        public string AddressLoction { get; set; }

        public virtual Driver Driver { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceCompany> InsuranceCompanies { get; set; }
    }
}

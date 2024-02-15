namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
            QuotationRequests = new HashSet<QuotationRequest>();
            VehicleSpecifications = new HashSet<VehicleSpecification>();
        }

        public Guid ID { get; set; }

        [StringLength(30)]
        public string SequenceNumber { get; set; }

        [StringLength(30)]
        public string CustomCardNumber { get; set; }

        public byte? Cylinders { get; set; }

        [StringLength(20)]
        public string LicenseExpiryDate { get; set; }

        [StringLength(20)]
        public string MajorColor { get; set; }

        [StringLength(20)]
        public string MinorColor { get; set; }

        public short? ModelYear { get; set; }

        public byte? PlateTypeCode { get; set; }

        [StringLength(20)]
        public string RegisterationPlace { get; set; }

        public byte VehicleBodyCode { get; set; }

        public int VehicleWeight { get; set; }

        public int VehicleLoad { get; set; }

        [StringLength(50)]
        public string VehicleMaker { get; set; }

        [Required]
        [StringLength(30)]
        public string VehicleModel { get; set; }

        [StringLength(30)]
        public string ChassisNumber { get; set; }

        public short? VehicleMakerCode { get; set; }

        public long? VehicleModelCode { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(1)]
        public string CarPlateText1 { get; set; }

        [StringLength(1)]
        public string CarPlateText2 { get; set; }

        [StringLength(1)]
        public string CarPlateText3 { get; set; }

        public short? CarPlateNumber { get; set; }

        public string CarOwnerNIN { get; set; }

        public string CarOwnerName { get; set; }

        public int? VehicleValue { get; set; }

        public bool? IsUsedCommercially { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public bool OwnerTransfer { get; set; }

        public int? EngineSizeId { get; set; }

        public int? VehicleUseId { get; set; }

        public decimal? CurrentMileageKM { get; set; }

        public decimal? MileageExpectedAnnualId { get; set; }

        public int? ParkingLocationId { get; set; }

        public int? TransmissionTypeId { get; set; }

        public decimal? AxleWeightId { get; set; }

        public bool HasModifications { get; set; }

        [StringLength(200)]
        public string ModificationDetails { get; set; }

        public int VehicleIdTypeId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationRequest> QuotationRequests { get; set; }

        public virtual VehicleBodyType VehicleBodyType { get; set; }

        public virtual VehiclePlateType VehiclePlateType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleSpecification> VehicleSpecifications { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Driver")]
    public partial class Driver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Driver()
        {
            Addresses = new HashSet<Address>();
            CheckoutAdditionalDrivers = new HashSet<CheckoutAdditionalDriver>();
            CheckoutDetails = new HashSet<CheckoutDetail>();
            DriverViolations = new HashSet<DriverViolation>();
            DriverLicenses = new HashSet<DriverLicense>();
            QuotationRequests = new HashSet<QuotationRequest>();
            QuotationRequests1 = new HashSet<QuotationRequest>();
        }

        public Guid DriverId { get; set; }

        public bool IsCitizen { get; set; }

        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }

        public DateTime? DateOfBirthG { get; set; }

        public short? NationalityCode { get; set; }

        [StringLength(100)]
        public string DateOfBirthH { get; set; }

        public string NIN { get; set; }

        public bool? IsSpecialNeed { get; set; }

        [StringLength(50)]
        public string IdIssuePlace { get; set; }

        [StringLength(50)]
        public string IdExpiryDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public int? ViolationId { get; set; }

        public int? OccupationId { get; set; }

        [StringLength(50)]
        public string ResidentOccupation { get; set; }

        public int GenderId { get; set; }

        public int? SocialStatusId { get; set; }

        public int? MedicalConditionId { get; set; }

        public int? DrivingPercentage { get; set; }

        public int EducationId { get; set; }

        public int? ChildrenBelow16Years { get; set; }

        public long? CityId { get; set; }

        public long? WorkCityId { get; set; }

        public int? NOALast5Years { get; set; }

        public int? NOCLast5Years { get; set; }

        public int? NCDFreeYears { get; set; }

        [StringLength(50)]
        public string NCDReference { get; set; }

        public int? MaritalStatusCode { get; set; }

        public int? NumOfChildsUnder16 { get; set; }

        public int? DrivingLicenseTypeCode { get; set; }

        public int? SaudiLicenseHeldYears { get; set; }

        public int? EligibleForNoClaimsDiscountYears { get; set; }

        public int? NumOfFaultAccidentInLast5Years { get; set; }

        public int? NumOfFaultclaimInLast5Years { get; set; }

        public string RoadConvictions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Addresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutAdditionalDriver> CheckoutAdditionalDrivers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        public virtual City City { get; set; }

        public virtual City City1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DriverViolation> DriverViolations { get; set; }

        public virtual Occupation Occupation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DriverLicense> DriverLicenses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationRequest> QuotationRequests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationRequest> QuotationRequests1 { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuotationRequest")]
    public partial class QuotationRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuotationRequest()
        {
            QuotationResponses = new HashSet<QuotationResponse>();
            Drivers = new HashSet<Driver>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string ExternalId { get; set; }

        public Guid MainDriverId { get; set; }

        public long CityCode { get; set; }

        public DateTime? RequestPolicyEffectiveDate { get; set; }

        public Guid VehicleId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string NajmNcdRefrence { get; set; }

        public int? NajmNcdFreeYears { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsComprehensiveGenerated { get; set; }

        public bool IsComprehensiveRequested { get; set; }

        public int InsuredId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual City City { get; set; }

        public virtual Driver Driver { get; set; }

        public virtual Insured Insured { get; set; }

        public virtual Vehicle Vehicle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationResponse> QuotationResponses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Driver> Drivers { get; set; }
    }
}

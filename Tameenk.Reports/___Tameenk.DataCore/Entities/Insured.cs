namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Insured")]
    public partial class Insured
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Insured()
        {
            QuotationRequests = new HashSet<QuotationRequest>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NationalId { get; set; }

        public int CardIdTypeId { get; set; }

        public DateTime BirthDate { get; set; }

        [StringLength(10)]
        public string BirthDateH { get; set; }

        public int? GenderId { get; set; }

        [StringLength(4)]
        public string NationalityCode { get; set; }

        public long IdIssueCityId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstNameAr { get; set; }

        [StringLength(50)]
        public string MiddleNameAr { get; set; }

        [Required]
        [StringLength(50)]
        public string LastNameAr { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstNameEn { get; set; }

        [StringLength(50)]
        public string MiddleNameEn { get; set; }

        [Required]
        [StringLength(50)]
        public string LastNameEn { get; set; }

        public int SocialStatusId { get; set; }

        public int? OccupationId { get; set; }

        [StringLength(50)]
        public string ResidentOccupation { get; set; }

        public int EducationId { get; set; }

        public int? ChildrenBelow16Years { get; set; }

        public long? WorkCityId { get; set; }

        public long? CityId { get; set; }

        public virtual City City { get; set; }

        public virtual City City1 { get; set; }

        public virtual City City2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationRequest> QuotationRequests { get; set; }

        public virtual Occupation Occupation { get; set; }
    }
}

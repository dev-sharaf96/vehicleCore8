namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InsuranceCompany")]
    public partial class InsuranceCompany
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InsuranceCompany()
        {
            Deductibles = new HashSet<Deductible>();
            InsuaranceCompanyBenefits = new HashSet<InsuaranceCompanyBenefit>();
            PromotionProgramCodes = new HashSet<PromotionProgramCode>();
            Invoices = new HashSet<Invoice>();
            Policies = new HashSet<Policy>();
            Products = new HashSet<Product>();
            QuotationResponses = new HashSet<QuotationResponse>();
        }

        public int InsuranceCompanyID { get; set; }

        [Required]
        [StringLength(50)]
        public string NameAR { get; set; }

        [Required]
        [StringLength(50)]
        public string NameEN { get; set; }

        [StringLength(1000)]
        public string DescAR { get; set; }

        [StringLength(1000)]
        public string DescEN { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public int? AddressId { get; set; }

        public int? ContactId { get; set; }

        [StringLength(1000)]
        public string TEMP { get; set; }

        [StringLength(200)]
        public string NamespaceTypeName { get; set; }

        [StringLength(200)]
        public string ClassTypeName { get; set; }

        [StringLength(200)]
        public string ReportTemplateName { get; set; }

        public bool isActive { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        public virtual Address Address { get; set; }

        public virtual Contact Contact { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Deductible> Deductibles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionProgramCode> PromotionProgramCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Policy> Policies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationResponse> QuotationResponses { get; set; }
    }
}

namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuotationResponse")]
    public partial class QuotationResponse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuotationResponse()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        public int? RequestId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        public DateTime CreateDateTime { get; set; }

        public bool? VehicleAgencyRepair { get; set; }

        public short? DeductibleValue { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferenceId { get; set; }

        public int InsuranceCompanyId { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        public virtual ProductType ProductType { get; set; }

        public virtual QuotationRequest QuotationRequest { get; set; }
    }
}

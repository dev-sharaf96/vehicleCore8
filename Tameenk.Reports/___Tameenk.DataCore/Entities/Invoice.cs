namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            Invoice_Benefit = new HashSet<Invoice_Benefit>();
        }

        public int Id { get; set; }

        public int InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime InvoiceDueDate { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(400)]
        public string ReferenceId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        public int? InsuranceCompanyId { get; set; }

        public int? PolicyId { get; set; }

        public decimal? ProductPrice { get; set; }

        public decimal? Fees { get; set; }

        public decimal? Vat { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? ExtraPremiumPrice { get; set; }

        public decimal? Discount { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Benefit> Invoice_Benefit { get; set; }

        public virtual Policy Policy { get; set; }

        public virtual ProductType ProductType { get; set; }

        public virtual InvoiceFile InvoiceFile { get; set; }
    }
}

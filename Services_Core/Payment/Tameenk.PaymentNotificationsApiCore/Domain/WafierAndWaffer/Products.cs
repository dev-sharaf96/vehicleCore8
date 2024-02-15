using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class Products
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ProductNameAr { get; set; }
        [Required]
        public string ProductNameEn { get; set; }
        [Required]
        public string ProductDescriptionAr { get; set; }
        [Required]
        public string ProductDescriptionEn { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal Percentage { get; set; }
        public int ProductPicture { get; set; }
        public int ProductLogo { get; set; }
        public bool Status { get; set; }
        [Required]
        [StringLength(128)]
        public string ProductUserId { get; set; }
        public DateTime ProductCreatedDate { get; set; }
        public DateTime ProductUpdatedDate { get; set; }
        public int ProductTypesID { get; set; }
        public virtual ProductTypes ProductTypes { get; set; }
        public int? ProductClassesID { get; set; }
        public virtual ProductClasses ProductClasses { get; set; }
        public int CompaniesID { get; set; }
        public virtual Companies Companies { get; set; }
        public virtual ICollection<ProductDetails> ProductDetails { get; set; }
        public byte[] ProposalTemplateFile { get; set; }
        public int? ProfessionalMalpracticeClassesID { get; set; }
        [DefaultValue(false)]
        public bool ActiveNcdDiscount { get; set; }
        [DefaultValue(0)]
        public int NCDFree1YearsDiscount { get; set; }
        [DefaultValue(0)]
        public int NCDFree2YearsDiscount { get; set; }
        [DefaultValue(0)]
        public int NCDFree3YearsDiscount { get; set; }
        [DefaultValue(false)]
        public bool ActiveShipping { get; set; }
    }
}
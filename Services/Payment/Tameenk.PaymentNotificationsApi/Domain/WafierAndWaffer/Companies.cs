using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class Companies
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string CompanyNameAr { get; set; }
        [Required]
        public string CompanyNameEn { get; set; }
        [Required]
        public string CompanyDescriptionAr { get; set; }
        [Required]
        public string CompanyDescriptionEn { get; set; }
        [Required]
        public int CompanyLogo { get; set; }
        public int CompanyImage { get; set; }
        public string CompanyWebsite { get; set; }
        [Required]
        public string CompanyEmail { get; set; }
        [Required]
        public string CompanyPhone { get; set; }
        [Required]
        public string CompanyAddressAr { get; set; }
        [Required]
        public string CompanyAddressEn { get; set; }
        [Required]
        [StringLength(128)]
        public string CompanyUserId { get; set; }
        [Required]
        public DateTime CompanyCreatedDate { get; set; }
        [Required]
        public DateTime CompanyUpdatedDate { get; set; }
    }
}
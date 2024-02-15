using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class ProductClasses
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ProductClassNameAr { get; set; }
        [Required]
        public string ProductClassNameEn { get; set; }
        [Required]
        [StringLength(128)]
        public string ProductClassUserId { get; set; }
        public DateTime ProductClassCreatedDate { get; set; }
        public DateTime ProductClassUpdatedDate { get; set; }
        public int CompaniesID { get; set; }
        public virtual Companies Companies { get; set; }
        public int ProductTypesID { get; set; }
        public virtual ProductTypes ProductTypes { get; set; }
    }
}
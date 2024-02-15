using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class ProductTypes
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ProductTypeNameAr { get; set; }
        [Required]
        public string ProductTypeNameEn { get; set; }
        public int ProductTypePicture { get; set; }
        public int ProductTypeLogo { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductTypeUserId { get; set; }
        public DateTime ProductTypeCreatedDate { get; set; }
        public DateTime ProductTypeUpdatedDate { get; set; }
    }
}
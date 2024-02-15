using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class PaymentMethods
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public PaymentMethodTypes PaymentMethodType { get; set; }
        [Required]
        [MaxLength(50)]
        public string PaymentMethodNameEn { get; set; }
        [Required]
        [MaxLength(50)]
        public string PaymentMethodNameAr { get; set; }
        [Required]
        [MaxLength(100)]
        public string PaymentMethodDescriptionEn { get; set; }
        [Required]
        [MaxLength(100)]
        public string PaymentMethodDescriptionAr { get; set; }
        public int PaymentMethodLogo { get; set; }
        public int PaymentMethodPicture { get; set; }
        [Required]
        public DateTime PaymentMethodCreatedDate { get; set; }
        [Required]
        [MaxLength(128)]
        public string PaymentMethodUserID { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public bool Active { get; set; }
    }
}
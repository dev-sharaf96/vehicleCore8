using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class PaymentConfigurations
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public int PaymentMethodsID { get; set; }
        public virtual PaymentMethods PaymentMethods { get; set; }
        [Required]
        public string ConfigurationName { get; set; }
        [Required]
        public string MerchantIdentifier { get; set; }
        [Required]
        public string Command { get; set; }
        [Required]
        public string AccessCode { get; set; }
        [Required]
        public string ReturnURL { get; set; }
        [Required]
        public string SHARequestPhrase { get; set; }
        [Required]
        public string SHAResponsePhrase { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        [MaxLength(128)]
        public string UserId { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        //[Range(1, 30)]
        [DefaultValue(1)]
        public int PaymentExpiryNumberDays { get; set; }
        public bool Defuilt { get; set; }
    }
}
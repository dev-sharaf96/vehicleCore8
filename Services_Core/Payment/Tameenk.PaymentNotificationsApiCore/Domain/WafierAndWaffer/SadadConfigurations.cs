using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class SadadConfigurations
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
        public string SadadUrl { get; set; }
        [Required]
        public string BillerId { get; set; }
        [Required]
        public string ExactPmtRq { get; set; }
        [Required]
        //[Range(1, 30)]
        [DefaultValue(1)]
        public int BillExpiryNumberDays { get; set; }
        [Required]
        public string BillerCode { get; set; }
        [Required]
        public string BillerNameEn { get; set; }
        [Required]
        public string BillerNameAr { get; set; }
        [Required]
        public string keyPath { get; set; }
        [Required]
        public string keyPassword { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        [MaxLength(128)]
        public string UserId { get; set; }
        public bool Defuilt { get; set; }
    }
}
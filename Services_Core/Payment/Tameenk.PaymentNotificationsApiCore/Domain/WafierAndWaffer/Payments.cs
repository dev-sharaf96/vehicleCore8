using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class Payments
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public double PaymentAmount { get; set; }
        public string PaymentReferenceCode { get; set; }
        public bool PaymentStatus { get; set; }
        [Required]
        public int PaymentMethodsID { get; set; }
        public virtual PaymentMethods PaymentMethod { get; set; }
        [Required]
        [MaxLength(128)]
        public string PaymentUserID { get; set; }
        public int InvoicesID { get; set; }
        //public virtual Invoices Invoices { get; set; }
        [Required]
        public DateTime PaymentCreatedDate { get; set; }
        [Required]
        public DateTime PaymentExpiryDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public ICollection<PaymentRequests> PaymentRequests { get; set; }
        public ICollection<PaymentResponse> PaymentResponse { get; set; }
    }
}
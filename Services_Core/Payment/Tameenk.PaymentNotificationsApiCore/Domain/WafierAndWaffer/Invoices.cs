using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class Invoices
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public double InvoiceTotalPrice { get; set; }

        [DefaultValue(0)]
        public decimal InvoiceSubTotalPrice { get; set; }
        [DefaultValue(0)]
        public decimal InvoiceTotalTax { get; set; }
        [DefaultValue(0)]
        public decimal InvoiceTotalTaxPercentage { get; set; }
        public int PaymentsID { get; set; }
        public virtual Payments Payments { get; set; }
        //[Required]
        public int? ShippingAddressesID { get; set; }
        public virtual ShippingAddresses ShippingAddresses { get; set; }
        [Required]
        public DateTime InvoiceCreatedDate { get; set; }
        [Required]
        public DateTime InvoiceUpdatedDate { get; set; }
        [Required]
        [MaxLength(128)]
        public string InvoiceUserID { get; set; }
        public virtual ICollection<InvoiceDetails> InvoiceDetails { get; set; }
        [Required]
        public int CustomersID { get; set; }
        public virtual Customers Customers { get; set; }
    }
}
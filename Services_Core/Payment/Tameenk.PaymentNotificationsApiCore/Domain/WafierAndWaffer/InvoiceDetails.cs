using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class InvoiceDetails
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int InvoiceDetailsQuantity { get; set; }
        [Required]
        public decimal InvoiceDetailsUnitPrice { get; set; }
        [DefaultValue(0)]
        public decimal InvoiceDetailsUnitSubPrice { get; set; }
        [DefaultValue(0)]
        public decimal InvoiceDetailsUnitTax { get; set; }
        [DefaultValue(0)]
        public decimal InvoiceDetailsUnitTaxPercentage { get; set; }
        [Required]
        public int InvoicesID { get; set; }
        public virtual Invoices Invoices { get; set; }
        [Required]
        public int ProductsID { get; set; }
        public virtual Products Products { get; set; }
        [Required]
        public DateTime InvoiceDetailsCreatedDate { get; set; }
    }
}
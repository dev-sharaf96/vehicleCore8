using System;

namespace Tameenk.Services.Profile.Component.Models
{
    public class MyInvoicesDB
    {
        public int Id { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string UserEmail { get; set; }
        public string InsuranceCompanyNameAR { get; set; }
        public string InsuranceCompanyNameEN { get; set; }
        public string TaxInvoiceNumber { get; set; }
    }
}
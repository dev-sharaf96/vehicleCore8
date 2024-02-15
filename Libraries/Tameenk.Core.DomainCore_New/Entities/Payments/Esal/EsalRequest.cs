using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Esal
{
    public class EsalRequest:BaseEntity
    {
        public EsalRequest()
        {
            //ProfitMargin = new ProfitMarginRequest();
            //LineItems = new List<LineItem>();
            //Supplier = new Supplier();
            //Customer = new Customer();
        }
        public int Id { get; set; }
        public string RequestID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SupplierId { get; set; }
        public Guid UserID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string SequenceNumber { get; set; }
        public string DriverNin { get; set; }
        public string ReferenceId { get; set; }
        //public virtual List<InvoiceRequest> Invoices { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceIssueDate { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public string Currency { get; set; }
        public Boolean ExemptedAmount { get; set; }
        public Boolean AmountNotSubjToTaxation { get; set; }
        public Boolean SelfAccountForTax { get; set; }
        public decimal TotalBeforeVAT { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string RemarksArabic { get; set; }
        public string RemarksEnglish { get; set; }
        public string NarrationArabic { get; set; }
        public string NarrationEnglish { get; set; }
        public Boolean MilestonePayments { get; set; }
        public int PercentOfCompletion { get; set; }
        public string ShipmentRefNumber { get; set; }
        public string SalespersonEnglish { get; set; }
        public string SalespersonArabic { get; set; }
        public bool HasEsalInvoice { get; set; }
        public DateTime ModifiedDate { get; set; }
        //public virtual ProfitMarginRequest ProfitMargin { get; set; }
        //public virtual List<LineItem> LineItems { get; set; }
        //public virtual Supplier Supplier { get; set; }
        //public virtual Customer Customer { get; set; }
    }
}

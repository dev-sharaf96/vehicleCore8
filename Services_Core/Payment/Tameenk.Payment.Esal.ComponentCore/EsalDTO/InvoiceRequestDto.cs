using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class InvoiceRequestDto
    {
        [JsonProperty("invoiceType")]
        public string InvoiceType { get; set; }
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }
        [JsonProperty("invoiceIssueDate")]
        public string InvoiceIssueDate { get; set; }
        [JsonProperty("invoiceReference")]
        public string InvoiceReference { get; set; }
        [JsonProperty("dueDate")]
        public string DueDate { get; set; }
        [JsonProperty("dateOfDelivery")]
        public string DateOfDelivery { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("exemptedAmount")]
        public Boolean ExemptedAmount { get; set; }
        [JsonProperty("amountNotSubjToTaxation")]
        public Boolean AmountNotSubjToTaxation { get; set; }
        [JsonProperty("selfAccountForTax")]
        public Boolean SelfAccountForTax { get; set; }
        [JsonProperty("totalBeforeVAT")]
        public decimal TotalBeforeVAT { get; set; }
        [JsonProperty("totalVAT")]
        public decimal TotalVAT { get; set; }
        [JsonProperty("grandTotal")]
        public decimal GrandTotal { get; set; }
        [JsonProperty("advanceAmount")]
        public decimal AdvanceAmount { get; set; }
        [JsonProperty("outstandingAmount")]
        public decimal OutstandingAmount { get; set; }
        [JsonProperty("remarksArabic")]
        public string RemarksArabic { get; set; }
        [JsonProperty("remarksEnglish")]
        public string RemarksEnglish { get; set; }
        [JsonProperty("narrationArabic")]
        public string NarrationArabic { get; set; }
        [JsonProperty("narrationEnglish")]
        public string NarrationEnglish { get; set; }
        [JsonProperty("milestonePayments")]
        public Boolean MilestonePayments { get; set; }
        [JsonProperty("percentOfCompletion")]
        public int PercentOfCompletion { get; set; }
        [JsonProperty("shipmentRefNumber")]
        public string ShipmentRefNumber { get; set; }
        [JsonProperty("salespersonEnglish")]
        public string SalespersonEnglish { get; set; }
        [JsonProperty("salespersonArabic")]
        public string SalespersonArabic { get; set; }
        [JsonProperty("profitMargin")]
        public ProfitMarginRequestDto ProfitMargin { get; set; }
        [JsonProperty("lineItems")]
        public List<LineItemDto> LineItems { get; set; }
        [JsonProperty("supplier")]
        public SupplierDto Supplier { get; set; }
        [JsonProperty("customer")]
        public CustomerDto Customer { get; set; }
    }
}

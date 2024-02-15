using System;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingWalletReportModel 
    {
        public  string? ReferenceId { get; set; }
        public decimal RemainingBalance { get; set; }
        public  string? PolicyNumber { get; set; }
        public  string? InsuranceCompanyName { get; set; }
        public  string? UserAccount { get; set; }
        public DateTime?  PolicyIssueDate { get; set; }
        public DateTime?  PolicyEffectiveDate { get; set; }
        public DateTime?  PolicyExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  string? InsuredName { get; set; }
        public  string? InsuredID { get; set; }
        public decimal? Amount { get; set; }
        public  string? PhoneNumber { get; set; }
        public  string? Method { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("WalaaPolicies")]
    public class WalaaPolicies
    {
        public int Id { get; set; }
        public   string? PolicyNo { get; set; }
        public   string? Referenceid { get; set; }
        public   string? ReportUrl { get; set; }
        public bool IsSent { get; set; }
        public int ProcessingTries { get; set; }
        public   string? ErrorDescription { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
    }
}

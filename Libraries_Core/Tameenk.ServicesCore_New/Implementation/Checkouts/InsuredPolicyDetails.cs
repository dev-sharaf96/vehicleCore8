using System;

namespace Tameenk.Services
{
    public class InsuredPolicyDetails
    {
        public  string? NationalId { get; set; }
        public int PolicyStatusId { get; set; }
        public  string? ReferenceId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public bool IsCompany { get; set; }
    }
}

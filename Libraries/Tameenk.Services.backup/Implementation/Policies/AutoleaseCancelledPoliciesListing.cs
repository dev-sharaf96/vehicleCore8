using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class AutoleaseCancelledPoliciesListing
    {
        public string CheckOutDetailsId { get; set; }
        public string policyNo { get; set; }
        public Guid? policyFileId { get; set; }
        public string insuredNIN { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishDescription { get; set; }
        public DateTime? policyIssueDate { get; set; }
        public string insuredFullNameAr { get; set; }
        public string insuredFullNameEn { get; set; }
        public string InsuranceCompanyNameAr { get; set; }
        public string InsuranceCompanyNameEn { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string IBAN { get; set; }
        public int MyProperty { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string NajmStatus { get; set; }
        public string Channel { get; set; }
        public string PolicyStatusKey { get; set; }
        public int PolicyStatusId { get; set; }
        public decimal? BenfitsPrice { get; set; }
        public string Email { get; set; }
        public string VehicleMakerName { get; set; }
        public string VehicleModelName { get; set; }
        public string VehicleId { get; set; }
        public string PlateNumber { get; set; }
    }
}

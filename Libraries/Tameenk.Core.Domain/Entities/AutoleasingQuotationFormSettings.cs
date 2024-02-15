using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingQuotationFormSettings : BaseEntity
    {
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string ExternalId { get; set; }
        public int? SelectedInsuranceCompany { get; set; }
        public string SelectedInsuranceCompanyName { get; set; }
        public string InsurancePercentage { get; set; }
        public string Premium { get; set; }
        public string RepairMethod { get; set; }
        public string Deductible { get; set; }
        public string MinimumPremium { get; set; }
        public string Total5YearsPremium { get; set; }
        public string Depreciation { get; set; }
        public int? BankId { get; set; }
        public string UserId { get; set; }
        public bool IsPurchased { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public decimal? Premium1 { get; set; }
        public decimal? Premium2 { get; set; }
        public decimal? Premium3 { get; set; }
        public decimal? Premium4 { get; set; }
        public decimal? Premium5 { get; set; }
    }
}

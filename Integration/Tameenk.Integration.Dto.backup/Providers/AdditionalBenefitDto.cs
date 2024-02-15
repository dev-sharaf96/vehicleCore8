using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AdditionalBenefitDto
    {
        public AdditionalBenefitDto(){}
        public AdditionalBenefitDto(int benefitCode, decimal price)
        {
            BenefitCode = benefitCode;
            BenefitPrice = price;
            IsReadOnly = false;
        }
        public int? BenefitCode { get; set; }
        public string BenefitId { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
        public string BenefitDescAr { get; set; }
        public string BenefitDescEn { get; set; }
        public decimal? BenefitPrice { get; set; }
        public bool IsSelected { get; set; }

        public bool IsReadOnly { get; set; }

        public string CoveredCountry { get; set; }
        public decimal? AveragePremium { get; set; }
        public DateTime? BenefitEffectiveDate { get; set; }
        public DateTime? BenefitExpiryDate { get; set; }
        public double? DeductibleValue { get; set; }
        public double? TaxableAmount { get; set; }
        public double? VATAmount { get; set; }
    }
}

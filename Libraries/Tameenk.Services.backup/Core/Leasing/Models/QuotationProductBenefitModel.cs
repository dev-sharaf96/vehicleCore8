using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class QuotationProductBenefitModel
    {
        public int Id { get; set; }
        public Guid? ProductId { get; set; }
        public short? BenefitId { get; set; }
        public string BenefitExternalId { get; set; }
        public decimal? BenefitPrice { get; set; }
        public double? TaxableAmount { get; set; }
        public double? VATAmount { get; set; }
        public bool? IsSelected { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsOld { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
        public string CoveredCountry { get; set; }
        public decimal? AveragePremium { get; set; }
    }
}

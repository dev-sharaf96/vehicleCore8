using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaTplissuePolicyRequestDto
    {
        public WataniyaTplissuePolicyRequestDeatilsDto Details { get; set; }
        public int InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public bool IsPurchased { get; set; }
        public long QuoteReferenceNo { get; set; } // returned from there side
        public string RequestReferenceNo { get; set; } // reference id
    }

    public class WataniyaTplissuePolicyRequestDeatilsDto
    {
        public int? AdditionalNumber { get; set; }
        public string BankCode { get; set; }
        public int? BuildingNumber { get; set; }
        public string City { get; set; }
        public decimal CreditAmount { get; set; }
        public string CreditNoteNumber { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
        public string District { get; set; }
        public string Email { get; set; }
        public string IBANNumber { get; set; }
        public string MobileNo { get; set; }
        public string Street { get; set; }
        public int? ZipCode { get; set; }
    }
}

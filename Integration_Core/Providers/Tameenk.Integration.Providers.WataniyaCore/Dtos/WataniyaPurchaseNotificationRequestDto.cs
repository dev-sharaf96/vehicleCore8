using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaPurchaseNotificationRequestDto
    {
        public short InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public bool IsPurchased { get; set; }
        public short? PurchaseStatus { get; set; }
        public string Email { get; set; }
        public int? BuildingNumber { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public int? ZipCode { get; set; }
        public int? AdditionalNumber { get; set; }
        public int CreditNoteNumber { get; set; }
        public int CreditAmount { get; set; }
        public string IBANNumber { get; set; }
        public string BankCode { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }

        // additional fields for tpl request
        public int MobileNo { get; set; }
        public int RequestReferenceNo { get; set; }
        public int QuoteReferenceNo { get; set; }

        // additional fields for comp request
        public int PolicyRequestReferenceNo { get; set; }
        public int PolicyReferenceNo { get; set; }
    }
}

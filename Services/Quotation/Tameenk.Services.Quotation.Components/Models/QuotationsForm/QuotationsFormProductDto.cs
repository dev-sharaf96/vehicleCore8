using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormProductDto
    {
        public Guid? ProductId { get; set; }
        public int QuotationId { get; set; }
        public short? DeductibleValue { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public string ProductNameAr { get; set; }
        public string ProductNameEn { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public int? InsurancecompanyKey { get; set; }
        public string PolicyNo { get; set; }
        public bool IsPurchased { get; set; }

        #region Price Details list

        public List<QuotationsFormPriceDetailsDto> PriceDetailsDto { get; set; }

        #endregion
    }
}

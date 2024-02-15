using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaVehicleIDUpdateRequestDto
    {
        public short InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public int NajmVehicleId { get; set; }
        public string PolicyUploadedDateTime { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }

        // additional fields for tpl request
        public int RequestReferenceNo { get; set; }
        public int QuoteReferenceNo { get; set; }

        // additional fields for comp request
        public int PolicyRequestReferenceNo { get; set; }
        public int PolicyReferenceNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationNew.Api
{
    public class ClientQuoteRequest
    {
        public string QtRqstExtrnlId { get; set; }
        public int InsuranceCompanyId { get; set; }
        public Guid ParentRequestId { get; set; }
        public int InsuranceTypeCode { get; set; }
        public Boolean VehicleAgencyRepair { get; set; }
        public int DeductibleValue { get; set; }
        public string Channel { get; set; }
    }
}
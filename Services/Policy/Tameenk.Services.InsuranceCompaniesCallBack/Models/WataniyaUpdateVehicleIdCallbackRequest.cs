using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class WataniyaUpdateVehicleIdCallbackRequest
    {
        public int InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public long NajmVehicleId { get; set; }
        public string PolicyHolderID { get; set; }
        public DateTime PolicyUploadedDateTime { get; set; }

        // for TPL only
        public string QuoteReferenceNo { get; set; }
        public string RequestReferenceNo { get; set; }

        // for sequence only
        public long? VehicleSequenceNumber { get; set; }

        // for COMP only
        public string PolicyReferenceNo { get; set; }
        public string PolicyRequestReferenceNo { get; set; }

        // for custom only
        public long? VehicleCustomID { get; set; }
    }
}
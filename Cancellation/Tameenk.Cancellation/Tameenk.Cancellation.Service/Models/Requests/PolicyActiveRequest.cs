using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.Service.Models
{
    public class PolicyActiveRequest
    {
        public string ReferenceId { get; set; }
        public int InsuredId { get; set; }
        public int VehicleId { get; set; }
        public int ReasonCode { get; set; }
        public int VehicleIdTypeCode { get; set; }
    }
}

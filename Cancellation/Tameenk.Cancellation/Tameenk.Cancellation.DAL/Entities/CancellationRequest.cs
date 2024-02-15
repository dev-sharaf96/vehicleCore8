using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.DAL.Entities
{
    public class CancellationRequest : BaseEntity
    {
        public string ServerIP { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string UserAgent { get; set; }
        public string ReferenceId { get; set; }
        public int InsuredId { get; set; }
        public int VehicleId { get; set; }
        public int ReasonCode { get; set; }
        public string ReasonDescription { get; set; }
        public int VehicleIdTypeCode { get; set; }
        public string VehicleIdType { get; set; }
        public string RequestStatus { get; set; }
        public Guid RequestId { get; set; }
        public string Channel { get; set; }
        public string UserIP { get; set; }
        public int CancelFromCompany { get; set; }
        public int? RegisterToCompany  { get; set; }
    }
}

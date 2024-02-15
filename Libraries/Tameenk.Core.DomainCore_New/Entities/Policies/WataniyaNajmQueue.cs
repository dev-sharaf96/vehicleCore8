using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class WataniyaNajmQueue : BaseEntity
    {
        public int Id { get; set; }
        public int? ProcessingTries { get; set; }
        public string ErrorDescription { get; set; }
        public int? ErrorCode { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public short? InsuranceTypeCode { get; set; }
        public string ServiceRequest { get; set; }
        public string ServiceResponse { get; set; }
        public string Channel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ServerIP { get; set; }
        public bool IsLocked { get; set; }
        public string PolicyReferenceNo { get; set; }
        public string PolicyNo { get; set; }
        public Guid  VehicleId { get; set; }

    }
}

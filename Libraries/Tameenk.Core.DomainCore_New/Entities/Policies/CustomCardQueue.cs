using System;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class CustomCardQueue : BaseEntity
    {
        public int Id { get; set; }
        public int? PriorityId { get; set; }
        public string ReferenceId { get; set; }
        public DateTime? DontProcessBeforeDate { get; set; }
        public int? ProcessingTries { get; set; }
        public string ErrorDescription { get; set; }
        public int? ErrorCode { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public short? InsuranceTypeCode { get; set; }
        public string CustomCardNumber { get; set; }
        public string ServiceRequest { get; set; }
        public string ServiceResponse { get; set; }
        public string Channel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ServerIP { get; set; }
        public bool? IsLocked { get; set; }
        public short? ModelYear { get; set; }
        public string PolicyNo { get; set; }
        public string UserId { get; set; }
        public Guid VehicleId { get; set; }
        public int? PolicyStatusId { get; set;}
    }
}

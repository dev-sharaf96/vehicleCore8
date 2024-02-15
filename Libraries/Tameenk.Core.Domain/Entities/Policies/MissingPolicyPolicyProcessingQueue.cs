using System;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class MissingPolicyPolicyProcessingQueue : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public int MergingProcessingTries { get; set; }
        public int ProcessingTries { get; set; }
        public string ErrorDescription { get; set; }
        public string MergingErrorDescription { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public string DriverNin { get; set; }
        public string VehicleId { get; set; }
        public string ServiceRequest { get; set; }
        public string ServiceResponse { get; set; }
        public string Chanel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public bool IsExist { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDone { get; set; }
    }
}

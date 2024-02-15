using System;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class PolicyProcessingQueue : BaseEntity
    {

        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int PriorityId { get; set; }


        /// <summary>
        /// Gets or sets the checkout reference id.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the date and time before which this prolicy should not be processed
        /// </summary>
        public DateTime? DontProcessBeforeDate { get; set; }

        /// <summary>
        /// Gets or sets the processing tries
        /// </summary>
        public int ProcessingTries { get; set; }

        public string ErrorDescription { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public Guid? RequestID { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public string DriverNin { get; set; }
        public string VehicleId { get; set; }

        public string ServiceRequest { get; set; }
        public string ServiceResponse { get; set; }

        public string Chanel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string UserName { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public string ServerIP { get; set; }
        ///// <summary>
        ///// Gets or sets the priority
        ///// </summary>
        //public PolicyProcessingQueuePriority Priority
        //{
        //    get
        //    {
        //        return (PolicyProcessingQueuePriority)this.PriorityId;
        //    }
        //    set
        //    {
        //        this.PriorityId = (int)value;
        //    }
        //}

        public bool IsLocked { get; set; }
        public int? ErrorCode{ get; set; }
    }
}

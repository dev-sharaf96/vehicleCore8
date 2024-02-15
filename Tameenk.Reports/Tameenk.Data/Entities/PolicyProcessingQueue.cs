namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyProcessingQueue")]
    public partial class PolicyProcessingQueue
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferenceId { get; set; }

        public int PriorityId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DontProcessBeforeDate { get; set; }

        public int ProcessingTries { get; set; }

        public DateTime? ProcessedOn { get; set; }
        
        public string VehicleId { get; set; }
        public string DriverNin { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public Guid? RequestID { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string ServiceRequest { get; set; }
        public string ErrorDescription { get; set; }
       
        public string ServiceResponse { get; set; }
    }
}

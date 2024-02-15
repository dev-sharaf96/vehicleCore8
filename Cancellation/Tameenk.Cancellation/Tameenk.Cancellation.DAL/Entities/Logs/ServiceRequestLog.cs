namespace Tameenk.Cancellation.DAL.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ServiceRequestLog")]
    public partial class ServiceRequestLog
    {
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UserID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string Method { get; set; }

        public int? CompanyID { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

        [StringLength(255)]
        public string ServiceURL { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        public Guid? RequestId { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        [StringLength(255)]
        public string Channel { get; set; }

        [StringLength(50)]
        public string ServiceErrorCode { get; set; }

        public string ReferenceId { get; set; }

        public string ServiceErrorDescription { get; set; }

        public string DriverNin { get; set; }

        public string VehicleId { get; set; }
    }
}

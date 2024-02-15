namespace Tameenk.Loggin.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PdfGenerationLog")]
    public partial class PdfGenerationLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UserID { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        public int? CompanyID { get; set; }

        [StringLength(255)]
        public  String? CompanyName { get; set; }

        [StringLength(255)]
        public  String? ServiceURL { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? ServiceRequest { get; set; }

        public  String? ServiceResponse { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        public Guid? RequestId { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        [StringLength(255)]
        public  String? Channel { get; set; }

        public  String? ReferenceId { get; set; }
        public  String? PolicyNo { get; set; }

        public int InsuranceTypeCode { get; set; }

        public  String? DriverNin { get; set; }

        public  String? VehicleId { get; set; }
        public  String? RetrievingMethod { get; set; }
        
    }
}

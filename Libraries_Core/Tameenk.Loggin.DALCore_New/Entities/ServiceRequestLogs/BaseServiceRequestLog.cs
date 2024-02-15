
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Tameenk.Loggin.DAL
{
  
    public abstract class BaseServiceRequestLog : IServiceRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? CreatedOn { get; set; }
        public Guid? UserID { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(255)]
        public  String? Method { get; set; }

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

        [StringLength(500)]
        public  String? ServiceErrorCode { get; set; }

        public  String? ServiceErrorDescription { get; set; }

        public  String? ReferenceId { get; set; }

        public int? InsuranceTypeCode { get; set; }

        public  String? DriverNin { get; set; }

        public  String? VehicleId { get; set; }

        public  String? PolicyNo { get; set; }
        public  String? VehicleMaker { get; set; }
        public  String? VehicleMakerCode { get; set; }
        public  String? VehicleModel { get; set; }
        public  String? VehicleModelCode { get; set; }
        public int? VehicleModelYear { get; set; }
        public  String? ExternalId { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public  String? City { get; set; }
        public  String? ChassisNumber { get; set; }
    }
}

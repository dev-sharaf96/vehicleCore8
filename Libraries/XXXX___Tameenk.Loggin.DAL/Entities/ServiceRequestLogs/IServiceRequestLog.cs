using System;

namespace Tameenk.Loggin.DAL
{
    public interface IServiceRequestLog
    {
         int ID { get; set; }

         DateTime? CreatedDate { get; set; }
         DateTime? CreatedOn { get; set; }
         Guid? UserID { get; set; }

         string UserName { get; set; }

         string Method { get; set; }

         int? CompanyID { get; set; }

         string CompanyName { get; set; }

         string ServiceURL { get; set; }

         int? ErrorCode { get; set; }

         string ErrorDescription { get; set; }

         string ServiceRequest { get; set; }

         string ServiceResponse { get; set; }

         string ServerIP { get; set; }

         Guid? RequestId { get; set; }

         double? ServiceResponseTimeInSeconds { get; set; }

         string Channel { get; set; }

         string ServiceErrorCode { get; set; }

         string ServiceErrorDescription { get; set; }

         string ReferenceId { get; set; }

         int? InsuranceTypeCode { get; set; }

         string DriverNin { get; set; }

         string VehicleId { get; set; }

         string PolicyNo { get; set; }
         string VehicleMaker { get; set; }
         string VehicleMakerCode { get; set; }
         string VehicleModel { get; set; }
         string VehicleModelCode { get; set; }
         int? VehicleModelYear { get; set; }
        string ExternalId { get; set; }
        bool? VehicleAgencyRepair { get; set; }
        string City { get; set; }
        string ChassisNumber { get; set; }
    }
}
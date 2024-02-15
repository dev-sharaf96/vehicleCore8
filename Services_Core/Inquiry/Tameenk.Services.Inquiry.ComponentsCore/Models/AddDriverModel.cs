using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Tameenk.Services.Inquiry.Components
{

    public class AddDriverModel : BaseModel
    {
        [Required]
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        [Required]
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        [Required]
        [JsonProperty("additionStartDate")]
        public DateTime AdditionStartDate { get; set; }

        [Required]
        [JsonProperty("driver")]
        public DriverModel Driver { set; get; }


        //public int CompanyId { set; get; }
        //[JsonProperty("driverId")]
        //public string DriverId { get; set; }
        //[JsonProperty("driverIdTypeCode")]
        //public int DriverIdTypeCode { get; set; }
        //[JsonProperty("birthDateYear")]
        //public int BirthDateYear { get; set; }
        //[JsonProperty("birthDateMonth")]
        //public int BirthDateMonth { get; set; }
        //[JsonProperty("driverDrivingPercentage")]
        //public int DriverDrivingPercentage { set; get; }
        //[JsonProperty("driverEducationCode")]
        //public int DriverEducationCode { set; get; }
        //[JsonProperty("driverChildrenBelow16Years")]
        //public int DriverChildrenBelow16Years { set; get; }
        //[JsonProperty("DriverMedicalConditionCode")]
        //public int DriverMedicalConditionCode { set; get; }
        //[JsonProperty("driverHomeCityCode")]
        //public string DriverHomeCityCode { set; get; }
        //[JsonProperty("driverHomeCity")]
        //public string DriverHomeCity { set; get; }
        //[JsonProperty("driverWorkCityCode")]
        //public string DriverWorkCityCode { set; get; }
        //[JsonProperty("driverWorkCity")]
        //public string DriverWorkCity { set; get; }
        //[JsonProperty("driverNOALast5Years")]
        //public int DriverNOALast5Years { set; get; }
        //[JsonProperty("driverNOCLast5Years")]
        //public int DriverNOCLast5Years { set; get; }
        //[JsonProperty("driverRelationship")]
        //public int DriverRelationship { set; get; }
        //[JsonProperty("driverLicenses")]
        //public List<DeiverLicense> DriverLicenses { set; get; }
        //public List<ViolationModel> DriverViolations { set; get; }
        //public string AdditionStartDate { get; internal set; }
    }
}
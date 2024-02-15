using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Integration.Dto.Yakeen
{
    public class DriverYakeenInfoRequestModel
    {
        [Required]
        public long Nin { get; set; }

        //[Range(1, 12)]
        public int? LicenseExpiryMonth { get; set; }

        public int? LicenseExpiryYear { get; set; }

        [Required]
        [Range(1, 12)]
        public int BirthMonth { get; set; }

        [Required]
        public int BirthYear { get; set; }

        public Guid? ParentRequestId { get; set; }

        public Guid? UserId { get; set; }
        public string UserName { get; set; }


        [Required]
        public int EducationId { get; set; }

        public int ChildrenBelow16Years { get; set; }

        public int DrivingPercentage { get; set; }

        public int MedicalConditionId { get; set; }
        public List<DriverExtraLicenseModel> DriverExtraLicenses { get; set; }

        public List<int> ViolationIds { get; set; }

        /// <summary>
        /// Driver Number of Accident Last 5 Years
        /// </summary>
        public int? DriverNOALast5Years { get; set; }

        /// <summary>
        /// Driver Work City Code
        /// </summary>
        public int? DriverWorkCityCode { get; set; }

        /// <summary>
        /// Driver Home City Code
        /// </summary>
        public int? DriverHomeCityCode { get; set; }

        public string CityName { get; set; }
        public string WorkCityName { get; set; }
    }
}
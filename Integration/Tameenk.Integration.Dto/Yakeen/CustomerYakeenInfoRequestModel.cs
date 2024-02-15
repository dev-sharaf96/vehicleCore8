using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Integration.Dto.Yakeen
{
    public class CustomerYakeenInfoRequestModel
    {
        [Required]
        public long Nin { get; set; }

        [Required]
        [Range(1, 12)]
        public int BirthMonth { get; set; }

        [Required]
        public int BirthYear { get; set; }

        [Required]
        public bool IsSpecialNeed { get; set; }

        [Required]
        public int EducationId { get; set; }

        public int ChildrenBelow16Years { get; set; }

        /// <summary>
        /// Driver percentage of using the vehicle related to other driver(s) useage.
        /// </summary>
        public int DrivingPercentage { get; set; }

        public int MedicalConditionId { get; set; }


        public Guid? ParentRequestId { get; set; }

        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public List<DriverExtraLicenseModel> DriverExtraLicenses { get; set; }

        public List<int> ViolationIds { get; set; }

        public int? NOALast5Years { get; set; }

        /// <summary>
        /// City code selected by the driver
        /// </summary>
        public long? CityId { get; set; }
        /// <summary>
        /// Driver Work City Code
        /// </summary>
        public long? WorkCityId { get; set; }


        //public long? CityId { get; set; }
        public string WorkCityName { get; set; }
        public string CityName { get; set; }
    }
}
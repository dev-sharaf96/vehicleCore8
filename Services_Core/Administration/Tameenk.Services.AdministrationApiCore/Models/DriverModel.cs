using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// driver model to represent driver Entity
    /// </summary>
    [JsonObject("driver")]
    public class DriverModel
    {

        /// <summary>
        /// full English Name
        /// </summary>
        [JsonProperty("fullEnglishName")]
        public string FullEnglishName
        {
            get
            {
                return EnglishFirstName + " " + EnglishSecondName + " " + EnglishLastName;
            }
        }
        /// <summary>
        /// Full Arabic Name
        /// </summary>
        [JsonProperty("fullArabicName")]
        public string FullArabicName
        {
            get
            {
                return FirstName + " " + SecondName + " " + LastName;
            }
        }

        /// <summary>
        /// Driver id
        /// </summary>
        [JsonProperty("driverId")]
        public string DriverId { get; set; }

        /// <summary>
        /// is citizen
        /// </summary>
        [JsonProperty("isCitizen")]
        public bool IsCitizen { get; set; }

        /// <summary>
        /// English First name
        /// </summary>
        [JsonProperty("englishFirstName")]
        public string EnglishFirstName { get; set; }

        /// <summary>
        /// English Last name
        /// </summary>
        [JsonProperty("englishLastName")]
        public string EnglishLastName { get; set; }


        /// <summary>
        /// English Second name
        /// </summary>
        [JsonProperty("englishSecondName")]
        public string EnglishSecondName { get; set; }

        /// <summary>
        /// english Third name
        /// </summary>
        [JsonProperty("englishThirdName")]
        public string EnglishThirdName { get; set; }

        /// <summary>
        /// Arabic last name
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Arabic second name
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        /// arabic first name
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// arabic third name
        /// </summary>
        [JsonProperty("thirdName")]
        public string ThirdName { get; set; }

        /// <summary>
        /// Subtribe Name
        /// </summary>
        [JsonProperty("subtribeName")]
        public string SubtribeName { get; set; }

        /// <summary>
        /// date of birth Georgean 
        /// </summary>
        [JsonProperty("dateOfBirthG")]
        public DateTime DateOfBirthG { get; set; }

        /// <summary>
        /// Nationality Code
        /// </summary>
        [JsonProperty("nationalityCode")]
        public short? NationalityCode { get; set; }

        /// <summary>
        /// Date of birth hijri
        /// </summary>
        [JsonProperty("dateOfBirthH")]
        public string DateOfBirthH { get; set; }

        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }


        /// <summary>
        /// Driver gender.
        /// </summary>
        [JsonProperty("genderId")]
        public int GenderId { get; set; }

        /// <summary>
        /// NCD Free Years
        /// </summary>
        [JsonProperty("ncdFreeYears")]
        public int? NCDFreeYears { get; set; }


        /// <summary>
        /// NCD Reference.
        /// </summary>
        [JsonProperty("ncdReference")]
        public string NCDReference { get; set; }


        /// <summary>
        /// is specail need
        /// </summary>
        [JsonProperty("isSpecialNeed")]
        public bool? IsSpecialNeed { get; set; }

        /// <summary>
        /// id issue place
        /// </summary>
        [JsonProperty("idIssuePlace")]
        public string IdIssuePlace { get; set; }

        /// <summary>
        /// id expire date
        /// </summary>
        [JsonProperty("idExpiryDate")]
        public string IdExpiryDate { get; set; }


        /// <summary>
        /// Driver Driving percentage of the vehicle
        /// </summary>
        [JsonProperty("drivingPercentage")]
        public int? DrivingPercentage { get; set; }

        /// <summary>
        /// Number of children under age 16 years.
        /// </summary>
        [JsonProperty("childrenBelow16Years")]
        public int? ChildrenBelow16Years { get; set; }


        /// <summary>
        /// education id
        /// </summary>
        [JsonProperty("educationId")]
        public int EducationId { get; set; }

        /// <summary>
        /// Education description En
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// Social status description
        /// </summary>
        public string SocialStatus { get; set; }

        /// <summary>
        /// social status id
        /// </summary>
        [JsonProperty("socialStatusId")]
        public int? SocialStatusId { get; set; }


        /// <summary>
        /// Driver Occupation identifier
        /// </summary>
        [JsonProperty("occupationId")]
        public int? OccupationId { get; set; }

        /// <summary>
        /// Medical condition id.
        /// </summary>
        [JsonProperty("medicalConditionId")]
        public int? MedicalConditionId { get; set; }

        /// <summary>
        /// Driver male/Femal resident occupation.
        /// </summary>
        [JsonProperty("residentOccupation")]
        public string ResidentOccupation { get; set; }

        /// <summary>
        /// City code selected by the driver
        /// </summary>
        [JsonProperty("cityId")]
        public long? CityId { get; set; }



        /// <summary>
        /// Driver Work City Code
        /// </summary>
        [JsonProperty("workCityId")]
        public long? WorkCityId { get; set; }

        /// <summary>
        /// Number of at-fault accidents in the last 5 years
        /// </summary>
        [JsonProperty("noaLast5Years")]
        public int? NOALast5Years { get; set; }


        /// <summary>
        /// Number of at-fault claims in the last 5 years
        /// </summary>
        [JsonProperty("nocLast5Years")]
        public int? NOCLast5Years { get; set; }

        /// <summary>
        /// city AR
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// Region Ar
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Work City Ar
        /// </summary>
        [JsonProperty("workCity")]
        public string WorkCity { get; set; }
    }
}
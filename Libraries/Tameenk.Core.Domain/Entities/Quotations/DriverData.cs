using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class DriverData
    {
        #region Main Data

        public Guid DriverId { get; set; }
        public bool IsCitizen { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishSecondName { get; set; }
        public string EnglishThirdName { get; set; }
        public string EnglishLastName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string SubtribeName { get; set; }
        public DateTime DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; }
        public short? NationalityCode { get; set; }
        public string NIN { get; set; }
        public int GenderId { get; set; }
        public int? NCDFreeYears { get; set; }
        public string NCDReference { get; set; }
        public bool? IsSpecialNeed { get; set; }
        public string IdIssuePlace { get; set; }
        public string IdExpiryDate { get; set; }
        public int? DrivingPercentage { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public int EducationId { get; set; }
        public int? SocialStatusId { get; set; }
        public int? OccupationId { get; set; }
        public int? MedicalConditionId { get; set; }
        public string ResidentOccupation { get; set; }
        public long? CityId { get; set; }
        public long? WorkCityId { get; set; }
        public int? NOALast5Years { get; set; }
        public int? NOCLast5Years { get; set; }

        #endregion

        #region Additional Data

        public string FullEnglishName
        {
            get
            {
                return EnglishFirstName + " " + EnglishSecondName + " " + EnglishLastName;
            }
        }
        public string FullArabicName
        {
            get
            {
                return FirstName + " " + SecondName + " " + LastName;
            }
        }
        // Gender data
        public string GenderCode { get; set; }
        public string GenderEngloishDescription { get; set; }
        public string GenderArabicDescription { get; set; }
        // Occupation data
        public string OccCode { get; set; }
        public string OccName { get; set; }
        // City data
        public long? CityYakeenCode { get; set; }
        public string CityArabicDescription { get; set; }
        // City data
        public long? WorkCityYakeenCode { get; set; }
        public string WorkCityArabicDescription { get; set; }
        public short? DriverRelationship { get; set; }

        #endregion
        public string NationalAddress { get; set; }
    }
}

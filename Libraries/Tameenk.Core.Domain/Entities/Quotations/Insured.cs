using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums.Quotations;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    /// <summary>
    /// Represent the details of insured person.
    /// </summary>
    public class Insured : BaseEntity
    {
        public Insured()
        {
            SocialStatusId = null;
            EducationId = (int)Education.Academic;
            InsuredExtraLicenses = new HashSet<InsuredExtraLicenses>();
        }
        /// <summary>
        /// The identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The national identifier type code.
        /// </summary>
        public int CardIdTypeId { get; set; }


        /// <summary>
        /// National ID / Iqama ID
        /// </summary>
        public string NationalId { get; set; }

        /// <summary>
        /// Insured Birth Date (Format: dd-MM-yyyy)
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Insured Birth Date (Format: dd-MM-yyyy)
        /// </summary>
        public string BirthDateH { get; set; }

        /// <summary>
        /// Insured Gender Code
        /// </summary>
        public int GenderId { get; set; }

        /// <summary>
        /// Insured Nationality Code
        /// </summary>
        public string NationalityCode { get; set; }

        /// <summary>
        /// Insured ID Issue Place in Arabic
        /// </summary>
        public long? IdIssueCityId { get; set; }

        /// <summary>
        /// Insured First Name in Arabic
        /// </summary>
        public string FirstNameAr { get; set; }

        /// <summary>
        /// Insured Middle Name in Arabic
        /// </summary>
        public string MiddleNameAr { get; set; }

        /// <summary>
        /// Insured Last Name in Arabic
        /// </summary>
        public string LastNameAr { get; set; }

        /// <summary>
        /// Insured First Name in English
        /// </summary>
        public string FirstNameEn { get; set; }

        /// <summary>
        /// Insured Middle Name in English
        /// </summary>
        public string MiddleNameEn { get; set; }

        /// <summary>
        /// Insured Last Name in English
        /// </summary>
        public string LastNameEn { get; set; }

        /// <summary>
        /// Insured Marital Status Code.
        /// </summary>
        public int? SocialStatusId { get; set; }

        /// <summary>
        /// Insured Occupation identifier
        /// </summary>
        public int? OccupationId { get; set; }

        /// <summary>
        /// Male/Femal resident occupation.
        /// </summary>
        public string ResidentOccupation { get; set; }

        /// <summary>
        /// Insured Education identifier.
        /// </summary>
        public int EducationId { get; set; }

        /// <summary>
        /// Number of children under age 16 years.
        /// </summary>
        public int? ChildrenBelow16Years { get; set; }

        /// <summary>
        /// Insured Work City Code
        /// </summary>
        public long? WorkCityId { get; set; }

        /// <summary>
        /// City code selected by the insured
        /// </summary>
        public long? CityId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public long? UserSelectedWorkCityId { get; set; }

        /// <summary>
        /// City code selected by the insured
        /// </summary>
        public long? UserSelectedCityId { get; set; }

        public int? AddressId { get; set; }
        
        /// <summary>
        /// Insured gender.
        /// </summary>
        public Gender Gender {
            get { return (Gender)GenderId; }
            set { GenderId = (int)value; }
        }

        /// <summary>
        /// Insured national id issue place 
        /// </summary>
        public City IdIssueCity { get; set; }
        /// <summary>
        /// Insured Marital Status.
        /// </summary>
        public SocialStatus? SocialStatus
        {
            get { return (SocialStatus)SocialStatusId; }
            set { SocialStatusId = null; }
        }

        /// <summary>
        /// The national card id type.
        /// </summary>
        public CardIdType CardIdType
        {
            get { return (CardIdType)CardIdTypeId; }
            set { CardIdTypeId = (int)value; }
        }

        /// <summary>
        /// Insured Occupation
        /// </summary>
        public virtual Occupation Occupation { get; set; }

        /// <summary>
        /// Insured education level.
        /// </summary>
        public Education Education
        {
            get { return (Education)EducationId; }
            set { EducationId = (int)Education.Academic; }
        }

        /// <summary>
        /// Insured Work City.
        /// </summary>
        public City WorkCity { get; set; }

        /// <summary>
        /// Insured city.
        /// </summary>
        public City City { get; set; }
        public ICollection<InsuredExtraLicenses> InsuredExtraLicenses { get; set; }
        public string OccupationName { get; set; }
        public string OccupationCode { get; set; }
    }
}

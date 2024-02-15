using System;
using Tameenk.Core.Domain.Enums.Quotations;

namespace Tameenk.Integration.Dto.Yakeen
{
    public class CustomerYakeenInfoModel
    {
        public bool Success { get; set; }

        public YakeenInfoErrorModel Error { get; set; }

        public Guid TameenkId { get; set; }

        public bool IsCitizen { get; set; }

        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }

        public DateTime DateOfBirthG { get; set; }

        public short? NationalityCode { get; set; }

        public string DateOfBirthH { get; set; }

        public string NIN { get; set; }

        public int? OccupationId { get; set; }

        public int? SocialStatusId { get; set; }

        public Gender Gender { get; set; }

        public bool? IsSpecialNeed { get; set; }

        public string IdIssuePlace { get; set; }

        public string IdExpiryDate { get; set; }

        public string CityName { get; set; }
        public int? AddressId { get; set; }
        public string WorkCityName { get; set; }
        public string OccupationName { get; set; }
        public string EducationName { get; set; }
        public string SocialStatusName { get; set; }
        public string PostCode { get; set; }
    }
}
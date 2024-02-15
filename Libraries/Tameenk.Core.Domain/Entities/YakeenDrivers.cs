using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums.Quotations;

namespace Tameenk.Core.Domain.Entities
{
    public class YakeenDrivers : BaseEntity
    {
        public YakeenDrivers(){}

        public Guid DriverId { get; set; }

        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }

        public DateTime? DateOfBirthG { get; set; }

        public short? NationalityCode { get; set; }
        
        public string DateOfBirthH { get; set; }

        public string NIN { get; set; }

        public int? GenderId { get; set; }

        public string IdIssuePlace { get; set; }
        
        public string IdExpiryDate { get; set; }

        public string OccupationCode { get; set; }

        public string SocialStatus { get; set; }

        public int? LogId { get; set; }
        public string OccupationDesc { get; set; }
        public string LicenseList { get; set; }
        
        public DateTime? CreatedDate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.YakeenBCareService;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class DriverInfoResult
    {
        public DriverInfoResult()
        {
            LicensesList = new List<DriverLicense>();
            Error = new YakeenError();
        }


        public bool Success { get; set; }
        public YakeenError Error { get; set; }

        /// <summary>
        /// 1 --> citizen , 2 --> aligen
        /// </summary>
        public int IsCitizen { get; set; }
        public int LogId { get; set; }


        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }

        public IList<DriverLicense> LicensesList { get; set; }

        public gender Gender { get; set; }
        public short NationalityCode { get; set; }

        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public DateTime DateOfBirthG { get; set; }
        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public string DateOfBirthH { get; set; }

        public Guid InternalIdentifier { get; set; }
    }
}

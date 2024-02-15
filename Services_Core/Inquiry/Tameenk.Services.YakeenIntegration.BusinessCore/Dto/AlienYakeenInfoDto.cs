using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.YakeenIntegration.Business.Dto.Enums;

namespace Tameenk.Services.YakeenIntegration.Business.Dto
{
    public class AlienYakeenInfoDto
    {
        public AlienYakeenInfoDto()
        {
            // Error = new YakeenErrorDto();
            Licenses = new List<DriverLicenseYakeenInfoDto>();
        }

        //public bool Success { get; set; }
        //  public YakeenErrorDto Error { get; set; }

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
        public IList<DriverLicenseYakeenInfoDto> Licenses { get; set; }
        public EGender Gender { get; set; }
        public short NationalityCode { get; set; }

        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public DateTime DateOfBirthG { get; set; }
        //for plate
        //VehiclePlateYakeenInfoDto
        public string DateOfBirthH { get; set; }

        public string ChassisNumber { get; set; }

        public int LogId { get; set; }

        public string OwnerName { get; set; }

        public short PlateNumber { get; set; }

        public string PlateText1 { get; set; }

        public string PlateText2 { get; set; }

        public string PlateText3 { get; set; }

    }
}

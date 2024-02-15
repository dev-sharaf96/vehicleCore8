using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using TameenkDAL.Store;
using TameenkDAL.UoW;
using Tamkeen.bll.Services.Yakeen.Models;
using Tamkeen.bll.YakeenBCareService;

namespace Tamkeen.bll.Business
{
    public class DriverBusiness
    {
        private readonly ITameenkUoW _tameenkUoW;
        private readonly DriverRepository _driverRepository;

        public DriverBusiness(ITameenkUoW tameenkUoW)
        {
            _tameenkUoW = tameenkUoW;
            _driverRepository = _tameenkUoW.DriverRepository;
        }

        public Driver GetDriverInfoByNIN(long nin)
        {
            var driver = _driverRepository.GetDriverInfoByNIN(nin.ToString());
            if (driver != null)
            {
                if (!validateDriverEntity(driver))
                    return null;
            }

            return driver;
        }

        public void UpdateDriverFromCustomerNameInfoResult(Driver driver, CustomerNameInfoResult customerNameInfoResult)
        {
            if (customerNameInfoResult.Success && driver != null)
            {
                driver.EnglishFirstName = customerNameInfoResult.EnglishFirstName;
                driver.EnglishSecondName = customerNameInfoResult.EnglishSecondName;
                driver.EnglishThirdName = customerNameInfoResult.EnglishThirdName;
                driver.EnglishLastName = customerNameInfoResult.EnglishLastName;
                driver.FirstName = customerNameInfoResult.FirstName;
                driver.SecondName = customerNameInfoResult.SecondName;
                driver.ThirdName = customerNameInfoResult.ThirdName;
                driver.LastName = customerNameInfoResult.LastName;
                driver.SubtribeName = customerNameInfoResult.SubtribeName;

                _driverRepository.Update(driver);
                _tameenkUoW.Save();
            }
        }

        public Guid CreateDriverEntityFromDriverInfo(DriverInfoResult driverInfo, string NIN, bool isCitizen)
        {
            var driver = new Driver()
            {
                DriverId = Guid.NewGuid(),
                IsCitizen = isCitizen,
                EnglishFirstName = driverInfo.EnglishFirstName,
                EnglishLastName = driverInfo.EnglishLastName,
                EnglishSecondName = driverInfo.EnglishSecondName,
                EnglishThirdName = driverInfo.EnglishThirdName,
                LastName = driverInfo.LastName,
                SecondName = driverInfo.SecondName,
                FirstName = driverInfo.FirstName,
                ThirdName = driverInfo.ThirdName,
                SubtribeName = driverInfo.SubtribeName,
                DateOfBirthG = driverInfo.DateOfBirthG,
                DateOfBirthH = driverInfo.DateOfBirthH,
                NationalityCode = driverInfo.NationalityCode,
                NIN = NIN,
                IsDeleted = false,
                CreatedDateTime = DateTime.Now
            };
            //driver.Gender = driverInfo.Gender
            //switch (driverInfo.Gender)
            //{
            //    case gender.F:
            //        driver.Gender = "F";
            //        break;
            //    case gender.M:
            //        driver.Gender = "M";
            //        break;
            //    case gender.U:
            //        driver.Gender = "U";
            //        break;
            //}


            if (driverInfo.LicensesList != null && driverInfo.LicensesList.Any())
            {
                foreach (var lic in driverInfo.LicensesList)
                {
                    driver.DriverLicenses.Add(new Tameenk.Core.Domain.Entities.VehicleInsurance.DriverLicense()
                    {
                        DriverId = driver.DriverId,
                        TypeDesc = lic.TypeDesc,
                        ExpiryDateH = lic.ExpiryDateH
                    });
                }
            }
            _driverRepository.Insert(driver);
            _tameenkUoW.Save();
            return driver.DriverId;
        }

        public Guid CreateDriverEntityFromCustomerIdInfo(CustomerIdInfoResult customerIdInfo, string NIN, bool isCitizen, bool isSpecialNeed)
        {
            var driver = new Driver()
            {
                DriverId = Guid.NewGuid(),
                IsCitizen = isCitizen,
                DateOfBirthG = customerIdInfo.DateOfBirthG,
                DateOfBirthH = customerIdInfo.DateOfBirthH,
                NationalityCode = customerIdInfo.NationalityCode,
                NIN = NIN,
                IdIssuePlace = customerIdInfo.IdIssuePlace,
                IdExpiryDate = customerIdInfo.IdExpiryDate,
                IsSpecialNeed = isSpecialNeed,
                IsDeleted = false,
                CreatedDateTime = DateTime.Now,
            };
            //switch (customerIdInfo.Gender)
            //{
            //    case gender.F:
            //        driver.Gender = "F";
            //        break;
            //    case gender.M:
            //        driver.Gender = "M";
            //        break;
            //    case gender.U:
            //        driver.Gender = "U";
            //        break;
            //}

            _driverRepository.Insert(driver);
            _tameenkUoW.Save();
            return driver.DriverId;
        }

        public Guid UpdateDriverEntityFromDriverInfo(DriverInfoResult driverInfo, Driver driver)
        {
            driver.EnglishFirstName = driverInfo.EnglishFirstName;
            driver.EnglishLastName = driverInfo.EnglishLastName;
            driver.EnglishSecondName = driverInfo.EnglishSecondName;
            driver.EnglishThirdName = driverInfo.EnglishThirdName;
            driver.LastName = driverInfo.LastName;
            driver.SecondName = driverInfo.SecondName;
            driver.FirstName = driverInfo.FirstName;
            driver.ThirdName = driverInfo.ThirdName;
            driver.SubtribeName = driverInfo.SubtribeName;
            driver.DateOfBirthG = driverInfo.DateOfBirthG;
            driver.DateOfBirthH = driverInfo.DateOfBirthH;
            driver.NationalityCode = driverInfo.NationalityCode;

            //switch (driverInfo.Gender)
            //{
            //    case gender.F:
            //        driver.Gender = "F";
            //        break;
            //    case gender.M:
            //        driver.Gender = "M";
            //        break;
            //    case gender.U:
            //        driver.Gender = "U";
            //        break;
            //}


            if (driverInfo.LicensesList != null && driverInfo.LicensesList.Any())
            {
                foreach (var lic in driverInfo.LicensesList)
                {
                    driver.DriverLicenses.Add(new Tameenk.Core.Domain.Entities.VehicleInsurance.DriverLicense()
                    {
                        DriverId = driver.DriverId,
                        TypeDesc = lic.TypeDesc,
                        ExpiryDateH = lic.ExpiryDateH
                    });
                }
            }
            _driverRepository.Update(driver);
            _tameenkUoW.Save();
            return driver.DriverId;
        }

        public Guid UpdateDriverEntityFromCustomerIdInfo(CustomerIdInfoResult customerIdInfo, Driver driver, bool isSpecialNeed)
        {
            driver.IdIssuePlace = customerIdInfo.IdIssuePlace;
            driver.IdExpiryDate = customerIdInfo.IdExpiryDate;
            driver.IsSpecialNeed = isSpecialNeed;

            _driverRepository.Update(driver);
            _tameenkUoW.Save();
            return driver.DriverId;
        }

        private bool validateDriverEntity(Driver driver)
        {
            var dateTimeDiff = driver.CreatedDateTime.HasValue
                        ? (DateTime.Now - driver.CreatedDateTime.Value)
                        : TimeSpan.FromDays(3);

            if (dateTimeDiff > TimeSpan.FromDays(2))
            {
                driver.IsDeleted = true;
                _driverRepository.Update(driver);
                _tameenkUoW.Save();
                return false;
            }

            return true;
        }
    }
}
